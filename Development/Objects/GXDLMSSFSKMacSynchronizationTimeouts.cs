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

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
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

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //SearchInitiatorTimeout
            if (CanRead(2))
            {
                attributes.Add(2);
            }
            //SynchronizationConfirmationTimeout
            if (CanRead(3))
            {
                attributes.Add(3);
            }
            //TimeOutNotAddressed
            if (CanRead(4))
            {
                attributes.Add(4);
            }
            //TimeOutFrameNotOK
            if (CanRead(5))
            {
                attributes.Add(5);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "SearchInitiatorTimeout", "SynchronizationConfirmationTimeout", "TimeOutNotAddressed", "TimeOutFrameNotOK" };
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
            if (index == 1)
            {
                return DataType.OctetString;
            }
            if (index == 2)
            {
                return DataType.UInt16;
            }
            if (index == 3)
            {
                return DataType.UInt16;
            }
            if (index == 4)
            {
                return DataType.UInt16;
            }
            if (index == 5)
            {
                return DataType.UInt16;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                return SearchInitiatorTimeout;
            }
            if (e.Index == 3)
            {
                return SynchronizationConfirmationTimeout;
            }
            if (e.Index == 4)
            {
                return TimeOutNotAddressed;
            }
            if (e.Index == 5)
            {
                return TimeOutFrameNotOK;
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
                SearchInitiatorTimeout = (UInt16)e.Value;
            }
            else if (e.Index == 3)
            {
                SynchronizationConfirmationTimeout = (UInt16)e.Value;
            }
            else if (e.Index == 4)
            {
                TimeOutNotAddressed = (UInt16)e.Value;
            }
            else if (e.Index == 5)
            {
                TimeOutFrameNotOK = (UInt16)e.Value;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
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
            writer.WriteElementString("SearchInitiatorTimeout", SearchInitiatorTimeout);
            writer.WriteElementString("SynchronizationConfirmationTimeout", SynchronizationConfirmationTimeout);
            writer.WriteElementString("TimeOutNotAddressed", TimeOutNotAddressed);
            writer.WriteElementString("TimeOutFrameNotOK", TimeOutFrameNotOK);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
