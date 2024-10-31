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
        : this(null, 0)
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
            Objects = new List<GXDLMSDataProtectionObject>();
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
        public List<GXDLMSDataProtectionObject> Objects
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

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    return Buffer;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
            return null;
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
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
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
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Buffer", GXDLMSTranslator.ToHex(Buffer), 2);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
