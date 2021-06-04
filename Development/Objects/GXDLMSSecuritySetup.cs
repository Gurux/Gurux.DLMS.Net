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
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Secure;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSSecuritySetup
    /// </summary>
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
        : this(ln, 0)
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


        /// <summary>
        /// Block cipher key.
        /// </summary>
        internal byte[] Guek
        {
            get;
            set;
        }

        /// <summary>
        /// Broadcast block cipher key.
        /// </summary>
        internal byte[] Gbek
        {
            get;
            set;
        }

        /// <summary>
        /// Authentication key.
        /// </summary>
        internal byte[] Gak
        {
            get;
            set;
        }

        /// <summary>
        /// Master key.
        /// </summary>
        internal byte[] Kek
        {
            get;
            set;
        }       

        /// <summary>
        /// Security policy.
        /// </summary>
        [XmlIgnore()]
        public SecurityPolicy SecurityPolicy
        {
            get;
            set;
        }

        /// <summary>
        /// Security suite.
        /// </summary>
        [XmlIgnore()]
        public SecuritySuite SecuritySuite
        {
            get;
            set;
        }

        /// <summary>
        /// Client system title.
        /// </summary>
        [XmlIgnore()]
        public byte[] ClientSystemTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Server system title.
        /// </summary>
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

        /// <summary>
        /// Get security enum as integer value.
        /// </summary>
        /// <param name="security">Security level.</param>
        /// <returns>Integer value of security level.</returns>
        private static int GetSecurityValue(Security security)
        {
            int value;
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
                    throw new ArgumentOutOfRangeException("Invalid Security enum.");
            }
            return value;
        }

        /// <summary>
        /// Activates and strengthens the security policy.
        /// </summary>
        /// <param name="client">DLMS client that is used to generate action.</param>
        /// <param name="security">New security level.</param>
        /// <returns>Generated action.</returns>
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
        /// <returns>Generated action.</returns>
        public byte[][] GlobalKeyTransfer(GXDLMSClient client, byte[] kek, List<KeyValuePair<GlobalKeyType, byte[]>> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new ArgumentException("Invalid list. It is empty.");
            }
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Array);
            bb.SetUInt8((byte)list.Count);
            byte[] tmp;
            foreach (KeyValuePair<GlobalKeyType, byte[]> it in list)
            {
                bb.SetUInt8(DataType.Structure);
                bb.SetUInt8(2);
                GXCommon.SetData(client.Settings, bb, DataType.Enum, it.Key);
                tmp = GXDLMSSecureClient.Encrypt(kek, it.Value);
                GXCommon.SetData(client.Settings, bb, DataType.OctetString, tmp);
            }
            return client.Method(this, 2, bb.Array(), DataType.Array);
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                SecurityPolicy = (SecurityPolicy)e.Parameters;
            }
            else if (e.Index == 2)
            {
                try
                {
                    foreach (List<object> item in e.Parameters as List<object>)
                    {
                        GlobalKeyType type = (GlobalKeyType)Convert.ToInt32(item[0]);
                        byte[] data = (byte[])item[1];
                        //if settings.Cipher is null non secure server is used.
                        //Keys are take in action after reply is generated.
                        switch (type)
                        {
                            case GlobalKeyType.UnicastEncryption:
                                Guek = GXDLMSSecureClient.Decrypt(settings.Kek, data);
                                break;
                            case GlobalKeyType.BroadcastEncryption:
                                Gbek = GXDLMSSecureClient.Decrypt(settings.Kek, data);
                                break;
                            case GlobalKeyType.Authentication:
                                Gak = GXDLMSSecureClient.Decrypt(settings.Kek, data);
                                break;
                            case GlobalKeyType.Kek:
                                Kek = GXDLMSSecureClient.Decrypt(settings.Kek, data);
                                break;
                            default:
                                //Invalid type
                                e.Error = ErrorCode.ReadWriteDenied;
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    e.Error = ErrorCode.InconsistentClass;
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            //Return standard reply.
            return null;
        }

        /// <summary>
        /// Start to use new keys after reply is generated.
        /// </summary>
        /// <param name="settings">DLMS settings.</param>
        /// <param name="e"></param>
        internal void ApplyKeys(GXDLMSSettings settings, ValueEventArgs e)
        {
            try
            {
                foreach (List<object> item in e.Parameters as List<object>)
                {
                    GlobalKeyType type = (GlobalKeyType)Convert.ToInt32(item[0]);
                    byte[] data = (byte[])item[1];
                    switch (type)
                    {
                        case GlobalKeyType.UnicastEncryption:
                            settings.Cipher.BlockCipherKey = Guek;
                            break;
                        case GlobalKeyType.BroadcastEncryption:
                            settings.Cipher.BroadcastBlockCipherKey = Gbek;
                            break;
                        case GlobalKeyType.Authentication:
                            //if settings.Cipher is null non secure server is used.
                            settings.Cipher.AuthenticationKey = Gak;
                            break;
                        case GlobalKeyType.Kek:
                            settings.Kek = Kek;
                            break;
                        default:
                            //Invalid type
                            e.Error = ErrorCode.InconsistentClass;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                e.Error = ErrorCode.InconsistentClass;
            }
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //SecurityPolicy
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //SecuritySuite
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //ClientSystemTitle
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //ServerSystemTitle
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Security Policy",
                                "Security Suite", "Client System Title", "Server System Title"
                            };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Security activate", "Key transfer" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 1;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 5;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 2;
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
            else
            {
                throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                return SecurityPolicy;
            }
            if (e.Index == 3)
            {
                return SecuritySuite;
            }
            if (e.Index == 4)
            {
                return ClientSystemTitle;
            }
            if (e.Index == 5)
            {
                return ServerSystemTitle;
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
                SecurityPolicy = (SecurityPolicy)Convert.ToByte(e.Value);
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
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            SecurityPolicy = (SecurityPolicy)reader.ReadElementContentAsInt("SecurityPolicy");
            //This is old functionality.It can be removed in some point.
            reader.ReadElementContentAsInt("SecurityPolicy0");
            SecuritySuite = (SecuritySuite)reader.ReadElementContentAsInt("SecuritySuite");
            string str = reader.ReadElementContentAsString("ClientSystemTitle");
            if (str == null)
            {
                ClientSystemTitle = null;
            }
            else
            {
                ClientSystemTitle = GXDLMSTranslator.HexToBytes(str);
            }
            str = reader.ReadElementContentAsString("ServerSystemTitle");
            if (str == null)
            {
                ServerSystemTitle = null;
            }
            else
            {
                ServerSystemTitle = GXDLMSTranslator.HexToBytes(str);
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("SecurityPolicy", (byte)SecurityPolicy, 2);
            writer.WriteElementString("SecuritySuite", (int)SecuritySuite, 3);
            writer.WriteElementString("ClientSystemTitle", GXDLMSTranslator.ToHex(ClientSystemTitle), 4);
            writer.WriteElementString("ServerSystemTitle", GXDLMSTranslator.ToHex(ServerSystemTitle), 5);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}