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
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSModemConfiguration
    /// </summary>
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
        : this("0.0.2.0.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSModemConfiguration(string ln)
        : this(ln, 0)
        {
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

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //CommunicationSpeed
            if (all || !base.IsRead(2))
            {
                attributes.Add(2);
            }
            //InitialisationStrings
            if (all || !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //ModemProfile
            if (all || !base.IsRead(4))
            {
                attributes.Add(4);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Communication Speed",
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

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
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
                return GXCommon.LogicalNameToBytes(LogicalName);
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
                return data.Array();
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                LogicalName = GXCommon.ToLogicalName(e.Value);
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
                    foreach (object tmp in (IEnumerable<object>)e.Value)
                    {
                        List<object> it;
                        if (tmp is List<object>)
                        {
                            it = (List<object>)tmp;
                        }
                        else
                        {
                            it = new List<object>((object[])tmp);
                        }
                        GXDLMSModemInitialisation item = new GXDLMSModemInitialisation();
                        item.Request = ASCIIEncoding.ASCII.GetString((byte[])it[0]);
                        item.Response = ASCIIEncoding.ASCII.GetString((byte[])it[1]);
                        if (it.Count > 2)
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
                    foreach (object it in (IEnumerable<object>)e.Value)
                    {
                        items.Add(GXDLMSClient.ChangeType((byte[])it, DataType.String, false).ToString().Trim());
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

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            CommunicationSpeed = (BaudRate)reader.ReadElementContentAsInt("CommunicationSpeed");
            if (reader.IsStartElement("InitialisationStrings", true))
            {
                while (reader.IsStartElement("Initialisation", true))
                {
                    GXDLMSModemInitialisation it = new GXDLMSModemInitialisation();
                    it.Request = reader.ReadElementContentAsString("Request");
                    it.Response = reader.ReadElementContentAsString("Response");
                    it.Delay = (UInt16)reader.ReadElementContentAsInt("Delay");
                }
                reader.ReadEndElement("InitialisationStrings");
            }
            ModemProfile = reader.ReadElementContentAsString("ModemProfile", "").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (CommunicationSpeed != BaudRate.Baudrate300)
            {
                writer.WriteElementString("CommunicationSpeed", ((int)CommunicationSpeed).ToString());
            }
            if (InitialisationStrings != null)
            {
                writer.WriteStartElement("InitialisationStrings");
                foreach (GXDLMSModemInitialisation it in InitialisationStrings)
                {
                    writer.WriteStartElement("Initialisation");
                    writer.WriteElementString("Request", it.Request);
                    writer.WriteElementString("Response", it.Response);
                    writer.WriteElementString("Delay", it.Delay);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            if (ModemProfile != null)
            {
                writer.WriteElementString("ModemProfile", string.Join(";", ModemProfile));
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
