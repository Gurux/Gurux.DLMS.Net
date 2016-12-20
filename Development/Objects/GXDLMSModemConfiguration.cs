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
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSModemConfiguration : GXDLMSObject, IGXDLMSBase
    {
        static string[] DefaultProfiles()
        {
            return new string[] {"OK", "CONNECT", "RING", "NO CARRIER", "ERROR", "CONNECT 1200", "NO DIAL TONE",
                             "BUSY", "NO ANSWER", "CONNECT 600", "CONNECT 2400", "CONNECT 4800", "CONNECT 9600",
                             "CONNECT 14 400", "CONNECT 28 800", "CONNECT 33 600", "CONNECT 56 000"
                            };
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSModemConfiguration()
        : base(ObjectType.ModemConfiguration, "0.0.2.0.0.255", 0)
        {
            InitialisationStrings = new GXDLMSModemInitialisation[0];
            CommunicationSpeed = BaudRate.Baudrate300;
            ModemProfile = DefaultProfiles();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSModemConfiguration(string ln)
        : base(ObjectType.ModemConfiguration, ln, 0)
        {
            InitialisationStrings = new GXDLMSModemInitialisation[0];
            CommunicationSpeed = BaudRate.Baudrate300;
            ModemProfile = DefaultProfiles();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSModemConfiguration(string ln, ushort sn)
        : base(ObjectType.ModemConfiguration, ln, sn)
        {
            InitialisationStrings = new GXDLMSModemInitialisation[0];
            CommunicationSpeed = BaudRate.Baudrate300;
            ModemProfile = DefaultProfiles();
        }

        [XmlIgnore()]
        public BaudRate CommunicationSpeed
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXDLMSModemInitialisation[] InitialisationStrings
        {
            get;
            set;
        }

        [XmlIgnore()]
        public string[] ModemProfile
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, CommunicationSpeed, InitialisationStrings, ModemProfile };
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
            //CommunicationSpeed
            if (!base.IsRead(2))
            {
                attributes.Add(2);
            }
            //InitialisationStrings
            if (!base.IsRead(3))
            {
                attributes.Add(3);
            }
            //ModemProfile
            if (!base.IsRead(4))
            {
                attributes.Add(4);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Gurux.DLMS.Properties.Resources.LogicalNameTxt, "Communication Speed",
                              "Initialisation Strings", "Modem Profile"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 4;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        public override DataType GetDataType(int index)
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
                return DataType.Array;
            }
            if (index == 4)
            {
                return DataType.Array;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return this.LogicalName;
            }
            if (e.Index == 2)
            {
                return CommunicationSpeed;
            }
            if (e.Index == 3)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                //Add count
                int cnt = 0;
                if (InitialisationStrings != null)
                {
                    cnt = InitialisationStrings.Length;
                }
                GXCommon.SetObjectCount(cnt, data);
                if (cnt != 0)
                {
                    foreach (GXDLMSModemInitialisation it in InitialisationStrings)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8((byte)3); //Count
                        GXCommon.SetData(settings, data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Request));
                        GXCommon.SetData(settings, data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Response));
                        GXCommon.SetData(settings, data, DataType.UInt16, it.Delay);
                    }
                }
                return data.Array();
            }
            if (e.Index == 4)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                //Add count
                int cnt = 0;
                if (ModemProfile != null)
                {
                    cnt = ModemProfile.Length;
                }
                GXCommon.SetObjectCount(cnt, data);
                if (cnt != 0)
                {
                    foreach (string it in ModemProfile)
                    {
                        GXCommon.SetData(settings, data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it));
                    }
                }
                return data;
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
                CommunicationSpeed = (BaudRate)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 3)
            {
                InitialisationStrings = null;
                if (e.Value != null)
                {
                    List<GXDLMSModemInitialisation> items = new List<GXDLMSModemInitialisation>();
                    foreach (object[] it in (Object[])e.Value)
                    {
                        GXDLMSModemInitialisation item = new GXDLMSModemInitialisation();
                        item.Request = GXDLMSClient.ChangeType((byte[])it[0], DataType.String).ToString();
                        item.Response = GXDLMSClient.ChangeType((byte[])it[1], DataType.String).ToString();
                        if (it.Length > 2)
                        {
                            item.Delay = Convert.ToUInt16(it[2]);
                        }
                        items.Add(item);
                    }
                    InitialisationStrings = items.ToArray();
                }
            }
            else if (e.Index == 4)
            {
                ModemProfile = null;
                if (e.Value != null)
                {
                    List<string> items = new List<string>();
                    foreach (object it in (Object[])e.Value)
                    {
                        items.Add(GXDLMSClient.ChangeType((byte[])it, DataType.String).ToString());
                    }
                    ModemProfile = items.ToArray();
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        #endregion
    }
}
