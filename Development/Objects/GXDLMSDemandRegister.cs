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
    public class GXDLMSDemandRegister : GXDLMSObject
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSDemandRegister()
            : base(ObjectType.DemandRegister)
        {
        }        

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSDemandRegister(string ln)
            : base(ObjectType.DemandRegister, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSDemandRegister(string ln, ushort sn)
            : base(ObjectType.DemandRegister, ln, 0)
        {
        }

        /// <summary>
        /// Current avarage value of COSEM Data object.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(2, Access = AccessMode.Read)]
        public object CurrentAvarageValue
        {
            get;
            set;
        }

        /// <summary>
        /// Last avarage value of COSEM Data object.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(3, Access = AccessMode.Read)]
        public object LastAvarageValue
        {
            get;
            set;
        }        

        /// <summary>
        /// Scaler of COSEM Register object.
        /// </summary>        
        [DefaultValue(1)]        
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
        /// Scaler and unit of COSEM object.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(4, Type = DataType.Array, Static = true, Access = AccessMode.Read, Order = 1)]
        internal int[] ScalerUnit
        {
            get;
            set;
        }        

        /// <summary>
        /// Scaler of COSEM Register object.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(5, Access = AccessMode.Read)]
        public int Status
        {
            get;
            set;
        }

        /// <summary>
        /// Capture time of COSEM Register object.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(6, DataType.DateTime, Access = AccessMode.Read)]        
        public DateTime CaptureTime
        {
            get;
            set;
        }        

        /// <summary>
        /// Current start time of COSEM Register object.
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(7, DataType.DateTime, Access = AccessMode.Read)]        
        public DateTime StartTimeCurrent
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(8, Access = AccessMode.Read)]
        public ulong Period
        {
            get;
            set;
        }

        [XmlIgnore()]
        [GXDLMSAttribute(9, Access = AccessMode.Read)]
        public uint NumberOfPeriods
        {
            get;
            set;
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, CurrentAvarageValue, LastAvarageValue, ScalerUnit, 
                            Status, CaptureTime, StartTimeCurrent, Period, NumberOfPeriods };
        }
    }
}
