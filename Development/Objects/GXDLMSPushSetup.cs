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
using System.Text;
using Gurux.DLMS;
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
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

        #region IGXDLMSBase Members

        /// <summary>
        /// Push interface do not have any methods.
        /// </summary>
        /// <param name="index"></param>
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
        /// <returns></returns>
        public byte[][] Activate(GXDLMSClient client)
        {
            return client.Method(this, 1, (byte)0);
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //PushObjectList
            if (CanRead(2))
            {
                attributes.Add(2);
            }
            //SendDestinationAndMethod
            if (CanRead(3))
            {
                attributes.Add(3);
            }
            //CommunicationWindow
            if (CanRead(4))
            {
                attributes.Add(4);
            }
            //RandomisationStartInterval
            if (CanRead(5))
            {
                attributes.Add(5);
            }
            //NumberOfRetries
            if (CanRead(6))
            {
                attributes.Add(6);
            }
            //RepetitionDelay
            if (CanRead(7))
            {
                attributes.Add(7);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Gurux.DLMS.Properties.Resources.LogicalNameTxt, "Push Object List",
                              "Send Destination And Method", "Communication Window", "Randomisation Start Interval", "Number Of Retries", "Repetition Delay"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 7;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
        }

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

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return this.LogicalName;
            }
            GXByteBuffer buff = new GXByteBuffer();
            if (e.Index == 2)
            {
                buff.SetUInt8(DataType.Array);
                GXCommon.SetObjectCount(PushObjectList.Count, buff);
                foreach (KeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in PushObjectList)
                {
                    buff.SetUInt8(DataType.Structure);
                    buff.SetUInt8(4);
                    GXCommon.SetData(buff, DataType.UInt16, it.Key.ObjectType);
                    GXCommon.SetData(buff, DataType.OctetString, it.Key.LogicalName);
                    GXCommon.SetData(buff, DataType.Int8, it.Value.AttributeIndex);
                    GXCommon.SetData(buff, DataType.UInt16, it.Value.DataIndex);
                }
                return buff.Array();
            }
            if (e.Index == 3)
            {
                buff.SetUInt8(DataType.Structure);
                buff.SetUInt8(3);
                GXCommon.SetData(buff, DataType.UInt8, Service);
                if (Destination != null)
                {
                    GXCommon.SetData(buff, DataType.OctetString, Destination);
                }
                else
                {
                    GXCommon.SetData(buff, DataType.OctetString, null);
                }
                GXCommon.SetData(buff, DataType.UInt8, Message);
                return buff.Array();
            }
            if (e.Index == 4)
            {
                buff.SetUInt8(DataType.Array);
                GXCommon.SetObjectCount(CommunicationWindow.Count, buff);
                foreach (KeyValuePair<GXDateTime, GXDateTime> it in CommunicationWindow)
                {
                    buff.SetUInt8(DataType.Structure);
                    buff.SetUInt8(2);
                    GXCommon.SetData(buff, DataType.OctetString, it.Key);
                    GXCommon.SetData(buff, DataType.OctetString, it.Value);
                }
                return buff.Array();
            }
            if (e.Index == 5)
            {
                return RandomisationStartInterval;
            }
            if (e.Index == 6)
            {
                return NumberOfRetries;
            }
            if (e.Index == 7)
            {
                return RepetitionDelay;
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                if (e.Value is string)
                {
                    LogicalName = e.Value.ToString();
                }
                else
                {
                    LogicalName = GXDLMSClient.ChangeType((byte[])e.Value, DataType.OctetString).ToString();
                }
            }
            else if (e.Index == 2)
            {
                PushObjectList.Clear();
                if (e.Value is Object[])
                {
                    foreach (object it in e.Value as Object[])
                    {
                        Object[] tmp = it as Object[];
                        ObjectType type = (ObjectType)Convert.ToUInt16(tmp[0]);
                        String ln = GXDLMSClient.ChangeType((byte[])tmp[1], DataType.OctetString).ToString();
                        GXDLMSObject obj = settings.Objects.FindByLN(type, ln);
                        if (obj == null)
                        {
                            obj = GXDLMSClient.CreateObject(type);
                            obj.LogicalName = ln;
                        }
                        GXDLMSCaptureObject co = new GXDLMSCaptureObject(Convert.ToInt32(tmp[2]), Convert.ToInt32(tmp[3]));
                        PushObjectList.Add(new KeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(obj, co));
                    }
                }
            }
            else if (e.Index == 3)
            {
                object[] tmp = e.Value as object[];
                if (tmp != null)
                {
                    Service = (ServiceType)Convert.ToInt32(tmp[0]);
                    Destination = (string)GXDLMSClient.ChangeType((byte[])tmp[1], DataType.OctetString);
                    Message = (MessageType)Convert.ToInt32(tmp[2]);
                }
            }
            else if (e.Index == 4)
            {
                CommunicationWindow.Clear();
                if (e.Value is Object[])
                {
                    foreach (object it in e.Value as Object[])
                    {
                        Object[] tmp = it as Object[];
                        GXDateTime start = GXDLMSClient.ChangeType((byte[])tmp[0], DataType.DateTime) as GXDateTime;
                        GXDateTime end = GXDLMSClient.ChangeType((byte[])tmp[1], DataType.DateTime) as GXDateTime;
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
        #endregion
    }
}
