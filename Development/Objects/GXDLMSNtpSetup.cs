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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// NTP Setup is used for time synchronisation.
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSNtpSetup
    /// </summary>
    public class GXDLMSNtpSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSNtpSetup()
        : this("0.0.25.10.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSNtpSetup(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSNtpSetup(string ln, ushort sn)
        : base(ObjectType.NtpSetup, ln, sn)
        {
            Port = 123;
            Keys = new SortedDictionary<UInt32, byte[]>();
        }

        /// <summary>
        /// Is NTP time synchronisation active.
        /// </summary>
        [XmlIgnore()]
        public bool Activated
        {
            get;
            set;
        }

        /// <summary>
        /// NTP server address.
        /// </summary>
        [XmlIgnore()]
        public string ServerAddress
        {
            get;
            set;
        }

        /// <summary>
        /// UDP port related to this protocol.
        /// </summary>
        [XmlIgnore()]
        public UInt16 Port
        {
            get;
            set;
        }


        /// <summary>
        /// UDP port related to this protocol.
        /// </summary>
        [XmlIgnore()]
        public NtpAuthenticationMethod Authentication
        {
            get;
            set;
        }

        /// <summary>
        /// Symmetric keys for authentication.
        /// </summary>
        [XmlIgnore()]
        public SortedDictionary<UInt32, byte[]> Keys
        {
            get;
            set;
        }

        /// <summary>
        /// Client key (NTP server public key).
        /// </summary>
        [XmlIgnore()]
        public byte[] ClientKey
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Activated, ServerAddress, Port, Authentication, Keys, ClientKey };
        }


        /// <summary>
        /// Synchronizes the time of the DLMS server with the NTP server.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Synchronize(GXDLMSClient client)
        {
            return client.Method(this, 1, (sbyte)0);
        }

        /// <summary>
        /// Adds a new symmetric authentication key to authentication key array.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="id">Authentication key Id.</param>
        /// <param name="key">authentication Key.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] AddAuthenticationKey(GXDLMSClient client, UInt32 id, byte[] key)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(2);
            bb.SetUInt8(DataType.UInt32);
            bb.SetUInt32(id);
            bb.SetUInt8(DataType.OctetString);
            GXCommon.SetObjectCount(key.Length, bb);
            bb.Set(key);
            return client.Method(this, 2, bb.Array(), DataType.Structure);
        }

        /// <summary>
        /// Remove symmetric authentication key.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="id">Authentication key Id.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] DeleteAuthenticationKey(GXDLMSClient client, UInt32 id)
        {
            return client.Method(this, 3, id);
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                //Server must handle this. Do nothing...
            }
            else if (e.Index == 2)
            {
                GXStructure tmp = (GXStructure)e.Parameters;
                Keys[(UInt32)tmp[0]] = (byte[])tmp[1];
            }
            else if (e.Index == 3)
            {
                Keys.Remove((UInt32)e.Parameters);
            }
            else
            {
                e.Error = ErrorCode.InconsistentClass;
            }
            return null;
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //Activated
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //ServerAddress
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //Port
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //Authentication
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //Keys
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //ClientKey
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { GXCommon.GetLogicalNameString(), "Activated", "ServerAddress", "Port", "Authentication", "Keys", "ClientKey" };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Synchronize", "Add authentication key", "Delete authentication key" };
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
            return 3;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetUIDataType"/>
        public override DataType GetUIDataType(int index)
        {
            if (index == 3)
            {
                return DataType.String;
            }
            return base.GetUIDataType(index);
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            DataType dt;
            switch (index)
            {
                case 1:
                    dt = DataType.OctetString;
                    break;
                case 2:
                    dt = DataType.Boolean;
                    break;
                case 3:
                    dt = DataType.OctetString;
                    break;
                case 4:
                    dt = DataType.UInt16;
                    break;
                case 5:
                    dt = DataType.Enum;
                    break;
                case 6:
                    dt = DataType.Array;
                    break;
                case 7:
                    dt = DataType.OctetString;
                    break;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
            return dt;
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            object ret;
            switch (e.Index)
            {
                case 1:
                    ret = GXCommon.LogicalNameToBytes(LogicalName);
                    break;
                case 2:
                    ret = Activated;
                    break;
                case 3:
                    ret = ServerAddress;
                    break;
                case 4:
                    ret = Port;
                    break;
                case 5:
                    ret = Authentication;
                    break;
                case 6:
                    GXByteBuffer bb = new GXByteBuffer();
                    bb.SetUInt8((byte)DataType.Array);
                    //Add count
                    GXCommon.SetObjectCount(Keys.Count, bb);
                    foreach (var it in Keys)
                    {
                        bb.SetUInt8(DataType.Structure);
                        bb.SetUInt8(2); //Count
                        bb.SetUInt8(DataType.UInt32);
                        bb.SetUInt32(it.Key);
                        bb.SetUInt8(DataType.OctetString);
                        GXCommon.SetObjectCount(it.Value.Length, bb);
                        bb.Set(it.Value);
                    }
                    ret = bb.Array();
                    break;
                case 7:
                    ret = ClientKey;
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
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    Activated = (bool)e.Value;
                    break;
                case 3:
                    if (e.Value is byte[])
                    {
                        ServerAddress = ASCIIEncoding.ASCII.GetString((byte[])e.Value);
                    }
                    else if (e.Value is string str)
                    {
                        ServerAddress = str;
                    }
                    else
                    {
                        ServerAddress = null;
                    }
                    break;
                case 4:
                    Port = Convert.ToUInt16(e.Value);
                    break;
                case 5:
                    Authentication = (NtpAuthenticationMethod)Convert.ToInt32(e.Value);
                    break;
                case 6:
                    Keys.Clear();
                    if (e.Value != null)
                    {
                        foreach (GXStructure it in (GXArray)e.Value)
                        {
                            Keys[(UInt32)it[0]] = (byte[])it[1];
                        }
                    }
                    break;
                case 7:
                    ClientKey = (byte[])e.Value;
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            Activated = reader.ReadElementContentAsInt("Activated", 1) != 0;
            ServerAddress = reader.ReadElementContentAsString("ServerAddress", null);
            Port = (UInt16)reader.ReadElementContentAsInt("Port", 0);
            Authentication = (NtpAuthenticationMethod)reader.ReadElementContentAsInt("Authentication", 0);
            Keys.Clear();
            if (reader.IsStartElement("Keys", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    UInt32 Id = (UInt32)reader.ReadElementContentAsInt("ID");
                    byte[] key = GXCommon.HexToBytes(reader.ReadElementContentAsString("Key"));
                    Keys[Id] = key;
                }
            }
            ClientKey = GXCommon.HexToBytes(reader.ReadElementContentAsString("ServerAddress", null));
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Activated", Activated, 2);
            writer.WriteElementString("ServerAddress", ServerAddress, 3);
            writer.WriteElementString("Port", Port, 4);
            writer.WriteElementString("Authentication", (int)Authentication, 5);
            writer.WriteStartElement("Keys", 6);
            foreach (var it in Keys)
            {
                writer.WriteStartElement("Item", 0);
                writer.WriteElementString("ID", it.Key.ToString(), 0);
                writer.WriteElementString("Key", GXCommon.ToHex(it.Value, false), 0);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();//Keys
            writer.WriteElementString("ClientKey", GXCommon.ToHex(ClientKey, false), 7);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
