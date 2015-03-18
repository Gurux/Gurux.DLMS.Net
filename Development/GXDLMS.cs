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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Internal;
using System.Security.Cryptography;
using System.IO;

namespace Gurux.DLMS
{            
    /// <summary>
    /// GXDLMS implements methods to communicate with DLMS/COSEM metering devices.
    /// </summary>
    class GXDLMS
    {
        object m_ClientID;
        object m_ServerID;
        byte[] ServerBuff;
        byte[] ClientBuff;
        internal byte[] CtoSChallenge;
        internal byte[] StoCChallenge;
        internal GXCiphering Ciphering;

        uint PacketIndex;
        /// <summary>
        /// HDLC sequence number.
        /// </summary>
        internal int ReceiveSequenceNo = -1, SendSequenceNo = -1;
        bool Segmented = false;

        bool bIsLastMsgKeepAliveMsg;
        internal int ExpectedFrame, FrameSequence;
        //Cached data.
        internal object CacheData;
        //Cached data type.
        internal DataType CacheType;
        //Index where last item found.
        internal int CacheIndex;
        //Cache Size
        internal int CacheSize;
        internal int ItemCount;
        internal int MaxItemCount;
        byte m_InvokeID;
        internal static Dictionary<Gurux.DLMS.ObjectType, Type> AvailableObjectTypes = new Dictionary<Gurux.DLMS.ObjectType, Type>();

        public Authentication Authentication
        {
            get;
            set;
        }

        internal byte GetInvokeIDPriority()
        {
            byte value = 0;
            if (Priority == Priority.High)
            {
                value |= 0x80;
            }
            if (ServiceClass == ServiceClass.Confirmed)
            {
                value |= 0x40;
            }
            value |= m_InvokeID;
            return value;
        }

        /// <summary>
        /// Used priority.
        /// </summary>
        public Priority Priority
        {
            get;
            set;
        }

        /// <summary>
        /// Used service class.
        /// </summary>
        public ServiceClass ServiceClass
        {
            get;
            set;
        }

        /// <summary>
        /// Invoke ID.
        /// </summary>
        public byte InvokeID
        {
            get
            {
                return m_InvokeID;
            }
            set
            {
                if (value > 0xF)
                {
                    throw new ArgumentOutOfRangeException("Invalid InvokeID");
                }
                m_InvokeID = value;
            }
        }

        public static GXDLMSObject CreateObject(Gurux.DLMS.ObjectType type)
        {
            lock (AvailableObjectTypes)
            {
                //Update objects.
                if (AvailableObjectTypes.Count == 0)
                {
                    GetObjectTypes();
                }
                if (AvailableObjectTypes.ContainsKey(type))
                {
                    return Activator.CreateInstance(AvailableObjectTypes[type]) as GXDLMSObject;
                }
            }
            GXDLMSObject obj = new GXDLMSObject();
            obj.ObjectType = type;
            return obj;
        }

        static public byte[] Chipher(Authentication auth, byte[] plainText, byte[] secret)
        {
            if (auth == Authentication.High)
            {
                byte[] p = new byte[plainText.Length];
                byte[] s;
                if (secret.Length < 16)
                {
                    s = new byte[16];
                }
                else
                {
                    s = new byte[secret.Length];
                }
                plainText.CopyTo(p, 0);
                secret.CopyTo(s, 0);
                GXAes128.Encrypt(p, s);
                return plainText;
            }
            if (auth == Authentication.HighMD5)
            {
                using (MD5 md5Hash = MD5.Create())
                {                    
                    return md5Hash.ComputeHash(plainText);
                }
            }
            if (auth == Authentication.HighSHA1)
            {
                using (SHA1 sha = new SHA1CryptoServiceProvider())
                {                    
                    return sha.ComputeHash(plainText);
                }
            }
            if (auth == Authentication.HighGMAC)
            {
                GXDLMSChipperingStream tmp = new GXDLMSChipperingStream(Security.Authentication, true, plainText, plainText, null, null);
                tmp.Write(plainText);
                return tmp.FlushFinalBlock();
            }
            return plainText;
        }

        /// <summary>
        /// Generate challenge for the meter.
        /// </summary>
        /// <returns>Generated challenge.</returns>
        static public byte[] GenerateChallenge()        
        {
            Random rnd = new Random();
            // Random challenge is 8 to 64 bytes.
            int len = rnd.Next(8, 64);
            byte[] result = new byte[len];
            for (int pos = 0; pos != len; ++pos)
            {
                // Allow printable characters only.
                result[pos] = (byte)rnd.Next(0x21, 0x7A);                
            }
            return result;
        }

