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
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSMacAddressSetup(string ln)
            : base(ObjectType.MacAddressSetup, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSMacAddressSetup(string ln, ushort sn)
            : base(ObjectType.MacAddressSetup, ln, 0)
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

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //MacAddress
            if (!base.IsRead(2))
            {
                attributes.Add(2);
            }
            return attributes.ToArray();
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 2;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        object IGXDLMSBase.GetValue(int index, out DataType type, byte[] parameters, bool raw)
        {
            if (index == 1)
            {
                type = DataType.OctetString;
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            if (index == 2)
            {
                type = DataType.OctetString;
                return MacAddress.Replace(':', '.');
            }
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
        }

        void IGXDLMSBase.SetValue(int index, object value, bool raw)
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
                MacAddress = GXDLMSClient.ChangeType((byte[])value, DataType.OctetString).ToString();
                MacAddress = MacAddress.Replace('.', ':');
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }

        byte[] IGXDLMSBase.Invoke(object sender, int index, object parameters)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
