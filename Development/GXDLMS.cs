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

        uint PacketIndex;
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
        internal static Dictionary<Gurux.DLMS.ObjectType, Type> AvailableObjectTypes = new Dictionary<Gurux.DLMS.ObjectType, Type>();        

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
            return null;
        }

        /// <summary>
        /// Returns object types.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This can be used with serizlization.
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
        /// The referencing is defined by the device manufacurer.
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
        internal byte[][] GenerateMessage(object name, int parameterCount, byte[] data, ObjectType interfaceClass, int AttributeOrdinal, Command cmd)
        {
            if (Limits.MaxInfoRX == null)
            {
                throw new GXDLMSException("Invalid arguement.");
            }
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
                        buff.Add(0x0); //Items count
                    }
                }
                else
                {
                    int len = data == null ? 0 : data.Length;
                    buff = new List<byte>(11 + len);
                    //Add name count.
                    if (name.GetType().IsArray)
                    {
                        foreach (object it in (Array)name)
                        {
                            buff.Add(2);
                            ushort base_address = Convert.ToUInt16(it);
                            base_address += (ushort)((AttributeOrdinal - 1) * 8);
                            GXCommon.SetUInt16(base_address, buff);
                        }
                    }
                    else
                    {
                        buff.Add(1);
                    }
                    if (cmd == Command.ReadResponse || cmd == Command.WriteResponse)
                    {
                        buff.Add(0x0);
                    }
                    else
                    {
                        buff.Add((byte)parameterCount);
                        ushort base_address = Convert.ToUInt16(name);
                        if (cmd == Command.MethodRequest)
                        {
                            base_address += (ushort)AttributeOrdinal;
                        }
                        else
                        {
                            base_address += (ushort)((AttributeOrdinal - 1) * 8);
                        }
                        GXCommon.SetUInt16(base_address, buff);
                    }
                }
                if (data != null && data.Length != 0)
                {
                    buff.AddRange(data);
                }            
            }
            return SplitToBlocks(buff, cmd);
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
            RequestTypes moreData = GetDataFromFrame(arr, 0, out frame, true, out error, false, out packetFull, out wrongCrc, out command);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enought data to parse frame.");
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
                return AddFrame(GenerateSupervisoryFrame((byte)0), false, null, 0, 0);
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
            buff.Add(0x81);
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
                bool packetFull, wrongCrc;
                byte command;
                GetDataFromFrame(new List<byte>(data), 0, out frame, true, out error, false, out packetFull, out wrongCrc, out command);
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
            GetDataFromFrame(new List<byte>(sendData), 0, out sendID, false, out error, false, out packetFull, out wrongCrc, out command);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enought data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            RequestTypes more = GetDataFromFrame(new List<byte>(receivedData), 0, out receiveID, true, out error, true, out packetFull, out wrongCrc, out command);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enought data to parse frame.");
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
            GetDataFromFrame(new List<byte>(sendData), 0, out sendID, false, out error, false, out packetFull, out wrongCrc, out command);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enought data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            RequestTypes more = GetDataFromFrame(new List<byte>(receivedData), 0, out receiveID, true, out error, true, out packetFull, out wrongCrc, out command);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enought data to parse frame.");
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
            RequestTypes more = GetDataFromFrame(tmp, 0, out id, true, out error, true, out packetFull, out wrongCrc, out command);
            if (!packetFull)
            {
                //Check send id.
                more = GetDataFromFrame(tmp, 0, out id, false, out error, false, out packetFull, out wrongCrc, out command);
                if (!packetFull)
                {
                    throw new GXDLMSException("Not enought data to parse frame.");
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


        /// <summary>
        /// Retrieves the password that is used in communication.
        /// </summary>
        /// <remarks>
        /// If authentication is set to none, password is not used.
        /// </remarks>
        /// <seealso cref="Authentication"/>
        public string Password
        {
            get;
            set;
        }

        byte[][] SplitToFrames(List<byte> packet, uint blockIndex, ref int index, int count, Command cmd)
        {
            List<byte> tmp = new List<byte>(count + 13);
            if (this.InterfaceType == InterfaceType.General)
            {
                if (Server)
                {
                    tmp.AddRange(GXCommon.LLCReplyBytes);
                }
                else
                {
                    tmp.AddRange(GXCommon.LLCSendBytes);
                }
            }
            if (cmd != Command.None && this.UseLogicalNameReferencing)
            {
                bool moreBlocks = packet.Count > MaxReceivePDUSize && packet.Count > index + count;
                //Command, multiple blocks and Invoke ID and priority.
                tmp.AddRange(new byte[] { (byte)cmd, (byte)(moreBlocks ? 2 : 1), 0x81 });
                if (Server)
                {
                    tmp.Add(0x0); // Get-Data-Result choice data
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
            int dataSize;
            if (this.InterfaceType == InterfaceType.Net)
            {
                dataSize = MaxReceivePDUSize;
            }
            else
            {
                if (cmd == Command.GetRequest || cmd == Command.MethodRequest || cmd == Command.ReadRequest || 
                    cmd == Command.SetRequest || cmd == Command.WriteRequest)
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
                }
                else
                {
                    id = GenerateNextFrame();
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
        internal byte[][] SplitToBlocks(List<byte> packet, Command cmd)
        {
            int index = 0;
            if (!UseLogicalNameReferencing)//SN
            {                                
                return SplitToFrames(packet, 0, ref index, packet.Count, cmd);
            }           
            //If LN           
            //Split to Blocks.
            List<byte[]> buff = new List<byte[]>();
            uint blockIndex = 0;
            do
            {
                byte[][] frames = SplitToFrames(packet, ++blockIndex, ref index, MaxReceivePDUSize, cmd);
                buff.AddRange(frames);
                if (frames.Length != 1)
                {
                    ExpectedFrame += 3;
                }
            }
            while (index < packet.Count);            
            return buff.ToArray(); 
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
            if (LNSettings == null && SNSettings == null) //TODO:
            {
                return null;
            }

            int err;
            byte frame;
            bool packetFull, wrongCrc;
            byte command;
            if (sendData != null)
            {
                GetDataFromFrame(new List<byte>(sendData), index, out frame, false, out err, false, out packetFull, out wrongCrc, out command);
                if (!packetFull)
                {
                    throw new GXDLMSException("Not enought data to parse frame.");
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
            RequestTypes moreData = GetDataFromFrame(new List<byte>(receivedData), index, out frame, true, out err, false, out packetFull, out wrongCrc, out command);
            if (!packetFull)
            {
                throw new GXDLMSException("Not enought data to parse frame.");
            }
            if (wrongCrc)
            {
                throw new GXDLMSException("Wrong Checksum.");
            }
            if (command == (int)Command.Rejected)
            {
                throw new GXDLMSException("Frame rejected.");
            }

            if (err != 0x00)
            {
                String str = null;
                switch (err)
                {
                    case 1:
                        str = "Access Error : Device reports a hardware fault.";
                        break;
                    case 2:
                        str = "Access Error : Device reports a temporary failure.";
                        break;
                    case 3:
                        str = "Access Error : Device reports Read-Write denied.";
                        break;
                    case 4:
                        str = "Access Error : Device reports a undefined object.";
                        break;
                    case 5:
                        str = "Access Error : Device reports a inconsistent Class or object.";
                        break;
                    case 6:
                        str = "Access Error : Device reports a unavailable object.";
                        break;
                    case 7:
                        str = "Access Error : Device reports a unmatched type.";
                        break;
                    case 8:
                        str = "Access Error : Device reports scope of access violated.";
                        break;
                    default:
                        str = "Unknown error: " + err;
                        break;
                }
                object[,] list = new object[1, 2];
                list[0, 0] = err;
                list[0, 1] = str;
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
                    ((send >> 5) & 0x7) == ((received >> 1) & 0x7) ||
                    //If U-Frame...
                    ((send & 0x1) == 0x1 && (received & 0x1) == 1);
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
        long GetAddress(List<byte> buff, ref int index, int size)
        {
            if (size == 1)
            {
                return buff[index] & 0xFF;
            }
            if (size == 2)
            {
                return GXCommon.GetUInt16(buff.ToArray(), ref index);
            }
            if (size == 4)
            {
                return GXCommon.GetUInt32(buff.ToArray(), ref index);
            }
            throw new GXDLMSException("Invalid address size.");
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        internal RequestTypes GetDataFromFrame(List<byte> buff, int index, out byte frame, bool bReply, out int pError, bool skipLLC, out bool packetFull, out bool wrongCrc, out byte command)
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
                if (frame == GXCommon.HDLCFrameTypeMoreData)
                {
                    MoreData = RequestTypes.Frame;
                }
                //If not enought data.
                FrameLen = buff[index++];
                //if (len - index + 2 < FrameLen)
                if (len < FrameLen + index - 1)
                {
                    packetFull = false;
                    return RequestTypes.None;
                }
                if ((frame != GXCommon.HDLCFrameType && frame != GXCommon.HDLCFrameTypeMoreData) || //Check BOP
                        (MoreData == RequestTypes.None && buff[FrameLen + PacketStartID + 1] != GXCommon.HDLCFrameStartEnd))//Check EOP
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
                        index = 2 + FrameLen;
                        DataLen = buff.Count - index - 1;                        
                        if (DataLen > 5)
                        {
                            return GetDataFromFrame(buff, index, out frame, bReply, out pError, skipLLC, out packetFull, out wrongCrc, out command);
                        }
                        packetFull = false;
                        return RequestTypes.None;
                    }
                }
                throw new GXDLMSException("Source addresses do not match. It is " + GetAddress(buff, ref index, clientBuff.Length) + ". It should be " + this.ClientID + ".");
            }             
            //Check that server addresses match.
            if (!GXCommon.Compare(buff.ToArray(), ref index, serverBuff))
            {
                throw new GXDLMSException("Destination addresses do not match. It is " + GetAddress(buff, ref index, serverBuff.Length) + ". It should be " + this.ServerID + ".");
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
                    //Check that header crc is corrent.
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
                        //TODO: LLC voi skipata SNRM ja Disconnect.
                        //Check response.
                        command = buff[index];
                        if (command == (byte)Command.Aarq ||
                            command == (byte)Command.DisconnectRequest)
                        {
                        }
                        else if (bReply && command != 0x61 && command != 0x60)
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
                    if (command == (byte)Command.Aarq ||
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
            ++index;
            if (res == (byte)Command.ReadResponse || res == (byte)Command.WriteResponse)
            {
                
                //Add reply status.
                ++index;
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
                case ObjectType.All:
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
                case ObjectType.MbusSetup:
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
                MoreData = RequestTypes.DataBlock;
            }
            else if (res == (int)Command.SetRequest)
            {
                //Mikko MoreData = RequestTypes.Write;
            }
            else if (res == (int)Command.MethodRequest)
            {
                //Mikko MoreData = RequestTypes.Action;
            }
            else
            {
                if (server && AttributeID == 0x01)
                {
                    //Mikko MoreData = res == 0xC0 ? RequestTypes.Read : RequestTypes.Write;
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
                        pError = buff[index++];
                        if (buff.Count - 1 < index)
                        {
                            return false;
                        }
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
                    }
                }
            }
            return true;
        }
    }
}
