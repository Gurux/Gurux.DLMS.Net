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
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSIecTwistedPairSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSIecTwistedPairSetup()
            : base(ObjectType.IecTwistedPairSetup)
        {
            PrimaryAddresses = new byte[0];
            Tabis = new sbyte[0];
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSIecTwistedPairSetup(string ln)
            : base(ObjectType.IecTwistedPairSetup, ln, 0)
        {
            PrimaryAddresses = new byte[0];
            Tabis = new sbyte[0];
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
        public byte[] PrimaryAddresses
        {
            get;
            set;
        }

        /// <summary>
        /// List of the TAB(i) for which the real equipment has been programmed 
        /// in the case of forgotten station call. 
        /// </summary>
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

        /// <summary>
        /// Data interface do not have any methods.
        /// </summary>
        /// <param name="index"></param>
        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e) 
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //Mode
            if (CanRead(2))
            {
                attributes.Add(2);
            }
            //Speed
            if (CanRead(3))
            {
                attributes.Add(3);
            }
            //PrimaryAddresses
            if (CanRead(4))
            {
                attributes.Add(4);
            }
            //Tabis
            if (CanRead(5))
            {
                attributes.Add(5);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Gurux.DLMS.Properties.Resources.LogicalNameTxt, "Value" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 5;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
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
                return DataType.Array;
            }
            if (index == 5)
            {
                return DataType.Array;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return this.LogicalName;
            }
            if (e.Index == 2)
            {
                return (byte) Mode;
            }
            if (e.Index == 3)
            {
                return (byte) Speed;
            }
            if (e.Index == 4)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (PrimaryAddresses == null)
                {
                    data.SetUInt8(0);
                }
                else
                {
                    data.SetUInt8((byte) PrimaryAddresses.Length);
                    foreach (byte it in PrimaryAddresses)
                    {
                        data.SetUInt8((byte)DataType.UInt8);
                        data.SetUInt8(it);
                    }
                }
                return data.Array();
            }
            if (e.Index == 5)
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
                        data.SetUInt8((byte)DataType.UInt8);
                        data.SetUInt8((byte) it);
                    }
                }
                return data.Array();
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
                Mode = (IecTwistedPairSetupMode)e.Value;
            }
            else if (e.Index == 3)
            {
                Speed = (BaudRate)e.Value;
            }
            else if (e.Index == 4)
            {
                GXByteBuffer list = new GXByteBuffer();
                foreach (object it in (object[])e.Value)
                {
                    list.Add((byte)it);
                }
                PrimaryAddresses = list.Array();
            }
            else if (e.Index == 5)
            {
                List<sbyte> list = new List<sbyte>();
                foreach (object it in (object[])e.Value)
                {
                    list.Add((sbyte)it);
                }
                Tabis = list.ToArray();
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }
        #endregion
    }
}
