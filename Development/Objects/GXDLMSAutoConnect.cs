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
    /// <summary>
    /// Auto Connect implements data transfer from the device to one or several destinations.
    /// </summary>
    public class GXDLMSAutoConnect : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSAutoConnect()
            : base(ObjectType.AutoConnect, "0.0.2.1.0.255", 0)
        {
            CallingWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSAutoConnect(string ln)
            : base(ObjectType.AutoConnect, ln, 0)
        {
            CallingWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSAutoConnect(string ln, ushort sn)
            : base(ObjectType.AutoConnect, ln, 0)
        {
            CallingWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
        }

        /// <summary>
        /// Defines the mode controlling the auto dial functionality concerning the
        /// timing, the message type to be sent and the infrastructure to be used.
        /// </summary>
        [XmlIgnore()]        
        public AutoConnectMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum number of trials in the case of unsuccessful dialling attempts.
        /// </summary>
        [XmlIgnore()]        
        public int Repetitions
        {
            get;
            set;
        }

        /// <summary>
        /// The time delay, expressed in seconds until an unsuccessful dial attempt can be repeated.
        /// </summary>
        [XmlIgnore()]        
        public int RepetitionDelay
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the start and end date/time stamp when the window becomes active.
        /// </summary>
        [XmlIgnore()]
        public List<KeyValuePair<GXDateTime, GXDateTime>> CallingWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the list of destinations (for example phone numbers, email 
        /// addresses or their combinations) where the message(s) have to be sent 
        /// under certain conditions. The conditions and their link to the elements of 
        /// the array are not defined here.
        /// </summary>
        [XmlIgnore()]        
        public string[] Destinations
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Mode, Repetitions, RepetitionDelay, 
                CallingWindow, Destinations };
        }

        #region IGXDLMSBase Members

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //Mode
            if (CanRead(2))
            {
                attributes.Add(2);
            }
            //Repetitions
            if (CanRead(3))
            {
                attributes.Add(3);
            }
            //RepetitionDelay
            if (CanRead(4))
            {
                attributes.Add(4);
            }
            //CallingWindow
            if (CanRead(5))
            {
                attributes.Add(5);
            }
            //Destinations
            if (CanRead(6))
            {
                attributes.Add(6);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Gurux.DLMS.Properties.Resources.LogicalNameTxt, 
                "Mode", 
                "Repetitions", 
                "Repetition Delay", 
                "Calling Window", 
                "Destinations"};
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 6;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        override public DataType GetDataType(int index)
        {
            if (index == 1)
            {
                return DataType.OctetString;
            }
            if (index == 2)
            {
                return DataType.Enum;
            }
            if (index == 3)
            {
                return DataType.UInt8;
            }
            if (index == 4)
            {
                return DataType.UInt16;
            }
            if (index == 5)
            {
                return DataType.Array;
            }
            if (index == 6)
            {
                return DataType.Array;
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
                return (byte) Mode;
            }
            if (index == 3)
            {
                return Repetitions;
            }
            if (index == 4)
            {
                return RepetitionDelay;
            }
            if (index == 5)
            {
                int cnt = CallingWindow.Count;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
                //Add count            
                GXCommon.SetObjectCount(cnt, data);
                if (cnt != 0)
                {
                    foreach (var it in CallingWindow)
                    {
                        data.Add((byte)DataType.Structure);
                        data.Add((byte)2); //Count
                        GXCommon.SetData(data, DataType.OctetString, it.Key); //start_time
                        GXCommon.SetData(data, DataType.OctetString, it.Value); //end_time
                    }
                }
                return data.ToArray();
            }
            if (index == 6)
            {
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
                if (Destinations == null)
                {
                    //Add count            
                    GXCommon.SetObjectCount(0, data);
                }
                else
                {
                    int cnt = Destinations.Length;                                        
                    //Add count            
                    GXCommon.SetObjectCount(cnt, data);
                    foreach (string it in Destinations)
                    {
                        GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it)); //destination
                    }
                }
                return data.ToArray();
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
                Mode = (AutoConnectMode) Convert.ToInt32(value);
            }
            else if (index == 3)
            {
                Repetitions = Convert.ToInt32(value);
            }
            else if (index == 4)
            {
                RepetitionDelay = Convert.ToInt32(value);
            }
            else if (index == 5)
            {
                CallingWindow.Clear();
                if (value != null)
                {
                    foreach (Object[] item in (Object[])value)
                    {
                        GXDateTime start = (GXDateTime)GXDLMSClient.ChangeType((byte[])item[0], DataType.DateTime);
                        GXDateTime end = (GXDateTime)GXDLMSClient.ChangeType((byte[])item[1], DataType.DateTime);
                        CallingWindow.Add(new KeyValuePair<GXDateTime, GXDateTime>(start, end));
                    }
                }
            }
            else if (index == 6)
            {
                Destinations = null;
                if (value != null)
                {
                    List<string> items = new List<string>();
                    foreach (byte[] item in (object[])value)
                    {
                        string it = GXDLMSClient.ChangeType(item, DataType.String).ToString();
                        items.Add(it);
                    }
                    Destinations = items.ToArray();
                }
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }

        byte[][] IGXDLMSBase.Invoke(object sender, int index, Object parameters)
        {
            throw new ArgumentException("Invoke failed. Invalid attribute index.");
        }
        #endregion
    }
}
