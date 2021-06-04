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
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSTcpUdpSetup
    /// </summary>
    public class GXDLMSTcpUdpSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSTcpUdpSetup()
        : this("0.0.25.0.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSTcpUdpSetup(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSTcpUdpSetup(string ln, ushort sn)
        : base(ObjectType.TcpUdpSetup, ln, sn)
        {
            Port = 4059;
            InactivityTimeout = 180;
            MaximumSegmentSize = 576;
        }

        /// <inheritdoc cref="GXDLMSObject.LogicalName"/>
        [DefaultValue("0.0.25.0.0.255")]
        override public string LogicalName
        {
            get;
            set;
        }

        /// <summary>
        /// TCP/UDP port number on which the physical device is
        /// listening for the DLMS/COSEM application.
        /// </summary>
        [XmlIgnore()]
        [DefaultValue(4059)]
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// References an IP setup object by its logical name. The referenced object
        /// contains information about the IP Address settings of the IP layer
        /// supporting the TCP-UDP layer.
        /// </summary>
        [XmlIgnore()]
        public string IPReference
        {
            get;
            set;
        }

        /// <summary>
        /// TCP can indicate the maximum receive segment size to its partner.
        /// </summary>
        [XmlIgnore()]
        [DefaultValue(576)]
        public int MaximumSegmentSize
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum number of simultaneous connections the COSEM
        /// TCP/UDP based transport layer is able to support.
        /// </summary>
        [XmlIgnore()]
        public int MaximumSimultaneousConnections
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the time, expressed in seconds over which, if no frame is
        /// received from the COSEM client, the inactive TCP connection shall be aborted.
        /// When this value is set to 0, this means that the inactivity_time_out is
        /// not operational. In other words, a TCP connection, once established,
        /// in normal conditions � no power failure, etc. � will never be aborted by the COSEM server.
        /// </summary>
        [XmlIgnore()]
        [DefaultValue(180)]
        public int InactivityTimeout
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Port, IPReference,
                              MaximumSegmentSize, MaximumSimultaneousConnections,
                              InactivityTimeout
                            };
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
            //Port
            if (all || !base.IsRead(2))
            {
                attributes.Add(2);
            }
            //IPReference
            if (all || !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //MaximumSegmentSize
            if (all || !base.IsRead(4))
            {
                attributes.Add(4);
            }
            //MaximumSimultaneousConnections
            if (all || !base.IsRead(5))
            {
                attributes.Add(5);
            }
            //InactivityTimeout
            if (all || !base.IsRead(6))
            {
                attributes.Add(6);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {

            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Port", "IP Reference",
                              "Maximum Segment Size", "Maximum Simultaneous Connections", "Inactivity Timeout"
                            };
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
            return 6;
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
                return DataType.OctetString;
            }
            if (index == 4)
            {
                return DataType.UInt16;
            }
            if (index == 5)
            {
                return DataType.UInt8;
            }
            if (index == 6)
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
                return Port;
            }
            if (e.Index == 3)
            {
                return GXCommon.LogicalNameToBytes(IPReference);
            }
            if (e.Index == 4)
            {
                return MaximumSegmentSize;
            }
            if (e.Index == 5)
            {
                return MaximumSimultaneousConnections;
            }
            if (e.Index == 6)
            {
                return InactivityTimeout;
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
                Port = Convert.ToInt32(e.Value);
            }
            else if (e.Index == 3)
            {
                IPReference = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 4)
            {
                MaximumSegmentSize = Convert.ToInt32(e.Value);
            }
            else if (e.Index == 5)
            {
                MaximumSimultaneousConnections = Convert.ToInt32(e.Value);
            }
            else if (e.Index == 6)
            {
                InactivityTimeout = Convert.ToInt32(e.Value);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            Port = reader.ReadElementContentAsInt("Port");
            IPReference = reader.ReadElementContentAsString("IPReference");
            MaximumSegmentSize = reader.ReadElementContentAsInt("MaximumSegmentSize");
            MaximumSimultaneousConnections = reader.ReadElementContentAsInt("MaximumSimultaneousConnections");
            InactivityTimeout = reader.ReadElementContentAsInt("InactivityTimeout");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Port", Port, 2);
            writer.WriteElementString("IPReference", IPReference, 3);
            writer.WriteElementString("MaximumSegmentSize", MaximumSegmentSize, 4);
            writer.WriteElementString("MaximumSimultaneousConnections", MaximumSimultaneousConnections, 5);
            writer.WriteElementString("InactivityTimeout", InactivityTimeout, 6);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }


        #endregion
    }
}
