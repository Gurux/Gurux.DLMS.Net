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
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    public enum CaptureMethod : byte
    {
        /// <summary>
        /// Data is captured with Capture-method.
        /// </summary>
        Invoke,
        /// <summary>
        /// Data is captured upon reading.
        /// </summary>
        Implicit
    }

    /// <summary>
    /// Online help:
    /// http://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCompactData
    /// </summary>
    public class GXDLMSCompactData : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSCompactData()
        : this(null, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSCompactData(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSCompactData(string ln, ushort sn)
        : base(ObjectType.CompactData, ln, sn)
        {
        }

        /// <summary>
        /// Compact buffer
        /// </summary>
        [XmlIgnore()]
        public string Buffer
        {
            get;
            set;
        }

        /// <summary>
        /// Capture objects.
        /// </summary>
        [XmlIgnore()]
        public object CaptureObjects
        {
            get;
            set;
        }

        /// <summary>
        /// Template ID.
        /// </summary>
        [XmlIgnore()]
        public byte TemplateId
        {
            get;
            set;
        }

        /// <summary>
        /// Template description.
        /// </summary>
        [XmlIgnore()]
        public byte[] TemplateDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Capture method.
        /// </summary>
        [XmlIgnore()]
        public CaptureMethod CaptureMethod
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Buffer, CaptureObjects, TemplateId, TemplateDescription, CaptureMethod };
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
            //Buffer
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //CaptureObjects
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //TemplateId
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //TemplateDescription
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //CaptureMethod
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Buffer", "CaptureObjects", "TemplateId", "TemplateDescription", "CaptureMethod" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 6;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 2;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.OctetString;
                case 3:
                    return DataType.Array;
                case 4:
                    return DataType.UInt8;
                case 5:
                    return DataType.OctetString;
                case 6:
                    return DataType.Enum;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    return Buffer;
                case 3:
                    return CaptureObjects;
                case 4:
                    return TemplateId;
                case 5:
                    return TemplateDescription;
                case 6:
                    return CaptureMethod;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
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
                if (e.Value is byte[])
                {
                    Buffer = GXCommon.ToHex((byte[])e.Value, true);
                }
                else
                {
                    Buffer = Convert.ToString(e.Value);
                }
            }
            else if (e.Index == 3)
            {
                CaptureObjects = e.Value;
            }
            else if (e.Index == 4)
            {
                TemplateId = (byte)e.Value;
            }
            else if (e.Index == 5)
            {
                TemplateDescription = (byte[])e.Value;
            }
            else if (e.Index == 6)
            {
                CaptureMethod = (CaptureMethod)e.Value;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
