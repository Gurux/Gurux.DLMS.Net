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
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSPrimeNbOfdmPlcApplicationsIdentification
    /// </summary>
    public class GXDLMSPrimeNbOfdmPlcApplicationsIdentification : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSPrimeNbOfdmPlcApplicationsIdentification()
        : this("0.0.28.7.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSPrimeNbOfdmPlcApplicationsIdentification(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSPrimeNbOfdmPlcApplicationsIdentification(string ln, ushort sn)
        : base(ObjectType.PrimeNbOfdmPlcApplicationsIdentification, ln, sn)
        {
        }

        /// <summary>
        ///Textual description of the firmware version running on the device.
        /// </summary>
        [XmlIgnore()]
        public string FirmwareVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Unique vendor identifier assigned by PRIME Alliance.
        /// </summary>
        [XmlIgnore()]
        public UInt16 VendorId
        {
            get;
            set;
        }

        /// <summary>
        /// Vendor assigned unique identifier for specific product.
        /// </summary>
        [XmlIgnore()]
        public UInt16 ProductId
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, FirmwareVersion, VendorId, ProductId };
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
            //FirmwareVersion
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //VendorId
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //ProductId
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "FirmwareVersion", "VendorId", "ProductId" };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[0];
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
            switch (index)
            {
                case 1:
                case 2:
                    return DataType.OctetString;
                case 3:
                case 4:
                    return DataType.UInt16;
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
                    if (FirmwareVersion != null)
                    {
                        return ASCIIEncoding.ASCII.GetBytes(FirmwareVersion);
                    }
                    break;
                case 3:
                    return VendorId;
                case 4:
                    return ProductId;
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
                    FirmwareVersion = ASCIIEncoding.ASCII.GetString((byte[])e.Value);
                    break;
                case 3:
                    VendorId = Convert.ToUInt16(e.Value);
                    break;
                case 4:
                    ProductId = Convert.ToUInt16(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            FirmwareVersion = reader.ReadElementContentAsString("FirmwareVersion");
            VendorId = (UInt16)reader.ReadElementContentAsInt("VendorId");
            ProductId = (UInt16)reader.ReadElementContentAsInt("ProductId");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("FirmwareVersion", FirmwareVersion, 2);
            writer.WriteElementString("VendorId", VendorId, 3);
            writer.WriteElementString("ProductId", ProductId, 4);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
