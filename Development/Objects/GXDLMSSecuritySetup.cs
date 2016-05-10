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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Secure;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSSecuritySetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSSecuritySetup()
            : this("0.0.43.0.0.255")
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSSecuritySetup(string ln)
            : base(ObjectType.SecuritySetup, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSSecuritySetup(string ln, ushort sn)
            : base(ObjectType.SecuritySetup, ln, sn)
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

        /// <summary>
        /// TODO:
        /// </summary>
        [XmlIgnore()]
        public Object Certificates
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

        /// <summary>
        /// Get security enum as integer value.
        /// </summary>
        /// <param name="security">Security level.</param>
        /// <returns>Integer value of security level.</returns>
        private static int GetSecurityValue(Security security)
        {
            int value = 0;
            switch (security)   
            {
                case Security.None:
                    value = 0;
                    break;
                case Security.Authentication:
                    value = 1;
                    break;
                case Security.Encryption:
                    value = 2;
                    break;
                case Security.AuthenticationEncryption:
                    value = 3;
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
            return value;
        }

        /// <summary>
        /// Activates and strengthens the security policy. 
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="security">New security level.</param>
        /// <returns></returns>
        public byte[][] Activate(GXDLMSClient client, Security security)
        {          
            return client.Method(this, 1, GetSecurityValue(security), DataType.Enum);
        }
       
        /// <summary>
        /// Updates one or more global keys. 
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="kek">Master key, also known as Key Encrypting Key.</param>
        /// <param name="list">List of Global key types and keys.</param>
        /// <returns></returns>
        public byte[][] GlobalKeyTransfer(GXDLMSClient client, byte[] kek, List<KeyValuePair<GlobalKeyType, byte[]>> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new ArgumentException("Invalid list. It is empty.");
            }
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Array);
            bb.SetUInt8((byte) list.Count);
            byte[] tmp;
            foreach(KeyValuePair<GlobalKeyType, byte[]> it in list)
            {
                bb.SetUInt8(DataType.Structure);
                bb.SetUInt8(2);
                GXCommon.SetData(bb, DataType.Enum, it.Key);
                tmp = GXDLMSSecureClient.Encrypt(kek, it.Value);
                GXCommon.SetData(bb, DataType.OctetString, tmp);
            }
            return client.Method(this, 2, bb.Array(), DataType.Array);
        }
         

        #region IGXDLMSBase Members

        /// <summary>
        /// Data interface do not have any methods.
        /// </summary>
        /// <param name="index"></param>
        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e) 
        {
            if (e.Index == 2)
            {
                foreach (object tmp in e.Parameters as object[])
                {
                    object[] item = tmp as object[];
                    GlobalKeyType type = (GlobalKeyType)Convert.ToInt32(item[0]);
                    byte[] data = (byte[])item[1];
                    switch (type)
                    {
                        case GlobalKeyType.UnicastEncryption:
                        case GlobalKeyType.BroadcastEncryption:
                           //Invalid type
                           e.Error = ErrorCode.ReadWriteDenied;
                           break;
                        case GlobalKeyType.Authentication:
                           //if settings.Cipher is null non secure server is used.
                            settings.Cipher.AuthenticationKey = GXDLMSSecureClient.Decrypt(settings.Kek, data);
                            break;
                        case GlobalKeyType.Kek:
                            settings.Kek = GXDLMSSecureClient.Decrypt(settings.Kek, data);
                            break;
                        default:
                            //Invalid type
                            e.Error = ErrorCode.ReadWriteDenied;
                            break;
                    }
                }
                //Return standard reply.
                return null;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
                return null;
            }
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
            if (this.Version > 0)
            {
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
                //Certificates
                if (CanRead(6))
                {
                    attributes.Add(6);
                } 
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            if (this.Version == 0)
            {
                return new string[] { Gurux.DLMS.Properties.Resources.LogicalNameTxt, "Security Policy", 
                "Security Suite"};
            }
            return new string[] { Gurux.DLMS.Properties.Resources.LogicalNameTxt, "Security Policy", 
                "Security Suite", "Client System Title", "Server System Title" , "Certificates"};            
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            if (this.Version == 0)
            {
                return 5;
            }
            return 6;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            if (this.Version == 0)
            {
                return 2;
            }
            return 8;
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
            if (this.Version > 0)
            {
                if (index == 6)
                {
                    return DataType.OctetString;
                }
                else
                {
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
                }
            }
            else
            {
                throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return this.LogicalName;
            }
            if (e.Index == 2)
            {
                return SecurityPolicy;
            }
            if (e.Index == 3)
            {
                return SecuritySuite;
            }
            if (this.Version > 0)
            {
                if (e.Index == 4)
                {
                    return ClientSystemTitle;
                }
                if (e.Index == 5)
                {
                    return ServerSystemTitle;
                }
                if (e.Index == 6)
                {
                    return Certificates;
                }
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
                SecurityPolicy = (SecurityPolicy)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 3)
            {
                SecuritySuite = (SecuritySuite)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 4)
            {
                ClientSystemTitle = (byte[])e.Value;
            }
            else if (e.Index == 5)
            {
                ServerSystemTitle = (byte[])e.Value;
            }
            else if (e.Index == 6)
            {
                Certificates = e.Value;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }
        #endregion
    }
}
