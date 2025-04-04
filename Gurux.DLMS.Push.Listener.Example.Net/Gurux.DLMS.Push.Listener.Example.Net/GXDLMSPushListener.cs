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
using System;
using Gurux.DLMS;
using Gurux.Net;
using Gurux.DLMS.Secure;
using Gurux.DLMS.Enums;
using System.Diagnostics;
using Gurux.Common;
using System.Text;
using System.Collections.Generic;
using Gurux.DLMS.Objects;

namespace GuruxDLMSServerExample
{
    /// <summary>
    /// All example servers are using same objects.
    /// </summary>
    class GXDLMSPushListener
    {
        /// <summary>
        /// Are messages traced.
        /// </summary>
        private bool trace = true;


        /// <summary>
        ///  TCP/IP port to listen.
        /// </summary>
        private GXNet media;
        /// <summary>
        /// Received data is saved to reply buffer because whole message is not always received in one packet.
        /// </summary>
        private GXByteBuffer reply = new GXByteBuffer();

        /// <summary>
        /// Received data. This is used if GBT is used and data is received on several data blocks.
        /// </summary>
        private GXReplyData notify = new GXReplyData();

        /// <summary>
        /// Client used to parse received data.
        /// </summary>
        private readonly GXDLMSSecureClient client;

        GXDLMSPushSetup push = new GXDLMSPushSetup();

        /// <summary>
        /// Get certificate keys from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnGetKeys(object sender, GXCryptoKeyParameter args)
        {
            if ((args.KeyType & CryptoKeyType.Authentication) != 0)
            {
                args.AuthenticationKey = GXCommon.HexToBytes("METER_AUTHENTICATION_KEY");
            }
            if ((args.KeyType & CryptoKeyType.BlockCipher) != 0)
            {
                args.BlockCipherKey = GXCommon.HexToBytes("METER_BLOCK_CIPHER_KEY");
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="port">Port to listen.</param>
        public GXDLMSPushListener(int port, InterfaceType interfaceType)
        {
            client = new GXDLMSSecureClient(true, 0, 0, Authentication.None, null, interfaceType);
            // TODO: Reset keys so they are asked with OnKeys.
            // If keys are set, they are used and OnKeys is not invoked.
            // client.Ciphering.BlockCipherKey = null;
            // client.Ciphering.AuthenticationKey = null;
            client.OnKeys += OnGetKeys;
            // TODO: Must set communication specific settings.
            media = new GXNet(interfaceType == InterfaceType.CoAP ? NetworkType.Udp : NetworkType.Tcp, port);
            media.Trace = TraceLevel.Verbose;
            media.OnReceived += new ReceivedEventHandler(OnReceived);
            media.OnClientConnected += new ClientConnectedEventHandler(OnClientConnected);
            media.OnClientDisconnected += new ClientDisconnectedEventHandler(OnClientDisconnected);
            media.OnError += new ErrorEventHandler(OnError);
            media.Open();
            //Comment this if the meter describes the content of the push message for the client in the received data.
            push.PushObjectList.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(new GXDLMSClock(), new GXDLMSCaptureObject(2, 0)));
        }

        void OnError(object sender, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }

        /// <summary>
        /// Client has close connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClientDisconnected(object sender, Gurux.Common.ConnectionEventArgs e)
        {
            Console.WriteLine("Client Disconnected.");
        }

        /// <summary>
        /// Client has made connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClientConnected(object sender, Gurux.Common.ConnectionEventArgs e)
        {
            Console.WriteLine("Client Connected.");
        }

        private static void PrintData(Object value, int offset)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(' ', 2 * offset);
            if (value is List<object>)
            {
                Console.WriteLine(sb + "{");
                ++offset;
                // Print received data.
                foreach (object it in (List<object>)value)
                {
                    PrintData(it, offset);
                }
                Console.WriteLine(sb + "}");
                --offset;
            }
            else if (value is byte[])
            {
                // Print value.
                Console.WriteLine(sb + GXCommon.ToHex((byte[])value, true));
            }
            else
            {
                // Print value.
                Console.WriteLine(sb + Convert.ToString(value));
            }
        }

        /// <summary>
        /// Client has send data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnReceived(object sender, ReceiveEventArgs e)
        {
            try
            {
                lock (this)
                {
                    if (trace)
                    {
                        Console.WriteLine("<- " + Gurux.Common.GXCommon.ToHex((byte[])e.Data, true));
                    }
                    reply.Set((byte[])e.Data);
                    //Example handles only notify messages.
                    client.GetData(reply, notify);
                    // If all data is received.
                    if (notify.IsComplete)
                    {
                        //Don't call clear here.
                        //There might be bytes from the next frame waiting.
                        reply.Trim();
                        if (notify.IsMoreData)
                        {
                            if (client.InterfaceType == InterfaceType.CoAP)
                            {
                                //Send ACK for the meter.
                                byte[] data = client.ReceiverReady(notify);
                                media.Send(data, e.SenderInfo);
                            }
                        }
                        else
                        {
                            try
                            {
                                // Make clone so we don't replace current values.
                                GXDLMSPushSetup clone = (GXDLMSPushSetup)push.Clone();
                                clone.GetPushValues(client, (List<object>)notify.Value);
                                //Comment this if the meter describes the content of the push message for the client in the received data.
                                foreach (var it in clone.PushObjectList)
                                {
                                    int index = it.Value.AttributeIndex - 1;
                                    Console.WriteLine(((IGXDLMSBase)it.Key).GetNames()[index] + ": " + it.Key.GetValues()[index]);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            try
                            {
                                //Show data as XML.
                                string xml;
                                GXDLMSTranslator t = new GXDLMSTranslator(TranslatorOutputType.SimpleXml);
                                t.DataToXml(notify.Data, out xml);
                                Console.WriteLine(xml);

                                // Print received data.
                                PrintData(notify.Value, 0);

                                //Un-comment this if the meter describes the content of the push message for the client in the received data.
                                /*
                                if (notify.Value is List<object>)
                                {
                                    List<object> tmp = notify.Value as List<object>;
                                    List<KeyValuePair<GXDLMSObject, int>> objects = client.ParsePushObjects((List<object>)tmp[0]);
                                    //Remove first item because it's not needed anymore.
                                    objects.RemoveAt(0);
                                    //Update clock.
                                    int Valueindex = 1;
                                    foreach (KeyValuePair<GXDLMSObject, int> it in objects)
                                    {
                                        client.UpdateValue(it.Key, it.Value, tmp[Valueindex]);
                                        ++Valueindex;
                                        //Print value
                                        Console.WriteLine(it.Key.ObjectType + " " + it.Key.LogicalName + " " + it.Value + ":" + it.Key.GetValues()[it.Value - 1]);
                                    }
                                }
                                */
                                Console.WriteLine("Server address:" + notify.ServerAddress + " Client Address:" + notify.ClientAddress);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            finally
                            {
                                notify.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
