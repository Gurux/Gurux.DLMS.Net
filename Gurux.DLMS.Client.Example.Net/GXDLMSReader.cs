//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
//
// More information of Gurux products: http://www.gurux.org
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------
using Gurux.Common;
using Gurux.DLMS.ASN;
using Gurux.DLMS.Client.Example;
using Gurux.DLMS.Ecdsa;
using Gurux.DLMS.Ecdsa.Enums;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Secure;
using Gurux.Net;
using Gurux.Serial;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Gurux.DLMS.Reader
{
    public class GXDLMSReader
    {
        /// <summary>
        /// Wait time in ms.
        /// </summary>
        public int WaitTime = 5000;
        /// <summary>
        /// Retry count.
        /// </summary>
        public int RetryCount = 3;
        IGXMedia Media;
        TraceLevel Trace;
        GXDLMSSecureClient Client;
        // Invocation counter (frame counter).
        string InvocationCounter = null;

        /// <summary>
        /// Notify caller from the notification event.
        /// </summary>
        public Action<object> OnNotification;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">DLMS Client.</param>
        /// <param name="media">Media.</param>
        /// <param name="trace">Trace level.</param>
        /// <param name="invocationCounter">Logical name of invocation counter.</param>
        public GXDLMSReader(GXDLMSSecureClient client, IGXMedia media, TraceLevel trace, string invocationCounter)
        {
            Trace = trace;
            Media = media;
            Client = client;
            //Get ECDSA keys when needed.
            Client.OnKeys += Client_OnKeys;
            InvocationCounter = invocationCounter;
        }

        /// <summary>
        /// Find ciphering keys.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Client_OnKeys(object sender, GXCryptoKeyParameter args)
        {
            try
            {
                if (args.Encrypt)
                {
                    //Find private key.
                    string path = GetPath(args.SecuritySuite, args.CertificateType, "Keys", args.SystemTitle);
                    args.PrivateKey = GXPkcs8.Load(path).PrivateKey;
                    Console.WriteLine("Client private key:" + path + " " + args.PrivateKey.ToHex());
                    Console.WriteLine("Client public key:" + path + " " + args.PrivateKey.GetPublicKey().ToHex());
                }
                else
                {
                    //Find public key.
                    if (args.SystemTitle == null)
                    {
                        string path = GetPath(args.SecuritySuite, args.CertificateType, "Certificates", args.SystemTitle);
                        GXx509Certificate pk = GXx509Certificate.Search(path, args.SystemTitle);
                        args.PublicKey = pk.PublicKey;
                        Console.WriteLine("Server public key:" + pk.SerialNumber);
                    }
                    else
                    {
                        string path = GetPath(args.SecuritySuite, args.CertificateType, "Certificates", args.SystemTitle);
                        GXx509Certificate pk = GXx509Certificate.Load(path);
                        args.PublicKey = pk.PublicKey;
                        Console.WriteLine("Server public key:" + pk.SerialNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Return correct path.
        /// </summary>
        /// <param name="securitySuite">Security Suite.</param>
        /// <param name="type">Certificate type.</param>
        /// <param name="path">Folder.</param>
        /// <param name="systemTitle">System title.</param>
        /// <returns>Path to the certificate file or folder if system title is not given.</returns>
        private static string GetPath(SecuritySuite securitySuite, CertificateType type, string path, byte[] systemTitle)
        {
            string pre;
            if (securitySuite == SecuritySuite.Suite2)
            {
                path = Path.Combine(path, "384");
            }
            if (systemTitle == null)
            {
                return path;
            }
            switch (type)
            {
                case CertificateType.DigitalSignature:
                    pre = "D";
                    break;
                case CertificateType.KeyAgreement:
                    pre = "A";
                    break;
                default:
                    throw new Exception("Invalid type.");
            }
            return Path.Combine(path, pre + GXDLMSTranslator.ToHex(systemTitle, false) + ".pem");
        }

        /// <summary>
        /// Generates a new server and client public/private keys and register them and import certificates to the meter.
        /// </summary>
        /// <param name="logicalName">Logical name of the security setup object.</param>
        public void GenerateCertificates(string logicalName)
        {
            if (!Directory.Exists("Keys"))
            {
                Directory.CreateDirectory("Keys");
            }
            if (!Directory.Exists("Certificates"))
            {
                Directory.CreateDirectory("Certificates");
            }
            if (!Directory.Exists("Keys384"))
            {
                Directory.CreateDirectory("Keys384");
            }
            if (!Directory.Exists("Certificates384"))
            {
                Directory.CreateDirectory("Certificates384");
            }
            string address = "https://certificates.gurux.fi/api/CertificateGenerator";
            GXx509Certificate[] certs;
            if (Client.Authentication < Authentication.Low)
            {
                throw new Exception("High authentication must be used to change the certificate keys.");
            }
            GXReplyData reply = new GXReplyData();
            InitializeConnection();
            GXDLMSSecuritySetup ss = new GXDLMSSecuritySetup(logicalName);
            List<GXCertificateRequest> certifications = new List<GXCertificateRequest>();
            //Read used security suite.
            Read(ss, 3);
            //Read client system title.
            Read(ss, 4);
            //Read server system title.
            Read(ss, 5);
            byte[] clientST = ss.ClientSystemTitle;
            if (clientST == null || clientST.Length != 8)
            {
                clientST = Client.Ciphering.SystemTitle;
            }
            //Get client subject.
            string subject = GXAsn1Converter.SystemTitleToSubject(clientST);
            //Generate new digital signature for the client. In this example P-256 keys are used.
            KeyValuePair<GXPublicKey, GXPrivateKey> kp = GXEcdsa.GenerateKeyPair(Ecc.P256);
            //Save private key in PKCS #8 format.
            GXPkcs8 key = new GXPkcs8(kp);
            key.Save(GXPkcs8.GetFilePath(Ecc.P256, CertificateType.DigitalSignature, clientST));
            //Generate x509 certificates.
            GXPkcs10 pkc10 = GXPkcs10.CreateCertificateSigningRequest(kp, subject);
            //All certigicates are generated with one request.
            certifications.Add(new GXCertificateRequest(CertificateType.DigitalSignature, pkc10));

            //Generate new key agreement for the client. In this example P-256 keys are used.
            kp = GXEcdsa.GenerateKeyPair(Ecc.P256);
            //Save private key in PKCS #8 format.
            key = new GXPkcs8(kp);
            key.Save(GXPkcs8.GetFilePath(Ecc.P256, CertificateType.KeyAgreement, clientST));
            //Generate x509 certificates.
            pkc10 = GXPkcs10.CreateCertificateSigningRequest(kp, subject);
            //All certigicates are generated with one request.
            certifications.Add(new GXCertificateRequest(CertificateType.KeyAgreement, pkc10));

            //Generate public/private key for digital signature.
            if (!ReadDataBlock(ss.GenerateKeyPair(Client, CertificateType.DigitalSignature), reply))
            {
                throw new GXDLMSException(reply.Error);
            }
            reply.Clear();
            //Generate public/private key for key agreement.
            if (!ReadDataBlock(ss.GenerateKeyPair(Client, CertificateType.KeyAgreement), reply))
            {
                throw new GXDLMSException(reply.Error);
            }
            reply.Clear();
            //Generate certification request.
            if (!ReadDataBlock(ss.GenerateCertificate(Client, CertificateType.DigitalSignature), reply))
            {
                throw new GXDLMSException(reply.Error);
            }
            //Generate server certification.
            pkc10 = new GXPkcs10((byte[])reply.Value);
            subject = GXAsn1Converter.SystemTitleToSubject(ss.ServerSystemTitle);
            //Validate subject.
            if (!pkc10.Subject.Contains(subject))
            {
                throw new Exception(string.Format("Server system title '{0}' is not the same as in the generated certificate request '{1}'.",
                   GXDLMSTranslator.ToHex(ss.ServerSystemTitle),
                   GXAsn1Converter.HexSystemTitleFromSubject(pkc10.Subject)));
            }
            certifications.Add(new GXCertificateRequest(CertificateType.DigitalSignature, pkc10));
            reply.Clear();

            //Generate certification request for key agreement.
            if (!ReadDataBlock(ss.GenerateCertificate(Client, CertificateType.KeyAgreement), reply))
            {
                throw new GXDLMSException(reply.Error);
            }
            //Generate server certification.
            pkc10 = new GXPkcs10((byte[])reply.Value);
            //Validate subject.
            if (!pkc10.Subject.Contains(subject))
            {
                throw new Exception(string.Format("Server system title '{0}' is not the same as in the generated certificate request '{1}'.",
                   GXDLMSTranslator.ToHex(ss.ServerSystemTitle),
                   GXAsn1Converter.HexSystemTitleFromSubject(pkc10.Subject)));
            }
            certifications.Add(new GXCertificateRequest(CertificateType.KeyAgreement, pkc10));
            reply.Clear();

            //Note! There is a limit how many request you can do in a day.
            certs = GXPkcs10.GetCertificate(address, certifications);
            //Save server certificates.
            foreach (GXx509Certificate it in certs)
            {
                it.Save(GXx509Certificate.GetFilePath(it));
            }
            reply.Clear();
            //Import server certificates.
            foreach (GXx509Certificate it in certs)
            {
                if (!ReadDataBlock(ss.ImportCertificate(Client, it), reply))
                {
                    throw new GXDLMSException(reply.Error);
                }
                reply.Clear();
            }
            //Export server certificates and verify it.
            foreach (GXx509Certificate it in certs)
            {
                CertificateEntity entity;
                byte[] st;
                if (it.Subject.Contains(GXAsn1Converter.SystemTitleToSubject(ss.ServerSystemTitle)))
                {
                    st = ss.ServerSystemTitle;
                    entity = CertificateEntity.Server;
                }
                else if (it.Subject.Contains(GXAsn1Converter.SystemTitleToSubject(clientST)))
                {
                    st = clientST;
                    entity = CertificateEntity.Client;
                }
                else
                {
                    //This is another certificate.
                    continue;
                }
                if (!ReadDataBlock(ss.ExportCertificateByEntity(Client, entity,
                    GXDLMSConverter.KeyUsageToCertificateType(it.KeyUsage), st), reply))
                {
                    throw new GXDLMSException(reply.Error);
                }
                //Verify certificate.
                GXx509Certificate exported = new GXx509Certificate((byte[])reply.Value);
                if (!exported.Equals(it))
                {
                    throw new ArgumentOutOfRangeException("Invalid server certificate.");
                }
                reply.Clear();
            }
            //Export server certificates using serial number and verify it.
            foreach (GXx509Certificate it in certs)
            {
                if (!ReadDataBlock(ss.ExportCertificateBySerial(Client, it.SerialNumber,
                    ASCIIEncoding.ASCII.GetBytes(it.Issuer)), reply))
                {
                    throw new GXDLMSException(reply.Error);
                }
                //Verify certificate.
                GXx509Certificate exported = new GXx509Certificate((byte[])reply.Value);
                if (!exported.Equals(it))
                {
                    throw new ArgumentOutOfRangeException("Invalid server certificate.");
                }
                reply.Clear();
            }
        }

        /// <summary>
        /// Export client and server certificates from the meter.
        /// </summary>
        /// <param name="logicalName">Logical name of the security setup object.</param>
        public void ExportMeterCertificates(string logicalName)
        {
            try
            {
                InitializeConnection();
                GXDLMSSecuritySetup ss = new GXDLMSSecuritySetup(logicalName);
                //Read used security suite.
                Read(ss, 3);
                //Client private keys are saved to this directory.
                //Client might be different system title for each meter.
                if (!Directory.Exists("Keys"))
                {
                    Directory.CreateDirectory("Keys");
                }
                if (!Directory.Exists("Certificates"))
                {
                    Directory.CreateDirectory("Certificates");
                }
                if (!Directory.Exists("Keys384"))
                {
                    Directory.CreateDirectory("Keys384");
                }
                if (!Directory.Exists("Certificates384"))
                {
                    Directory.CreateDirectory("Certificates384");
                }
                //Read server system title.
                Read(ss, 5);
                //Read certificates.
                Read(ss, 6);
                //Export meter certificates and save them.
                GXReplyData reply = new GXReplyData();
                foreach (GXDLMSCertificateInfo it in ss.Certificates)
                {
                    reply.Clear();
                    //Export certification and verify it.
                    if (!ReadDataBlock(ss.ExportCertificateBySerial(Client, it.SerialNumber, it.IssuerAsn1), reply))
                    {
                        throw new GXDLMSException(reply.Error);
                    }
                    GXx509Certificate cert = new GXx509Certificate((byte[])reply.Value);
                    string path = GXx509Certificate.GetFilePath(cert);
                    cert.Save(path);
                }
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// Read all data from the meter.
        /// </summary>
        public void ReadAll(string outputFile)
        {
            try
            {
                InitializeConnection();
                if (GetAssociationView(outputFile))
                {
                    GetScalersAndUnits();
                    GetProfileGenericColumns();
                }
                GetReadOut();
                GetProfileGenerics();
                if (outputFile != null)
                {
                    try
                    {
                        Client.Objects.Save(outputFile, new GXXmlWriterSettings() { UseMeterTime = true, IgnoreDefaultValues = false });
                    }
                    catch (Exception)
                    {
                        //It's OK if this fails.
                    }
                }
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// Send SNRM Request to the meter.
        /// </summary>
        public void SNRMRequest()
        {
            GXReplyData reply = new GXReplyData();
            byte[] data;
            data = Client.SNRMRequest();
            if (data != null)
            {
                if (Trace > TraceLevel.Info)
                {
                    Console.WriteLine("Send SNRM request." + GXCommon.ToHex(data, true));
                }
                ReadDataBlock(data, reply);
                if (Trace == TraceLevel.Verbose)
                {
                    Console.WriteLine("Parsing UA reply." + reply.ToString());
                }
                //Has server accepted client.
                Client.ParseUAResponse(reply.Data);
                if (Trace > TraceLevel.Info)
                {
                    Console.WriteLine("Parsing UA reply succeeded.");
                }
            }
        }

        /// <summary>
        /// Send AARQ Request to the meter.
        /// </summary>
        public void AarqRequest()
        {
            GXReplyData reply = new GXReplyData();
            //Generate AARQ request.
            //Split requests to multiple packets if needed.
            //If password is used all data might not fit to one packet.
            var aarq = Client.AARQRequest();
            //AARQ is not used for pre-established connections.
            if (aarq.Length != 0)
            {
                foreach (byte[] it in aarq)
                {
                    if (Trace > TraceLevel.Info)
                    {
                        Console.WriteLine("Send AARQ request", GXCommon.ToHex(it, true));
                    }
                    reply.Clear();
                    ReadDataBlock(it, reply);
                }
                if (Trace > TraceLevel.Info)
                {
                    Console.WriteLine("Parsing AARE reply" + reply.ToString());
                }
                //Parse reply.
                Client.ParseAAREResponse(reply.Data);
                reply.Clear();
                //Get challenge Is HLS authentication is used.
                if (Client.Authentication > Authentication.Low)
                {
                    foreach (byte[] it in Client.GetApplicationAssociationRequest())
                    {
                        reply.Clear();
                        ReadDataBlock(it, reply);
                    }
                    Client.ParseApplicationAssociationResponse(reply.Data);
                }
                if (Trace > TraceLevel.Info)
                {
                    Console.WriteLine("Parsing AARE reply succeeded.");
                }
            }
        }

        /// <summary>
        /// Read Invocation counter (frame counter) from the meter and update it.
        /// </summary>
        private void UpdateFrameCounter()
        {
            //Read frame counter if GeneralProtection is used.
            if (!string.IsNullOrEmpty(InvocationCounter) && Client.Ciphering != null && Client.Ciphering.Security != Security.None)
            {
                //Media settings are saved and they are restored when HDLC with mode E is used.
                string mediaSettings = Media.Settings;
                InitializeOpticalHead();
                byte[] data;
                GXReplyData reply = new GXReplyData();
                Client.ProposedConformance |= Conformance.GeneralProtection;
                int add = Client.ClientAddress;
                int serverAddress = Client.ServerAddress;
                Authentication auth = Client.Authentication;
                Security security = Client.Ciphering.Security;
                Signing signing = Client.Ciphering.Signing;
                byte[] challenge = Client.CtoSChallenge;
                try
                {
                    Client.ClientAddress = 16;
                    Client.Authentication = Authentication.None;
                    Client.Ciphering.Security = Security.None;
                    Client.Ciphering.Signing = Signing.None;
                    if (Media is GXNet net && Client.InterfaceType == InterfaceType.CoAP)
                    {
                        //Update Client Address.
                        //Client SAP.
                        Client.Coap.Options[65003] = (byte)Client.ClientAddress;
                        //Server SAP
                        Client.Coap.Options[65005] = (byte)1;
                    }
                    data = Client.SNRMRequest();
                    if (data != null)
                    {
                        if (Trace > TraceLevel.Info)
                        {
                            Console.WriteLine("Send SNRM request." + GXCommon.ToHex(data, true));
                        }
                        ReadDataBlock(data, reply);
                        if (Trace == TraceLevel.Verbose)
                        {
                            Console.WriteLine("Parsing UA reply." + reply.ToString());
                        }
                        //Has server accepted client.
                        Client.ParseUAResponse(reply.Data);
                        if (Trace > TraceLevel.Info)
                        {
                            Console.WriteLine("Parsing UA reply succeeded.");
                        }
                    }
                    //Generate AARQ request.
                    //Split requests to multiple packets if needed.
                    //If password is used all data might not fit to one packet.
                    foreach (byte[] it in Client.AARQRequest())
                    {
                        if (Trace > TraceLevel.Info)
                        {
                            Console.WriteLine("Send AARQ request", GXCommon.ToHex(it, true));
                        }
                        reply.Clear();
                        ReadDataBlock(it, reply);
                    }
                    if (Trace > TraceLevel.Info)
                    {
                        Console.WriteLine("Parsing AARE reply" + reply.ToString());
                    }
                    try
                    {
                        //Parse reply.
                        Client.ParseAAREResponse(reply.Data);
                        reply.Clear();
                        GXDLMSData d = new GXDLMSData(InvocationCounter);
                        Read(d, 2);
                        Client.Ciphering.InvocationCounter = 1 + Convert.ToUInt32(d.Value);
                        Console.WriteLine("Invocation counter: " + Convert.ToString(Client.Ciphering.InvocationCounter));
                        reply.Clear();
                        Disconnect();
                        //Reset media settings back to default.
                        if (Client.InterfaceType == InterfaceType.HdlcWithModeE)
                        {
                            Media.Close();
                            Media.Settings = mediaSettings;
                            InitializeOpticalHead();
                        }
                    }
                    catch (Exception Ex)
                    {
                        Disconnect();
                        throw Ex;
                    }
                }
                finally
                {
                    Client.ClientAddress = add;
                    Client.ServerAddress = serverAddress;
                    Client.Authentication = auth;
                    Client.Ciphering.Security = security;
                    Client.CtoSChallenge = challenge;
                    Client.Ciphering.Signing = signing;
                    if (Media is GXNet && Client.InterfaceType == InterfaceType.CoAP)
                    {
                        //Update Client Address.
                        //Client SAP.
                        Client.Coap.Options[65003] = (byte)Client.ClientAddress;
                        //Server SAP
                        Client.Coap.Options[65005] = (byte)Client.ServerAddress;
                    }
                }
            }
        }

        /// <summary>
        /// Send IEC disconnect message.
        /// </summary>
        void DiscIEC()
        {
            ReceiveParameters<string> p = new ReceiveParameters<string>()
            {
                AllData = false,
                Eop = (byte)0x0A,
                WaitTime = WaitTime * 1000
            };
            string data = (char)0x01 + "B0" + (char)0x03 + "\r\n";
            Media.Send(data, null);
            p.Eop = "\n";
            p.AllData = true;
            p.Count = 1;

            Media.Receive(p);
        }
        /// <summary>
        /// Initialize optical head.
        /// </summary>
        void InitializeOpticalHead()
        {
            if (Client.InterfaceType != InterfaceType.HdlcWithModeE)
            {
                return;
            }
            GXSerial serial = Media as GXSerial;
            byte Terminator = (byte)0x0A;
            Media.Open();
            //Some meters need a little break.
            Thread.Sleep(1000);
            //Query device information.
            string data = "/?!\r\n";
            if (Trace > TraceLevel.Info)
            {
                Console.WriteLine("IEC Sending:" + data);
            }
            ReceiveParameters<string> p = new ReceiveParameters<string>()
            {
                AllData = false,
                Eop = Terminator,
                WaitTime = WaitTime * 1000
            };
            lock (Media.Synchronous)
            {
                Media.Send(data, null);
                if (!Media.Receive(p))
                {
                    //Try to move away from mode E.
                    try
                    {
                        Disconnect();
                    }
                    catch (Exception)
                    {
                    }
                    DiscIEC();
                    string str = "Failed to receive reply from the device in given time.";
                    if (Trace > TraceLevel.Info)
                    {
                        Console.WriteLine(str);
                    }
                    Media.Send(data, null);
                    if (!Media.Receive(p))
                    {
                        throw new Exception(str);
                    }
                }
                //If echo is used.
                if (p.Reply == data)
                {
                    p.Reply = null;
                    if (!Media.Receive(p))
                    {
                        //Try to move away from mode E.
                        GXReplyData reply = new GXReplyData();
                        Disconnect();
                        if (serial != null)
                        {
                            DiscIEC();
                            serial.DtrEnable = serial.RtsEnable = false;
                            serial.BaudRate = 9600;
                            serial.DtrEnable = serial.RtsEnable = true;
                            DiscIEC();
                        }
                        data = "Failed to receive reply from the device in given time.";
                        if (Trace > TraceLevel.Info)
                        {
                            Console.WriteLine(data);
                        }
                        throw new Exception(data);
                    }
                }
            }
            if (Trace > TraceLevel.Info)
            {
                Console.WriteLine("IEC received: " + p.Reply);
            }
            int pos = 0;
            //With some meters there might be some extra invalid chars. Remove them.
            while (pos < p.Reply.Length && p.Reply[pos] != '/')
            {
                ++pos;
            }
            if (p.Reply[pos] != '/')
            {
                p.WaitTime = 100;
                Media.Receive(p);
                DiscIEC();
                throw new Exception("Invalid responce.");
            }
            string manufactureID = p.Reply.Substring(1 + pos, 3);
            char baudrate = p.Reply[4 + pos];
            int BaudRate = 0;
            switch (baudrate)
            {
                case '0':
                    BaudRate = 300;
                    break;
                case '1':
                    BaudRate = 600;
                    break;
                case '2':
                    BaudRate = 1200;
                    break;
                case '3':
                    BaudRate = 2400;
                    break;
                case '4':
                    BaudRate = 4800;
                    break;
                case '5':
                    BaudRate = 9600;
                    break;
                case '6':
                    BaudRate = 19200;
                    break;
                default:
                    throw new Exception("Unknown baud rate.");
            }
            if (Trace > TraceLevel.Info)
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString() + "\tBaudRate is : " + BaudRate.ToString());
            }
            //Send ACK
            //Send Protocol control character
            // "2" HDLC protocol procedure (Mode E)
            byte controlCharacter = (byte)'2';
            //Send Baud rate character
            //Mode control character
            byte ModeControlCharacter = (byte)'2';
            //"2" //(HDLC protocol procedure) (Binary mode)
            //Set mode E.
            byte[] arr = new byte[] { 0x06, controlCharacter, (byte)baudrate, ModeControlCharacter, 13, 10 };
            if (Trace > TraceLevel.Info)
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString() + "\tMoving to mode E.", arr);
            }
            lock (Media.Synchronous)
            {
                p.Reply = null;
                Media.Send(arr, null);
                //Some meters need this sleep. Do not remove.
                Thread.Sleep(200);
                p.WaitTime = 2000;
                //Note! All meters do not echo this.
                Media.Receive(p);
                if (p.Reply != null)
                {
                    if (Trace > TraceLevel.Info)
                    {
                        Console.WriteLine("Received: " + p.Reply);
                    }
                }
                if (serial != null)
                {
                    Media.Close();
                    serial.BaudRate = BaudRate;
                    serial.DataBits = 8;
                    serial.Parity = Parity.None;
                    serial.StopBits = StopBits.One;
                    Media.Open();
                }
                //Some meters need this sleep. Do not remove.
                Thread.Sleep(800);
            }
        }


        /// <summary>
        /// Initialize connection to the meter.
        /// </summary>
        public void InitializeConnection()
        {
            Console.WriteLine("Standard: " + Client.Standard);
            if (Client.Ciphering.Security != Security.None)
            {
                Console.WriteLine("Security: " + Client.Ciphering.Security);
                Console.WriteLine("System title: " + GXCommon.ToHex(Client.Ciphering.SystemTitle, true));
                Console.WriteLine("Authentication key: " + GXCommon.ToHex(Client.Ciphering.AuthenticationKey, true));
                Console.WriteLine("Block cipher key " + GXCommon.ToHex(Client.Ciphering.BlockCipherKey, true));
                if (Client.Ciphering.DedicatedKey != null)
                {
                    Console.WriteLine("Dedicated key: " + GXCommon.ToHex(Client.Ciphering.DedicatedKey, true));
                }
            }
            UpdateFrameCounter();
            InitializeOpticalHead();
            GXReplyData reply = new GXReplyData();
            SNRMRequest();
            //Generate AARQ request.
            //Split requests to multiple packets if needed.
            //If password is used all data might not fit to one packet.
            foreach (byte[] it in Client.AARQRequest())
            {
                if (Trace > TraceLevel.Info)
                {
                    Console.WriteLine("Send AARQ request", GXCommon.ToHex(it, true));
                }
                reply.Clear();
                ReadDataBlock(it, reply);
            }
            if (Trace > TraceLevel.Info)
            {
                Console.WriteLine("Parsing AARE reply" + reply.ToString());
            }
            //Parse reply.
            Client.ParseAAREResponse(reply.Data);
            Console.WriteLine("Conformance: " + Client.NegotiatedConformance);
            reply.Clear();
            //Get challenge Is HLS authentication is used.
            if (Client.Authentication > Authentication.Low)
            {
                foreach (byte[] it in Client.GetApplicationAssociationRequest())
                {
                    reply.Clear();
                    ReadDataBlock(it, reply);
                }
                Client.ParseApplicationAssociationResponse(reply.Data);
            }
            if (Trace > TraceLevel.Info)
            {
                Console.WriteLine("Parsing AARE reply succeeded.");
            }
        }

        /// <summary>
        /// This method is used to update meter firmware.
        /// </summary>
        /// <param name="target">Image transfer object.</param>
        /// <param name="identification">Image identification.</param>
        /// <param name="image">Updated image.</param>
        public void ImageUpdate(GXDLMSImageTransfer target, byte[] identification, byte[] image)
        {
            //Check that image transfer is enabled.
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.Read(target, 5), reply);
            Client.UpdateValue(target, 5, reply.Value);
            if (!target.ImageTransferEnabled)
            {
                throw new Exception("Image transfer is not enabled");
            }

            //Step 1: Read image block size.
            ReadDataBlock(Client.Read(target, 2), reply);
            Client.UpdateValue(target, 2, reply.Value);

            // Step 2: Initiate the Image transfer process.
            ReadDataBlock(target.ImageTransferInitiate(Client, identification, image.Length), reply);

            // Step 3: Transfers ImageBlocks.
            int imageBlockCount;
            ReadDataBlock(target.ImageBlockTransfer(Client, image, out imageBlockCount), reply);

            //Step 4: Check the completeness of the Image.
            ReadDataBlock(Client.Read(target, 3), reply);
            Client.UpdateValue(target, 3, reply.Value);

            // Step 5: The Image is verified;
            ReadDataBlock(target.ImageVerify(Client), reply);
            // Step 6: Before activation, the Image is checked;

            //Get list to images to activate.
            ReadDataBlock(Client.Read(target, 7), reply);
            Client.UpdateValue(target, 7, reply.Value);
            bool bFound = false;
            foreach (GXDLMSImageActivateInfo it in target.ImageActivateInfo)
            {
                if (GXCommon.EqualBytes(it.Identification, identification))
                {
                    bFound = true;
                    break;
                }
            }

            //Read image transfer status.
            ReadDataBlock(Client.Read(target, 6), reply);
            Client.UpdateValue(target, 6, reply.Value);
            if (target.ImageTransferStatus != Gurux.DLMS.Objects.Enums.ImageTransferStatus.VerificationSuccessful)
            {
                throw new Exception("Image transfer status is " + target.ImageTransferStatus.ToString());
            }

            if (!bFound)
            {
                throw new Exception("Image not found.");
            }

            //Step 7: Activate image.
            ReadDataBlock(target.ImageActivate(Client), reply);
        }
        /// <summary>
        /// Read association view.
        /// </summary>
        public bool GetAssociationView(string outputFile)
        {
            if (outputFile != null)
            {
                //Save Association view to the cache so it is not needed to retrieve every time.
                if (File.Exists(outputFile))
                {
                    try
                    {
                        Client.Objects.Clear();
                        Client.Objects.AddRange(GXDLMSObjectCollection.Load(outputFile));
                        return false;
                    }
                    catch (Exception)
                    {
                        if (File.Exists(outputFile))
                        {
                            File.Delete(outputFile);
                        }
                    }
                }
            }
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.GetObjectsRequest(), reply);
            Client.ParseObjects(reply.Data, true);
            //Access rights must read differently when short Name referencing is used.
            if (!Client.UseLogicalNameReferencing)
            {
                GXDLMSAssociationShortName sn = (GXDLMSAssociationShortName)Client.Objects.FindBySN(0xFA00);
                if (sn != null && sn.Version > 0)
                {
                    Read(sn, 3);
                }
            }
            if (outputFile != null)
            {
                try
                {
                    Client.Objects.Save(outputFile, new GXXmlWriterSettings() { Values = false });
                }
                catch (Exception)
                {
                    //It's OK if this fails.
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Read scalers and units.
        /// </summary>
        public void GetScalersAndUnits()
        {
            GXDLMSObjectCollection objs = Client.Objects.GetObjects(new ObjectType[] { ObjectType.Register, ObjectType.ExtendedRegister, ObjectType.DemandRegister });
            //If trace is info.
            if (Trace > TraceLevel.Warning)
            {
                Console.WriteLine("Read scalers and units from the device.");
            }
            //Access services are available only for general protection.
            if ((Client.NegotiatedConformance & Conformance.Access) != 0 &&
                (Client.Ciphering.Security == Security.None ||
                (Client.NegotiatedConformance & Conformance.GeneralProtection) != 0))
            {
                List<GXDLMSAccessItem> list = new List<GXDLMSAccessItem>();
                foreach (GXDLMSObject it in objs)
                {
                    if ((it is GXDLMSRegister || it is GXDLMSExtendedRegister) && Client.CanRead(it, 3))
                    {
                        list.Add(new GXDLMSAccessItem(AccessServiceCommandType.Get, it, 3));
                    }
                    else if (it is GXDLMSDemandRegister && Client.CanRead(it, 4))
                    {
                        list.Add(new GXDLMSAccessItem(AccessServiceCommandType.Get, it, 4));
                    }
                }
                ReadByAccess(list);
            }
            else if ((Client.NegotiatedConformance & Gurux.DLMS.Enums.Conformance.MultipleReferences) != 0)
            {
                List<KeyValuePair<GXDLMSObject, int>> list = new List<KeyValuePair<GXDLMSObject, int>>();
                foreach (GXDLMSObject it in objs)
                {
                    if ((it is GXDLMSRegister || it is GXDLMSExtendedRegister) && Client.CanRead(it, 3))
                    {
                        list.Add(new KeyValuePair<GXDLMSObject, int>(it, 3));
                    }
                    if (it is GXDLMSDemandRegister && Client.CanRead(it, 4))
                    {
                        list.Add(new KeyValuePair<GXDLMSObject, int>(it, 4));
                    }
                }
                if (list.Count != 0)
                {
                    try
                    {
                        ReadList(list);
                    }
                    catch (Exception)
                    {
                        Client.NegotiatedConformance &= ~Gurux.DLMS.Enums.Conformance.MultipleReferences;
                    }
                }
            }
            if ((Client.NegotiatedConformance & Gurux.DLMS.Enums.Conformance.MultipleReferences) == 0)
            {
                //Read values one by one.
                foreach (GXDLMSObject it in objs)
                {
                    try
                    {
                        if (it is GXDLMSRegister && Client.CanRead(it, 3))
                        {
                            Console.WriteLine(it.Name);
                            Read(it, 3);
                        }
                        if (it is GXDLMSDemandRegister && Client.CanRead(it, 4))
                        {
                            Console.WriteLine(it.Name);
                            Read(it, 4);
                        }
                    }
                    catch
                    {
                        //Actaric SL7000 can return error here. Continue reading.
                    }
                }
            }
        }

        /// <summary>
        /// Read profile generic columns.
        /// </summary>
        public void GetProfileGenericColumns()
        {
            //Read Profile Generic columns first.
            foreach (GXDLMSObject it in Client.Objects.GetObjects(ObjectType.ProfileGeneric))
            {
                try
                {
                    //If info.
                    if (Trace > TraceLevel.Warning)
                    {
                        Console.WriteLine(it.LogicalName);
                    }
                    Read(it, 3);
                    //If info.
                    if (Trace > TraceLevel.Warning)
                    {
                        GXDLMSObject[] cols = (it as GXDLMSProfileGeneric).GetCaptureObject();
                        StringBuilder sb = new StringBuilder();
                        bool First = true;
                        foreach (GXDLMSObject col in cols)
                        {
                            if (!First)
                            {
                                sb.Append(" | ");
                            }
                            First = false;
                            sb.Append(col.Name);
                            sb.Append(" ");
                            sb.Append(col.Description);
                        }
                        Console.WriteLine(sb.ToString());
                    }
                }
                catch (Exception)
                {
                    //Continue reading.
                }
            }
        }

        public void ShowValue(object val, int pos)
        {
            //If trace is info.
            if (Trace > TraceLevel.Warning)
            {
                //If data is array.
                if (val is byte[])
                {
                    val = GXCommon.ToHex((byte[])val, true);
                }
                else if (val is Array)
                {
                    string str = "";
                    for (int pos2 = 0; pos2 != (val as Array).Length; ++pos2)
                    {
                        if (str != "")
                        {
                            str += ", ";
                        }
                        if ((val as Array).GetValue(pos2) is byte[])
                        {
                            str += GXCommon.ToHex((byte[])(val as Array).GetValue(pos2), true);
                        }
                        else
                        {
                            str += (val as Array).GetValue(pos2).ToString();
                        }
                    }
                    val = str;
                }
                else if (val is System.Collections.IList)
                {
                    string str = "[";
                    bool empty = true;
                    foreach (object it2 in val as System.Collections.IList)
                    {
                        if (!empty)
                        {
                            str += ", ";
                        }
                        empty = false;
                        if (it2 is byte[])
                        {
                            str += GXCommon.ToHex((byte[])it2, true);
                        }
                        else
                        {
                            str += it2.ToString();
                        }
                    }
                    str += "]";
                    val = str;
                }
                Console.WriteLine("Index: " + pos + " Value: " + val);
            }
        }

        public void GetProfileGenerics()
        {
            //Find profile generics register objects and read them.
            foreach (GXDLMSObject it in Client.Objects.GetObjects(ObjectType.ProfileGeneric))
            {
                //If trace is info.
                if (Trace > TraceLevel.Warning)
                {
                    Console.WriteLine("-------- Reading " + it.GetType().Name + " " + it.Name + " " + it.Description);
                }
                long entriesInUse = -1;
                if ((it.GetAccess(7) & AccessMode.Read) != 0)
                {
                    entriesInUse = Convert.ToInt64(Read(it, 7));
                }
                long entries = -1;
                if ((it.GetAccess(8) & AccessMode.Read) != 0)
                {
                    entries = Convert.ToInt64(Read(it, 8));
                }
                //If trace is info.
                if (Trace > TraceLevel.Warning)
                {
                    Console.WriteLine("Entries: " + entriesInUse + "/" + entries);
                }
                //If there are no columns or rows.
                if (entriesInUse == 0 || (it as GXDLMSProfileGeneric).CaptureObjects.Count == 0)
                {
                    continue;
                }
                //All meters are not supporting parameterized read.
                if ((Client.NegotiatedConformance & (Gurux.DLMS.Enums.Conformance.ParameterizedAccess | Gurux.DLMS.Enums.Conformance.SelectiveAccess)) != 0)
                {
                    try
                    {
                        //Read first row from Profile Generic.
                        object[] rows = ReadRowsByEntry(it as GXDLMSProfileGeneric, 1, 1);
                        //If trace is info.
                        if (Trace > TraceLevel.Warning)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (object[] row in rows)
                            {
                                foreach (object cell in row)
                                {
                                    if (cell is byte[])
                                    {
                                        sb.Append(GXCommon.ToHex((byte[])cell, true));
                                    }
                                    else
                                    {
                                        sb.Append(Convert.ToString(cell));
                                    }
                                    sb.Append(" | ");
                                }
                                sb.Append("\r\n");
                            }
                            Console.WriteLine(sb.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error! Failed to read first row: " + ex.Message);
                        //Continue reading.
                    }
                }
                //All meters are not supporting parameterized read.
                if ((Client.NegotiatedConformance & (Gurux.DLMS.Enums.Conformance.ParameterizedAccess | Gurux.DLMS.Enums.Conformance.SelectiveAccess)) != 0)
                {
                    try
                    {
                        //Read last day from Profile Generic.

                        object[] rows = ReadRowsByRange(it as GXDLMSProfileGeneric, DateTime.Now.Date, DateTime.MaxValue);
                        //If trace is info.
                        if (Trace > TraceLevel.Warning)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (object[] row in rows)
                            {
                                foreach (object cell in row)
                                {
                                    if (cell is byte[])
                                    {
                                        sb.Append(GXCommon.ToHex((byte[])cell, true));
                                    }
                                    else
                                    {
                                        sb.Append(Convert.ToString(cell));
                                    }
                                    sb.Append(" | ");
                                }
                                sb.Append("\r\n");
                            }
                            Console.WriteLine(sb.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error! Failed to read last day: " + ex.Message);
                        //Continue reading.
                    }
                }
            }
        }

        /// <summary>
        /// Read all objects from the meter.
        /// </summary>
        /// <remarks>
        /// It's not normal to read all data from the meter. This is just an example.
        /// </remarks>
        public void GetReadOut()
        {
            foreach (GXDLMSObject it in Client.Objects)
            {
                // Profile generics are read later because they are special cases.
                // (There might be so lots of data and we so not want waste time to read all the data.)
                if (it is GXDLMSProfileGeneric)
                {
                    continue;
                }
                if (!(it is IGXDLMSBase))
                {
                    //If interface is not implemented.
                    //Example manufacturer spesific interface.
                    if (Trace > TraceLevel.Error)
                    {
                        Console.WriteLine("Unknown Interface: " + it.ObjectType.ToString());
                    }
                    continue;
                }
                if (Trace > TraceLevel.Warning)
                {
                    Console.WriteLine("-------- Reading " + it.GetType().Name + " " + it.Name + " " + it.Description);
                }
                foreach (int pos in (it as IGXDLMSBase).GetAttributeIndexToRead(true))
                {
                    try
                    {
                        object val = Read(it, pos);
                        ShowValue(val, pos);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error! " + it.GetType().Name + " " + it.Name + "Index: " + pos + " " + ex.Message);
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Read DLMS Data from the device.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <returns>Received data.</returns>
        public void ReadDLMSPacket(byte[] data, GXReplyData reply)
        {
            if (data == null && !reply.IsStreaming())
            {
                return;
            }
            GXReplyData notify = new GXReplyData();
            reply.Error = 0;
            object eop = (byte)0x7E;
            //In network connection terminator is not used.
            if (Client.InterfaceType != InterfaceType.HDLC &&
                Client.InterfaceType != InterfaceType.HdlcWithModeE)
            {
                eop = null;
            }
            int pos = 0;
            bool succeeded = false;
            GXByteBuffer rd = new GXByteBuffer();
            ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
            {
                Eop = eop,
                Count = Client.GetFrameSize(rd),
                AllData = true,
                WaitTime = WaitTime,
            };
            lock (Media.Synchronous)
            {
                while (!succeeded && pos != 3)
                {
                    if (!reply.IsStreaming())
                    {
                        WriteTrace("TX:\t" + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(data, true));
                        p.Reply = null;
                        Media.Send(data, null);
                    }
                    succeeded = Media.Receive(p);
                    if (!succeeded)
                    {
                        if (++pos >= RetryCount)
                        {
                            throw new Exception("Failed to receive reply from the device in given time.");
                        }
                        //If Eop is not set read one byte at time.
                        if (p.Eop == null)
                        {
                            p.Count = 1;
                        }
                        //Try to read again...
                        System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                    }
                }
                rd = new GXByteBuffer(p.Reply);
                try
                {
                    pos = 0;
                    //Loop until whole COSEM packet is received.
                    while (!Client.GetData(rd, reply, notify))
                    {
                        p.Reply = null;
                        if (notify.IsComplete && notify.Data.Data != null)
                        {
                            //Handle notify.
                            if (!notify.IsMoreData)
                            {
                                if (notify.PrimeDc != null)
                                {
                                    OnNotification?.Invoke(notify.PrimeDc);
                                    Console.WriteLine(notify.PrimeDc);
                                }
                                else
                                {
                                    //Show received push message as XML.
                                    string xml;
                                    GXDLMSTranslator t = new GXDLMSTranslator(TranslatorOutputType.SimpleXml);
                                    t.DataToXml(notify.Data, out xml);
                                    OnNotification?.Invoke(xml);
                                    Console.WriteLine(xml);
                                }
                                notify.Clear();
                                continue;
                            }
                        }
                        if (p.Eop == null)
                        {
                            p.Count = Client.GetFrameSize(rd);
                        }
                        while (!Media.Receive(p))
                        {
                            if (++pos >= RetryCount)
                            {
                                throw new Exception("Failed to receive reply from the device in given time.");
                            }
                            p.Reply = null;
                            Media.Send(data, null);
                            //Try to read again...
                            System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                        }
                        rd.Set(p.Reply);
                    }
                }
                catch (Exception ex)
                {
                    WriteTrace("RX:\t" + DateTime.Now.ToLongTimeString() + "\t" + rd);
                    throw ex;
                }
            }
            WriteTrace("RX:\t" + DateTime.Now.ToLongTimeString() + "\t" + rd);
            if (reply.Error != 0)
            {
                if (reply.Error == (short)ErrorCode.Rejected)
                {
                    Thread.Sleep(1000);
                    ReadDLMSPacket(data, reply);
                }
                else
                {
                    throw new GXDLMSException(reply.Error);
                }
            }
        }

        /// <summary>
        /// Send data block(s) to the meter.
        /// </summary>
        /// <param name="data">Send data block(s).</param>
        /// <param name="reply">Received reply from the meter.</param>
        /// <returns>Return false if frame is rejected.</returns>
        public bool ReadDataBlock(byte[][] data, GXReplyData reply)
        {
            if (data == null)
            {
                return true;
            }
            foreach (byte[] it in data)
            {
                reply.Clear();
                ReadDataBlock(it, reply);
            }
            return reply.Error == 0;
        }

        /// <summary>
        /// Read data block from the device.
        /// </summary>
        /// <param name="data">data to send</param>
        /// <param name="text">Progress text.</param>
        /// <param name="multiplier"></param>
        /// <returns>Received data.</returns>
        public void ReadDataBlock(byte[] data, GXReplyData reply)
        {
            ReadDLMSPacket(data, reply);
            lock (Media.Synchronous)
            {
                while (reply.IsMoreData &&
                    (Client.ConnectionState != ConnectionState.None ||
                    Client.PreEstablishedConnection))
                {
                    if (reply.IsStreaming())
                    {
                        data = null;
                    }
                    else
                    {
                        data = Client.ReceiverReady(reply);
                    }
                    ReadDLMSPacket(data, reply);
                }
            }
        }

        /// <summary>
        /// Read attribute value.
        /// </summary>
        /// <param name="it">COSEM object to read.</param>
        /// <param name="attributeIndex">Attribute index.</param>
        /// <returns>Read value.</returns>
        public object Read(GXDLMSObject it, int attributeIndex)
        {
            if (Client.CanRead(it, attributeIndex))
            {
                GXReplyData reply = new GXReplyData();
                if (!ReadDataBlock(Client.Read(it, attributeIndex), reply))
                {
                    if (reply.Error != (short)ErrorCode.Rejected)
                    {
                        throw new GXDLMSException(reply.Error);
                    }
                    reply.Clear();
                    Thread.Sleep(1000);
                    if (!ReadDataBlock(Client.Read(it, attributeIndex), reply))
                    {
                        throw new GXDLMSException(reply.Error);
                    }
                }
                //Update data type.
                if (it.GetDataType(attributeIndex) == DataType.None)
                {
                    it.SetDataType(attributeIndex, reply.DataType);
                }
                return Client.UpdateValue(it, attributeIndex, reply.Value);
            }
            else
            {
                Console.WriteLine("Can't read " + it.ToString() + ". Not enought acccess rights.");
            }
            return null;
        }

        /// <summary>
        /// Read list of attributes.
        /// </summary>
        public void ReadList(List<KeyValuePair<GXDLMSObject, int>> list)
        {
            byte[][] data = Client.ReadList(list);
            GXReplyData reply = new GXReplyData();
            List<object> values = new List<object>();
            foreach (byte[] it in data)
            {
                ReadDataBlock(it, reply);
                if (!reply.IsMoreData)
                {
                    //Value is null if data is send in multiple frames.
                    if (reply.Value is IEnumerable<object>)
                    {
                        values.AddRange((IEnumerable<object>)reply.Value);
                    }
                }
                reply.Clear();
            }
            if (values.Count != list.Count)
            {
                throw new Exception("Invalid reply. Read items count do not match.");
            }
            Client.UpdateValues(list, values);
        }

        /// <summary>
        /// Write list of attributes.
        /// </summary>
        public void WriteList(List<KeyValuePair<GXDLMSObject, int>> list)
        {
            byte[][] data = Client.WriteList(list);
            GXReplyData reply = new GXReplyData();
            foreach (byte[] it in data)
            {
                ReadDataBlock(it, reply);
                reply.Clear();
            }
        }

        /// <summary>
        /// Write attribute value.
        /// </summary>
        public void Write(GXDLMSObject it, int attributeIndex)
        {
            if (Client.CanWrite(it, attributeIndex))
            {
                GXReplyData reply = new GXReplyData();
                ReadDataBlock(Client.Write(it, attributeIndex), reply);
            }
        }

        /// <summary>
        /// Method attribute value.
        /// </summary>
        public void Method(GXDLMSObject it, int attributeIndex, object value, DataType type)
        {
            if (Client.CanInvoke(it, attributeIndex))
            {
                GXReplyData reply = new GXReplyData();
                ReadDataBlock(Client.Method(it, attributeIndex, value, type), reply);
            }
        }

        /// <summary>
        /// Read Profile Generic Columns by entry.
        /// </summary>
        public object[] ReadRowsByEntry(GXDLMSProfileGeneric it, UInt32 index, UInt32 count)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.ReadRowsByEntry(it, index, count), reply);
            return (object[])Client.UpdateValue(it, 2, reply.Value);
        }

        /// <summary>
        /// Read Profile Generic Columns by range.
        /// </summary>
        public object[] ReadRowsByRange(GXDLMSProfileGeneric it, DateTime start, DateTime end)
        {
            GXReplyData reply = new GXReplyData();
            ReadDataBlock(Client.ReadRowsByRange(it, start, end), reply);
            return (object[])Client.UpdateValue(it, 2, reply.Value);
        }

        /// <summary>
        /// Disconnect.
        /// </summary>
        public void Disconnect()
        {
            if (Media != null && Client != null)
            {
                try
                {
                    if (Trace > TraceLevel.Info)
                    {
                        Console.WriteLine("Disconnecting from the meter.");
                    }
                    try
                    {
                        Release();
                    }
                    catch (Exception)
                    {
                        //All meters don't support release.
                    }
                    GXReplyData reply = new GXReplyData();
                    ReadDLMSPacket(Client.DisconnectRequest(), reply);
                }
                catch
                {
                    //All meters don't support release.
                }
            }
        }

        /// <summary>
        /// Release.
        /// </summary>
        public void Release()
        {
            if (Media != null && Client != null)
            {
                try
                {
                    if (Trace > TraceLevel.Info)
                    {
                        Console.WriteLine("Release from the meter.");
                    }
                    //Release is call only for secured connections.
                    //All meters are not supporting Release and it's causing problems.
                    if (Client.InterfaceType == InterfaceType.WRAPPER ||
                        (Client.Ciphering.Security != (byte)Security.None &&
                        !Client.PreEstablishedConnection))
                    {
                        GXReplyData reply = new GXReplyData();
                        ReadDataBlock(Client.ReleaseRequest(), reply);
                    }
                }
                catch (Exception ex)
                {
                    //All meters don't support Release.
                    Console.WriteLine("Release failed. " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Close connection to the meter.
        /// </summary>
        public void Close()
        {
            if (Media != null && Client != null)
            {
                try
                {
                    if (Trace > TraceLevel.Info)
                    {
                        Console.WriteLine("Disconnecting from the meter.");
                    }
                    try
                    {
                        Release();
                    }
                    catch (Exception)
                    {
                    }
                    GXReplyData reply = new GXReplyData();
                    ReadDLMSPacket(Client.DisconnectRequest(), reply);
                }
                catch
                {

                }
                Media.Close();
                Media = null;
                Client = null;
            }
        }

        /// <summary>
        /// Write trace.
        /// </summary>
        /// <param name="line"></param>
        void WriteTrace(string line)
        {
            if (Trace > TraceLevel.Info)
            {
                Console.WriteLine(line);
            }
            using (FileStream fs = File.Open("trace.txt", FileMode.Append))
            {
                using (TextWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Read values using Access request.
        /// </summary>
        /// <param name="list">Object to read.</param>
        void ReadByAccess(List<GXDLMSAccessItem> list)
        {
            if (list.Count != 0)
            {
                GXReplyData reply = new GXReplyData();
                byte[][] data = Client.AccessRequest(DateTime.MinValue, list);
                ReadDataBlock(data, reply);
                Client.ParseAccessResponse(list, reply.Data);
            }
        }
    }
}
