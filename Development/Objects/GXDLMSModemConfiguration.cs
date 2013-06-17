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
    public class GXDLMSModemConfiguration : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSModemConfiguration()
            : base(ObjectType.ModemConfiguration, "0.0.2.0.0.255", 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSModemConfiguration(string ln)
            : base(ObjectType.ModemConfiguration, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSModemConfiguration(string ln, ushort sn)
            : base(ObjectType.ModemConfiguration, ln, 0)
        {
        }       

        [XmlIgnore()]
        [GXDLMSAttribute(2)]
        public BaudRate CommunicationSpeed
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(3)]
        public GXDLMSModemInitialisation[] InitialisationStrings
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(4)]
        public string[] ModemProfile
        {
            get;
            set;
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, CommunicationSpeed, InitialisationStrings, ModemProfile };
        }

        #region IGXDLMSBase Members

        int IGXDLMSBase.GetAttributeCount()
        {
            return 4;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        object IGXDLMSBase.GetValue(int index, out DataType type, byte[] parameters)
        {
            if (index == 1)
            {
                type = DataType.OctetString;
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            if (index == 2)
            {
                type = DataType.DateTime;
                return CommunicationSpeed;
            }
            if (index == 3)
            {
                type = DataType.Array;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
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
                        data.Add((byte)DataType.Structure);
                        data.Add((byte)3); //Count
                        GXCommon.SetData(data, DataType.OctetString, it.Request);
                        GXCommon.SetData(data, DataType.OctetString, it.Response);
                        GXCommon.SetData(data, DataType.UInt16, it.Delay);
                    }
                }
                return data.ToArray();
            }
            if (index == 4)
            {
                type = DataType.Array;
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
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
                        GXCommon.SetData(data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it));
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
                LogicalName = GXDLMSClient.ChangeType((byte[])value, DataType.OctetString).ToString();
            }
            else if (index == 2)
            {
                CommunicationSpeed = (BaudRate)Convert.ToInt32(value);
            }
            else if (index == 3)
            {
                InitialisationStrings = null;
                if (value != null)
                {
                    List<GXDLMSModemInitialisation> items = new List<GXDLMSModemInitialisation>();
                    foreach (object[] it in (Object[])value)
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
            else if (index == 4)
            {
                ModemProfile = null;
                if (value != null)
                {
                    List<string> items = new List<string>();
                    foreach (object it in (Object[])value)
                    {
                        items.Add(GXDLMSClient.ChangeType((byte[])it, DataType.String).ToString());
                    }
                    ModemProfile = items.ToArray();
                }                   
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }

        void IGXDLMSBase.Invoke(int index, object parameters)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
