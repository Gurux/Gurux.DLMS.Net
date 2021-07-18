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
            PushObjectList = new List<KeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
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
        /// Upon a call of the push (data) method the selected attributes are sent to the desti-nation
        /// defined in send_destination_and_method.
        /// </summary>
        [XmlIgnore()]
        public List<KeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> PushObjectList
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

        [XmlIgnore()]
        public UInt16 RepetitionDelay
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, PushObjectList, Service + " " + Destination + " " + Message,
                              CommunicationWindow, RandomisationStartInterval, NumberOfRetries, RepetitionDelay
                            };
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
            foreach (KeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in PushObjectList)
            {
                objects.Add(new KeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(it.Key,
                    new GXDLMSCaptureObject(it.Value.AttributeIndex, it.Value.DataIndex)));
                client.UpdateValue(it.Key, it.Value.AttributeIndex, values[pos]);
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
        public byte[][] Activate(GXDLMSClient client)
        {
            return client.Method(this, 1, (sbyte)0);
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
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Object List",
                              "Send Destination And Method", "Communication Window", "Randomisation Start Interval", "Number Of Retries", "Repetition Delay"
                            };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Push" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 7;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
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
                return DataType.UInt16;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        private object GetPushObjectList(GXDLMSSettings settings)
        {
            GXByteBuffer buff = new GXByteBuffer();
            buff.SetUInt8(DataType.Array);
            GXCommon.SetObjectCount(PushObjectList.Count, buff);
            foreach (KeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in PushObjectList)
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
                        GXCommon.SetData(settings, buff, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(Destination));
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
                    ret = RepetitionDelay;
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
                        PushObjectList.Add(new KeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(obj, co));
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
                RepetitionDelay = (UInt16)e.Value;
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
                    PushObjectList.Add(new KeyValuePair<Objects.GXDLMSObject, Objects.GXDLMSCaptureObject>(obj, co));
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
            RepetitionDelay = (ushort)reader.ReadElementContentAsInt("RepetitionDelay");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (PushObjectList != null)
            {
                writer.WriteStartElement("ObjectList", 2);
                foreach (KeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in PushObjectList)
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
            writer.WriteElementString("Destination", Destination, 4);
            writer.WriteElementString("Message", (int)Message, 5);
            if (CommunicationWindow != null)
            {
                writer.WriteStartElement("CommunicationWindow", 6);
                foreach (KeyValuePair<GXDateTime, GXDateTime> it in CommunicationWindow)
                {
                    writer.WriteStartElement("Item", 0);
                    writer.WriteElementString("Start", it.Key, 0);
                    writer.WriteElementString("End", it.Value, 0);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteElementString("RandomisationStartInterval", RandomisationStartInterval, 7);
            writer.WriteElementString("NumberOfRetries", NumberOfRetries, 8);
            writer.WriteElementString("RepetitionDelay", RepetitionDelay, 9);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
