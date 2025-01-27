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
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSDisconnectControl
    /// </summary>
    public class GXDLMSDisconnectControl : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSDisconnectControl()
        : this("0.0.96.3.10.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSDisconnectControl(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSDisconnectControl(string ln, ushort sn)
        : base(ObjectType.DisconnectControl, ln, sn)
        {
            Version = 1;
        }

        /// <summary>
        /// Output state of COSEM Disconnect Control object.
        /// </summary>
        [XmlIgnore()]
        public bool OutputState
        {
            get;
            set;
        }

        /// <summary>
        /// Output state of COSEM Disconnect Control object.
        /// </summary>
        [XmlIgnore()]
        public ControlState ControlState
        {
            get;
            set;
        }

        /// <summary>
        ///  Control mode of COSEM Disconnect Control object.
        /// </summary>
        [XmlIgnore()]
        public ControlMode ControlMode
        {
            get;
            set;
        }

        /// <summary>
        /// Forces the disconnect control object into 'disconnected' state
        /// if remote disconnection is enabled(control mode > 0).
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] RemoteDisconnect(GXDLMSClient client)
        {
            return client.Method(this, 1, (sbyte)0);
        }

        /// <summary>
        /// Forces the disconnect control object into the 'ready_for_reconnection'
        /// state if a direct remote reconnection is disabled(control_mode = 1, 3, 5, 6).
        /// Forces the disconnect control object into the 'connected' state if
        /// a direct remote reconnection is enabled(control_mode = 2, 4).
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] RemoteReconnect(GXDLMSClient client)
        {
            return client.Method(this, 2, (sbyte)0);
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, OutputState, ControlState, ControlMode };
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
            //OutputState
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //ControlState
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //ControlMode
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Internal.GXCommon.GetLogicalNameString(),
                             "Output State",
                             "Control State",
                             "Control Mode"
                            };
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Remote disconnect", "Remote reconnect" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 1;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 4;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 2;
        }

        /// <inheritdoc />
        public override DataType GetDataType(int index)
        {
            if (index == 1)
            {
                return DataType.OctetString;
            }
            if (index == 2)
            {
                return DataType.Boolean;
            }
            if (index == 3)
            {
                return DataType.Enum;
            }
            if (index == 4)
            {
                return DataType.Enum;
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
                return OutputState;
            }
            if (e.Index == 3)
            {
                return ControlState;
            }
            if (e.Index == 4)
            {
                return ControlMode;
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
                OutputState = Convert.ToBoolean(e.Value);
            }
            else if (e.Index == 3)
            {
                ControlState = (ControlState)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 4)
            {
                ControlMode = (ControlMode)Convert.ToInt32(e.Value);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            OutputState = reader.ReadElementContentAsInt("OutputState") != 0;
            ControlState = (ControlState)reader.ReadElementContentAsInt("ControlState");
            ControlMode = (ControlMode)reader.ReadElementContentAsInt("ControlMode");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("OutputState", OutputState, 2);
            writer.WriteElementString("ControlState", (int)ControlState, 0, 3);
            writer.WriteElementString("ControlMode", (int)ControlMode, 0, 4);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