        /// <summary>
        /// Returns object types.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This can be used with serialization.
        /// </remarks>
        public static Type[] GetObjectTypes()
        {
            lock (AvailableObjectTypes)
            {
                if (AvailableObjectTypes.Count == 0)
                {
                    foreach (Type type in typeof(GXDLMS).Assembly.GetTypes())
                    {
                        if (!type.IsAbstract && typeof(GXDLMSObject).IsAssignableFrom(type))
                        {
                            GXDLMSObject obj = Activator.CreateInstance(type) as GXDLMSObject;
                            AvailableObjectTypes[obj.ObjectType] = type;
                        }
                    }
                }
                return AvailableObjectTypes.Values.ToArray();
            }            
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMS(bool server)
        {
            if (server)
            {
                SendSequenceNo = -1;
                ReceiveSequenceNo = 0;
            }
            else
            {
                ReceiveSequenceNo = SendSequenceNo = -1;
            }
            Priority = Priority.High;
            InvokeID = 1;
            Ciphering = new GXCiphering(ASCIIEncoding.ASCII.GetBytes("ABCDEFGH"));
            Server = server;
            UseCache = true;
            this.InterfaceType = InterfaceType.General;
            DLMSVersion = 6;
            this.MaxReceivePDUSize = 0xFFFF;
            ClearProgress();
            GenerateFrame = true;
            Limits = new GXDLMSLimits();
            GetObjectTypes();
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        internal void ClearProgress()
        {
            CacheIndex = CacheSize = ItemCount = MaxItemCount = 0;
            CacheData = null;
            CacheType = DataType.None;
        }

        /// <summary>
        /// Client ID is the identification of the device that is used as a client.
        /// Client ID is aka HDLC Address.
        /// </summary>
        public object ClientID
        {
            get
            {
                return m_ClientID;
            }
            set
            {
                if (m_ClientID != value)
                {
                    m_ClientID = value;
                    ClientBuff = GetAddress(m_ClientID);
                }
            }
        }

        /// <summary>
        /// Server ID is the indentification of the device that is used as a server.
        /// Server ID is aka HDLC Address.
        /// </summary>
        public object ServerID
        {
            get
            {
                return m_ServerID;
            }
            set
            {
                if (m_ServerID != value)
                {
                    m_ServerID = value;
                    ServerBuff = GetAddress(m_ServerID);
                }
            }
        }

        public bool GenerateFrame
        {
            get;
            set;
        }

        /// <summary>
        /// Is cache used. Default value is True;
        /// </summary>
        public bool UseCache
        {
            get;
            set;
        }

        [DefaultValue(6)]
        internal byte DLMSVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Retrieves the maximum size of PDU receiver.
        /// </summary>
        /// <remarks>
        /// PDU size tells maximum size of PDU packet.
        /// Value can be from 0 to 0xFFFF. By default the value is 0xFFFF.
        /// </remarks>
        /// <seealso cref="ClientID"/>
        /// <seealso cref="ServerID"/>
        /// <seealso cref="DLMSVersion"/>
        /// <seealso cref="UseLogicalNameReferencing"/>
        /// <seealso cref="Authentication"/>    
        [DefaultValue(0xFFFF)]
        public ushort MaxReceivePDUSize
        {
            get;
            set;
        }

        /// <summary>
        /// Determines, whether Logical, or Short name, referencing is used.     
        /// </summary>
        /// <remarks>
        /// Referencing depends on the device to communicate with.
        /// Normally, a device supports only either Logical or Short name referencing.
        /// The referencing is defined by the device manufacturer.
        /// If the referencing is wrong, the SNMR message will fail.
        /// </remarks>
        /// <seealso cref="ClientID"/>
        /// <seealso cref="ServerID"/>
        /// <seealso cref="DLMSVersion"/>
        /// <seealso cref="Authentication"/>
        /// <seealso cref="MaxReceivePDUSize"/>
        [DefaultValue(false)]
        public bool UseLogicalNameReferencing
        {
            get;
            set;
        }              
        
        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        internal byte[][] GenerateMessage(object name, byte[] data, ObjectType interfaceClass, int AttributeOrdinal, Command cmd)
        {
            if (Limits.MaxInfoRX == null)
            {
                throw new GXDLMSException("Invalid argument.");
            }
            bool asList = false;
            List<byte> buff = null;
            if (name is byte[])
            {
                buff = new List<byte>((byte[])name);
            }
            else
            {                
                if (name == null)
                {
                    buff = new List<byte>(data);
                }
                else if (UseLogicalNameReferencing)
                {
                    int len = data == null ? 0 : data.Length;
                    buff = new List<byte>(20 + len);                     
                    if (cmd == Command.GetRequest || cmd == Command.SetRequest || cmd == Command.MethodRequest)
                    {
                        if (name is List<KeyValuePair<GXDLMSObject, int>>)
                        {
                            asList = true;
                            List<KeyValuePair<GXDLMSObject, int>> tmp = (List<KeyValuePair<GXDLMSObject, int>>)name;
                            //Item count
                            buff.Add((byte)tmp.Count);
                            foreach (KeyValuePair<GXDLMSObject, int> it in (List<KeyValuePair<GXDLMSObject, int>>)name)
                            {
                                //Interface class.
                                GXCommon.SetUInt16((ushort)it.Key.ObjectType, buff);
                                //Add LN
                                string[] items = it.Key.LogicalName.Split('.');
                                if (items.Length != 6)
                                {
                                    throw new GXDLMSException("Invalid Logical Name.");
                                }
                                foreach (string it2 in items)
                                {
                                    buff.Add(Convert.ToByte(it2));
                                }
                                buff.Add((byte)it.Value);
                                //Add Access type.
                                buff.Add(0);
                            }
                        }
                        else
                        {
                            //Interface class.
                            GXCommon.SetUInt16((ushort)interfaceClass, buff);
                            //Add LN
                            string[] items = ((string)name).Split('.');
                            if (items.Length != 6)
                            {
                                throw new GXDLMSException("Invalid Logical Name.");
                            }
                            foreach (string it in items)
                            {
                                buff.Add(Convert.ToByte(it));
                            }
                            buff.Add((byte)AttributeOrdinal);
                            if (data == null || data.Length == 0 || cmd == Command.SetRequest)
                            {
                                buff.Add(0); //Items count
                            }
                            else
                            {
                                buff.Add(1); //Items count
                            }
                        }
                    }
                }
                else
                {
                    int len = data == null ? 0 : data.Length;
                    buff = new List<byte>(11 + len);                    
                    if (name is List<KeyValuePair<GXDLMSObject, int>>)
                    {
                        List<KeyValuePair<GXDLMSObject, int>> tmp = (List<KeyValuePair<GXDLMSObject, int>>)name;
                        //Item count
                        buff.Add((byte) tmp.Count);
                        foreach (KeyValuePair<GXDLMSObject, int> it in (List<KeyValuePair<GXDLMSObject, int>>)name)
                        {
                            //Size
                            buff.Add(2);
                            ushort base_address = Convert.ToUInt16(it.Key.ShortName);
                            base_address += (ushort)((it.Value - 1) * 8);
                            GXCommon.SetUInt16(base_address, buff);
                        }
                    }
                    else
                    {
                        //Add item count.
                        buff.Add(1);
                        if (cmd == Command.ReadResponse || cmd == Command.WriteResponse)
                        {
                            buff.Add(0x0);
                        }
                        else
                        {
                            if (cmd == Command.WriteRequest || data == null || data.Length == 0)
                            {
                                buff.Add(2);
                            }
                            else //if Parameterized Access
                            {
                                buff.Add(4);
                            }
                            ushort base_address = Convert.ToUInt16(name);
                            //AttributeOrdinal is count already for action.
                            if (AttributeOrdinal != 0)
                            {
                                base_address += (ushort)((AttributeOrdinal - 1) * 8);
                            }
                            GXCommon.SetUInt16(base_address, buff);
                        }
                    }                    
                }
                if (data != null && data.Length != 0)
                {
                    if (cmd == Command.WriteRequest)
                    {
                        buff.Add(1);
                    }
                    buff.AddRange(data);
                }            
            }
            return SplitToBlocks(buff, cmd, asList);
        }

        /// <summary>
        /// Is operated as server or client.
        /// </summary>
        [DefaultValue(false)]
        public bool Server
        {
            get;
            set;
        }

        /// <summary>
        /// Information from the connection size that server can handle.
        /// </summary>
        public GXDLMSLimits Limits
        {
            get;
            set;
        }               
       
        /// <summary>
        /// Removes the HDLC header from the packet, and returns COSEM data only.
        /// </summary>
        /// <param name="packet">The received packet, from the device, as byte array.</param>
        /// <param name="data">The exported data.</param>
        /// <returns>COSEM data.</returns>
        public RequestTypes GetDataFromPacket(byte[] packet, ref byte[] data, out byte frame, out byte command)
        {
            if (packet == null || packet.Length == 0)
            {
                throw new ArgumentException("Packet is invalid.");
            }            
            int error;            
            List<byte> arr = new List<byte>(packet);
            bool packetFull, wrongCrc;
            RequestTypes moreData = GetDataFromFrame(arr, 0, out frame, true, out error, false, out packetFull, out wrongCrc, out command, true);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enough data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }            
            if (command == (int) Command.Rejected)
            {
                throw new GXDLMSException("Packet rejected.");
            }
            int len = 0;
            if (data != null)
            {
                len = data.Length;
            }
            byte[] tmp = new byte[len + arr.Count];
            if (data != null)
            {
                Array.Copy(data, 0, tmp, 0, len);
            }
            Array.Copy(arr.ToArray(), 0, tmp, len, arr.Count);
            data = tmp;
            if (moreData == RequestTypes.None && Ciphering.Security != Security.None &&
                (command == (byte)Command.GloGetResponse ||
                command == (byte)Command.GloSetResponse ||
                command == (byte)Command.GloMethodResponse || 
                command == (byte)Command.None) &&
                data.Length != 0)
            {
                Command cmd;
                data = Decrypt(data, out cmd, out error);
                command = (byte)cmd;
            }
            return moreData;
        }        

        /// <summary>
        /// Generates an acknowledgment message, with which the server is informed to 
        /// send next packets.
        /// </summary>
        /// <param name="type">Frame type</param>
        /// <returns>Acknowledgment message as byte array.</returns>
        public byte[] ReceiverReady(RequestTypes type)
        {
            if (!UseLogicalNameReferencing || type == RequestTypes.Frame)
            {
                byte id = GenerateSupervisoryFrame((byte)0);
                byte id2 = GenerateACK();
                if (id != id2)
                {
                    System.Diagnostics.Debug.WriteLine("TODO: " + id.ToString() + " " + id2.ToString());
                }
                return AddFrame(id, false, null, 0, 0);
            }
            List<byte> buff = new List<byte>(10);
            if (this.InterfaceType == InterfaceType.General)
            {
                if (Server)
                {                    
                    buff.AddRange(GXCommon.LLCReplyBytes);
                }
                else
                {
                    buff.AddRange(GXCommon.LLCSendBytes);
                }                
            }
            //Get request normal
            buff.Add(0xC0);
            buff.Add(0x02);
            //Invoke ID and priority.
            buff.Add(GetInvokeIDPriority());
            GXCommon.SetUInt32(PacketIndex, buff);
            return AddFrame(GenerateIFrame(), false, buff, 0, buff.Count);
        }           

        /// <summary>
        /// Determines, whether the DLMS packet is completed.
        /// </summary>
        /// <param name="data">The data to be parsed, to complete the DLMS packet.</param>
        /// <returns>True, when the DLMS packet is completed.</returns>
        public bool IsDLMSPacketComplete(byte[] data)
        {
            byte frame;
            int error;
            try
            {
                if (this.InterfaceType == DLMS.InterfaceType.General)
                {
                    if (data.Length < 5)
                    {
                        return false;
                    }
                    bool compleate = false;
                    //Find start of HDLC frame.
                    for (int index = 0; index < data.Length; ++index)
                    {
                        if (data[index] == GXCommon.HDLCFrameStartEnd)
                        {
                            if (2 + data[index + 2] <= data.Count())
                            {
                                compleate = true;                                
                            }
                            break;
                        }
                    }
                    if (!compleate)
                    {
                        return false;
                    }
                }
                else if (this.InterfaceType == DLMS.InterfaceType.Net)
                {
                    if (data.Length < 6)
                    {
                        return false;
                    }
                }
                else
                {
                    throw new Exception("Unknown interface type.");
                }
                bool packetFull, wrongCrc;
                byte command;
                GetDataFromFrame(new List<byte>(data), 0, out frame, true, out error, false, out packetFull, out wrongCrc, out command, false);
                return packetFull;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        internal void ParseReplyData(Gurux.DLMS.Internal.ActionType action, byte[] buff, out object value, out DataType type)
        {
            type = DataType.None; 
            if (!UseCache)
            {
                ClearProgress();
            }
            int read, total, index = 0;
            value = GXCommon.GetData(buff, ref index, action, out total, out read, ref type, ref CacheIndex);
            if (UseCache)
            {
                CacheSize = buff.Length;
                //If array.
                if (CacheData != null && CacheData.GetType().IsArray)
                {
                    if (value != null)
                    {
                        Array oldData = (Array)CacheData;
                        if (value.GetType().IsArray)
                        {
                            Array newData = (Array)value;
                            object[] data = new object[oldData.Length + newData.Length];
                            Array.Copy(oldData, data, oldData.Length);
                            Array.Copy(newData, 0, data, oldData.Length, newData.Length);
                            CacheData = data;
                        }
                        else
                        {
                            object[] data = new object[oldData.Length + 1];
                            Array.Copy(oldData, data, oldData.Length);
                            data[oldData.Length] = value;
                            CacheData = data;
                        }
                    }
                }
                else
                {
                    CacheData = value;
                    CacheType = type;
                }
            }
            else
            {
                CacheData = value;
            }
            ItemCount += read;
            MaxItemCount = total;           
        }

        /// <summary>
        /// This method is used to solve current index of received DLMS packet, 
        /// by retrieving the current progress status.
        /// </summary>
        /// <param name="data">DLMS data to parse.</param>
        /// <returns>The current index of the packet.</returns>
        public int GetCurrentProgressStatus(byte[] data)
        {
            try
            {
                //We can't get progress status if ciphering is used.
                if (Ciphering.Security != Security.None)
                {
                    return 0;
                }
                //Return cache size.
                if (UseCache && data.Length == CacheSize)
                {
                    return ItemCount;
                }                
                DataType type;
                object value = null;
                ParseReplyData(UseCache ? ActionType.Index : ActionType.Index, data, out value, out type);
                return ItemCount;
            }
            catch
            {
                return ItemCount;
            }
        }

        /// <summary>
        /// This method is used to solve the total amount of received items,
        /// by retrieving the maximum progress status.
        /// </summary>
        /// <param name="data">DLMS data to parse.</param>
        /// <returns>Total amount of received items.</returns>
        public int GetMaxProgressStatus(byte[] data)
        {
            try
            {
                //We can't get progress status if ciphering is used.
                if (Ciphering.Security != Security.None || data == null || data.Length == 0 ||
                    data[0] != 1)
                {
                    return 0;
                }

                //Return cache size.
                if (UseCache && data.Length == CacheSize)
                {
                    return MaxItemCount;
                }
                if (!UseCache)
                {
                    ClearProgress();
                }                
                object value = null;
                DataType type;
                ParseReplyData(UseCache ? ActionType.Index : ActionType.Count, data, out value, out type);
                return MaxItemCount;
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// Checks, whether the received packet is a reply to the sent packet.
        /// </summary>
        /// <param name="sendData">The sent data as a byte array. </param>
        /// <param name="receivedData">The received data as a byte array.</param>
        /// <returns>True, if the received packet is a reply to the sent packet. False, if not.</returns>
        public bool IsReplyPacket(byte[] sendData, byte[] receivedData)
        {
            byte sendID;
            byte receiveID;
            int error;            
            bool packetFull, wrongCrc;
            byte command;
            GetDataFromFrame(new List<byte>(sendData), 0, out sendID, false, out error, false, out packetFull, out wrongCrc, out command, false);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enough data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            RequestTypes more = GetDataFromFrame(new List<byte>(receivedData), 0, out receiveID, true, out error, true, out packetFull, out wrongCrc, out command, false);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enough data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            if (command == (int) Command.Rejected)
            {
                throw new GXDLMSException("Frame rejected.");
            }
            int sid = (sendID & 0xFF);
            int rid = (receiveID & 0xFF);
            bool ret = rid == (int)FrameType.Rejected || (sid == (int)FrameType.Disconnect && rid == (int)FrameType.UA) ||
                IsExpectedFrame((int)sid, (int)rid);
            return ret;
        }

        /// <summary>
        /// Checks, whether the received packet is a reply to the sent packet.
        /// </summary>
        /// <param name="sendData">The sent data as a byte array. </param>
        /// <param name="receivedData">The received data as a byte array.</param>
        /// <returns>True, if the received packet is a reply to the sent packet. False, if not.</returns>
        public bool IsPreviousPacket(byte[] sendData, byte[] receivedData)
        {
            byte sendID;
            byte receiveID;
            int error;
            bool packetFull, wrongCrc;
            byte command;
            GetDataFromFrame(new List<byte>(sendData), 0, out sendID, false, out error, false, out packetFull, out wrongCrc, out command, false);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enough data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            RequestTypes more = GetDataFromFrame(new List<byte>(receivedData), 0, out receiveID, true, out error, true, out packetFull, out wrongCrc, out command, false);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enough data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            if (command == (int) Command.Rejected)
            {
                throw new GXDLMSException("Frame rejected.");
            }
            int sid, rid;
            //If I-Frame.
            if ((sendID & 0x1) == 1)
            {
                sid = ((sendID >> 5) & 0x7);
                rid = ((receiveID >> 1) & 0x7);
                return ((sid - 1) & 0x7) == rid;
            }
            sid = ((sendID >> 5) & 0x7);
            rid = ((receiveID >> 5) & 0x7);
            return sid == ((rid - 2) & 0x7);
        }

        /// <summary>
        /// Returns frame number.
        /// </summary>
        /// <param name="data">Byte array where frame number is try to found.</param>
        /// <returns>Frame number between Zero to seven (0-7).</returns>
        public int GetFrameNumber(byte[] data)
        {            
            byte id;
            int error;
            bool packetFull, wrongCrc;
            //Check reply id.
            List<byte> tmp = new List<byte>(data);
            byte command;
            RequestTypes more = GetDataFromFrame(tmp, 0, out id, true, out error, true, out packetFull, out wrongCrc, out command, false);
            if (!packetFull)
            {
                //Check send id.
                more = GetDataFromFrame(tmp, 0, out id, false, out error, false, out packetFull, out wrongCrc, out command, false);
                if (!packetFull)
                {
                    throw new GXDLMSException("Not enough data to parse frame.");
                }
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            if (command == (int) Command.Rejected)
            {
                throw new GXDLMSException("Frame rejected.");
            }
            return ((id >> 5) & 0x7);
        }

        /// <summary>
        /// Determines the type of the connection
        /// </summary>
        /// <remarks>
        /// All DLMS meters do not support the IEC 62056-47 standard.  
        /// If the device does not support the standard, and the connection is made 
        /// using TCP/IP, set the type to InterfaceType.General.
        /// </remarks>    
        public InterfaceType InterfaceType
        {
            get;
            set;
        }       

        public byte[] GetCipheredData(byte[] data, GXCiphering ciphering)
        {
            return GXDLMSChippering.DecryptAesGcm(data, ciphering.SystemTitle, ciphering.BlockCipherKey, ciphering.AuthenticationKey);

        }

        /// <summary>
        /// Split packet to frames.
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="blockIndex"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        internal byte[][] SplitToFrames(List<byte> packet, uint blockIndex, ref int index, int count, Command cmd, byte resultChoice, bool asList)
        {
            List<byte> tmp = new List<byte>(count + 13);            
            if (cmd != Command.None && this.UseLogicalNameReferencing)
            {
                bool moreBlocks = packet.Count > MaxReceivePDUSize && packet.Count > index + count;
                //Command, multiple blocks and Invoke ID and priority.
                if (asList)
                {
                    tmp.AddRange(new byte[] { (byte)cmd, 3, GetInvokeIDPriority() });
                }
                else
                {
                    tmp.AddRange(new byte[] { (byte)cmd, (byte)(moreBlocks ? 2 : 1), GetInvokeIDPriority() });
                }
                if (Server)
                {
                    tmp.Add(resultChoice); // Get-Data-Result choice data
                }
                if (moreBlocks)
                {
                    GXCommon.SetUInt32(blockIndex, tmp);
                    tmp.Add(0);
                    GXCommon.SetObjectCount(count, tmp);
                }
            }
            else if (cmd != Command.None && !this.UseLogicalNameReferencing)
            {
                tmp.Add((byte)cmd);
            }
            //Crypt message in first run.
            if (Ciphering.Security != Security.None && FrameSequence != -1)
            {
                tmp.AddRange(packet.ToArray());
                packet.Clear();
                ++Ciphering.FrameCounter;
                Command gloCmd;
                if (cmd == Command.ReadRequest || cmd == Command.GetRequest)
                {
                    gloCmd = Command.GloGetRequest;
                }
                else if (cmd == Command.WriteRequest || cmd == Command.SetRequest)
                {
                    gloCmd = Command.GloSetRequest;
                }
                else if (cmd == Command.MethodRequest)
                {
                    gloCmd = Command.GloMethodRequest;
                }
                else if (cmd == Command.ReadResponse || cmd == Command.GetResponse)
                {
                    gloCmd = Command.GloGetResponse;
                }
                else if (cmd == Command.WriteResponse || cmd == Command.SetResponse)
                {
                    gloCmd = Command.GloSetResponse;
                }
                else if (cmd == Command.MethodResponse)
                {
                    gloCmd = Command.GloMethodResponse;
                }
                else
                {
                    throw new GXDLMSException("Invalid GLO command.");
                }
                packet.AddRange(GXDLMSChippering.EncryptAesGcm(gloCmd, Ciphering.Security, 
                    Ciphering.FrameCounter, Ciphering.SystemTitle, Ciphering.BlockCipherKey, 
                    Ciphering.AuthenticationKey, tmp.ToArray()));
                tmp.Clear();
                count = packet.Count;
            }
            if (this.InterfaceType == InterfaceType.General)
            {
                if (Server)
                {
                    tmp.InsertRange(0, GXCommon.LLCReplyBytes);
                }
                else
                {
                    tmp.InsertRange(0, GXCommon.LLCSendBytes);
                }
            }

            int dataSize;
            if (this.InterfaceType == InterfaceType.Net)
            {
                dataSize = MaxReceivePDUSize;
            }
            else
            {
                if (this.Server)
                {
                    dataSize = Convert.ToInt32(Limits.MaxInfoTX);
                }
                else
                {
                    dataSize = Convert.ToInt32(Limits.MaxInfoRX);
                }
            }
            if (count + index > packet.Count)
            {
                count = packet.Count - index;
            }            
            tmp.AddRange(packet.GetRange(index, count));
            index += count;
            count = tmp.Count;            
            if (count < dataSize)
            {
                dataSize = count;
            }            
            int cnt = (int)(count / dataSize);
            if (count % dataSize != 0)
            {
                ++cnt;
            }            
            int start = 0;
            byte[][] buff = new byte[cnt][];            
            for (uint pos = 0; pos < cnt; ++pos)
            {
                byte id = 0;
                if (pos == 0)
                {
                    id = GenerateIFrame();
                    byte id2 = GenerateIFrame(true);
                    if (id != id2)
                    {
                        System.Diagnostics.Debug.WriteLine("TODO : " + id.ToString() + " " + id2.ToString());
                    }
                }
                else
                {
                    id = GenerateNextFrame();
                    byte id2 = GenerateIFrame(false);
                    if (id != id2)
                    {
                        System.Diagnostics.Debug.WriteLine("TODO : " + id.ToString() + " " + id2.ToString());
                    }
                }
                if (start + dataSize > tmp.Count)
                {
                    dataSize = tmp.Count - start;
                }
                buff[pos] = AddFrame(id, cnt != 1 && pos < cnt - 1, tmp, start, dataSize);
                start += dataSize;
            }           
            return buff; 
        }

        /// <summary>
        /// Split the send packet to a size that the device can handle.
        /// </summary>
        /// <param name="packet">Packet to send.</param>
        /// <returns>Array of byte arrays that are sent to device.</returns>
        internal byte[][] SplitToBlocks(List<byte> packet, Command cmd, bool asList)
        {            
            int index = 0;
            if (!UseLogicalNameReferencing)//SN
            {
                return SplitToFrames(packet, 1, ref index, packet.Count, cmd, 0, asList);
            }           
            //If LN           
            //Split to Blocks.
            List<byte[]> buff = new List<byte[]>();
            uint blockIndex = 0;
            bool multibleFrames = false;
            do
            {
                byte[][] frames = SplitToFrames(packet, ++blockIndex, ref index, MaxReceivePDUSize, cmd, 0, asList);
                buff.AddRange(frames);
                if (frames.Length != 1)
                {
                    ExpectedFrame += 3;
                    multibleFrames = true;
                }
            }
            while (index < packet.Count);
            if (multibleFrames)
            {
                ExpectedFrame -= 3;
            }
            return buff.ToArray(); 
        }

        static internal string GetDescription(int errCode)
        {
            string str = null;
            switch (errCode)
            {
                case -1:
                    str = "Not a reply";
                    break;
                case 1: //Access Error : Device reports a hardware fault
                    str = Gurux.DLMS.Properties.Resources.HardwareFaultTxt;
                    break;
                case 2: //Access Error : Device reports a temporary failure
                    str = Gurux.DLMS.Properties.Resources.TemporaryFailureTxt;
                    break;
                case 3: // Access Error : Device reports Read-Write denied
                    str = Gurux.DLMS.Properties.Resources.ReadWriteDeniedTxt;
                    break;
                case 4: // Access Error : Device reports a undefined object
                    str = Gurux.DLMS.Properties.Resources.UndefinedObjectTxt;
                    break;
                case 9: // Access Error : Device reports a inconsistent Class or object
                    str = Gurux.DLMS.Properties.Resources.InconsistentClassTxt;
                    break;
                case 11: // Access Error : Device reports a unavailable object
                    str = Gurux.DLMS.Properties.Resources.UnavailableObjectTxt;
                    break;
                case 12: // Access Error : Device reports a unmatched type
                    str = Gurux.DLMS.Properties.Resources.UnmatchedTypeTxt;
                    break;
                case 13: // Access Error : Device reports scope of access violated
                    str = Gurux.DLMS.Properties.Resources.AccessViolatedTxt;
                    break;
                case 14: // Access Error : Data Block Unavailable. 
                    str = Gurux.DLMS.Properties.Resources.DataBlockUnavailableTxt;
                    break;
                case 15: // Access Error : Long Get Or Read Aborted.
                    str = Gurux.DLMS.Properties.Resources.LongGetOrReadAbortedTxt;
                    break;
                case 16: // Access Error : No Long Get Or Read In Progress.
                    str = Gurux.DLMS.Properties.Resources.NoLongGetOrReadInProgressTxt;
                    break;
                case 17: // Access Error : Long Set Or Write Aborted.
                    str = Gurux.DLMS.Properties.Resources.LongSetOrWriteAbortedTxt;
                    break;
                case 18: // Access Error : No Long Set Or Write In Progress.
                    str = Gurux.DLMS.Properties.Resources.NoLongSetOrWriteInProgressTxt;
                    break;
                case 19: // Access Error : Data Block Number Invalid.
                    str = Gurux.DLMS.Properties.Resources.DataBlockNumberInvalidTxt;
                    break;
                case 250: // Access Error : Other Reason.
                    str = Gurux.DLMS.Properties.Resources.OtherReasonTxt;
                    break;
                default:
                    str = Gurux.DLMS.Properties.Resources.UnknownErrorTxt;
                    break;
            }
            return str;
        }

        /// <summary>
        /// Checks, whether there are any errors on received packet.
        /// </summary>
        /// <param name="sendData">Sent data. </param>
        /// <param name="receivedData">Received data. </param>
        /// <returns>True, if there are any errors on received data.</returns>
        public object[,] CheckReplyErrors(byte[] sendData, byte[] receivedData)
        {
            int index = 0;
            bool ret = true;
            if (sendData != null)
            {
                ret = IsReplyPacket(sendData, receivedData);
            }
            //If packet is not reply for send packet...
            if (!ret)
            {
                object[,] list = new object[1, 2];
                list[0, 0] = -1;
                list[0, 1] = "Not a reply.";
                return list;
            }

            //If we are checking UA or AARE messages.
            if (LNSettings == null && SNSettings == null)
            {
                return null;
            }

            int err;
            byte frame;
            bool packetFull, wrongCrc;
            byte command;
            if (sendData != null)
            {
                GetDataFromFrame(new List<byte>(sendData), index, out frame, false, out err, false, out packetFull, out wrongCrc, out command, false);
                if (!packetFull)
                {
                    throw new GXDLMSException("Not enough data to parse frame.");
                }
                if (wrongCrc)
                {
                    throw new GXDLMSException("Wrong Checksum.");
                }                
                if (IsReceiverReadyRequest(frame) || frame == (byte)FrameType.Disconnect)
                {
                    return null;
                }
            }
            List<byte> data = new List<byte>(receivedData);
            RequestTypes moreData = GetDataFromFrame(data, index, out frame, true, out err, false, out packetFull, out wrongCrc, out command, false);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enough data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            if (command == (int)Command.Rejected)
            {
                throw new GXDLMSException("Frame rejected.");
            }
            if (moreData == RequestTypes.None && Ciphering.Security != Security.None &&
                (command == (byte)Command.GloGetResponse ||
                command == (byte)Command.GloSetResponse ||
                command == (byte)Command.GloMethodResponse ||
                command == (byte)Command.None) &&
                data.Count() != 0)
            {
                Command cmd;
                Decrypt(data.ToArray(), out cmd, out err);
                command = (byte)cmd;
            }

            if (err != 0x00)
            {
                object[,] list = new object[1, 2];
                list[0, 0] = err;
                list[0, 1] = GetDescription(err);
                return list;
            }
            return null;
        }

        /// <summary>
        /// Gets Logical Name settings, read from the device. 
        /// </summary>
        public GXDLMSLNSettings LNSettings
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets Short Name settings, read from the device.
        /// </summary>
        public GXDLMSSNSettings SNSettings
        {
            get;
            internal set;
        }

        /// <summary>
        /// Quality Of Service is an analysis of nonfunctional aspects of the software properties.
        /// </summary>
        /// <returns></returns>
        public int ValueOfQualityOfService
        {
            get;
            internal set;
        }

        /// <summary>
        /// Retrieves the amount of unused bits.
        /// </summary>
        /// <returns></returns>
        public int NumberOfUnusedBits
        {
            get;
            internal set;
        }
        internal enum UFrameMode
        {
            SNRM = 0x10, 
            //SNRME, SARM, SARME, SABM, SABME, 
            UA = 0xC
                //, DM, RIM, SIM, RD, DISC
        }

        //TODO:
        static internal byte GenerateUFrame(UFrameMode mode)
        {
            return (byte) ((((int)mode & 0x1C) << 3) | 0x10 | (((int)mode & 3) << 2) | 3);
        }

        /// <summary>
        /// Generate I-frame: Information frame. Reserved for internal use.
        /// </summary>
        /// <returns></returns>
        internal byte GenerateIFrame(bool Pf) //TODO:
        {
            int value = 0;
            //if (Pf)
            {
                value |= 0x10;
            }
            if (Pf)
            {
                ++ReceiveSequenceNo;
            }
            if (!Segmented)
            {
                SendSequenceNo = ++SendSequenceNo & 0x7;
            }
            else
            {
                if (Server)
                {
                    SendSequenceNo = (SendSequenceNo + 3) & 0x7;
                    //SendSequenceNo = ++SendSequenceNo & 0x7;
                }
                else
                {
                    SendSequenceNo = ++SendSequenceNo & 0x7;
                }
            }
            value |= ((ReceiveSequenceNo & 7) << 5 | ((SendSequenceNo & 0x7) << 1));
            if (Segmented)
            {
                if (Server)
                {
                    //++SendSequenceNo;
                   // SendSequenceNo += 2;
                }
                else
                {
                    //--SendSequenceNo;
                }
                Segmented = false;
            }

            /*
            int value = 0;
            if (Server)
            {
                value |= 0x10;
                ++ReceiveSequenceNo;
                ++SendSequenceNo;
            }
            value |= ((ReceiveSequenceNo & 7) << 5 | ((SendSequenceNo & 0x7) << 1));
            if (!Server)
            {
                value |= 0x10;
                ++ReceiveSequenceNo;
                ++SendSequenceNo;
            }
             * */
            return (byte)value;
        }

        byte GenerateACK()
        {
            Segmented = true;
            int value = ((++ReceiveSequenceNo & 7) << 5 | 1);
            value |= 0x10;
            return (byte) value;
        }

        /// <summary>
        /// Generate I-frame: Information frame. Reserved for internal use.
        /// </summary>
        /// <returns></returns>
        internal byte GenerateIFrame()
        {
            //Expected frame number is increased only when first keep alive msg is send...
            if (!bIsLastMsgKeepAliveMsg)
            {
                ExpectedFrame = (++ExpectedFrame & 0x7);
            }
            return GenerateNextFrame();
        }

        /// <summary>
        /// Generate ACK message. Reserved for internal use.
        /// </summary>
        /// <returns></returns>
        byte GenerateNextFrame()
        {
            FrameSequence = (++FrameSequence & 0x7);
            byte val = (byte)(((FrameSequence & 0x7) << 1) & 0xF);
            val |= (byte)(((((ExpectedFrame & 0x7) << 1) | 0x1) & 0xF) << 4);
            bIsLastMsgKeepAliveMsg = false;
            return val;
        }

        /// <summary>
        /// Generates Keep Alive frame for keep alive message. Reserved for internal use.
        /// </summary>
        /// <returns></returns>
        internal byte GenerateAliveFrame()
        {
            //Expected frame number is increased only when first keep alive msg is send...
            if (!bIsLastMsgKeepAliveMsg && !Server)
            {
                ExpectedFrame = (++ExpectedFrame & 0x7);
                bIsLastMsgKeepAliveMsg = true;
            }
            byte val = 1;
            val |= (byte)((((((ExpectedFrame) & 0x7) << 1) | 0x1) & 0xF) << 4);
            return val;
        }

        bool IsKeepAlive(byte value)
        {
            if (((value >> 5) & 0x7) == ExpectedFrame)
            {                
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if frame sequences are same. Reserved for internal use.
        /// </summary>
        /// <param name="send"></param>
        /// <param name="received"></param>
        /// <returns></returns>
        bool IsExpectedFrame(int send, int received)
        {
            //In keep alive msg send ID might be same as receiver ID.
            bool ret = send == received || //If echo.
                    (((send >> 5) & 0x7) == ((received >> 1) & 0x7) &&
                    ((received >> 5) & 0x7) == ((((send >> 1) & 0x7) + 1) & 0x7)) ||
                    //If U-Frame or S-Frame
                    ((send & 0x1) == 0x1 || (received & 0x1) == 1);
            if (!ret)
            {
                return ret;
            }
            return ret;
        }

        /// <summary>
        /// Generate I-frame: Information frame. Reserved for internal use. 
        ///
        /// 0 Receive Ready (denoted RR(R)) is a positive acknowledge ACK of all frames
        /// up to and including frame number R-1.
        /// 1 Reject (denoted RE.J(R)) is a negative acknowledge NAK
        /// of a Go-back-N mechanism. ie start retransmitting from frame number R.
        /// 2 Receive Not Ready (denoted RNR(R)) is a positive acknowledge of all
        /// frames up to and including R-1 but the sender must pause until a
        /// Receive Ready arrives. This can be used to pause the sender because of
        /// temporary problems at the receiver.
        /// 3 Selective Reject (denoted SREJ(R)) is a negative acknowledge in a
        /// Selective Repeat mechanism. ie resend only frame R. It is not
        /// supported in several implementations.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        byte GenerateSupervisoryFrame(byte type)
        {
            ExpectedFrame = (++ExpectedFrame & 0x7);
            byte val = (byte)((((type & 0x3) << 2) | 0x1) & 0xF);
            val |= (byte)(((((ExpectedFrame & 0x7) << 1) | 0x1) & 0xF) << 4);
            bIsLastMsgKeepAliveMsg = false;
            return val;
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        bool IsReceiverReadyRequest(byte val)
        {
            bool b = (val & 0xF) == 1 && (val >> 4) == (((ExpectedFrame & 0x7) << 1) | 0x1);
            return b;
        }


        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        bool IsRejectedFrame(byte val)
        {
            return (val & 0x07) == 0x07;
        }        
        
        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        internal void CheckInit()
        {
            if (this.ClientID == null)
            {
                throw new GXDLMSException("Invalid Client ID");
            }
            if (this.ServerID == null)
            {
                throw new GXDLMSException("Invalid Server ID");
            }
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        byte[] GetAddress(object address)
        {
            if (this.InterfaceType == InterfaceType.Net)
            {
                List<byte> tmp = new List<byte>();
                GXCommon.SetUInt16(Convert.ToUInt16(address), tmp);
                return tmp.ToArray();
            }
            if (address is Byte)
            {
                return new byte[1] { ((byte)address) };
            }
            List<byte> b = new List<byte>();
            if (address is UInt16)
            {
                GXCommon.SetUInt16((ushort)address, b);
                return b.ToArray();
            }
            if (address is UInt32)
            {
                GXCommon.SetUInt32((uint)address, b);
                return b.ToArray();
            }
            if (Server)
            {
                return null;                
            }
            throw new GXDLMSException("Invalid Server Address.");            
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>        
        internal byte[] AddFrame(byte Type, bool moreFrames, List<byte> data, int index, int count)
        {
            CheckInit();
            //Set packet size. BOP + Data size + dest and source size + EOP.
            int len = 7 + ServerBuff.Length + ClientBuff.Length;                        
            //If data is added. CRC is count for HDLC frame.
            if (count > 0)
            {
                len += count + 2;
            }
            List<byte> buff = new List<byte>(len);
            if (this.InterfaceType == InterfaceType.Net)
            {
                //Add version 0x0001
                buff.Add(0x00);
                buff.Add(0x01);
                if (Server)
                {
                    //Add Destination (Server)
                    buff.AddRange(ServerBuff);
                    //Add Source (Client)
                    buff.AddRange(ClientBuff);
                }
                else
                {
                    //Add Source (Client)
                    buff.AddRange(ClientBuff);
                    //Add Destination (Server)
                    buff.AddRange(ServerBuff);
                }                
                //Add data length. 2 bytes.
                GXCommon.SetUInt16((ushort)count, buff);
                if (data != null)
                {
                    buff.AddRange(data);
                }
            }
            else
            {                
                if (this.GenerateFrame)
                {
                    //HDLC frame opening flag.
                    buff.Add(GXCommon.HDLCFrameStartEnd);
                }
                //Frame type
                buff.Add((byte)(moreFrames ? 0xA8 : 0xA0));
                //Length of msg.
                buff.Add((byte)(len - 2));
                if (this.Server)
                {
                    //Client address
                    buff.AddRange(ClientBuff);
                    //Server address
                    buff.AddRange(ServerBuff);
                }
                else
                {
                    //Server address
                    buff.AddRange(ServerBuff);
                    //Client address
                    buff.AddRange(ClientBuff);
                }
                //Add DLMS frame type
                buff.Add(Type);
                //Count CRC for header.
                ushort crc = 0;
                if (count > 0)
                {
                    int start = 0;
                    int cnt = buff.Count;
                    if (this.GenerateFrame)
                    {
                        --cnt;
                        start = 1;
                    }
                    crc = GXFCS16.CountFCS16(buff.ToArray(), start, cnt);
                    GXCommon.SetUInt16(crc, buff);                    
                    buff.AddRange(data.GetRange(index, count));
                }
                //If framework is not generating CRC and EOP.
                if (this.GenerateFrame)
                {
                    //Count CRC for HDLC frame.
                    crc = GXFCS16.CountFCS16(buff.ToArray(), 1, buff.Count - 1);
                    GXCommon.SetUInt16(crc, buff);
                    //EOP
                    buff.Add(GXCommon.HDLCFrameStartEnd);
                }
            }
            return buff.ToArray();
        }

        /// <summary>
        /// Check LLC bytes. Reserved for internal use.
        /// </summary>
        bool CheckLLCBytes(List<byte> buff, ref int index)
        {
            if (InterfaceType == InterfaceType.General)
            {
                if (Server)
                {
                    return GXCommon.Compare(buff.ToArray(), ref index, GXCommon.LLCSendBytes);
                }
                return GXCommon.Compare(buff.ToArray(), ref index, GXCommon.LLCReplyBytes);
            }
            return false;
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        internal object GetAddress(byte[] buff, ref int index)
        {
            int size = 0;
            if (InterfaceType == InterfaceType.Net)
            {
                return GXCommon.GetInt16(buff, ref index);
            }
            for (int pos = index; pos != buff.Length; ++pos)
            {
                ++size;
                if ((buff[pos] & 0x1) == 1)
                {
                    break;
                }
            }
            if (size == 1)
            {
                return buff[index++];
            }
            else if (size == 2)
            {
                return GXCommon.GetUInt16(buff, ref index);
            }
            else if (size == 4)
            {
                return GXCommon.GetUInt32(buff, ref index);
            }
            throw new OutOfMemoryException();
        }        

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        internal RequestTypes GetDataFromFrame(List<byte> buff, int index, out byte frame, bool bReply, out int pError, bool skipLLC, out bool packetFull, out bool wrongCrc, out byte command, bool ciphering)
        {
            command = 0;
            wrongCrc = false;
            packetFull = true;
            frame = 0;            
            pError = 0;
            int DataLen = buff.Count - index;
            RequestTypes MoreData = RequestTypes.None;
            int PacketStartID = 0, len = buff.Count;
            int FrameLen = 0;
            //If DLMS frame is generated.
            if (InterfaceType != InterfaceType.Net)
            {
                //Find start of HDLC frame.
                for (; index < len; ++index)
                {
                    if (buff[index] == GXCommon.HDLCFrameStartEnd)
                    {
                        PacketStartID = index;
                        ++index;
                        break;
                    }
                }
                if (index == len) //Not a HDLC frame.
                {
                    throw new GXDLMSException("Invalid data format.");
                }
                frame = buff[index++];
                //Is there more data available.
                if ((frame & 0x8) != 0)
                {
                    MoreData = RequestTypes.Frame;
                }
                //Check frame length.
                if ((frame & 0x7) != 0)
                {
                    FrameLen = ((frame & 0x7) << 8);
                }
                //If not enough data.
                FrameLen += buff[index++];
                if (len < FrameLen + index - 1)
                {
                    packetFull = false;
                    return RequestTypes.None;
                }
                //Check EOP
                if (MoreData == RequestTypes.None && buff[FrameLen + PacketStartID + 1] != GXCommon.HDLCFrameStartEnd)
                {
                    throw new GXDLMSException("Invalid data format.");
                }
            }
            else
            {
                //Get version
                int ver = (buff[index++] & 0xFF) << 8;
                ver |= buff[index++] & 0xFF;
                if (ver != 1)
                {
                    throw new GXDLMSException("Unknown version.");
                }
            }
            byte[] serverBuff, clientBuff;
            if (((Server || !bReply) && InterfaceType == InterfaceType.General) ||
                (!Server && bReply && (InterfaceType == InterfaceType.Net)))
            {
                serverBuff = ClientBuff;
                clientBuff = ServerBuff;
            }
            else
            {
                serverBuff = ServerBuff;
                clientBuff = ClientBuff;
            }
            //Check that client addresses match.
            if (!GXCommon.Compare(buff.ToArray(), ref index, clientBuff))
            {
                //If echo.
                if (InterfaceType != InterfaceType.Net && FrameLen != 0)
                {
                    //Check that client addresses match.
                    if (GXCommon.Compare(buff.ToArray(), ref index, serverBuff) &&
                        //Check that server addresses match.
                       GXCommon.Compare(buff.ToArray(), ref index, clientBuff))
                    {
                        index = PacketStartID + 2 + FrameLen;
                        DataLen = buff.Count - index - 1;                        
                        if (DataLen > 5)
                        {
                            return GetDataFromFrame(buff, index, out frame, bReply, out pError, skipLLC, out packetFull, out wrongCrc, out command, ciphering);
                        }
                        packetFull = false;
                        return RequestTypes.None;
                    }
                }
                throw new GXDLMSException("Source addresses do not match. It is " + GetAddress(buff.ToArray(), ref index) + ". It should be " + this.ClientID + ".");
            }             
            //Check that server addresses match.
            if (!GXCommon.Compare(buff.ToArray(), ref index, serverBuff))
            {
                throw new GXDLMSException("Destination addresses do not match. It is " + GetAddress(buff.ToArray(), ref index) + ". It should be " + this.ServerID + ".");
            }
            if (InterfaceType != InterfaceType.Net)
            {
                //Get frame type.
                frame = buff[index++];
                //If server has left.
                if (frame == (byte) FrameType.DisconnectMode ||
                        frame == (byte) FrameType.Rejected)
                {
                    command = (byte) Command.Rejected;
                    return RequestTypes.None;
                }
                if (frame == (byte)FrameType.SNRM ||
                    frame == (byte)FrameType.Disconnect)
                {
                    //Check that CRC match.
                    int crcRead = GXCommon.GetUInt16(buff, ref index);
                    int crcCount = GXFCS16.CountFCS16(buff.ToArray(), PacketStartID + 1, len - PacketStartID - 4);                                        
                    if (crcRead != crcCount)
                    {
                        packetFull = false;
                        wrongCrc = true;
                        return RequestTypes.None;
                    }
                    if (frame == (byte)FrameType.SNRM)
                    {
                        command = (byte) Command.Snrm;
                        return RequestTypes.None;
                    }
                    if (frame == (byte)FrameType.Disconnect)
                    {
                        command = (byte)Command.DisconnectRequest;
                        return RequestTypes.None;
                    }
                    throw new Exception("Invalid frame.");
                }
                else
                {
                    //If S-frame
                    if ((frame & 3) == 0x1)
                    {
                        Segmented = true;
                    }
                    //Check that header crc is correct.
                    int crcCount = GXFCS16.CountFCS16(buff.ToArray(), PacketStartID + 1, index - PacketStartID - 1);
                    int crcRead = GXCommon.GetUInt16(buff, ref index);
                    if (crcRead != crcCount)
                    {
                        //Do nothing because Actaris is counting wrong CRC to the header.
                    }
                    //Check that CRC match.
                    crcCount = GXFCS16.CountFCS16(buff.ToArray(), PacketStartID + 1, len - PacketStartID - 4);
                    int pos = len - 3;
                    crcRead = GXCommon.GetUInt16(buff, ref pos);
                    if (crcRead != crcCount)
                    {
                        wrongCrc = true;
                        return RequestTypes.None;
                    }
                    //CheckLLCBytes returns false if LLC bytes are not used.
                    if (!skipLLC && CheckLLCBytes(buff, ref index))
                    {
                        //TODO: LLC can be skipped with SNRM ja Disconnect.
                        //Check response.
                        command = buff[index];                                                
                        //If ciphering is used.
                        if (command == (byte)Command.GloGetRequest ||
                            command == (byte)Command.GloGetResponse ||
                            command == (byte)Command.GloSetRequest ||
                            command == (byte)Command.GloSetResponse ||
                            command == (byte)Command.GloMethodRequest ||
                            command == (byte)Command.GloMethodResponse ||
                            command == (byte)Command.Aarq ||
                            command == (byte)Command.DisconnectRequest ||
                            command == 0x60 ||
                            command == 0x61)
                        {
                        }
                        else if (bReply && (command == (byte)Command.GetRequest || command == (byte)Command.GetResponse ||
                            command == (byte)Command.MethodRequest || command == (byte)Command.MethodResponse ||
                            command == (byte)Command.ReadRequest || command == (byte)Command.ReadResponse ||
                            command == (byte)Command.SetRequest || command == (byte)Command.SetResponse ||
                            command == (byte)Command.WriteRequest || command == (byte)Command.WriteResponse))
                        {
                            //If LN is used, check is there more data available.
                            if (this.UseLogicalNameReferencing)
                            {
                                if (!GetLNData(buff, ref index, ref pError, ref MoreData, command))
                                {
                                    packetFull = false;
                                    return RequestTypes.None;
                                }
                            }
                            else
                            {
                                GetSNData(buff, ref index, ref pError, command);
                            }
                        }
                        else
                        {
                            throw new GXDLMSException(string.Format("Invalid command %d", command));
                        }
                    }                   
                    //Skip data header and data CRC and EOP.
                    if (index + 3 > buff.Count)
                    {
                        if (Server)
                        {
                            MoreData |= RequestTypes.Frame;                            
                        }
                        buff.Clear();
                    }
                    else
                    {
                        //Remove all except payload.
                        buff.RemoveRange(len - 3, 3);
                        buff.RemoveRange(0, index);
                    }
                }
            }
            else
            {
                DataLen = GXCommon.GetUInt16(buff, ref index);
                if (DataLen + index > len) //If frame is not read complete.
                {
                    packetFull = false;
                    return RequestTypes.None;
                }
                if (DataLen != 0)
                {
                    // IEC62056-53 Sections 8.3 and 8.6.1
                    // If Get.Response.Normal.
                    command = buff[index];                   
                    //If chiphering is used.
                    if (command == (byte)Command.GloGetRequest ||
                        command == (byte)Command.GloGetResponse ||
                        command == (byte)Command.GloSetRequest ||
                        command == (byte)Command.GloSetResponse ||
                        command == (byte)Command.GloMethodRequest ||
                        command == (byte)Command.GloMethodResponse ||
                        command == (byte)Command.Aarq ||
                        command == (byte)Command.DisconnectRequest ||
                        command == (byte)Command.DisconnectResponse)
                    {
                    }
                    else if (bReply && command != 0x61 && command != 0x60)
                    {
                        //If LN is used, check is there more data available.
                        if (this.UseLogicalNameReferencing)
                        {
                            GetLNData(buff, ref index, ref pError, ref MoreData, command);
                        }
                        else
                        {
                            GetSNData(buff, ref index, ref pError, command);
                        }
                    }
                }
                buff.RemoveRange(0, index);
            }
            return MoreData;
        }

        public byte[] Decrypt(byte[] buff, out Command command, out int error)
        {
            error = 0;
            int index = 0;
            command = (Command) buff[index++];
            if (!(command == Command.GloGetRequest ||
                command == Command.GloSetRequest ||
                command == Command.GloMethodRequest ||
                command == Command.GloGetResponse ||
                command == Command.GloSetResponse ||
                command == Command.GloMethodResponse))            
            {
                throw new GXDLMSException("Invalid data format.");
            }
            int len = Gurux.DLMS.Internal.GXCommon.GetObjectCount(buff, ref index);
            if (buff.Length - index != len)
            {
                throw new GXDLMSException("Invalid data format.");
            }
            byte value = buff[index++];
            if (value != 0x10 && value != 0x20 && value != 0x30)
            {
                throw new ArgumentOutOfRangeException("Invalid security value.");
            }
            if (this.Ciphering.Security == Security.None)
            {
                this.Ciphering.Security = (Security)value;
            }
            else if (this.Ciphering.Security != (Security)value)
            {
                throw new GXDLMSException("Security method can't change after initialized.");
            }
            List<byte> tmp = new List<byte>(GetCipheredData(buff, Ciphering));
            command = (Command)tmp[0];
            index = 0;
            RequestTypes MoreData = RequestTypes.None;
            if (this.UseLogicalNameReferencing)
            {
                if (!GetLNData(tmp, ref index, ref error, ref MoreData, tmp[0]))
                {
                    throw new GXDLMSException("Invalid data format.");
                }
            }
            else
            {
                GetSNData(tmp, ref index, ref error, tmp[0]);
            }
            tmp.RemoveRange(0, index);
            return tmp.ToArray();
        }

        private static void GetSNData(List<byte> buff, ref int index, ref int pError, byte res)
        {
            //Check that this is reply
            if (res != (byte)Command.ReadRequest && res != (byte)Command.WriteRequest &&
                res != (byte)Command.SetRequest && res != (byte)Command.SetResponse && 
                res != (byte)Command.ReadResponse && res != (byte)Command.WriteResponse &&
                res != (byte)Command.GetRequest && res != (byte)Command.GetResponse &&
                res != (byte)Command.MethodRequest && res != (byte)Command.MethodResponse)
            {
                throw new GXDLMSException("Invalid command");
            }
            //Ship invoke ID and priority.
            ++index;            
            if (res == (byte)Command.ReadResponse || res == (byte)Command.WriteResponse)
            {
                ++index;
                //Add reply status.                
                bool bIsError = (buff[index++] != 0);
                if (bIsError)
                {
                    pError = buff[index++];
                }
            }            
        }
        
        enum StateError
        {
            ServiceNotAllowed = 1,
            ServiceUnknown = 2
        };

        enum ServiceError
        {
            OperationNotPossible = 1,
            ServiceNotSupported = 2,
            OtherReason = 3
        }

        static internal void GetActionInfo(ObjectType objectType, out int value, out int count)
        {
            count = value = 0;
            switch (objectType)
            {
                case ObjectType.Data:
                case ObjectType.ActionSchedule:
                case ObjectType.None:
                case ObjectType.AutoAnswer:
                case ObjectType.AutoConnect:
                case ObjectType.MacAddressSetup:
                case ObjectType.Event:
                case ObjectType.GprsSetup:
                case ObjectType.IecHdlcSetup:
                case ObjectType.IecLocalPortSetup:
                case ObjectType.IecTwistedPairSetup:
                case ObjectType.ModemConfiguration:
                case ObjectType.PppSetup:
                case ObjectType.RegisterMonitor:
                case ObjectType.RemoteAnalogueControl:
                case ObjectType.RemoteDigitalControl:
                case ObjectType.Schedule:
                case ObjectType.SmtpSetup:
                case ObjectType.StatusMapping:
                case ObjectType.TcpUdpSetup:
                case ObjectType.Tunnel:
                case ObjectType.UtilityTables:
                    throw new Exception("Target do not support Action.");
                case ObjectType.ImageTransfer:
                    value = 0x40;
                    count = 4;
                    break;
                case ObjectType.ActivityCalendar:
                    value = 0x50;
                    count = 1;
                    break;
                case ObjectType.AssociationLogicalName:
                    value = 0x60;
                    count = 4;
                    break;
                case ObjectType.AssociationShortName:
                    value = 0x20;
                    count = 8;
                    break;
                case ObjectType.Clock:
                    value = 0x60;
                    count = 6;
                    break;
                case ObjectType.DemandRegister:
                    value = 0x48;
                    count = 2;
                    break;
                case ObjectType.ExtendedRegister:
                    value = 0x38;
                    count = 1;
                    break;
                case ObjectType.Ip4Setup:
                    value = 0x60;
                    count = 3;
                    break;
                case ObjectType.MBusSlavePortSetup:
                    value = 0x60;
                    count = 8;
                    break;
                case ObjectType.ProfileGeneric:
                    value = 0x58;
                    count = 4;
                    break;
                case ObjectType.Register:
                    value = 0x28;
                    count = 1;
                    break;
                case ObjectType.RegisterActivation:
                    value = 0x30;
                    count = 3;
                    break;
                case ObjectType.RegisterTable:
                    value = 0x28;
                    count = 2;
                    break;
                case ObjectType.SapAssignment:
                    value = 0x20;
                    count = 1;
                    break;
                case ObjectType.ScriptTable:
                    value = 0x20;
                    count = 1;
                    break;
                case ObjectType.SpecialDaysTable:
                    value = 0x10;
                    count = 2;
                    break;
            }
        }
       
        private bool GetLNData(List<byte> buff, ref int index, ref int pError, ref RequestTypes MoreData, byte res)
        {
            ++index;
            if (buff.Count - 1 < index)
            {
                return false;
            }
            //If meter returns exception.
            if (res == 0xD8)
            {
                StateError StateError = (StateError) buff[index++];
                ServiceError ServiceError = (ServiceError) buff[index++];
                throw new GXDLMSException(StateError.ToString() + " " + ServiceError.ToString());
            }
            bool server = res == 0xC0;
            server = this.Server;
            if (res != 0x60 && res != 0x63 && res != (int)Command.GetResponse && res != (int)Command.SetResponse &&
                res != (int)Command.SetRequest && res != (int)Command.GetRequest && res != (int)Command.MethodRequest &&
                res != (int)Command.MethodResponse)
            {
                throw new GXDLMSException("Invalid response");
            }
            byte AttributeID = buff[index++];
            if (buff.Count - 1 < index)
            {
                return false;
            }
            //Skip Invoke ID and priority.
            byte InvokeID = buff[index++];
            if (buff.Count - 1 < index)
            {
                return false;
            }
            if (server && AttributeID == 0x2)
            {
                MoreData |= RequestTypes.DataBlock;
            }
            else if (res == (int)Command.SetRequest)
            {
                
            }
            else if (res == (int)Command.SetResponse || res == (int)Command.MethodResponse)
            {
                if ((pError = buff[index++]) != 0)
                {
                    return true;
                }
            }
            else
            {
                if (server && AttributeID == 0x01)
                {                    
                }
                else
                {
                    byte Priority = buff[index++];
                    if (buff.Count - 1 < index)
                    {
                        return false;
                    }
                    if (AttributeID == 0x01 && Priority != 0)
                    {
                        if (buff.Count - 1 < index)
                        {
                            return false;
                        }
                        pError = buff[index++];                        
                    }
                    else
                    {
                        if (AttributeID == 0x02)
                        {
                            if (Priority == 0)
                            {
                                MoreData |= RequestTypes.DataBlock;
                            }
                            PacketIndex = GXCommon.GetUInt32(buff, ref index);
                            index += 1;
                            //Get data length.
                            int dataLength = GXCommon.GetObjectCount(buff, ref index);
                        }
                        else if (AttributeID == 0x03) //If list.
                        {
                            index += 1;
                        }
                    }
                }
            }
            return true;
        }
    }
}
