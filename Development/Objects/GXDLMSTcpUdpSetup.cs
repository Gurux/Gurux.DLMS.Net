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

namespace Gurux.DLMS.Objects
{
    public class GXDLMSTcpUdpSetup : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSTcpUdpSetup()
            : base(ObjectType.TcpUdpSetup, "0.0.25.0.0.255", 0)
        {
            Port = 4059;
            InactivityTimeout = 180;
            MaximumSegmentSize = 576;
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSTcpUdpSetup(string ln)
            : base(ObjectType.TcpUdpSetup, ln, 0)
        {
            Port = 4059;
            InactivityTimeout = 180;
            MaximumSegmentSize = 576;
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSTcpUdpSetup(string ln, ushort sn)
            : base(ObjectType.TcpUdpSetup, ln, 0)
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
        [GXDLMSAttribute(2)]
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
        [GXDLMSAttribute(3, DataType.OctetString)]
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
        [GXDLMSAttribute(4)]
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
        [GXDLMSAttribute(5)]
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
        /// in normal conditions – no power failure, etc. – will never be aborted by the COSEM server.
        /// </summary>
        [XmlIgnore()]
        [DefaultValue(180)]
        [GXDLMSAttribute(6)]
        public int InactivityTimeout
        {
            get;
            set;
        }

        public override object[] GetValues()
        {
            return new object[] { LogicalName, Port, IPReference, 
                MaximumSegmentSize, MaximumSimultaneousConnections, 
                InactivityTimeout };
        }

        #region IGXDLMSBase Members

        void IGXDLMSBase.Invoke(int index, Object parameters)
        {
            throw new ArgumentException("Invoke failed. Invalid attribute index.");
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 6;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
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
                type = DataType.UInt16;
                return Port;
            }
            if (index == 3)
            {
                type = DataType.OctetString;
                return IPReference;
            }
            if (index == 4)
            {
                type = DataType.UInt16;
                return MaximumSegmentSize;
            }
            if (index == 5)
            {
                type = DataType.UInt8;
                return MaximumSimultaneousConnections;
            }
            if (index == 6)
            {
                type = DataType.UInt16;
                return InactivityTimeout;
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
                Port = Convert.ToInt32(value);
            }
            else if (index == 3)
            {
                IPReference = Convert.ToString(value);
            }
            else if (index == 4)
            {
                MaximumSegmentSize = Convert.ToInt32(value);
            }
            else if (index == 5)
            {
                MaximumSimultaneousConnections = Convert.ToInt32(value);
            }
            else if (index == 6)
            {
                InactivityTimeout = Convert.ToInt32(value);
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }
        #endregion
    }
}
