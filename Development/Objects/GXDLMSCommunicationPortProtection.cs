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
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSCommunicationPortProtection
    /// </summary>
    public class GXDLMSCommunicationPortProtection : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSCommunicationPortProtection()
        : this("0.0.44.2.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSCommunicationPortProtection(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSCommunicationPortProtection(string ln, ushort sn)
        : base(ObjectType.CommunicationPortProtection, ln, sn)
        {
            ProtectionMode = ProtectionMode.LockedOnFailedAttempts;
            SteepnessFactor = 1;
            ProtectionStatus = ProtectionStatus.Unlocked;
        }

        /// <summary>
        /// Controls the protection mode. 
        /// </summary>
        [XmlIgnore()]
        public ProtectionMode ProtectionMode
        {
            get;
            set;
        }

        /// <summary>
        /// Number of allowed failed communication attempts before port is disabled.
        /// </summary>
        [XmlIgnore()]
        public UInt16 AllowedFailedAttempts
        {
            get;
            set;
        }

        /// <summary>
        /// The lockout time. 
        /// </summary>
        [XmlIgnore()]
        public UInt32 InitialLockoutTime
        {
            get;
            set;
        }

        /// <summary>
        /// Holds a factor that controls how the lockout time is increased with 
        /// each failed attempt.
        /// </summary>
        [XmlIgnore()]
        public byte SteepnessFactor
        {
            get;
            set;
        }

        /// <summary>
        /// The lockout time. 
        /// </summary>
        [XmlIgnore()]
        public UInt32 MaxLockoutTime
        {
            get;
            set;
        }

        /// <summary>
        /// The communication port being protected
        /// </summary>
        [XmlIgnore()]
        public GXDLMSObject Port
        {
            get;
            set;
        }

        /// <summary>
        /// Current protection status.
        /// </summary>
        [XmlIgnore()]
        public ProtectionStatus ProtectionStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Failed attempts. 
        /// </summary>
        [XmlIgnore()]
        public UInt32 FailedAttempts
        {
            get;
            set;
        }

        /// <summary>
        /// Total failed attempts. 
        /// </summary>
        [XmlIgnore()]
        public UInt32 CumulativeFailedAttempts
        {
            get;
            set;
        }

        /// <summary>
        /// Resets FailedAttempts and current lockout time to zero.
        /// Protection status is set to unlocked.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Reset(GXDLMSClient client)
        {
            return client.Method(this, 1, (sbyte)0);
        }

        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, ProtectionMode, AllowedFailedAttempts, InitialLockoutTime , SteepnessFactor,
            MaxLockoutTime, Port, ProtectionStatus, FailedAttempts, CumulativeFailedAttempts};
        }

        #region IGXDLMSBase Members

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                FailedAttempts = 0;
                //TODO: Reset current lockout time.
                if (ProtectionMode == ProtectionMode.LockedOnFailedAttempts)
                {
                    ProtectionStatus = ProtectionStatus.Unlocked;
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
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
            //ProtectionMode
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //AllowedFailedAttempts
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //InitialLockoutTime
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //SteepnessFactor
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //MaxLockoutTime
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            //Port
            if (all || CanRead(7))
            {
                attributes.Add(7);
            }
            //ProtectionStatus
            if (all || CanRead(8))
            {
                attributes.Add(8);
            }
            //FailedAttempts
            if (all || CanRead(9))
            {
                attributes.Add(9);
            }
            //CumulativeFailedAttempts
            if (all || CanRead(10))
            {
                attributes.Add(10);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Protection mode",
                "Allowed failed attempts", "Initial lockout time", "Steepness factor",
            "Max lockout time", "Port", "Protection status", "Failed attempts", "Cumulative failed attempts" };
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Reset" };
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 10;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
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
                    ret = DataType.Enum;
                    break;
                case 3:
                    ret = DataType.UInt16;
                    break;
                case 4:
                    ret = DataType.UInt32;
                    break;
                case 5:
                    ret = DataType.UInt8;
                    break;
                case 6:
                    ret = DataType.UInt32;
                    break;
                case 7:
                    ret = DataType.OctetString;
                    break;
                case 8:
                    ret = DataType.Enum;
                    break;
                case 9:
                    ret = DataType.UInt32;
                    break;
                case 10:
                    ret = DataType.UInt32;
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
                    ret = ProtectionMode;
                    break;
                case 3:
                    ret = AllowedFailedAttempts;
                    break;
                case 4:
                    ret = InitialLockoutTime;
                    break;
                case 5:
                    ret = SteepnessFactor;
                    break;
                case 6:
                    ret = MaxLockoutTime;
                    break;
                case 7:
                    if (Port == null)
                    {
                        ret = GXCommon.LogicalNameToBytes(null);
                    }
                    else
                    {
                        ret = GXCommon.LogicalNameToBytes(Port.LogicalName);
                    }
                    break;
                case 8:
                    ret = ProtectionStatus;
                    break;
                case 9:
                    ret = FailedAttempts;
                    break;
                case 10:
                    ret = CumulativeFailedAttempts;
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
                    ProtectionMode = (ProtectionMode)Convert.ToByte(e.Value);
                    break;
                case 3:
                    AllowedFailedAttempts = Convert.ToUInt16(e.Value);
                    break;
                case 4:
                    InitialLockoutTime = Convert.ToUInt32(e.Value);
                    break;
                case 5:
                    SteepnessFactor = Convert.ToByte(e.Value);
                    break;
                case 6:
                    MaxLockoutTime = Convert.ToUInt32(e.Value);
                    break;
                case 7:
                    Port = settings.Objects.FindByLN(ObjectType.None, GXCommon.ToLogicalName(e.Value));
                    break;
                case 8:
                    ProtectionStatus = (ProtectionStatus)Convert.ToByte(e.Value);
                    break;
                case 9:
                    FailedAttempts = Convert.ToUInt32(e.Value);
                    break;
                case 10:
                    CumulativeFailedAttempts = Convert.ToUInt32(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            ProtectionMode = (ProtectionMode)reader.ReadElementContentAsInt("ProtectionMode");
            AllowedFailedAttempts = (UInt16)reader.ReadElementContentAsInt("AllowedFailedAttempts");
            InitialLockoutTime = (UInt32)reader.ReadElementContentAsLong("InitialLockoutTime");
            SteepnessFactor = (byte)reader.ReadElementContentAsInt("SteepnessFactor");
            MaxLockoutTime = (UInt32)reader.ReadElementContentAsLong("MaxLockoutTime");
            string port = reader.ReadElementContentAsString("Port");
            if (string.IsNullOrEmpty(port))
            {
                Port = null;
            }
            else
            {
                Port = reader.Objects.FindByLN(ObjectType.None, port);
                //Save port object for data object if it's not loaded yet.
                if (Port == null)
                {
                    Port = GXDLMSClient.CreateObject(ObjectType.Data);
                    Port.Version = 0;
                    Port.LogicalName = port;
                }
            }
            ProtectionStatus = (ProtectionStatus)reader.ReadElementContentAsInt("ProtectionStatus");
            FailedAttempts = (UInt32)reader.ReadElementContentAsLong("FailedAttempts");
            CumulativeFailedAttempts = (UInt32)reader.ReadElementContentAsLong("CumulativeFailedAttempts");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("ProtectionMode", (int)ProtectionMode, 2);
            writer.WriteElementString("AllowedFailedAttempts", AllowedFailedAttempts, 3);
            writer.WriteElementString("InitialLockoutTime", InitialLockoutTime, 4);
            writer.WriteElementString("SteepnessFactor", SteepnessFactor, 5);
            writer.WriteElementString("MaxLockoutTime", MaxLockoutTime, 6);
            if (Port == null)
            {
                writer.WriteElementString("Port", "", 7);
            }
            else
            {
                writer.WriteElementString("Port", Port.LogicalName, 7);
            }
            writer.WriteElementString("ProtectionStatus", (int)ProtectionStatus, 8);
            writer.WriteElementString("FailedAttempts", FailedAttempts, 9);
            writer.WriteElementString("CumulativeFailedAttempts", CumulativeFailedAttempts, 10);
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
            if (Port is GXDLMSData)
            {
                Port = reader.Objects.FindByLN(ObjectType.None, Port.LogicalName);
            }
        }
        #endregion
    }
}
