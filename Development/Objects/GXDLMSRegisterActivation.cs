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
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSObjectDefinition
    {
        public ObjectType ClassId
        {
            get;
            set;
        }
        
        public string LogicalName
        {
            get;
            set;
        }
    }

    public class GXDLMSRegisterActivation : GXDLMSObject, IGXDLMSBase
    {        
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSRegisterActivation()
            : base(ObjectType.RegisterActivation)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSRegisterActivation(string ln)
            : base(ObjectType.RegisterActivation, ln, 0)
        {
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSRegisterActivation(string ln, ushort sn)
            : base(ObjectType.RegisterActivation, ln, 0)
        {
        }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(2)]
        public GXDLMSObjectDefinition[] RegisterAssignment
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(3)]
        public object MaskList
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore()]
        [GXDLMSAttribute(4, DataType.OctetString)]
        public string ActiveMask
        {
            get;
            set;
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, RegisterAssignment, MaskList, ActiveMask };
        }

        #region IGXDLMSBase Members

        /// <summary>
        /// Data interface do not have any methods.
        /// </summary>
        /// <param name="index"></param>
        void IGXDLMSBase.Invoke(int index, Object parameters)
        {
            throw new ArgumentException("Invoke failed. Invalid attribute index.");
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 4;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 3;
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
                type = DataType.Array;
                List<byte> data = new List<byte>();     
                data.Add((byte) DataType.Array);
                if (RegisterAssignment == null)
                {
                    data.Add(0);
                }
                else
                {
                    data.Add((byte) RegisterAssignment.Length);
                    foreach (GXDLMSObjectDefinition it in RegisterAssignment)
                    {
                        data.Add((byte) DataType.Structure);
                        data.Add(2);
                        GXCommon.SetData(data, DataType.UInt16, it.ClassId);
                        GXCommon.SetData(data, DataType.OctetString, it.LogicalName);
                    }
                }
                return data.ToArray();
            }
            if (index == 4)
            {
                type = DataType.OctetString;
                return ASCIIEncoding.ASCII.GetBytes(ActiveMask);
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
                RegisterAssignment = null;
                if (value != null)
                {
                    List<GXDLMSObjectDefinition> items = new List<GXDLMSObjectDefinition>();
                    foreach(Object[] it in (Object[]) value)
                    {
                        GXDLMSObjectDefinition item = new GXDLMSObjectDefinition();
                        item.ClassId = (ObjectType) Convert.ToInt32(it[0]);
                        item.LogicalName = GXDLMSObject.toLogicalName((byte[]) it[1]);
                        items.Add(item);
                    }
                    RegisterAssignment = items.ToArray();
                }  
            }
            else if (index == 4)
            {
                if (value == null)
                {
                    ActiveMask = null;
                }
                else
                {
                    ActiveMask = GXDLMSClient.ChangeType((byte[])value, DataType.String).ToString();
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
