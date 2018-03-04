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
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSMacAddressSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSMacAddressSetup()
        : base(ObjectType.MacAddressSetup, "0.0.25.2.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSMacAddressSetup(string ln)
        : base(ObjectType.MacAddressSetup, ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSMacAddressSetup(string ln, ushort sn)
        : base(ObjectType.MacAddressSetup, ln, sn)
        {
        }

        /// <summary>
        /// Value of COSEM Data object.
        /// </summary>
        [XmlIgnore()]
        public string MacAddress
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, MacAddress };
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
            //MacAddress
            if (all || !base.IsRead(2))
            {
                attributes.Add(2);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "MAC Address" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 2;
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
                return DataType.OctetString;
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
                return GXCommon.HexToBytes(MacAddress);
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
                MacAddress = GXCommon.ToHex((byte[])e.Value, true);
                MacAddress = MacAddress.Replace(' ', ':');
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
            MacAddress = reader.ReadElementContentAsString("MacAddress");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("MacAddress", MacAddress);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
