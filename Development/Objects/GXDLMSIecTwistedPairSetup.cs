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
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSIecTwistedPairSetup
    /// </summary>
    public class GXDLMSIecTwistedPairSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSIecTwistedPairSetup() : this("0.0.23.0.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSIecTwistedPairSetup(string ln) : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSIecTwistedPairSetup(string ln, ushort sn)
        : base(ObjectType.IecTwistedPairSetup, ln, sn)
        {
            PrimaryAddresses = new byte[0];
            Tabis = new sbyte[0];
        }

        /// <summary>
        /// Working mode.
        /// </summary>
        [XmlIgnore()]
        public IecTwistedPairSetupMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Communication speed.
        /// </summary>
        [XmlIgnore()]
        public BaudRate Speed
        {
            get;
            set;
        }

        /// <summary>
        /// List of Primary Station Addresses.
        /// </summary>
        [XmlIgnore()]
        public byte[] PrimaryAddresses
        {
            get;
            set;
        }

        /// <summary>
        /// List of the TAB(i) for which the real equipment has been programmed
        /// in the case of forgotten station call.
        /// </summary>
        [XmlIgnore()]
        public sbyte[] Tabis
        {
            get;
            set;
        }


        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Mode, Speed, PrimaryAddresses, Tabis };
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
            //Mode
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //Speed
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //PrimaryAddresses
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //Tabis
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Mode", "Speed", "PrimaryAddresses", "Tabis" };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[0];
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 5;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            DataType ret;
            switch (index)
            {
                case 1:
                    ret = DataType.OctetString;
                    break;
                case 2:
                case 3:
                    ret = DataType.Enum;
                    break;
                case 4:
                case 5:
                    ret = DataType.Array;
                    break;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
            return ret;
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
                    ret = (byte)Mode;
                    break;
                case 3:
                    ret = (byte)Speed;
                    break;
                case 4:
                    {
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Array);
                        if (PrimaryAddresses == null)
                        {
                            data.SetUInt8(0);
                        }
                        else
                        {
                            data.SetUInt8((byte)PrimaryAddresses.Length);
                            foreach (byte it in PrimaryAddresses)
                            {
                                data.SetUInt8((byte)DataType.UInt8);
                                data.SetUInt8(it);
                            }
                        }
                        ret = data.Array();
                        break;
                    }
                case 5:
                    {
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Array);
                        if (Tabis == null)
                        {
                            data.SetUInt8(0);
                        }
                        else
                        {
                            data.SetUInt8((byte)Tabis.Length);
                            foreach (sbyte it in Tabis)
                            {
                                data.SetUInt8((byte)DataType.Int8);
                                data.SetUInt8((byte)it);
                            }
                        }
                        ret = data.Array();
                        break;
                    }

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
                    Mode = (IecTwistedPairSetupMode)Convert.ToByte(e.Value);
                    break;
                case 3:
                    Speed = (BaudRate)Convert.ToByte(e.Value);
                    break;
                case 4:
                    {
                        GXByteBuffer list = new GXByteBuffer();
                        if (e.Value != null)
                        {
                            foreach (object it in (List<object>)e.Value)
                            {
                                list.Add((byte)it);
                            }
                        }
                        PrimaryAddresses = list.Array();
                        break;
                    }
                case 5:
                    {
                        List<sbyte> list = new List<sbyte>();
                        if (e.Value != null)
                        {
                            foreach (object it in (List<object>)e.Value)
                            {
                                list.Add((sbyte)it);
                            }
                        }
                        Tabis = list.ToArray();
                        break;
                    }
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            Mode = (IecTwistedPairSetupMode)reader.ReadElementContentAsInt("Mode");
            Speed = (BaudRate)reader.ReadElementContentAsInt("Speed");
            PrimaryAddresses = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("PrimaryAddresses"));
            byte[] tmp = GXDLMSTranslator.HexToBytes(reader.ReadElementContentAsString("Tabis"));
            Tabis = new sbyte[tmp.Length];
            if (tmp.Length != 0)
            {
                Buffer.BlockCopy(tmp, 0, Tabis, 0, tmp.Length);
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Mode", (int)Mode, 2);
            writer.WriteElementString("Speed", (int)Speed, 3);
            writer.WriteElementString("PrimaryAddresses", GXDLMSTranslator.ToHex(PrimaryAddresses), 4);
            if (Tabis != null)
            {
                byte[] tmp = new byte[Tabis.Length];
                Buffer.BlockCopy(Tabis, 0, tmp, 0, Tabis.Length);
                writer.WriteElementString("Tabis", GXDLMSTranslator.ToHex(tmp), 5);
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
