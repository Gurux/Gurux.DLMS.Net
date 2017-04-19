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

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// The IEC 14908 identification interface class allows the identification of the network on which the device is connected to.
    /// </summary>
    public class GXDLMSIEC14908Identification : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSIEC14908Identification()
        : this("0.0.34.0.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSIEC14908Identification(string ln)
        : base(ObjectType.IEC14908Identification, ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSIEC14908Identification(string ln, ushort sn)
        : base(ObjectType.IEC14908Identification, ln, sn)
        {
        }

        /// <summary>
        /// Node ID.
        /// </summary>
        [XmlIgnore()]
        public byte NodeId
        {
            get;
            set;
        }

        /// <summary>
        /// Subnet ID.
        /// </summary>
        [XmlIgnore()]
        public byte SubnetId
        {
            get;
            set;
        }

        /// <summary>
        /// Domain ID.
        /// </summary>
        [XmlIgnore()]
        public string DomainId
        {
            get;
            set;
        }
        /// <summary>
        /// Program ID.
        /// </summary>
        [XmlIgnore()]
        public string ProgramId
        {
            get;
            set;
        }
        /// <summary>
        /// Unique node ID.
        /// </summary>
        [XmlIgnore()]
        public string UniqueNodeId
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, NodeId, SubnetId, DomainId, null, ProgramId, UniqueNodeId };
        }

        #region IGXDLMSBase Members

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

            //NodeId
            if (CanRead(2))
            {
                attributes.Add(2);
            }

            //SubnetId
            if (CanRead(3))
            {
                attributes.Add(3);
            }

            //DomainId
            if (CanRead(4))
            {
                attributes.Add(4);
            }

            //ProgramId
            if (CanRead(6))
            {
                attributes.Add(6);
            }

            //UniqueNodeId
            if (CanRead(7))
            {
                attributes.Add(7);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Gurux.DLMS.Properties.Resources.LogicalNameTxt, "NodeId", "SubnetId", "DomainId", "SelfIdentification", "ProgramId", "UniqueNodeId" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 7;
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
                return DataType.UInt8;
            }
            if (index == 3)
            {
                return DataType.UInt8;
            }
            if (index == 4)
            {
                return DataType.OctetString;
            }
            if (index == 5)
            {
                return DataType.None;
            }
            if (index == 6)
            {
                return DataType.OctetString;
            }
            if (index == 7)
            {
                return DataType.OctetString;
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
                return NodeId;
            }
            if (e.Index == 3)
            {
                return SubnetId;
            }
            if (e.Index == 4)
            {
                return DomainId;
            }
            if (e.Index == 5)
            {
                return ProgramId;
            }
            if (e.Index == 6)
            {
                return UniqueNodeId;
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
                    LogicalName = GXDLMSClient.ChangeType((byte[])e.Value, DataType.OctetString, settings.UseUtc2NormalTime).ToString();
                }
            }
            else if (e.Index == 2)
            {
                NodeId = (byte)e.Value;
            }
            else if (e.Index == 3)
            {
                SubnetId = (byte)e.Value;
            }
            else if (e.Index == 4)
            {
                if (e.Value is string)
                {
                    DomainId = e.Value.ToString();
                }
                else
                {
                    DomainId = GXDLMSClient.ChangeType((byte[])e.Value, DataType.OctetString, false).ToString();
                }
            }
            else if (e.Index == 5)
            {
                if (e.Value is string)
                {
                    ProgramId = e.Value.ToString();
                }
                else
                {
                    ProgramId = GXDLMSClient.ChangeType((byte[])e.Value, DataType.OctetString, false).ToString();
                }
            }
            else if (e.Index == 6)
            {
                if (e.Value is string)
                {
                    UniqueNodeId = e.Value.ToString();
                }
                else
                {
                    UniqueNodeId = GXDLMSClient.ChangeType((byte[])e.Value, DataType.OctetString, settings.UseUtc2NormalTime).ToString();
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }
        #endregion
    }
}
