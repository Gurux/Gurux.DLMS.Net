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
// More information of Gurux products: https://www.gurux.org
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using System.Globalization;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSPushSetup
    /// </summary>
    public class GXDLMSPushSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSPushSetup()
        : this("0.7.25.9.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSPushSetup(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSPushSetup(string ln, ushort sn)
        : base(ObjectType.PushSetup, ln, sn)
        {
            CommunicationWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
            PushObjectList = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
            RepetitionDelay2 = new GXRepetitionDelay();
            ConfirmationParameters = new GXPushConfirmationParameter();
        }

        public ServiceType Service
        {
            get;
            set;
        }
        public string Destination
        {
            get;
            set;
        }
        public MessageType Message
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the list of attributes or objects to be pushed.
        /// Upon a call of the push (data) method the selected attributes are sent to the destination
        /// defined in send_destination_and_method.
        /// </summary>
        public List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> PushObjectList
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the start and end date/time
        /// stamp when the communication window(s) for the push become active
        /// (for the start instant), or inac-tive (for the end instant).
        /// </summary>
        [XmlIgnore()]
        public List<KeyValuePair<GXDateTime, GXDateTime>> CommunicationWindow
        {
            get;
            set;
        }

        /// <summary>
        /// To avoid simultaneous network connections of a lot of devices at ex-actly
        /// the same point in time, a randomisation interval in seconds can be defined.
        /// This means that the push operation is not started imme-diately at the
        /// beginning of the first communication window but started randomly delayed.
        /// </summary>
        [XmlIgnore()]
        public UInt16 RandomisationStartInterval
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum number of retrials in case of unsuccessful push at-tempts. After a successful push no further push attempts are made until the push setup is triggered again.
        /// A value of 0 means no repetitions, i.e. only the initial connection at-tempt is made.
        /// </summary>
        [XmlIgnore()]
        public byte NumberOfRetries
        {
            get;
            set;
        }


        /// <summary>
        /// Repetition delay.
        /// </summary>
        /// <remarks>
        /// Version 2 is using RepetitionDelay2.
        /// </remarks>
        [XmlIgnore()]
        public UInt16 RepetitionDelay
        {
            get;
            set;
        }

        /// <summary>
        /// Repetition delay for Version2.
        /// </summary>
        /// <remarks>
        /// Version 0 and 1 use RepetitionDelay.
        /// </remarks>
        [XmlIgnore()]
        public GXRepetitionDelay RepetitionDelay2
        {
            get;
            set;
        }

        /// <summary>
        /// The logical name of a communication port setup object.
        /// </summary>
        [XmlIgnore()]
        public GXDLMSObject PortReference
        {
            get;
            set;
        }

        /// <summary>
        /// Push client SAP.
        /// </summary>
        [XmlIgnore()]
        public sbyte PushClientSAP
        {
            get;
            set;
        }

        /// <summary>
        /// Push protection parameters.
        /// </summary>
        [XmlIgnore()]
        public GXPushProtectionParameters[] PushProtectionParameters
        {
            get;
            set;
        }

        /// <summary>
        /// Push operation method.
        /// </summary>
        [XmlIgnore()]
        public PushOperationMethod PushOperationMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Push confirmation parameter.
        /// </summary>
        [XmlIgnore()]
        public GXPushConfirmationParameter ConfirmationParameters
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXDateTime LastConfirmationDateTime
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            if (Version < 2)
            {
                return new object[] { LogicalName,
                PushObjectList,
                Service + " " + Destination + " " + Message,
                CommunicationWindow,
                RandomisationStartInterval,
                NumberOfRetries,
                RepetitionDelay,
                PortReference,
                PushClientSAP,
                PushProtectionParameters,
                PushOperationMethod,
                ConfirmationParameters,
                LastConfirmationDateTime};
            }
            return new object[] { LogicalName,
                PushObjectList,
                Service + " " + Destination + " " + Message,
                CommunicationWindow,
                RandomisationStartInterval,
                NumberOfRetries,
                RepetitionDelay2,
                PortReference,
                PushClientSAP,
                PushProtectionParameters,
                PushOperationMethod,
                ConfirmationParameters,
                LastConfirmationDateTime};
        }

        /// <summary>
        /// Get received objects from push message.
        /// </summary>
        /// <param name="values">Received values.</param>
        /// <returns>Clone of captured COSEM objects.</returns>
        public void GetPushValues(
            GXDLMSClient client,
            List<object> values)
        {
            if (values.Count != PushObjectList.Count)
            {
                throw new Exception("Size of the push object list is different than values.");
            }
            int pos = 0;
            List<KeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> objects = new List<KeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
            foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in PushObjectList)
            {
                objects.Add(new KeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(it.Key,
                    new GXDLMSCaptureObject(it.Value.AttributeIndex, it.Value.DataIndex)));
                if (it.Value.AttributeIndex == 0)
                {
                    List<object> tmp = (List<object>)values[pos];
                    for (int index = 1; index <= (it.Key as IGXDLMSBase).GetAttributeCount(); ++index)
                    {
                        client.UpdateValue(it.Key, index, tmp[index - 1]);
                    }
                }
                else
                {
                    client.UpdateValue(it.Key, it.Value.AttributeIndex, values[pos]);
                }
                ++pos;
            }
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                //Only TCP/IP push is allowed at the moment.
                if (Service != ServiceType.Tcp || Message != MessageType.CosemApdu ||
                        PushObjectList.Count == 0)
                {
                    e.Error = ErrorCode.HardwareFault;
                    return null;
                }
                return null;
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        /// <summary>
        /// Activates the push process.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        [Obsolete("Use Push method instead.")]
        public byte[][] Activate(GXDLMSClient client)
        {
            return client.Method(this, 1, (sbyte)0);
        }

        /// <summary>
        /// Activates the push process.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Push(GXDLMSClient client)
        {
            return client.Method(this, 1, (sbyte)0);
        }

        /// <summary>
        /// Reset the push process.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Reset(GXDLMSClient client)
        {
            return client.Method(this, 2, (sbyte)0);
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //PushObjectList
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //SendDestinationAndMethod
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //CommunicationWindow
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //RandomisationStartInterval
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //NumberOfRetries
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //RepetitionDelay
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            if (Version > 0)
            {
                //PortReference
                if (all || CanRead(8))
                {
                    attributes.Add(8);
                }
                //PushClientSAP
                if (all || CanRead(9))
                {
                    attributes.Add(9);
                }
                //PushProtectionParameters
                if (all || CanRead(10))
                {
                    attributes.Add(10);
                }
                if (Version < 1)
                {
                    //PushOperationMethod
                    if (all || CanRead(11))
                    {
                        attributes.Add(11);
                    }
                    //ConfirmationParameters
                    if (all || CanRead(12))
                    {
                        attributes.Add(12);
                    }
                    //LastConfirmationDateTime
                    if (all || CanRead(13))
                    {
                        attributes.Add(13);
                    }
                }
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(),
                "Object List",
                "Send Destination And Method",
                "Communication Window",
                "Randomisation Start Interval",
                "Number Of Retries",
                "Repetition Delay",
                "Port reference",
                "Push client SAP",
                "Push protection parameters",
                "Push operation method",
                "Confirmation parameters",
                "Last confirmation date time"};
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            if (Version < 2)
            {
                return new string[] { "Push" };
            }
            return new string[] { "Push", "Reset" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 2;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            if (Version == 0)
            {
                return 7;
            }
            if (Version == 1)
            {
                return 10;
            }
            return 13;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            if (Version < 2)
            {
                return 1;
            }
            return 2;
        }

        /// <inheritdoc />
        public override DataType GetDataType(int index)
        {
            if (index == 1)
            {
                return DataType.OctetString;
            }
            if (index == 2)
            {
                return DataType.Array;
            }
            if (index == 3)
            {
                return DataType.Structure;
            }
            if (index == 4)
            {
                return DataType.Array;
            }
            if (index == 5)
            {
                return DataType.UInt16;
            }
            if (index == 6)
            {
                return DataType.UInt8;
            }
            if (index == 7)
            {
                if (Version < 2)
                {
                    return DataType.UInt16;
                }
                return DataType.Structure;
            }
            if (Version > 0)
            {
                //PortReference
                if (index == 8)
                {
                    return DataType.OctetString;
                }
                //PushClientSAP
                if (index == 9)
                {
                    return DataType.Int8;
                }
                //PushProtectionParameters
                if (index == 10)
                {
                    return DataType.Array;
                }
                if (Version < 1)
                {
                    //PushOperationMethod
                    if (index == 11)
                    {
                        return DataType.Enum;
                    }
                    //ConfirmationParameters
                    if (index == 12)
                    {
                        return DataType.Structure;
                    }
                    //LastConfirmationDateTime
                    if (index == 13)
                    {
                        return DataType.DateTime;
                    }
                }
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        private object GetPushObjectList(GXDLMSSettings settings)
        {
            GXByteBuffer buff = new GXByteBuffer();
            buff.SetUInt8(DataType.Array);
            GXCommon.SetObjectCount(PushObjectList.Count, buff);
            foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in PushObjectList)
            {
                buff.SetUInt8(DataType.Structure);
                buff.SetUInt8(4);
                GXCommon.SetData(settings, buff, DataType.UInt16, it.Key.ObjectType);
                GXCommon.SetData(settings, buff, DataType.OctetString, GXCommon.LogicalNameToBytes(it.Key.LogicalName));
                GXCommon.SetData(settings, buff, DataType.Int8, it.Value.AttributeIndex);
                GXCommon.SetData(settings, buff, DataType.UInt16, it.Value.DataIndex);
            }
            return buff.Array();
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            GXByteBuffer buff = new GXByteBuffer();
            object ret;
            switch (e.Index)
            {
                case 1:
                    ret = GXCommon.LogicalNameToBytes(LogicalName);
                    break;
                case 2:
                    ret = GetPushObjectList(settings);
                    break;
                case 3:
                    buff.SetUInt8(DataType.Structure);
                    buff.SetUInt8(3);
                    GXCommon.SetData(settings, buff, DataType.Enum, Service);
                    if (Destination != null)
                    {
                        //LN can be used with HDLC
                        if (Service == ServiceType.Hdlc)
                        {
                            try
                            {
                                byte[] tmp = GXCommon.LogicalNameToBytes(Destination);
                                if (tmp.Length == 6 && tmp[5] == 0xFF)
                                {
                                    GXCommon.SetData(settings, buff, DataType.OctetString, tmp);
                                }
                                else
                                {
                                    GXCommon.SetData(settings, buff, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(Destination));
                                }
                            }
                            catch (Exception)
                            {
                                GXCommon.SetData(settings, buff, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(Destination));
                            }
                        }
                        else
                        {
                            GXCommon.SetData(settings, buff, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(Destination));
                        }
                    }
                    else
                    {
                        GXCommon.SetData(settings, buff, DataType.OctetString, null);
                    }
                    GXCommon.SetData(settings, buff, DataType.Enum, Message);
                    ret = buff.Array();
                    break;
                case 4:
                    buff.SetUInt8(DataType.Array);
                    GXCommon.SetObjectCount(CommunicationWindow.Count, buff);
                    foreach (KeyValuePair<GXDateTime, GXDateTime> it in CommunicationWindow)
                    {
                        buff.SetUInt8(DataType.Structure);
                        buff.SetUInt8(2);
                        GXCommon.SetData(settings, buff, DataType.OctetString, it.Key);
                        GXCommon.SetData(settings, buff, DataType.OctetString, it.Value);
                    }
                    ret = buff.Array();
                    break;
                case 5:
                    ret = RandomisationStartInterval;
                    break;
                case 6:
                    ret = NumberOfRetries;
                    break;
                case 7:
                    if (Version < 2)
                    {
                        ret = RepetitionDelay;
                    }
                    else
                    {
                        buff.SetUInt8(DataType.Structure);
                        GXCommon.SetObjectCount(3, buff);
                        GXCommon.SetData(settings, buff, DataType.UInt16, RepetitionDelay2.Min);
                        GXCommon.SetData(settings, buff, DataType.UInt16, RepetitionDelay2.Exponent);
                        GXCommon.SetData(settings, buff, DataType.UInt16, RepetitionDelay2.Max);
                        ret = buff.Array();
                    }
                    break;
                case 8:
                    if (PortReference != null)
                    {
                        ret = GXCommon.LogicalNameToBytes(PortReference.LogicalName);
                    }
                    else
                    {
                        ret = null;
                    }
                    break;
                case 9:
                    ret = PushClientSAP;
                    break;
                case 10:
                    buff.SetUInt8(DataType.Array);
                    GXCommon.SetObjectCount(PushProtectionParameters.Length, buff);
                    foreach (GXPushProtectionParameters it in PushProtectionParameters)
                    {
                        buff.SetUInt8(DataType.Structure);
                        buff.SetUInt8(2);
                        GXCommon.SetData(settings, buff, DataType.Enum, it.ProtectionType);
                        buff.SetUInt8(DataType.Structure);
                        buff.SetUInt8(5);
                        GXCommon.SetData(settings, buff, DataType.OctetString, it.TransactionId);
                        GXCommon.SetData(settings, buff, DataType.OctetString, it.OriginatorSystemTitle);
                        GXCommon.SetData(settings, buff, DataType.OctetString, it.RecipientSystemTitle);
                        GXCommon.SetData(settings, buff, DataType.OctetString, it.OtherInformation);
                        buff.SetUInt8(DataType.Structure);
                        buff.SetUInt8(2);
                        GXCommon.SetData(settings, buff, DataType.Enum, it.KeyInfo.DataProtectionKeyType);
                        buff.SetUInt8(DataType.Structure);
                        if (it.KeyInfo.DataProtectionKeyType == DataProtectionKeyType.Identified)
                        {
                            buff.SetUInt8(1);
                            GXCommon.SetData(settings, buff, DataType.Enum, it.KeyInfo.IdentifiedKey.KeyType);
                        }
                        else if (it.KeyInfo.DataProtectionKeyType == DataProtectionKeyType.Wrapped)
                        {
                            buff.SetUInt8(2);
                            GXCommon.SetData(settings, buff, DataType.Enum, it.KeyInfo.WrappedKey.KeyType);
                            GXCommon.SetData(settings, buff, DataType.OctetString, it.KeyInfo.WrappedKey.Key);
                        }
                        else if (it.KeyInfo.DataProtectionKeyType == DataProtectionKeyType.Agreed)
                        {
                            buff.SetUInt8(2);
                            GXCommon.SetData(settings, buff, DataType.OctetString, it.KeyInfo.AgreedKey.Parameters);
                            GXCommon.SetData(settings, buff, DataType.OctetString, it.KeyInfo.AgreedKey.Data);
                        }
                    }
                    ret = buff.Array();
                    break;
                case 11:
                    ret = PushOperationMethod;
                    break;
                case 12:
                    buff.SetUInt8(DataType.Structure);
                    GXCommon.SetObjectCount(2, buff);
                    GXCommon.SetData(settings, buff, DataType.DateTime, ConfirmationParameters.StartDate);
                    GXCommon.SetData(settings, buff, DataType.UInt32, ConfirmationParameters.Interval);
                    ret = buff.Array();
                    break;
                case 13:
                    ret = LastConfirmationDateTime;
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    ret = null;
                    break;
            }
            return ret;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                LogicalName = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 2)
            {
                PushObjectList.Clear();
                List<object> it;
                if (e.Value != null)
                {
                    foreach (object t in (IEnumerable<object>)e.Value)
                    {
                        if (t is List<object>)
                        {
                            it = (List<object>)t;
                        }
                        else
                        {
                            it = new List<object>((object[])t);
                        }
                        ObjectType type = (ObjectType)Convert.ToUInt16(it[0]);
                        String ln = GXCommon.ToLogicalName(it[1]);
                        GXDLMSObject obj = settings.Objects.FindByLN(type, ln);
                        if (obj == null)
                        {
                            obj = GXDLMSClient.CreateObject(type);
                            obj.LogicalName = ln;
                        }
                        GXDLMSCaptureObject co = new GXDLMSCaptureObject(Convert.ToInt32(it[2]), Convert.ToInt32(it[3]));
                        PushObjectList.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(obj, co));
                    }
                }
            }
            else if (e.Index == 3)
            {
                List<object> tmp = null;
                if (e.Value is List<object>)
                {
                    tmp = (List<object>)e.Value;
                }
                else
                {
                    tmp = new List<object>((object[])e.Value);
                }
                if (tmp != null)
                {
                    Service = (ServiceType)Convert.ToInt32(tmp[0]);
                    //LN can be used with HDLC
                    if (Service == ServiceType.Hdlc && ((byte[])tmp[1]).Length == 6 && ((byte[])tmp[1])[5] == 0xFF)
                    {
                        Destination = GXCommon.ToLogicalName((byte[])tmp[1]);
                    }
                    else
                    {
                        object str = GXDLMSClient.ChangeType((byte[])tmp[1], DataType.String, settings.UseUtc2NormalTime);
                        if (str is string)
                        {
                            Destination = (string)str;
                        }
                        else
                        {
                            Destination = GXCommon.ToHex((byte[])tmp[1], true);
                        }
                    }
                    Message = (MessageType)Convert.ToInt32(tmp[2]);
                }
            }
            else if (e.Index == 4)
            {
                CommunicationWindow.Clear();
                if (e.Value is List<object>)
                {
                    foreach (List<object> it in e.Value as List<object>)
                    {
                        GXDateTime start = GXDLMSClient.ChangeType((byte[])it[0], DataType.DateTime, settings.UseUtc2NormalTime) as GXDateTime;
                        GXDateTime end = GXDLMSClient.ChangeType((byte[])it[1], DataType.DateTime, settings.UseUtc2NormalTime) as GXDateTime;
                        CommunicationWindow.Add(new KeyValuePair<GXDateTime, GXDateTime>(start, end));
                    }
                }
            }
            else if (e.Index == 5)
            {
                RandomisationStartInterval = (UInt16)e.Value;
            }
            else if (e.Index == 6)
            {
                NumberOfRetries = (byte)e.Value;
            }
            else if (e.Index == 7)
            {
                if (Version < 2)
                {
                    RepetitionDelay = (UInt16)e.Value;
                }
                else
                {
                    if (e.Value is GXStructure s)
                    {
                        RepetitionDelay2.Min = (UInt16)s[0];
                        RepetitionDelay2.Exponent = (UInt16)s[1];
                        RepetitionDelay2.Max = (UInt16)s[2];
                    }
                }
            }
            else if (Version > 0 && e.Index == 8)
            {
                PortReference = null;
                if (e.Value is byte[] bv)
                {
                    string ln = GXCommon.ToLogicalName(bv);
                    PortReference = settings.Objects.FindByLN(ObjectType.None, ln);                    
                }
            }
            else if (Version > 0 && e.Index == 9)
            {
                PushClientSAP = (sbyte)e.Value;
            }
            else if (Version > 0 && e.Index == 10)
            {
                List<GXPushProtectionParameters> list = new List<GXPushProtectionParameters>();
                if (e.Value != null)
                {
                    foreach (GXStructure it in (GXArray)e.Value)
                    {
                        GXPushProtectionParameters p = new GXPushProtectionParameters();
                        p.ProtectionType = (ProtectionType)Convert.ToInt32(it[0]);
                        GXStructure options = (GXStructure) it[1];
                        p.TransactionId = (byte[])options[0];
                        p.OriginatorSystemTitle = (byte[])options[1];
                        p.RecipientSystemTitle = (byte[])options[2];
                        p.OtherInformation = (byte[])options[3];
                        GXStructure keyInfo = (GXStructure)options[4];
                        p.KeyInfo.DataProtectionKeyType = (DataProtectionKeyType)Convert.ToInt32(keyInfo[0]);
                        if (p.KeyInfo.DataProtectionKeyType == DataProtectionKeyType.Identified)
                        {
                            GXStructure identified = (GXStructure)keyInfo[1];
                            p.KeyInfo.IdentifiedKey.KeyType = (DataProtectionIdentifiedKeyType)Convert.ToInt32(identified[0]);
                        }
                        else if (p.KeyInfo.DataProtectionKeyType == DataProtectionKeyType.Wrapped)
                        {
                            GXStructure wrapped = (GXStructure)keyInfo[1];
                            p.KeyInfo.WrappedKey.KeyType = (DataProtectionWrappedKeyType)Convert.ToInt32(wrapped[0]);
                            p.KeyInfo.WrappedKey.Key = (byte[])wrapped[1];
                        }
                        else if (p.KeyInfo.DataProtectionKeyType == DataProtectionKeyType.Agreed)
                        {
                            GXStructure agreed  = (GXStructure)keyInfo[1];
                            p.KeyInfo.AgreedKey.Parameters = (byte[])agreed[0];
                            p.KeyInfo.AgreedKey.Data = (byte[])agreed[1];
                        }
                        list.Add(p);
                    }
                }
                PushProtectionParameters = list.ToArray();
            }
            else if (Version > 1 && e.Index == 11)
            {
                PushOperationMethod = (PushOperationMethod)Convert.ToInt32(e.Value);
            }
            else if (Version > 1 && e.Index == 12)
            {
                List<object> it;
                if (e.Value != null)
                {
                    if (e.Value is List<object>)
                    {
                        it = (List<object>)e.Value;
                    }
                    else
                    {
                        it = new List<object>((object[])e.Value);
                    }
                    ConfirmationParameters.StartDate = (GXDateTime)it[0];
                    ConfirmationParameters.Interval = Convert.ToUInt32(it[0]);
                }
            }
            else if (Version > 1 && e.Index == 13)
            {
                LastConfirmationDateTime = (GXDateTime)e.Value;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            PushObjectList.Clear();
            if (reader.IsStartElement("ObjectList", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    ObjectType ot = (ObjectType)reader.ReadElementContentAsInt("ObjectType");
                    string ln = reader.ReadElementContentAsString("LN");
                    int ai = reader.ReadElementContentAsInt("AI");
                    int di = reader.ReadElementContentAsInt("DI");
                    reader.ReadEndElement("Item");
                    GXDLMSCaptureObject co = new GXDLMSCaptureObject(ai, di);
                    GXDLMSObject obj = reader.Objects.FindByLN(ot, ln);
                    if (obj == null)
                    {
                        obj = GXDLMSClient.CreateObject(ot);
                        obj.LogicalName = ln;
                    }
                    PushObjectList.Add(new GXKeyValuePair<Objects.GXDLMSObject, Objects.GXDLMSCaptureObject>(obj, co));
                }
                reader.ReadEndElement("ObjectList");
            }

            Service = (ServiceType)reader.ReadElementContentAsInt("Service");
            Destination = reader.ReadElementContentAsString("Destination");
            Message = (MessageType)reader.ReadElementContentAsInt("Message");
            CommunicationWindow.Clear();
            if (reader.IsStartElement("CommunicationWindow", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDateTime start = new GXDateTime(reader.ReadElementContentAsString("Start"), CultureInfo.InvariantCulture);
                    GXDateTime end = new GXDateTime(reader.ReadElementContentAsString("End"), CultureInfo.InvariantCulture);
                    CommunicationWindow.Add(new KeyValuePair<DLMS.GXDateTime, DLMS.GXDateTime>(start, end));
                }
                reader.ReadEndElement("CommunicationWindow");
            }
            RandomisationStartInterval = (ushort)reader.ReadElementContentAsInt("RandomisationStartInterval");
            NumberOfRetries = (byte)reader.ReadElementContentAsInt("NumberOfRetries");
            if (Version < 2)
            {
                RepetitionDelay = (ushort)reader.ReadElementContentAsInt("RepetitionDelay");
            }
            else
            {
                if (reader.IsStartElement("RepetitionDelay", true))
                {
                    RepetitionDelay2.Min = (ushort)reader.ReadElementContentAsInt("Min");
                    RepetitionDelay2.Exponent = (ushort)reader.ReadElementContentAsInt("Exponent");
                    RepetitionDelay2.Max = (ushort)reader.ReadElementContentAsInt("Max");
                }
                reader.ReadEndElement("RepetitionDelay");
            }
            if (Version > 0)
            {
                PortReference = null;
                string ln = reader.ReadElementContentAsString("PortReference");
                PortReference = reader.Objects.FindByLN(ObjectType.None, ln);
                if (PortReference == null)
                {
                    PortReference = GXDLMSClient.CreateObject(ObjectType.IecHdlcSetup);
                    PortReference.LogicalName = ln;
                }
                PushClientSAP = (sbyte)reader.ReadElementContentAsInt("PushClientSAP");
                if (reader.IsStartElement("PushProtectionParameters", true))
                {
                    List<GXPushProtectionParameters> list = new List<GXPushProtectionParameters>();
                    while (reader.IsStartElement("Item", true))
                    {
                        GXPushProtectionParameters it = new GXPushProtectionParameters();
                        it.ProtectionType = (ProtectionType)reader.ReadElementContentAsInt("ProtectionType");
                        it.TransactionId = GXCommon.HexToBytes(reader.ReadElementContentAsString("TransactionId"));
                        it.OriginatorSystemTitle = GXCommon.HexToBytes(reader.ReadElementContentAsString("OriginatorSystemTitle"));
                        it.RecipientSystemTitle = GXCommon.HexToBytes(reader.ReadElementContentAsString("RecipientSystemTitle"));
                        it.OtherInformation = GXCommon.HexToBytes(reader.ReadElementContentAsString("OtherInformation"));
                        it.KeyInfo.DataProtectionKeyType = (DataProtectionKeyType)reader.ReadElementContentAsInt("DataProtectionKeyType");
                        it.KeyInfo.IdentifiedKey.KeyType = (DataProtectionIdentifiedKeyType)reader.ReadElementContentAsInt("IdentifiedKey");
                        it.KeyInfo.WrappedKey.KeyType = (DataProtectionWrappedKeyType)reader.ReadElementContentAsInt("WrappedKeyType");
                        it.KeyInfo.WrappedKey.Key = GXCommon.HexToBytes(reader.ReadElementContentAsString("WrappedKey"));
                        it.KeyInfo.AgreedKey.Parameters = GXCommon.HexToBytes(reader.ReadElementContentAsString("WrappedKeyParameters"));
                        it.KeyInfo.AgreedKey.Data = GXCommon.HexToBytes(reader.ReadElementContentAsString("AgreedKeyData"));
                        list.Add(it);
                    }
                    reader.ReadEndElement("PushProtectionParameters");
                    PushProtectionParameters = list.ToArray();
                }
                if (Version > 1)
                {
                    PushOperationMethod = (PushOperationMethod)reader.ReadElementContentAsInt("PushOperationMethod");
                    ConfirmationParameters.StartDate = reader.ReadElementContentAsDateTime("ConfirmationParametersStartDate");
                    ConfirmationParameters.Interval = (UInt32)reader.ReadElementContentAsInt("ConfirmationParametersInterval");
                    LastConfirmationDateTime = reader.ReadElementContentAsDateTime("LastConfirmationDateTime");
                }
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (PushObjectList != null)
            {
                writer.WriteStartElement("ObjectList", 2);
                foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in PushObjectList)
                {
                    writer.WriteStartElement("Item", 0);
                    writer.WriteElementString("ObjectType", (int)it.Key.ObjectType, 0);
                    writer.WriteElementString("LN", it.Key.LogicalName, 0);
                    writer.WriteElementString("AI", it.Value.AttributeIndex, 0);
                    writer.WriteElementString("DI", it.Value.DataIndex, 0);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteElementString("Service", (int)Service, 3);
            writer.WriteElementString("Destination", Destination, 3);
            writer.WriteElementString("Message", (int)Message, 3);
            if (CommunicationWindow != null)
            {
                writer.WriteStartElement("CommunicationWindow", 4);
                foreach (KeyValuePair<GXDateTime, GXDateTime> it in CommunicationWindow)
                {
                    writer.WriteStartElement("Item", 0);
                    writer.WriteElementString("Start", it.Key, 0);
                    writer.WriteElementString("End", it.Value, 0);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteElementString("RandomisationStartInterval", RandomisationStartInterval, 5);
            writer.WriteElementString("NumberOfRetries", NumberOfRetries, 6);
            if (Version < 2)
            {
                writer.WriteElementString("RepetitionDelay", RepetitionDelay, 7);
            }
            else
            {
                writer.WriteStartElement("RepetitionDelay", 7);
                writer.WriteElementString("Min", RepetitionDelay2.Min, 0);
                writer.WriteElementString("Exponent", RepetitionDelay2.Exponent, 0);
                writer.WriteElementString("Max", RepetitionDelay2.Max, 0);
                writer.WriteEndElement();
            }
            if (Version > 0)
            {
                if (PortReference != null)
                {
                    writer.WriteElementString("PortReference", PortReference.LogicalName, 8);
                }
                writer.WriteElementString("PushClientSAP", PushClientSAP, 9);
                if (PushProtectionParameters != null)
                {
                    writer.WriteStartElement("PushProtectionParameters", 10);
                    foreach (var it in PushProtectionParameters)
                    {
                        writer.WriteStartElement("Item", 0);
                        writer.WriteElementString("ProtectionType", (byte)it.ProtectionType, 0);
                        writer.WriteElementString("TransactionId", GXCommon.ToHex(it.TransactionId, false), 0);
                        writer.WriteElementString("OriginatorSystemTitle", GXCommon.ToHex(it.OriginatorSystemTitle, false), 0);
                        writer.WriteElementString("RecipientSystemTitle", GXCommon.ToHex(it.RecipientSystemTitle, false), 0);
                        writer.WriteElementString("OtherInformation", GXCommon.ToHex(it.OtherInformation, false), 0);
                        writer.WriteElementString("DataProtectionKeyType", (int)it.KeyInfo.DataProtectionKeyType, 0);
                        writer.WriteElementString("IdentifiedKey", (int)it.KeyInfo.IdentifiedKey.KeyType, 0);
                        writer.WriteElementString("WrappedKeyType", (int)it.KeyInfo.WrappedKey.KeyType, 0);
                        writer.WriteElementString("WrappedKey", GXCommon.ToHex(it.KeyInfo.WrappedKey.Key, false), 0);
                        writer.WriteElementString("WrappedKeyParameters", GXCommon.ToHex(it.KeyInfo.AgreedKey.Parameters, false), 0);
                        writer.WriteElementString("AgreedKeyData", GXCommon.ToHex(it.KeyInfo.AgreedKey.Data, false), 0);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                if (Version > 1)
                {
                    writer.WriteElementString("PushOperationMethod", (int)PushOperationMethod, 11);
                    writer.WriteElementString("ConfirmationParametersStartDate", ConfirmationParameters.StartDate, 12);
                    writer.WriteElementString("ConfirmationParametersInterval", ConfirmationParameters.Interval, 12);
                    writer.WriteElementString("LastConfirmationDateTime", LastConfirmationDateTime, 13);
                }
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
            //Update port reference.
            if (PortReference != null)
            {
                GXDLMSObject target = (GXDLMSObject)reader.Objects.FindByLN(ObjectType.None,
                    PortReference.LogicalName);
                if (target != null && target != PortReference)
                {
                    PortReference = target;
                }
            }
        }

        #endregion
    }
}
