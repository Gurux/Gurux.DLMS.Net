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
using System.ComponentModel;
using Gurux.DLMS;
using System.Xml.Serialization;
using Gurux.DLMS.ManufacturerSettings;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSRegister : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSRegister()
            : base(ObjectType.Register)
        {
            Scaler = 1;
            Unit = Unit.None;
        }

        internal GXDLMSRegister(ObjectType type, string ln, ushort sn)
            : base(type, ln, sn)
        {
            Scaler = 1;
            Unit = Unit.None;
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSRegister(string ln)
            : this(ObjectType.Register, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSRegister(string ln, ushort sn)
            : this(ObjectType.Register, ln, 0)
        {
        }

        /// <summary>
        /// Scaler of COSEM Register object.
        /// </summary>        
        [DefaultValue(1.0)]                
        public double Scaler
        {
            get
            {
                if (ScalerUnit == null)
                {
                    return 1;
                }
                return Math.Pow(10, Convert.ToInt32(ScalerUnit[0]));
            }
            set
            {
                if (ScalerUnit == null)
                {
                    ScalerUnit = new int[2];
                    ScalerUnit[1] = 0;
                }
                ScalerUnit[0] = (int)Math.Log10(value);
            }
        }

        /// <summary>
        /// Unit of COSEM Register object.
        /// </summary>
        [DefaultValue(Unit.NoUnit)]                
        public Unit Unit
        {
            get
            {
                if (ScalerUnit == null)
                {
                    return Unit.NoUnit;
                }
                return (Unit)Convert.ToInt32(ScalerUnit[1]);
            }
            set
            {
                if (ScalerUnit == null)
                {
                    ScalerUnit = new int[2];
                    ScalerUnit[0] = 0;
                }
                ScalerUnit[1] = (int)value;
            }
        }

        /// <summary>
        /// Value of COSEM Register object.
        /// </summary>
        /// <remarks>
        /// Register value is not serialized because XML serializer can't handle all cases.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [System.Xml.Serialization.XmlIgnore()]
        [GXDLMSAttribute(2)]
        public object Value
        {
            get;
            set;
        }       

        /// <summary>
        /// Scaler and unit of COSEM object.
        /// </summary>
        [GXDLMSAttribute(3, Type = DataType.Array, Static = true, Access = AccessMode.Read, Order = 1)]
        internal int[] ScalerUnit
        {
            get;
            set;
        }

        /// <summary>
        /// Reset value.
        /// </summary>
        /// <returns></returns>
        public byte[][] Reset(GXDLMSClient client)
        {
            byte[] ret = client.Method(this, 1, (int)0);
            return new byte[][] { ret };
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, Value, ScalerUnit};
        }

        #region IGXDLMSBase Members


        void IGXDLMSBase.Invoke(int index, Object parameters)
        {
            // Resets the value to the default value. 
            // The default value is an instance specific constant.
            if (index == 1)
            {
                Value = null;
            }
            else
            {
                throw new ArgumentException("Invoke failed. Invalid attribute index.");
            }
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 3;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
        }        

        object IGXDLMSBase.GetValue(int index, out DataType type, byte[] parameters)
        {
            if (index == 1)
            {
                type = DataType.OctetString;
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            if (index == 2)
            {
                type = DataType.None;
                return Value;
            }
            if (index == 3)
            {
                type = DataType.Structure;
                return ScalerUnit;
            }
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
        }

        void IGXDLMSBase.SetValue(int index, object value)
        {
            if (index == 1)
            {
                LogicalName = GXDLMSClient.ChangeType((byte[])value, DataType.OctetString).ToString();
            }
            else if (index == 2)
            {
                Value = value;
            }
            else if (index == 3)
            {
                if (ScalerUnit == null)
                {
                    ScalerUnit = new int[2];
                }
                //Set default values.
                if (value == null)
                {
                    ScalerUnit[0] = ScalerUnit[1] = 0;
                }
                else
                {
                    object[] arr = (object[])value;
                    if (arr.Length != 2)
                    {
                        throw new Exception("setValue failed. Invalid scaler unit value.");
                    }
                    ScalerUnit[0] = Convert.ToInt32(arr[0]);
                    ScalerUnit[1] = Convert.ToInt32(arr[1]);
                }                
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }
        #endregion
    }
}
