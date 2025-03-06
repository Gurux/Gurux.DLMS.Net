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
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSDataProtection
    /// </summary>
    public class GXDLMSDataProtection : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSDataProtection()
        : this("0.0.43.2.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSDataProtection(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSDataProtection(string ln, ushort sn)
        : base(ObjectType.DataProtection, ln, sn)
        {
            Objects = new List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>>();
            GetParameters = new List<GXDLMSDataProtectionParameter>();
            SetParameters = new List<GXDLMSDataProtectionParameter>();
        }

        /// <summary>
        /// Protection buffer.
        /// </summary>
        [XmlIgnore()]
        public byte[] Buffer
        {
            get;
            set;
        }

        /// <summary>
        /// Protected objects.
        /// </summary>
        [XmlIgnore()]
        public List<GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>> Objects
        {
            get;
            set;
        }

        /// <summary>
        /// Get parameters.
        /// </summary>
        [XmlIgnore()]
        public List<GXDLMSDataProtectionParameter> GetParameters
        {
            get;
            set;
        }

        /// <summary>
        /// Set parameters.
        /// </summary>
        [XmlIgnore()]
        public List<GXDLMSDataProtectionParameter> SetParameters
        {
            get;
            set;
        }

        /// <summary>
        /// Required protection.
        /// </summary>
        [XmlIgnore()]
        public RequiredProtection RequiredProtection
        {
            get;
            set;
        }

        /// <summary>
        /// Get protected attributes.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] GetProtectedAttributes(GXDLMSClient client, List<GXDLMSObject> objects, List<GXDLMSDataProtectionParameter> parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parse Get protected attributes response.
        /// </summary>
        /// <param name="data">Received data from the meter.</param>
        public void ParseGetProtectedAttributes(byte[] data, List<GXDLMSObject> objects, List<GXDLMSDataProtectionParameter> parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set protected attributes.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] SetProtectedAttributes(GXDLMSClient client, List<GXDLMSObject> objects, List<GXDLMSDataProtectionParameter> parameters, byte[] attributes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set protected attributes.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] InvokeProtectedMethod(GXDLMSClient client, List<GXDLMSObject> objects, List<GXDLMSDataProtectionParameter> parameters, byte[] attributes)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Buffer, Objects, GetParameters, SetParameters, RequiredProtection };
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
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
            //Buffer
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //Objects
            if (all || !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //GetParameters
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //SetParameters
            if (all || !base.IsRead(5))
            {
                attributes.Add(5);
            }
            //RequiredProtection
            if (all || !base.IsRead(6))
            {
                attributes.Add(6);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Value" };
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "GetProtectedAttributes", "SetProtectedAttributes", "InvokeProtectedMethod" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 6;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 3;
        }

        /// <inheritdoc />
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                case 2:
                    return DataType.OctetString;
                case 3:
                case 4:
                case 5:
                    return DataType.Array;
                case 6:
                    return DataType.Enum;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        private static byte[] GetProtectionParameters(GXDLMSSettings settings, List<GXDLMSDataProtectionParameter> list)
        {
            GXByteBuffer buff = new GXByteBuffer();
            buff.SetUInt8(DataType.Array);
            if (list == null)
            {
                GXCommon.SetObjectCount(0, buff);
            }
            else
            {
                GXCommon.SetObjectCount(list.Count, buff);
                foreach (var it in list)
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
            }
            return buff.Array();
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
                    ret = Buffer;
                    break;
                case 3:
                    ret = GetObjectList(settings);
                    break;
                case 4:
                    ret = GetProtectionParameters(settings, GetParameters);
                    break;
                case 5:
                    ret = GetProtectionParameters(settings, SetParameters);
                    break;
                case 6:
                    ret = RequiredProtection;
                    break;
                default:
                    ret = null;
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
            return ret;
        }

        private byte[] GetObjectList(GXDLMSSettings settings)
        {
            GXByteBuffer buff = new GXByteBuffer();
            buff.SetUInt8(DataType.Array);
            GXCommon.SetObjectCount(Objects.Count, buff);
            foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in Objects)
            {
                buff.SetUInt8(DataType.Structure);
                if (Version < 1)
                {
                    buff.SetUInt8(4);
                    GXCommon.SetData(settings, buff, DataType.UInt16, it.Key.ObjectType);
                    GXCommon.SetData(settings, buff, DataType.OctetString, GXCommon.LogicalNameToBytes(it.Key.LogicalName));
                    GXCommon.SetData(settings, buff, DataType.Int8, it.Value.AttributeIndex);
                    GXCommon.SetData(settings, buff, DataType.UInt16, it.Value.DataIndex);
                }
                else
                {
                    buff.SetUInt8((byte)(Version == 1 ? 5 : 6));
                    GXCommon.SetData(settings, buff, DataType.UInt16, it.Key.ObjectType);
                    GXCommon.SetData(settings, buff, DataType.OctetString, GXCommon.LogicalNameToBytes(it.Key.LogicalName));
                    GXCommon.SetData(settings, buff, DataType.Int8, it.Value.AttributeIndex);
                    GXCommon.SetData(settings, buff, DataType.UInt16, it.Value.DataIndex);
                    //restriction_element
                    buff.SetUInt8(DataType.Structure);
                    buff.SetUInt8(2);
                    GXCommon.SetData(settings, buff, DataType.Enum, it.Value.Restriction.Type);
                    switch (it.Value.Restriction.Type)
                    {
                        case RestrictionType.None:
                            GXCommon.SetData(settings, buff, DataType.None, null);
                            break;
                        case RestrictionType.Date:
                            buff.SetUInt8(DataType.Structure);
                            buff.SetUInt8(2);
                            GXCommon.SetData(settings, buff, DataType.OctetString, it.Value.Restriction.From);
                            GXCommon.SetData(settings, buff, DataType.OctetString, it.Value.Restriction.To);
                            break;
                        case RestrictionType.Entry:
                            buff.SetUInt8(DataType.Structure);
                            buff.SetUInt8(2);
                            GXCommon.SetData(settings, buff, DataType.UInt16, it.Value.Restriction.From);
                            GXCommon.SetData(settings, buff, DataType.UInt16, it.Value.Restriction.To);
                            break;
                    }
                }
            }
            return buff.Array();
        }

        private void SetObjects(GXDLMSSettings settings, ValueEventArgs e)
        {
            Objects.Clear();
            if (e.Value != null)
            {
                foreach (GXStructure it in (GXArray)e.Value)
                {
                    ObjectType type = (ObjectType)Convert.ToUInt16(it[0]);
                    string ln = GXCommon.ToLogicalName(it[1]);
                    GXDLMSObject obj = settings.Objects.FindByLN(type, ln);
                    if (obj == null)
                    {
                        obj = GXDLMSClient.CreateObject(type);
                        obj.LogicalName = ln;
                    }
                    GXDLMSCaptureObject co = new GXDLMSCaptureObject(Convert.ToInt32(it[2]), Convert.ToInt32(it[3]));
                    Objects.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(obj, co));
                    GXStructure restriction = (GXStructure)it[4];
                    co.Restriction.Type = (RestrictionType)Convert.ToInt32(restriction[0]);
                    switch (co.Restriction.Type)
                    {
                        case RestrictionType.None:
                            break;
                        case RestrictionType.Date:
                        case RestrictionType.Entry:
                            co.Restriction.From = restriction[1];
                            co.Restriction.To = restriction[2];
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Invalid restriction type.");
                    }
                }
            }
        }

        private List<GXDLMSDataProtectionParameter> SetProtectionParameters(ValueEventArgs e)
        {
            List<GXDLMSDataProtectionParameter> list = new List<GXDLMSDataProtectionParameter>();
            if (e.Value != null)
            {
                foreach (GXStructure it in (GXArray)e.Value)
                {
                    GXDLMSDataProtectionParameter p = new GXDLMSDataProtectionParameter();
                    p.ProtectionType = (ProtectionType)Convert.ToInt32(it[0]);
                    GXStructure options = (GXStructure)it[1];
                    p.TransactionId = (byte[])options[0];
                    p.OriginatorSystemTitle = (byte[])options[1];
                    p.RecipientSystemTitle = (byte[])options[2];
                    p.OtherInformation = (byte[])options[3];
                    GXStructure keyInfo = (GXStructure)options[4];
                    p.KeyInfo.DataProtectionKeyType = (DataProtectionKeyType)Convert.ToInt32(keyInfo[0]);
                    if (p.KeyInfo.DataProtectionKeyType == DataProtectionKeyType.Identified)
                    {
                        p.KeyInfo.IdentifiedKey.KeyType = (DataProtectionIdentifiedKeyType)Convert.ToInt32(keyInfo[0]);
                    }
                    else if (p.KeyInfo.DataProtectionKeyType == DataProtectionKeyType.Wrapped)
                    {
                        GXStructure data = (GXStructure)keyInfo[1];
                        p.KeyInfo.WrappedKey.KeyType = (DataProtectionWrappedKeyType)Convert.ToInt32(data[0]);
                        p.KeyInfo.WrappedKey.Key = (byte[])data[1];
                    }
                    else if (p.KeyInfo.DataProtectionKeyType == DataProtectionKeyType.Agreed)
                    {
                        GXStructure data = (GXStructure)keyInfo[1];
                        p.KeyInfo.AgreedKey.Parameters = (byte[])data[0];
                        p.KeyInfo.AgreedKey.Data = (byte[])data[1];
                    }
                    list.Add(p);
                }
            }
            return list;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    Buffer = (byte[])e.Value;
                    break;
                case 3:
                    SetObjects(settings, e);
                    break;
                case 4:
                    GetParameters = SetProtectionParameters(e);
                    break;
                case 5:
                    SetParameters = SetProtectionParameters(e);
                    break;
                case 6:
                    RequiredProtection = (RequiredProtection)Convert.ToByte(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        private static void LoadParameters(GXXmlReader reader, List<GXDLMSDataProtectionParameter> list, string name)
        {
            if (reader.IsStartElement(name, true))
            {
                list.Clear();
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSDataProtectionParameter it = new GXDLMSDataProtectionParameter();
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
                reader.ReadEndElement(name);
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            string str = reader.ReadElementContentAsString("Buffer");
            if (string.IsNullOrEmpty(str))
            {
                Buffer = null;
            }
            else
            {
                Buffer = GXDLMSTranslator.HexToBytes(str);
            }
            Objects.Clear();
            if (reader.IsStartElement("Objects", true))
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
                    Objects.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(obj, co));
                }
                reader.ReadEndElement("Objects");
            }
            LoadParameters(reader, GetParameters, "GetParameters");
            LoadParameters(reader, SetParameters, "SetParameters");
            RequiredProtection = (RequiredProtection)reader.ReadElementContentAsInt("RequiredProtection", 0);
        }

        private static void SaveParameters(GXXmlWriter writer, List<GXDLMSDataProtectionParameter> list, string name)
        {
            if (list != null)
            {
                writer.WriteStartElement(name, 10);
                foreach (var it in list)
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
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Buffer", GXDLMSTranslator.ToHex(Buffer), 2);
            if (Objects != null)
            {
                writer.WriteStartElement("Objects", 2);
                foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in Objects)
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
            SaveParameters(writer, GetParameters, "GetParameters");
            SaveParameters(writer, SetParameters, "SetParameters");
            writer.WriteElementString("RequiredProtection", (int)RequiredProtection, 6);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
