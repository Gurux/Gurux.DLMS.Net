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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Secure;

namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMS implements methods to communicate with DLMS/COSEM metering devices.
    /// </summary>
    sealed class GXDLMS
    {
        /// <summary>
        /// Generates Invoke ID and priority.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <returns>Invoke ID and priority.</returns>
        static byte GetInvokeIDPriority(GXDLMSSettings settings)
        {
            byte value = 0;
            if (settings.Priority == Priority.High)
            {
                value = 0x80;
            }
            if (settings.ServiceClass == ServiceClass.Confirmed)
            {
                value |= 0x40;
            }
            value |= (byte)(settings.InvokeID & 0xF);
            return value;
        }

        /// <summary>
        /// Generates Invoke ID and priority.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <returns>Invoke ID and priority.</returns>
        static UInt32 GetLongInvokeIDPriority(GXDLMSSettings settings)
        {
            UInt32 value = 0;
            if (settings.Priority == Priority.High)
            {
                value = 0x80000000;
            }
            if (settings.ServiceClass == ServiceClass.Confirmed)
            {
                value |= 0x40000000;
            }
            value |= (UInt32)(settings.longInvokeID & 0xFFFFFF);
            ++settings.longInvokeID;
            return value;
        }

        /// <summary>
        /// Returns object types.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This can be used with serialization.
        /// </remarks>
        public static Type[] GetObjectTypes(Dictionary<ObjectType, Type> availableObjectTypes)
        {
            lock (availableObjectTypes)
            {
                if (availableObjectTypes.Count == 0)
                {
                    foreach (Type type in typeof(GXDLMS).Assembly.GetTypes())
                    {
                        if (!type.IsAbstract && typeof(GXDLMSObject).IsAssignableFrom(type))
                        {
                            GXDLMSObject obj = Activator.CreateInstance(type) as GXDLMSObject;
                            availableObjectTypes[obj.ObjectType] = type;
                        }
                    }
                }
                return availableObjectTypes.Values.ToArray();
            }
        }

        /// <summary>
        /// Get available COSEM objects.
        /// </summary>
        /// <param name="availableObjectTypes"></param>
        internal static void GetAvailableObjects(Dictionary<ObjectType, Type> availableObjectTypes)
        {
            lock (availableObjectTypes)
            {
                GetObjectTypes(availableObjectTypes);
            }
        }

        internal static GXDLMSObject CreateObject(ObjectType type, Dictionary<ObjectType, Type> availableObjectTypes)
        {
            lock (availableObjectTypes)
            {
                //Update objects.
                if (availableObjectTypes.Count == 0)
                {
                    GetObjectTypes(availableObjectTypes);
                }
                if (availableObjectTypes.ContainsKey(type))
                {
                    return Activator.CreateInstance(availableObjectTypes[type]) as GXDLMSObject;
                }
            }
            GXDLMSObject obj = new GXDLMSObject();
            obj.ObjectType = type;
            return obj;
        }

        ///<summary>
        ///Generates an acknowledgment message, with which the server is informed to send next packets.
        ///</summary>
        ///<param name="type">
        /// Frame type.
        /// </param>
        ///<returns>
        ///Acknowledgment message as byte array.
        ///</returns>
        internal static byte[] ReceiverReady(GXDLMSSettings settings, RequestTypes type)
        {
            if (type == RequestTypes.None)
            {
                throw new ArgumentException("Invalid receiverReady RequestTypes parameter.");
            }
            // Get next frame.
            if ((type & RequestTypes.Frame) != 0)
            {
                byte id = settings.ReceiverReady();
                return GetHdlcFrame(settings, id, null);
            }
            // Get next block.
            GXByteBuffer bb = new GXByteBuffer(6);
            bb.SetUInt32(settings.BlockIndex);
            settings.IncreaseBlockIndex();
            if (settings.IsServer)
            {
                return GXDLMS.GetMessages(settings, Command.GetResponse, 2, bb, DateTime.MinValue)[0];
            }
            return GXDLMS.GetMessages(settings, Command.GetRequest, 2, bb, DateTime.MinValue)[0];
        }

        /// <summary>
        /// Get error description.
        /// </summary>
        /// <param name="error">Error number.</param>
        /// <returns>Error as plain text.</returns>
        internal static string GetDescription(ErrorCode error)
        {
            string str = null;
            switch (error)
            {
                case ErrorCode.Ok:
                    str = "";
                    break;
                case ErrorCode.DisconnectRequest:
                    str = "Connection closed.";
                    break; 
                case ErrorCode.Rejected:
                    str = "Connection rejected.";
                    break;
                case ErrorCode.InvalidHdlcReply:
                    str = "Not a reply";
                    break;
                case ErrorCode.HardwareFault: //Access Error : Device reports a hardware fault
                    str = Gurux.DLMS.Properties.Resources.HardwareFaultTxt;
                    break;
                case ErrorCode.TemporaryFailure: //Access Error : Device reports a temporary failure
                    str = Gurux.DLMS.Properties.Resources.TemporaryFailureTxt;
                    break;
                case ErrorCode.ReadWriteDenied: // Access Error : Device reports Read-Write denied
                    str = Gurux.DLMS.Properties.Resources.ReadWriteDeniedTxt;
                    break;
                case ErrorCode.UndefinedObject: // Access Error : Device reports a undefined object
                    str = Gurux.DLMS.Properties.Resources.UndefinedObjectTxt;
                    break;
                case ErrorCode.InconsistentClass: // Access Error : Device reports a inconsistent Class or object
                    str = Gurux.DLMS.Properties.Resources.InconsistentClassTxt;
                    break;
                case ErrorCode.UnavailableObject: // Access Error : Device reports a unavailable object
                    str = Gurux.DLMS.Properties.Resources.UnavailableObjectTxt;
                    break;
                case ErrorCode.UnmatchedType: // Access Error : Device reports a unmatched type
                    str = Gurux.DLMS.Properties.Resources.UnmatchedTypeTxt;
                    break;
                case ErrorCode.AccessViolated: // Access Error : Device reports scope of access violated
                    str = Gurux.DLMS.Properties.Resources.AccessViolatedTxt;
                    break;
                case ErrorCode.DataBlockUnavailable: // Access Error : Data Block Unavailable. 
                    str = Gurux.DLMS.Properties.Resources.DataBlockUnavailableTxt;
                    break;
                case ErrorCode.LongGetOrReadAborted: // Access Error : Long Get Or Read Aborted.
                    str = Gurux.DLMS.Properties.Resources.LongGetOrReadAbortedTxt;
                    break;
                case ErrorCode.NoLongGetOrReadInProgress: // Access Error : No Long Get Or Read In Progress.
                    str = Gurux.DLMS.Properties.Resources.NoLongGetOrReadInProgressTxt;
                    break;
                case ErrorCode.LongSetOrWriteAborted: // Access Error : Long Set Or Write Aborted.
                    str = Gurux.DLMS.Properties.Resources.LongSetOrWriteAbortedTxt;
                    break;
                case ErrorCode.NoLongSetOrWriteInProgress: // Access Error : No Long Set Or Write In Progress.
                    str = Gurux.DLMS.Properties.Resources.NoLongSetOrWriteInProgressTxt;
                    break;
                case ErrorCode.DataBlockNumberInvalid: // Access Error : Data Block Number Invalid.
                    str = Gurux.DLMS.Properties.Resources.DataBlockNumberInvalidTxt;
                    break;
                case ErrorCode.OtherReason: // Access Error : Other Reason.
                    str = Gurux.DLMS.Properties.Resources.OtherReasonTxt;
                    break;
                default:
                    str = Gurux.DLMS.Properties.Resources.UnknownErrorTxt;
                    break;
            }
            return str;
        }      

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        internal static void CheckInit(GXDLMSSettings settings)
        {
            if (settings.ClientAddress == 0)
            {
                throw new ArgumentException("Invalid Client Address");
            }
            if (settings.ServerAddress == 0)
            {
                throw new ArgumentException("Invalid Server Address");
            }
        }
     
        internal static void AppendData(GXDLMSObject obj, int index, GXByteBuffer bb, Object value)
        {
            DataType tp = obj.GetDataType(index);
            if (tp == DataType.Array)
            {
                if (value is byte[])
                {
                    bb.Set((byte[])value);
                    return;
                }
                else if (value is GXByteBuffer)
                {
                    bb.Set((GXByteBuffer)value);
                    return;
                }
            }
            else
            {
                if (tp == DataType.None)
                {
                    tp = GXCommon.GetValueType(value);
                    //If data type is not defined for Date Time it is write as Octect string.
                    if (tp == DataType.DateTime)
                    {
                        tp = DataType.OctetString;
                    }
                }
            }
            GXCommon.SetData(bb, tp, value);
        }

        /// <summary>
        /// Get used glo message.
        /// </summary>
        /// <param name="command">Executed command.</param>
        /// <returns>Integer value of glo message.</returns>
        private static byte GetGloMessage(Command cmd) 
        {
            switch (cmd)
            {
                case Command.ReadRequest:
                case Command.GetRequest:
                    cmd = Command.GloGetRequest;
                    break;
                case Command.WriteRequest:
                case Command.SetRequest:
                    cmd = Command.GloSetRequest;
                    break;
                case Command.MethodRequest:
                    cmd = Command.GloMethodRequest;
                    break;
                case Command.ReadResponse:
                case Command.GetResponse:
                    cmd = Command.GloGetResponse;
                    break;
                case Command.WriteResponse:
                case Command.SetResponse:
                    cmd = Command.GloSetResponse;
                    break;
                case Command.MethodResponse:
                    cmd = Command.GloMethodResponse;
                    break;
                default:
                    throw new GXDLMSException("Invalid GLO command.");
            }
            return (byte)cmd;
        }

        /// <summary>
        /// Is multiple blocks needed for send data.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="bb">Send data.</param>
        /// <returns>Returns true if multiple blocks are needed.</returns>
        internal static bool MultipleBlocks(GXDLMSSettings settings, GXByteBuffer bb)
        {
            if (!settings.UseLogicalNameReferencing)
            {
                return false;
            }
            return bb.Size - bb.Position > settings.MaxReceivePDUSize;
        }        

        internal static UInt16 GetMaxPduSize(GXDLMSSettings settings, GXByteBuffer data, GXByteBuffer bb)
        {
            int size = data.Size - data.Position;
            int offset = 0;
            if (bb != null)
            {
                offset = bb.Size;
            }
            if (size + offset > settings.MaxReceivePDUSize)
            {
                size = settings.MaxReceivePDUSize - offset;
                size -= GXCommon.GetObjectCountSizeInBytes(size);                    
            }
            else if (size + GXCommon.GetObjectCountSizeInBytes(size) > settings.MaxReceivePDUSize)
            {
                size = (size - GXCommon.GetObjectCountSizeInBytes(size));
            }
            return (UInt16)size;
        }

        internal static byte[][] GetMessages(GXDLMSSettings settings, Command command, byte commandType, GXByteBuffer data, DateTime time)
        {
            // Save original position.
            ushort pos = data.Position;
            byte[][] reply;
            if (settings.UseLogicalNameReferencing)
            {
                reply = GetLnMessages(settings, command, commandType, data, time);
            }
            else
            {
                reply = GetSnMessages(settings, command, commandType, data, time);
            }
            data.Position = pos;
            return reply;

        }

        private static byte[][] GetLnMessages(GXDLMSSettings settings, Command command, byte commandType,
               GXByteBuffer data, DateTime time)
        {
            GXByteBuffer bb = new GXByteBuffer();
            List<byte[]> messages = new List<byte[]>();
            byte frame = 0;
            if (command == Command.Aarq)
            {
                frame = 0x10;
            }
            bool multipleBlocks = MultipleBlocks(settings, data);
            do 
            {
                if (command == Command.Aarq)
                {
                    if (settings.InterfaceType == InterfaceType.HDLC)
                    {
                        bb.Set(GXCommon.LLCSendBytes);
                    }
                    bb.Set(data);
                }
                else
                {
                    GetLNPdu(settings, command, commandType, data, bb, 0xFF, multipleBlocks, true, time);
                }
                while (bb.Position != bb.Size)
                {
                    if (settings.InterfaceType == Enums.InterfaceType.WRAPPER)
                    {
                        messages.Add(GXDLMS.GetWrapperFrame(settings, bb));
                    }
                    else
                    {
                        messages.Add(GXDLMS.GetHdlcFrame(settings, frame, bb));
                        frame = 0;
                    }
                }
                bb.Clear(); 
            }
            while (data.Position != data.Size);
            return messages.ToArray();
        }        

        internal static void GetLNPdu(GXDLMSSettings settings, Command command, byte commandType,
                GXByteBuffer data, GXByteBuffer bb, byte status, bool multipleBlocks, bool lastBlock, DateTime date)
        {
            bool ciphering = settings.Cipher != null && settings.Cipher.Security != Security.None;
            int offset = 0;
            int len = 0;
            if (settings.InterfaceType == InterfaceType.HDLC)
            {
                if (settings.IsServer)
                {
                    bb.Set(0, GXCommon.LLCReplyBytes);
                }
                else
                {
                    bb.Set(0, GXCommon.LLCSendBytes);
                }
                offset = 3;
            }
            if (multipleBlocks && settings.LnSettings.GeneralBlockTransfer)
            {
                bb.SetUInt8((byte)Command.GeneralBlockTransfer);
                //If multiple blocks.
                if (multipleBlocks)
                {
                    //If this is a last block make sure that all data is fit to it.
                    if (lastBlock)
                    {
                        lastBlock = !MultipleBlocks(settings, data);
                    }
                }
                // Is last block
                if (!lastBlock)
                {
                    bb.SetUInt8(0);
                }
                else
                {
                    bb.SetUInt8(0x80);
                }
                // Set block number sent.
                bb.SetUInt8(0);
                // Set block number acknowledged
                bb.SetUInt8((byte)settings.BlockIndex);
                ++settings.BlockIndex;
                // Add APU tag.
                bb.SetUInt8(0);
                // Add Addl fields
                bb.SetUInt8(0);
            }
            // Add command.
            bb.SetUInt8((byte)command);

            if (command != Command.DataNotification)
            {
                bb.SetUInt8(commandType);
                // Add Invoke Id And Priority.
                bb.SetUInt8(GetInvokeIDPriority(settings));
            }
            else
            {
                // Add Long-Invoke-Id-And-Priority
                bb.SetUInt32(GetLongInvokeIDPriority(settings));
                // Add date time.
                if (date == DateTime.MinValue || date == DateTime.MaxValue)
                {
                    bb.SetUInt8(DataType.None);
                }
                else
                {
                    // Data is send in octet string. Remove data type.
                    int pos = bb.Size;
                    GXCommon.SetData(bb, DataType.OctetString, date);
                    bb.Move(pos + 1, pos, bb.Size - pos - 1);
                }
            }

            if (command != Command.DataNotification && !settings.LnSettings.GeneralBlockTransfer)
            {
                //If multiple blocks.
                if (multipleBlocks)
                {
                    //If this is a last block make sure that all data is fit to it.
                    if (lastBlock)
                    {
                        lastBlock = !MultipleBlocks(settings, data);
                    }
                    // Is last block.
                    if (lastBlock)
                    {
                        bb.SetUInt8(1);
                        settings.Count = settings.Index = 0;
                    }
                    else
                    {
                        bb.SetUInt8(0);
                    }
                    // Block index.
                    bb.SetUInt32(settings.BlockIndex);
                    if (status != 0)
                    {
                        bb.SetUInt8(1);
                    }
                    bb.SetUInt8(status);
                    //Block size.
                    if (bb != null && bb.Size != 0)
                    {
                        len = GetMaxPduSize(settings, data, bb);
                        GXCommon.SetObjectCount(len, bb);
                    }
                }
                else if (status != 0xFF)
                {
                    //If error has occurred.
                    if (status != 0 && command != Command.MethodResponse && command != Command.SetResponse)
                    {
                        bb.SetUInt8(1);
                    }
                    bb.SetUInt8(status);
                }
            }
            else if (bb != null && bb.Size != 0)
            {
            // Block size.
            len = GetMaxPduSize(settings, data, bb);
            }
            
            //Add data
            if (data != null && data.Size != 0)
            {
                if (len == 0)
                {
                    len = data.Size - data.Position;
                    if (len > settings.MaxReceivePDUSize - bb.Size)
                    {
                        len = settings.MaxReceivePDUSize - bb.Size;
                    }
                }
                bb.Set(data, len);
            }
            if (ciphering)
            {
                byte[] tmp = settings.Cipher.Encrypt((byte)GetGloMessage(command), settings.Cipher.SystemTitle, bb.SubArray(offset, bb.Size - offset));
                bb.Size = (UInt16)offset;
                bb.Set(tmp);
            }
        }

        private static byte[][] GetSnMessages(GXDLMSSettings settings, Command command, byte commandType,
               GXByteBuffer data, DateTime time)
        {
            GXByteBuffer bb = new GXByteBuffer();
            GetSNPdu(settings, command, data, bb);
            List<byte[]> messages = new List<byte[]>();
            byte frame = 0x0;
            if (command == Command.Aarq)
            {
                frame = 0x10;
            }
            while (bb.Position != bb.Size)
            {
                if (settings.InterfaceType == Enums.InterfaceType.WRAPPER)
                {
                    messages.Add(GXDLMS.GetWrapperFrame(settings, bb));
                }
                else
                {
                    messages.Add(GXDLMS.GetHdlcFrame(settings, frame, bb));
                    frame = 0;
                }
            }
            return messages.ToArray();           
        }

        internal static void GetSNPdu(GXDLMSSettings settings, Command command, GXByteBuffer data, GXByteBuffer bb)
        {
            if (settings.InterfaceType == InterfaceType.HDLC)
            {
                if (settings.IsServer)
                {
                    bb.Set(GXCommon.LLCReplyBytes);
                }
                else if(bb.Size == 0)
                {
                    bb.Set(GXCommon.LLCSendBytes);
                }
            }
            // Add command. 
            GXByteBuffer tmp = new GXByteBuffer();
            if (command != Command.Aarq && command != Command.Aare)
            {
                tmp.SetUInt8((byte)command);
            }
            // Add data.
            tmp.Set(data);
            // If Ciphering is used.
            if (settings.Cipher != null && settings.Cipher.Security != Security.None
                && command != Command.Aarq && command != Command.Aare)
            {
                bb.Set(settings.Cipher.Encrypt((byte) GetGloMessage(command), settings.Cipher.SystemTitle, tmp.Array()));
            }
            else
            {
                bb.Set(tmp);
            }
        }            

        internal static Object GetAddress(int value, byte size)
        {
            if (size < 2 && value < 0x80)
            {
                return (byte)(value << 1 | 1);
            }
            if (size < 4 && value < 0x4000)
            {
                return (UInt16)((value & 0x3F80) << 2 | (value & 0x7F) << 1 | 1);
            }
            if (value < 0x10000000)
            {
                return (UInt32)((value & 0xFE00000) << 4 | (value & 0x1FC000) << 3
                        | (value & 0x3F80) << 2 | (value & 0x7F) << 1 | 1);
            }
            throw new ArgumentException("Invalid address.");
        }

        /// <summary>
        /// Convert HDLC address to bytes.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="size">Address size in bytes.</param>
        /// <returns></returns>
        private static byte[] GetAddressBytes(int value, byte size)
        {
            Object tmp = GetAddress(value, size);
            GXByteBuffer bb = new GXByteBuffer();
            if (tmp is byte)
            {
                bb.SetUInt8((byte)tmp);
            }
            else if (tmp is UInt16)
            {
                bb.SetUInt16((UInt16)tmp);
            }
            else if (tmp is UInt32)
            {
                bb.SetUInt32((UInt32)tmp);
            }
            else
            {
                throw new ArgumentException("Invalid address type.");
            }
            return bb.Array();
        }

        /// <summary>
        /// Split DLMS PDU to wrapper frames.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="data"> Wrapped data.</param>
        /// <returns>Wrapper frames</returns>
        internal static byte[] GetWrapperFrame(GXDLMSSettings settings, GXByteBuffer data)
        {
            GXByteBuffer bb = new GXByteBuffer();
            // Add version.
            bb.SetUInt16(1);
            if (settings.IsServer)
            {
                bb.SetUInt16((UInt16)settings.ServerAddress);
                bb.SetUInt16((UInt16)settings.ClientAddress);
            }
            else
            {
                bb.SetUInt16((UInt16)settings.ClientAddress);
                bb.SetUInt16((UInt16)settings.ServerAddress);
            }
            if (data == null)
            {
                // Data length.
                bb.SetUInt16(0);
            }
            else
            {
                // Data length.
                bb.SetUInt16(data.Size);
                // Data
                bb.Set(data);
            }
            //Remove sent data in server side.
            if (settings.IsServer)
            {
                if (data.Size == data.Position)
                {
                    data.Clear();
                }
                else
                {
                    data.Move(data.Position, 0, data.Size - data.Position);
                    data.Position = 0;
                }
            }
            return bb.Array();
        }

        /// <summary>
        /// Get HDLC frame for data.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="frame">Frame ID. If zero new is generated.</param>
        /// <param name="data">Data to add.</param>
        /// <returns>HDLC frames.</returns>
        internal static byte[] GetHdlcFrame(GXDLMSSettings settings, byte frame, GXByteBuffer data)
        {
            GXByteBuffer bb = new GXByteBuffer();
            int frameSize, len = 0;
            byte[] primaryAddress, secondaryAddress;
            if (settings.IsServer)
            {
                primaryAddress = GetAddressBytes(settings.ClientAddress, 0);
                secondaryAddress = GetAddressBytes(settings.ServerAddress, settings.ServerAddressSize);
            }
            else
            {
                primaryAddress = GetAddressBytes(settings.ServerAddress, settings.ServerAddressSize);
                secondaryAddress = GetAddressBytes(settings.ClientAddress, 0);
            }
            // Add BOP
            bb.SetUInt8(GXCommon.HDLCFrameStartEnd);
            frameSize = Convert.ToInt32(settings.Limits.MaxInfoTX);
            // If no data
            if (data == null || data.Size == 0)
            {
                bb.SetUInt8(0xA0);
            }
            else if (data.Size - data.Position <= frameSize)
            {
                // Is last packet.
                bb.SetUInt8(0xA0);
                len = data.Size - data.Position;
            }
            else
            {
                // More data to left.
                bb.SetUInt8(0xA8);
                len = frameSize;
            }
            //Frame len.
            if (len == 0)
            {
                bb.SetUInt8((byte)(5 + primaryAddress.Length + secondaryAddress.Length + len));
            }
            else
            {
                bb.SetUInt8((byte)(7 + primaryAddress.Length + secondaryAddress.Length + len));
            }
            // Add primary address.
            bb.Set(primaryAddress);
            // Add secondary address.
            bb.Set(secondaryAddress);

            //Add frame ID.
            if (frame == 0)
            {
                frame = settings.NextSend();
            }
            bb.SetUInt8(frame);
            // Add header CRC.
            UInt16 crc = GXFCS16.CountFCS16(bb.Data, 1, bb.Size - 1);
            bb.SetUInt16(crc);
            if (len != 0)
            {
                //Add data.
                bb.Set(data, len);
                // Add data CRC.
                crc = GXFCS16.CountFCS16(bb.Data, 1, bb.Size - 1);
                bb.SetUInt16(crc);
            }
            // Add EOP
            bb.SetUInt8(GXCommon.HDLCFrameStartEnd);
            if (data != null)
            {
                //Remove sent data in server side.
                if (settings.IsServer)
                {
                    if (data.Size == data.Position)
                    {
                        data.Clear();
                    }
                    else
                    {
                        data.Move(data.Position, 0, data.Size - data.Position);
                        data.Position = 0;
                    }
                }
            }
            return bb.Array();            
        }        

        /// <summary>
        ///  Check LLC bytes.
        /// </summary>
        /// <param name="server">Is server.</param>
        /// <param name="data">Received data.</param>
        private static void GetLLCBytes(bool server,
            GXByteBuffer data)
        {
            if (server)
            {
                data.Compare(GXCommon.LLCSendBytes);
            }
            else
            {
                data.Compare(GXCommon.LLCReplyBytes);
            }
        }

        static short GetHdlcData(bool server, GXDLMSSettings settings, GXByteBuffer reply, GXReplyData data)
        {
            short ch;
            int pos, packetStartID = reply.Position, frameLen = 0;
            int crc, crcRead;
            // If whole frame is not received yet.
            if (reply.Size - reply.Position < 9)
            {
                data.IsComplete = false;
                return 0;
            }
            data.IsComplete = true;
            // Find start of HDLC frame.
            for (pos = reply.Position; pos < reply.Size; ++pos)
            {
                ch = reply.GetUInt8();
                if (ch == GXCommon.HDLCFrameStartEnd)
                {
                    packetStartID = pos;
                    break;
                }
            }
            // Not a HDLC frame. 
            // Sometimes meters can send some strange data between DLMS frames.
            if (reply.Position == reply.Size)
            {
                data.IsComplete = false;
                // Not enough data to parse;
                return 0;
            }
            byte frame = reply.GetUInt8();
            if ((frame & 0xF0) != 0xA0)
            {
                //If same strage data.
                return GetHdlcData(server, settings, reply, data);
            }
            // Check frame length.
            if ((frame & 0x7) != 0)
            {
                frameLen = ((frame & 0x7) << 8);
            }
            ch = reply.GetUInt8();
            // If not enough data.
            frameLen += ch;
            if (reply.Size - reply.Position + 1 < frameLen)
            {
                data.IsComplete = false;
                reply.Position = (UInt16) packetStartID;
                // Not enough data to parse;
                return 0;
            }
            int eopPos = frameLen + packetStartID + 1;
            ch = reply.GetUInt8(eopPos);
            if (ch != GXCommon.HDLCFrameStartEnd)
            {
                throw new GXDLMSException("Invalid data format.");
            }

            // Check addresses.
            if (!CheckHdlcAddress(server, settings, reply, data, eopPos))
            {
                //If echo,
                return GetHdlcData(server, settings, reply, data);
            }

            // Is there more data available.
            if ((frame & 0x8) != 0)
            {
                data.MoreData = (RequestTypes)(data.MoreData | RequestTypes.Frame);
            }
            else
            {
                data.MoreData = (RequestTypes)(data.MoreData & ~RequestTypes.Frame);
            }
            // Get frame type.
            frame = reply.GetUInt8();
            if (!settings.CheckFrame(frame))
            {
                reply.Position = (UInt16) (eopPos + 1);
                return GetHdlcData(server, settings, reply, data);
            }
            // Check that header CRC is correct.
            crc = GXFCS16.CountFCS16(reply.Data, packetStartID + 1, reply.Position - packetStartID - 1);
            crcRead = reply.GetUInt16();
            if (crc != crcRead)
            {
                throw new Exception("Wrong CRC.");
            }           
            // Check that packet CRC match only if there is a data part.
            if (reply.Position != packetStartID + frameLen + 1)
            {
                crc = GXFCS16.CountFCS16(reply.Data, packetStartID + 1,
                        frameLen - 2);
                crcRead = reply.GetUInt16(packetStartID + frameLen - 1);
                if (crc != crcRead)
                {
                    throw new Exception("Wrong CRC.");
                }
            }
           
            if ((frame & (byte)HdlcFrameType.Uframe) == (byte)HdlcFrameType.Uframe)
            {
                //Get Eop if there is no data.
                if (reply.Position == packetStartID + frameLen + 1)
                {
                    // Get EOP.
                    reply.GetUInt8();
                }
                data.Command = (Command)frame;
            }
            //If S-frame
            else if ((frame & (byte)HdlcFrameType.Sframe) == (byte)HdlcFrameType.Sframe)
            {
                //If frame is rejected.
                int tmp = (frame >> 2) & 0x3;
                if (tmp == (byte)HdlcControlFrame.Reject)
                {
                    data.Error = (int)ErrorCode.Rejected;
                }
                else if (tmp == (byte)HdlcControlFrame.ReceiveNotReady)
                {
                    data.Error = (int)ErrorCode.Rejected;
                }
                else if (tmp == (byte)HdlcControlFrame.ReceiveReady)
                {
                    System.Diagnostics.Debug.WriteLine("Get next frame.");
                }
                //Get Eop if there is no data.
                if (reply.Position == packetStartID + frameLen + 1)
                {
                    // Get EOP.
                    reply.GetUInt8();
                }
            }
            else //Iframe
            {
                //Get Eop if there is no data.
                if (reply.Position == packetStartID + frameLen + 1)
                {
                    // Get EOP.
                    reply.GetUInt8();
                    if ((frame & 0x1) == 0x1)
                    {
                        data.MoreData = RequestTypes.Frame;
                    }
                }
                else
                {
                    GetLLCBytes(server, reply);
                }
            }           
            // Skip data CRC and EOP.
            if (reply.Position != reply.Size)
            {
                reply.Size = (UInt16)(eopPos - 2);
            }
            return frame;
        }

        private static void GetServerAddress(int address, out int logical, out int physical)
        {
            if (address < 0x4000)
            {
                logical = address >> 7;
                physical = address & 0x7F;
            }
            else
            {
                logical = address >> 14;
                physical = address & 0x3FFF;
            }
        }

        private static bool CheckHdlcAddress(bool server, GXDLMSSettings settings, GXByteBuffer reply,
                GXReplyData data, int index)
        {
            int source, target;
            // Get destination and source addresses.
            target = GXCommon.GetHDLCAddress(reply);
            source = GXCommon.GetHDLCAddress(reply);
            if (server)
            {
                // Check that server addresses match.
                if (settings.ServerAddress != 0
                        && settings.ServerAddress != target)
                {
                    throw new GXDLMSException(
                            "Destination addresses do not match. It is "
                                    + target.ToString() + ". It should be "
                                    + settings.ServerAddress.ToString() + ".");
                }
                else
                {
                    settings.ServerAddress = target;
                }
                // Check that client addresses match.
                if (settings.ClientAddress != 0 && settings.ClientAddress != source)
                {
                    throw new GXDLMSException(
                            "Source addresses do not match. It is "
                                    + source.ToString() + ". It should be "
                                    + settings.ClientAddress.ToString()
                                    + ".");
                }
                else
                {
                    settings.ClientAddress = source;
                }
            }
            else
            {
                // Check that client addresses match.
                if (settings.ClientAddress != target)
                {
                    //If echo.
                    if (settings.ClientAddress == source && 
                        settings.ServerAddress == target)
                    {
                        reply.Position = (UInt16)(index + 1);
                        return false;
                    }
                    throw new GXDLMSException(
                            "Destination addresses do not match. It is "
                                    + target.ToString() + ". It should be "
                                    + settings.ClientAddress.ToString()
                                    + ".");
                }
                // Check that server addresses match.
                if (settings.ServerAddress != source)
                {
                    //Check logical and physical address separately.
                    //This is done because some meters might send four bytes 
                    //when only two bytes is needed.
                    int readLogical, readPhysical, logical, physical;
                    GetServerAddress(source, out readLogical, out readPhysical);
                    GetServerAddress(settings.ServerAddress, out logical, out physical);
                    if (readLogical != logical || readPhysical != physical)
                    {
                        throw new GXDLMSException(
                                "Source addresses do not match. It is "
                                        + source.ToString() + ". It should be "
                                        + settings.ServerAddress.ToString()
                                        + ".");
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Get data from TCP/IP frame.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="buff">Received data.</param>
        /// <param name="data"> Reply information.</param>
        static void GetTcpData(GXDLMSSettings settings,
                GXByteBuffer buff, GXReplyData data)
        {
            // If whole frame is not received yet.
            if (buff.Size - buff.Position < 8)
            {
                data.IsComplete = false;
                return;
            }
            UInt16 pos = buff.Position;
            int value;
            // Get version
            value = buff.GetUInt16();
            if (value != 1)
            {
                throw new GXDLMSException("Unknown version.");
            }

            // Check TCP/IP addresses.
            CheckWrapperAddress(settings, buff);
            // Get length.
            value = buff.GetUInt16();
            data.IsComplete = !((buff.Size - buff.Position) < value);
            if (!data.IsComplete)
            {
                buff.Position = pos;
            }
        }

        private static void CheckWrapperAddress(GXDLMSSettings settings,
                GXByteBuffer buff)
        {
            int value;
            if (settings.IsServer)
            {
                value = buff.GetUInt16();
                // Check that client addresses match.
                if (settings.ClientAddress != 0
                        && settings.ClientAddress != value)
                {
                    throw new GXDLMSException(
                            "Source addresses do not match. It is "
                                    + value.ToString() + ". It should be "
                                    + settings.ClientAddress.ToString()
                                    + ".");
                }
                else
                {
                    settings.ClientAddress = value;
                }

                value = buff.GetUInt16();
                // Check that server addresses match.
                if (settings.ServerAddress != 0
                        && settings.ServerAddress != value)
                {
                    throw new GXDLMSException(
                            "Destination addresses do not match. It is "
                                    + value.ToString() + ". It should be "
                                    + settings.ServerAddress.ToString()
                                    + ".");
                }
                else
                {
                    settings.ServerAddress = value;
                }
            }
            else
            {
                value = buff.GetUInt16();
                // Check that server addresses match.
                if (settings.ClientAddress != 0
                        && settings.ServerAddress != value)
                {
                    throw new GXDLMSException(
                            "Source addresses do not match. It is "
                                    + value.ToString() + ". It should be "
                                    + settings.ServerAddress.ToString()
                                    + ".");

                }
                else
                {
                    settings.ServerAddress = value;
                }

                value = buff.GetUInt16();
                // Check that client addresses match.
                if (settings.ClientAddress != 0
                        && settings.ClientAddress != value)
                {
                    throw new GXDLMSException(
                            "Destination addresses do not match. It is "
                                    + value.ToString() + ". It should be "
                                    + settings.ClientAddress.ToString()
                                    + ".");
                }
                else
                {
                    settings.ClientAddress = value;
                }
            }
        }

        /// <summary>
        /// Handle read response and get data from block and/or update error status.
        /// </summary>
        /// <param name="data">Received data from the client.</param>
        static bool HandleReadResponse(GXReplyData data)
        {
            UInt16 pos = data.Data.Position;
            //If we are reading more than one value it is handled later.
            if (GXCommon.GetObjectCount(data.Data) != 1)
            {
                data.Data.Position = pos;
                GetDataFromBlock(data.Data, 0);
                return false;
            }
            else
            {
                byte ch;
                // Get status code.
                ch = data.Data.GetUInt8();
                if (ch == 0)
                {
                    data.Error = 0;
                    GetDataFromBlock(data.Data, 0);
                }
                else
                {
                    // Get error code.
                    data.Error = data.Data.GetUInt8();
                }
            }
            return true;
        }

        /// <summary>
        /// Handle method response and get data from block and/or update error status.
        /// </summary>
        /// <param name="data">Received data from the client.</param>
        static void HandleMethodResponse(GXDLMSSettings settings,
                GXReplyData data)
        {
            short type;

            // Get type.
            type = data.Data.GetUInt8();
            // Get invoke ID and priority.
            data.Data.GetUInt8();
            byte ret = data.Data.GetUInt8();
            if (ret != 0)
            {
                data.Error = ret;
            }
            if (type == 1)
            {
                // Response normal. Get data if exists.
                if (data.Data.Position < data.Data.Size)
                {
                    int size = data.Data.GetUInt8();
                    if (size != 0)
                    {
                        if (size != 1)
                        {
                            throw new GXDLMSException(
                                    "parseApplicationAssociationResponse failed. "
                                            + "Invalid tag.");
                        }
                        ret = data.Data.GetUInt8();
                        if (ret != 0)
                        {
                            data.Error = data.Data.GetUInt8();
                        }
                        else
                        {
                            GetDataFromBlock(data.Data, 0);
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException("Invalid Command.");
            }
        }        

        static void HandleSetResponse(GXDLMSSettings settings, GXReplyData data)
        {
            byte ret = data.Data.GetUInt8();
            //SetResponseNormal
            if (ret == 1)
            {
                //Invoke ID and priority.
                data.Data.GetUInt8();
                ret = data.Data.GetUInt8();
                if (ret != 0)
                {
                    data.Error = ret;
                }
            }
        }

        /// <summary>
        /// Handle write response and get data from block.
        /// </summary>
        /// <param name="reply">Received data from the client.</param>
        static void HandleWriteResponse(GXReplyData data)
        {
            int cnt = GXCommon.GetObjectCount(data.Data);
            byte ret;
            for (int pos = 0; pos != cnt; ++pos)
            {
                ret = data.Data.GetUInt8();
                if (ret != 0)
                {
                    ret = data.Data.GetUInt8();
                    data.Error = ret;
                }
            }
        }

        /// <summary>
        /// Handle get response and get data from block and/or update error status.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="reply">Received data from the client.</param>
        /// <param name="index">Block index number.</param>
        static bool HandleGetResponse(GXDLMSSettings settings,
                GXReplyData reply, int index)
        {
            long number;
            short ch = 0, type;
            GXByteBuffer data = reply.Data;

            // Get type.
            type = data.GetUInt8();
            // Get invoke ID and priority.
            ch = data.GetUInt8();
            // Response normal
            if (type == 1)
            {
                // Result
                ch = data.GetUInt8();
                if (ch != 0)
                {
                    reply.Error = data.GetUInt8();
                }
                GetDataFromBlock(data, 0);
            }
            else if (type == 2)
            {
                // GetResponsewithDataBlock
                // Is Last block.
                ch = data.GetUInt8();
                if (ch == 0)
                {
                    reply.MoreData = (RequestTypes)(reply.MoreData | RequestTypes.DataBlock);
                }
                else
                {
                    reply.MoreData = (RequestTypes)(reply.MoreData & ~RequestTypes.DataBlock);
                }
                // Get Block number.
                number = data.GetUInt32();
                //If meter's block index is zero based.
                if (number == 0 && settings.BlockIndex == 1)
                {
                    settings.BlockIndex = 0;
                }
                UInt32 expectedIndex = settings.BlockIndex;
                if (number != expectedIndex)
                {
                    throw new ArgumentException(
                            "Invalid Block number. It is " + number
                                    + " and it should be " + expectedIndex + ".");
                }
                // Get status.
                ch = data.GetUInt8();
                if (ch != 0)
                {
                    reply.Error = data.GetUInt8();
                }
                if (data.Position != data.Size)
                {
                    // Get data size.
                    reply.BlockLength = GXCommon.GetObjectCount(data);
                    // if whole block is read.
                    if ((reply.MoreData & RequestTypes.Frame) == 0)
                    {
                        // Check Block length.
                        if (reply.BlockLength > data.Size - data.Position)
                        {
                            throw new OutOfMemoryException();
                        }
                        reply.Command = Command.None;
                    }
                    GetDataFromBlock(data, index);
                    // If last packet and data is not try to peek.
                    if (reply.MoreData == RequestTypes.None)
                    {
                        if (!reply.Peek)
                        {
                            data.Position = 0;
                            settings.ResetBlockIndex();
                        }
                    }
                }
            }
            else if (type == 3)
            {
                //Get response with list.
                GetDataFromBlock(data, 0);
                return false;
            }
            else
            {
                throw new ArgumentException("Invalid Get response.");
            }
            return true;
        }

        /// <summary>
        /// Handle General block transfer message.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="data"></param>
        private static void HandleGbt(GXDLMSSettings settings, GXReplyData data)
        {
            data.Gbt = true;
            int index = data.Data.Position - 1;
            byte ch = data.Data.GetUInt8();

            //Is streaming active.
            bool streaming = (ch & 0x40) == 1;
            byte window = (byte)(ch & 0x3F);
            //Block number. 
            byte bn = data.Data.GetUInt8();
            //Block number acknowledged. 
            byte bna = data.Data.GetUInt8();
            // Get APU tag.
            if (data.Data.GetUInt8() != 0)
            {
                throw new Exception("Invalid APU.");
            }
            // Get Addl tag.
            if (data.Data.GetUInt8() != 0)
            {
                throw new Exception("Invalid APU.");
            }
            data.Command = Command.None;
            if (window != 0)
            {
                int len = GXCommon.GetObjectCount(data.Data);
                if (len != data.Data.Size - data.Data.Position)
                {
                    data.IsComplete = false;
                    return;
                }
            }
            GetPdu(settings, data);
            GetDataFromBlock(data.Data, index);
            //Is Last block, 
            if ((ch & 0x80) == 0)
            {
                data.MoreData |= RequestTypes.DataBlock;
            }
            else
            {
                data.MoreData &= ~RequestTypes.DataBlock;
            }
            // Get data if all data is read or we want to peek data.
            if (data.Data.Position != data.Data.Size
                    && (data.Command == Command.ReadResponse
                            || data.Command == Command.GetResponse
                            || data.Command == Command.DataNotification)
                    && (data.MoreData == RequestTypes.None
                            || data.Peek))
            {
                GetValueFromData(settings, data);
            }
        }

        /// <summary>
        /// Get PDU from the packet.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="data">received data.</param>
        public static void GetPdu(GXDLMSSettings settings,
                GXReplyData data)
        {
            short ch;
            Command cmd = data.Command;
            // If header is not read yet or GBT message.
            if (data.Command == Command.None || data.Gbt)
            {
                //If PDU is missing.
                if (data.Data.Size - data.Data.Position == 0)
                {
                    throw new InvalidOperationException("Invalid PDU.");
                }
                UInt16 index = data.Data.Position;
                // Get command.
                ch = data.Data.GetUInt8();
                cmd = (Command)ch;
                data.Command = cmd;
                switch (cmd)
                {
                    case Command.ReadResponse:
                        if (!HandleReadResponse(data))
                        {
                            return;
                        }
                        break;
                    case Command.GetResponse:
                        if (!HandleGetResponse(settings, data, index))
                        {
                            return;
                        }
                        break;
                    case Command.SetResponse:
                        HandleSetResponse(settings, data);
                        break;
                    case Command.WriteResponse:
                        HandleWriteResponse(data);
                        break;
                    case Command.MethodResponse:
                        HandleMethodResponse(settings, data);
                        break;
                    case Command.GeneralBlockTransfer:
                        HandleGbt(settings, data);
                        break;
                    case Command.Aarq:
                    case Command.Aare:
                        // This is parsed later.
                        --data.Data.Position;
                        break;
                    case Command.DisconnectResponse:
                        break;
                    case Command.ExceptionResponse:
                        throw new GXDLMSException((StateError)data.Data.GetUInt8(), 
                            (ServiceError)data.Data.GetUInt8());
                    case Command.GetRequest:
                    case Command.ReadRequest:
                    case Command.WriteRequest:
                    case Command.SetRequest:
                    case Command.MethodRequest:
                    case Command.DisconnectRequest:
                        // Server handles this.
                        if ((data.MoreData & RequestTypes.Frame) != 0)
                        {
                            break;
                        }
                        break;
                    case Command.GloGetRequest:
                    case Command.GloSetRequest:
                    case Command.GloMethodRequest:
                        if (settings.Cipher == null)
                        {
                            throw new Exception(
                                    "Secure connection is not supported.");
                        }
                        //If all frames are read.
                        if ((data.MoreData & RequestTypes.Frame) == 0)
                        {
                            --data.Data.Position;
                            settings.Cipher.Decrypt(settings.SourceSystemTitle, data.Data);
                            // Get command.
                            ch = data.Data.GetUInt8();
                            cmd = (Command)ch;
                            data.Command = cmd;
                        }
                        else
                        {
                            --data.Data.Position;
                        }
                        // Server handles this.
                        break;
                    case Command.GloGetResponse:
                    case Command.GloSetResponse:
                    case Command.GloMethodResponse:
                        if (settings.Cipher == null)
                        {
                            throw new Exception(
                                    "Secure connection is not supported.");
                        }
                        //If all frames are read.
                        if ((data.MoreData & RequestTypes.Frame) == 0)
                        {
                            --data.Data.Position;
                            GXByteBuffer bb = new GXByteBuffer(data.Data);
                            data.Data.Position = data.Data.Size = index;
                            settings.Cipher.Decrypt(settings.SourceSystemTitle, bb);
                            data.Data.Set(bb);
                            data.Command = Command.None;
                            GetPdu(settings, data);
                            data.CipherIndex = data.Data.Size;
                        }                       
                        break;
                    case Command.DataNotification:
                        HandleDataNotification(data);
                        //Client handles this.
                        break;
                    default:
                        throw new ArgumentException("Invalid Command.");
                }
            }
            else if ((data.MoreData & RequestTypes.Frame) == 0)
            {
                // Is whole block is read and if last packet and data is not try to
                // peek.
                if (!data.Peek && data.MoreData == RequestTypes.None)
                {
                    if (data.Command == Command.Aare || data.Command == Command.Aarq)
                    {
                        data.Data.Position = 0;
                    }
                    else
                    {
                        data.Data.Position = 1;
                    }
                    settings.ResetBlockIndex();
                }
                // Get command if operating as a server.
                if (settings.IsServer)
                {
                    //Ciphered messages are handled after whole PDU is received.
                    switch (cmd)
                    {
                        case Command.GloGetRequest:
                        case Command.GloSetRequest:
                        case Command.GloMethodRequest:
                            data.Command = Command.None;
                            data.Data.Position = data.CipherIndex;
                            GetPdu(settings, data);
                            break;
                        default:
                            break;
                    }                    
                }
                else
                {
                    // Client do not need a command any more.
                    data.Command = Command.None;
                    //Ciphered messages are handled after whole PDU is received.
                    switch (cmd)
                    {
                        case Command.GloGetResponse:
                        case Command.GloSetResponse:
                        case Command.GloMethodResponse:
                            data.Data.Position = data.CipherIndex;
                            GetPdu(settings, data);
                            break;
                        default:
                            break;
                    }
                }
            }

            // Get data if all data is read or we want to peek data.
            if (data.Data.Position != data.Data.Size &&
                (cmd == Command.ReadResponse || cmd == Command.GetResponse)
                    && (data.MoreData == RequestTypes.None || data.Peek))
            {
                GetValueFromData(settings, data);
            }
        }

        private static void HandleDataNotification(GXReplyData data)
        {
            int index = data.Data.Position - 1;
            //Get invoke id.
            data.Data.GetUInt32();
            data.Time = DateTime.MinValue;
            int len = data.Data.GetUInt8();
            // If date time is given.
            if (len != 0)
            {
                byte[] tmp = new byte[len];
                data.Data.Get(tmp);
                data.Time = (GXDateTime)GXDLMSClient.ChangeType(tmp, DataType.DateTime);
            }
            GetDataFromBlock(data.Data, index);
        }

        /// <summary>
        /// Get value from the data.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="reply"> Received data.</param>
        internal static void GetValueFromData(GXDLMSSettings settings, GXReplyData reply)
        {
            GXByteBuffer data = reply.Data;
            GXDataInfo info = new GXDataInfo();
            if (reply.Value is Object[])
            {
                info.Type = DataType.Array;
                info.Count = reply.TotalCount;
                info.Index = reply.Count;
            }
            UInt16 index = data.Position;
            data.Position = reply.ReadPosition;
            try
            {
                Object value = GXCommon.GetData(data, info);
                if (value != null)
                {
                    lock (reply)
                    {
                        // If new data.
                        if (!(value is Object[]))
                        {
                            reply.DataType = info.Type;
                            reply.Value = value;
                            reply.TotalCount = 0;
                        }
                        else
                        {
                            if (((Object[])value).Length != 0)
                            {
                                if (reply.Value == null)
                                {
                                    reply.Value = value;
                                }
                                else
                                {
                                    // Add items to collection.
                                    List<Object> list = new List<Object>();
                                    list.AddRange((Object[])reply.Value);
                                    list.AddRange((Object[])value);
                                    reply.Value = list.ToArray();
                                }
                            }
                            reply.ReadPosition = data.Position;
                            // Element count.
                            reply.TotalCount = info.Count;
                        }
                    }
                }
            }
            finally
            {
                data.Position = index;
            }

            // If last data frame of the data block is read.
            if (reply.MoreData == RequestTypes.None)
            {
                // If all blocks are read.
                settings.ResetBlockIndex();
                data.Position = 0;
            }
        }

        public static bool GetData(GXDLMSSettings settings,
                GXByteBuffer reply, GXReplyData data)
        {
            short frame = 0;
            // If DLMS frame is generated.
            if (settings.InterfaceType == InterfaceType.HDLC)
            {
                frame = GetHdlcData(settings.IsServer, settings, reply, data);
            }
            else if (settings.InterfaceType == InterfaceType.WRAPPER)
            {
                GetTcpData(settings, reply, data);
            }
            else
            {
                throw new ArgumentException("Invalid Interface type.");
            }
            // If all data is not read yet.
            if (!data.IsComplete)
            {
                return false;
            }

            GetDataFromFrame(reply, data.Data);
            // If keepalive or get next frame request.
            if ((frame & 0x1) != 0)
            {
                return true;
            }

            GetPdu(settings, data);
            return true;
        }

        /// <summary>
        /// Get data from HDLC or wrapper frame.
        /// </summary>
        /// <param name="reply">Received data that includes HDLC frame.</param>
        /// <param name="data"> Stored data.</param>
        private static void GetDataFromFrame(GXByteBuffer reply, GXByteBuffer data)
        {
            UInt16 offset = data.Size;
            int cnt = reply.Size - reply.Position;
            if (cnt != 0)
            {
                data.Capacity = (UInt16)(offset + cnt);
                data.Set(reply.Data, reply.Position, cnt);
            }
            // Set position to begin of new data.
            data.Position = offset;
        }
     
        /// <summary>
        /// Get data from Block.
        /// </summary>
        /// <param name="data">Stored data block.</param>
        /// <param name="index">Position where data starts.</param>
        /// <returns>Amount of removed bytes.</returns>
        private static int GetDataFromBlock(GXByteBuffer data,
                int index)
        {
            int len = data.Position - index;
            System.Buffer.BlockCopy(data.Data, data.Position, data.Data,
                    data.Position - len, data.Size - data.Position);
            data.Size = (UInt16)(data.Size - len);
            data.Position = (UInt16)(data.Position - len);
            return len;
        }

        /// <summary>
        /// Returns action method information.
        /// </summary>
        /// <param name="objectType">object type.</param>
        /// <param name="value">Starting address of action methods.</param>
        /// <param name="count">Count of action methods</param>
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
                case ObjectType.GprsSetup:
                case ObjectType.IecHdlcSetup:
                case ObjectType.IecLocalPortSetup:
                case ObjectType.IecTwistedPairSetup:
                case ObjectType.ModemConfiguration:
                case ObjectType.PppSetup:
                case ObjectType.RegisterMonitor:
                case ObjectType.SapAssignment:
                case ObjectType.ZigBeeSasStartup:
                case ObjectType.ZigBeeSasJoin:
                case ObjectType.ZigBeeSasApsFragmentation:
                case ObjectType.Schedule:
                case ObjectType.SmtpSetup:
                case ObjectType.StatusMapping:
                case ObjectType.TcpUdpSetup:
                case ObjectType.UtilityTables:
                    value = 0;
                    count = 0;
                    break;
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
    }
}