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
    public class GXDLMSSecuritySetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSSecuritySetup()
            : base(ObjectType.SecuritySetup)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSSecuritySetup(string ln)
            : base(ObjectType.SecuritySetup, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSSecuritySetup(string ln, ushort sn)
            : base(ObjectType.SecuritySetup, ln, 0)
        {
        }

        [XmlIgnore()]
        public SecurityPolicy SecurityPolicy
        {
            get;
            set;
        }

        [XmlIgnore()]
        public SecuritySuite SecuritySuite
        {
            get;
            set;
        }

        [XmlIgnore()]
        public byte[] ClientSystemTitle
        {
            get;
            set;
        }

        [XmlIgnore()]
        public byte[] ServerSystemTitle
        {
            get;
            set;
        }        

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, SecurityPolicy, SecuritySuite, 
            ClientSystemTitle, ServerSystemTitle};
        }

        #region IGXDLMSBase Members

        /// <summary>
        /// Data interface do not have any methods.
        /// </summary>
        /// <param name="index"></param>
        byte[][] IGXDLMSBase.Invoke(object sender, int index, Object parameters)
        {
            throw new ArgumentException("Invoke failed. Invalid attribute index.");
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //SecurityPolicy
            if (CanRead(2))
            {
                attributes.Add(2);
            }
            //SecuritySuite
            if (CanRead(3))
            {
                attributes.Add(3);
            }

            //ClientSystemTitle
            if (CanRead(4))
            {
                attributes.Add(4);
            }
            //ServerSystemTitle
            if (CanRead(5))
            {
                attributes.Add(5);
            }
            return attributes.ToArray();
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 5;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 2;
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
                return DataType.Enum;                
            }
            if (index == 4)
            {
                return DataType.OctetString;                
            }
            if (index == 5)
            {
                return DataType.OctetString;                
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(int index, int selector, object parameters)
        {
            if (index == 1)
            {
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            if (index == 2)
            {
                return SecurityPolicy;
            }
            if (index == 3)
            {
                return SecuritySuite;
            }
            if (index == 4)
            {
                return ClientSystemTitle;
            }
            if (index == 5)
            {
                return ServerSystemTitle;
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
                SecurityPolicy = (SecurityPolicy)Convert.ToInt32(value);
            }
            else if (index == 3)
            {
                SecuritySuite = (SecuritySuite)Convert.ToInt32(value);
            }
            else if (index == 4)
            {
                ClientSystemTitle = (byte[])value;
            }
            else if (index == 5)
            {
               ServerSystemTitle = (byte[])value;
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }
        #endregion
    }
}
