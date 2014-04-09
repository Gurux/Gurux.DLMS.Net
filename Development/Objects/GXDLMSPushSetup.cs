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

namespace Gurux.DLMS.Objects
{    
    public class GXDLMSPushSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSPushSetup()
            : base(ObjectType.PushSetup)
        {
            CommunicationWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
            SendDestinationAndMethod = new GXSendDestinationAndMethod();
            PushObjectList = new List<GXDLMSPushObject>();
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSPushSetup(string ln)
            : base(ObjectType.PushSetup, ln, 0)
        {
            CommunicationWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
            SendDestinationAndMethod = new GXSendDestinationAndMethod();
            PushObjectList = new List<GXDLMSPushObject>();
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSPushSetup(string ln, ushort sn)
            : base(ObjectType.PushSetup, ln, 0)
        {
            CommunicationWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
            SendDestinationAndMethod = new GXSendDestinationAndMethod();
            PushObjectList = new List<GXDLMSPushObject>();
        }        
        
        /// <summary>
        /// Defines the list of attributes or objects to be pushed. 
        /// Upon a call of the push (data) method the selected attributes are sent to the desti-nation 
        /// defined in send_destination_and_method.
        /// </summary>        
        [XmlIgnore()]
        public List<GXDLMSPushObject> PushObjectList
        {
            get;
            internal set;
        }        

        [XmlIgnore()]
        public GXSendDestinationAndMethod SendDestinationAndMethod
        {
            get;
            internal set;
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
            internal set;
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
            return new object[] { LogicalName, PushObjectList, SendDestinationAndMethod,
            CommunicationWindow, RandomisationStartInterval, NumberOfRetries, RepetitionDelay};
        }

        #region IGXDLMSBase Members

        /// <summary>
        /// Data interface do not have any methods.
        /// </summary>
        /// <param name="index"></param>
        byte[][] IGXDLMSBase.Invoke(object sender, int index, Object parameters)
        {
            throw new ArgumentException("Invoke failed. Invalid attribute index.");
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

        int IGXDLMSBase.GetAttributeCount()
        {
            return 7;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
        }

        override public DataType GetDataType(int index)
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

        object IGXDLMSBase.GetValue(int index, int selector, object parameters)
        {
            if (index == 1)
            {
                return this.LogicalName;
            }
            if (index == 2)
            {
                List<byte> buff = new List<byte>();
                buff.Add((byte)DataType.Array);
                GXCommon.SetObjectCount(PushObjectList.Count, buff);
                foreach (GXDLMSPushObject it in PushObjectList)
                {
                    buff.Add((byte)DataType.Structure);
                    buff.Add(4);
                    GXCommon.SetData(buff, DataType.UInt8, it.Type);
                    GXCommon.SetData(buff, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.LogicalName));
                    GXCommon.SetData(buff, DataType.UInt8, it.AttributeIndex);
                    GXCommon.SetData(buff, DataType.UInt8, it.DataIndex);
                }
                return buff.ToArray();                
            }
            if (index == 3)
            {
                List<byte> buff = new List<byte>();
                buff.Add((byte)DataType.Structure);
                buff.Add(3);
                GXCommon.SetData(buff, DataType.UInt8, SendDestinationAndMethod.Service);
                GXCommon.SetData(buff, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(SendDestinationAndMethod.Destination));
                GXCommon.SetData(buff, DataType.UInt8, SendDestinationAndMethod.Message);
                return buff.ToArray();     
            }
            if (index == 4)
            {
                List<byte> buff = new List<byte>();
                buff.Add((byte)DataType.Array);
                GXCommon.SetObjectCount(CommunicationWindow.Count, buff);
                foreach (KeyValuePair<GXDateTime, GXDateTime> it in CommunicationWindow)
                {
                    buff.Add((byte)DataType.Structure);
                    buff.Add(2);
                    GXCommon.SetData(buff, DataType.DateTime, it.Key);
                    GXCommon.SetData(buff, DataType.DateTime, it.Value);
                }
                return buff.ToArray(); 
            }
            if (index == 5)
            {
                return RandomisationStartInterval;
            }
            if (index == 6)
            {
                return NumberOfRetries;
            }
            if (index == 7)
            {
                return RepetitionDelay;
            }
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
        }

        void IGXDLMSBase.SetValue(int index, object value)
        {
            if (index == 1)
            {
                if (value is string)
                {
                    LogicalName = value.ToString();
                }
                else
                {
                    LogicalName = GXDLMSClient.ChangeType((byte[])value, DataType.OctetString).ToString();
                }                
            }
            else if (index == 2)
            {
                PushObjectList.Clear();
                if (value is Object[])
                {
                    foreach (object it in value as Object[])
                    {
                        Object[] tmp = it as Object[];
                        GXDLMSPushObject obj = new GXDLMSPushObject();
                        obj.Type = (ObjectType)Convert.ToInt32(tmp[0]);
                        obj.LogicalName = GXDLMSClient.ChangeType((byte[])tmp[1], DataType.BitString).ToString();
                        obj.AttributeIndex = Convert.ToInt32(tmp[2]);
                        obj.DataIndex = Convert.ToInt32(tmp[3]);
                        PushObjectList.Add(obj);
                    }
                }
            }
            else if (index == 3)
            {
                object[] tmp = value as object[];
                if (tmp != null)
                {
                    SendDestinationAndMethod.Service = (ServiceType)Convert.ToInt32(tmp[0]);
                    SendDestinationAndMethod.Destination = ASCIIEncoding.ASCII.GetString((byte[]) tmp[1]);
                    SendDestinationAndMethod.Message = (MessageType)Convert.ToInt32(tmp[2]);
                }
            }
            else if (index == 4)
            {
                CommunicationWindow.Clear();
                if (value is Object[])
                {
                    foreach(object it in value as Object[])
                    {
                        Object[] tmp = it as Object[];
                        GXDateTime start = GXDLMSClient.ChangeType((byte[]) tmp[0], DataType.DateTime) as GXDateTime;
                        GXDateTime end = GXDLMSClient.ChangeType((byte[]) tmp[1], DataType.DateTime) as GXDateTime;
                        CommunicationWindow.Add(new KeyValuePair<GXDateTime, GXDateTime>(start, end));
                    }
                }
            }
            else if (index == 5)
            {
                RandomisationStartInterval = (UInt16) value;
            }
            else if (index == 6)
            {
                NumberOfRetries = (byte) value;
            }
            else if (index == 7)
            {
                RepetitionDelay = (UInt16)value;
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }
        #endregion
    }
}
