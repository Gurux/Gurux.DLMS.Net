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
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSSFSKMacSynchronizationTimeouts
    /// </summary>
    public class GXDLMSSFSKMacSynchronizationTimeouts : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSFSKMacSynchronizationTimeouts()
        : this("0.0.26.2.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSSFSKMacSynchronizationTimeouts(string ln)
        : base(ObjectType.SFSKMacSynchronizationTimeouts, ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSSFSKMacSynchronizationTimeouts(string ln, ushort sn)
        : base(ObjectType.SFSKMacSynchronizationTimeouts, ln, sn)
        {
        }

        /// <summary>
        /// Search initiator timeout.
        /// </summary>
        [XmlIgnore()]
        public UInt16 SearchInitiatorTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Synchronization confirmation timeout.
        /// </summary>
        [XmlIgnore()]
        public UInt16 SynchronizationConfirmationTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Time out not addressed.
        /// </summary>
        [XmlIgnore()]
        public UInt16 TimeOutNotAddressed
        {
            get;
            set;
        }

        /// <summary>
        /// Time out frame not OK.
        /// </summary>
        [XmlIgnore()]
        public UInt16 TimeOutFrameNotOK
        {
            get;
            set;
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, SearchInitiatorTimeout, SynchronizationConfirmationTimeout, TimeOutNotAddressed, TimeOutFrameNotOK };
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
            //SearchInitiatorTimeout
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //SynchronizationConfirmationTimeout
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //TimeOutNotAddressed
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //TimeOutFrameNotOK
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "SearchInitiatorTimeout", "SynchronizationConfirmationTimeout", "TimeOutNotAddressed", "TimeOutFrameNotOK" };
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override DataType GetDataType(int index)
        {
            DataType ret;
            switch (index)
            {
                case 1:
                    ret = DataType.OctetString;
                    break;
                case 2:
                    ret = DataType.UInt16;
                    break;
                case 3:
                    ret = DataType.UInt16;
                    break;
                case 4:
                    ret = DataType.UInt16;
                    break;
                case 5:
                    ret = DataType.UInt16;
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
                    ret = SearchInitiatorTimeout;
                    break;
                case 3:
                    ret = SynchronizationConfirmationTimeout;
                    break;
                case 4:
                    ret = TimeOutNotAddressed;
                    break;
                case 5:
                    ret = TimeOutFrameNotOK;
                    break;
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
                    SearchInitiatorTimeout = (ushort)e.Value;
                    break;
                case 3:
                    SynchronizationConfirmationTimeout = (ushort)e.Value;
                    break;
                case 4:
                    TimeOutNotAddressed = (ushort)e.Value;
                    break;
                case 5:
                    TimeOutFrameNotOK = (ushort)e.Value;
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            SearchInitiatorTimeout = (UInt16)reader.ReadElementContentAsInt("SearchInitiatorTimeout");
            SynchronizationConfirmationTimeout = (UInt16)reader.ReadElementContentAsInt("SynchronizationConfirmationTimeout");
            TimeOutNotAddressed = (UInt16)reader.ReadElementContentAsInt("TimeOutNotAddressed");
            TimeOutFrameNotOK = (UInt16)reader.ReadElementContentAsInt("TimeOutFrameNotOK");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("SearchInitiatorTimeout", SearchInitiatorTimeout, 2);
            writer.WriteElementString("SynchronizationConfirmationTimeout", SynchronizationConfirmationTimeout, 3);
            writer.WriteElementString("TimeOutNotAddressed", TimeOutNotAddressed, 4);
            writer.WriteElementString("TimeOutFrameNotOK", TimeOutFrameNotOK, 5);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
