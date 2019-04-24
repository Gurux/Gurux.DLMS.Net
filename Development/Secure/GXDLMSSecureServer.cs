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
using System.Text;
#if !WINDOWS_UWP
using System.Security.Cryptography;
#endif
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;

namespace Gurux.DLMS.Secure
{
    public abstract class GXDLMSSecureServer : GXDLMSServer
    {
        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="logicalNameReferencing">
        /// Is logical name referencing used.
        ///</param>
        ///<param name="type">
        /// Interface type.
        ///</param>
        [Obsolete("Use other constructor.")]
        public GXDLMSSecureServer(bool logicalNameReferencing, InterfaceType type) :
            base(logicalNameReferencing, type)
        {
            Ciphering = new GXCiphering(ASCIIEncoding.ASCII.GetBytes("ABCDEFGH"));
            Settings.Cipher = Ciphering;
        }

        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="logicalNameReferencing">
        /// Is logical name referencing used.
        ///</param>
        ///<param name="type">
        /// Interface type.
        ///</param>
        public GXDLMSSecureServer(GXDLMSAssociationLogicalName ln, GXDLMSHdlcSetup hdlc) :
            base(ln, hdlc)
        {
            Ciphering = new GXCiphering(ASCIIEncoding.ASCII.GetBytes("ABCDEFGH"));
            Settings.Cipher = Ciphering;
        }

        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="ln">Logical name settings. </param>
        ///<param name="hdlc">
        /// Interface type.
        ///</param>
        ///<param name="flagID">
        /// Three letters FLAG ID.
        ///</param>
        ///<param name="serialNumber">
        /// Meter serial number. Size of serial number is 5 bytes.
        ///</param>
        public GXDLMSSecureServer(GXDLMSAssociationLogicalName ln, GXDLMSHdlcSetup hdlc, string flagID, UInt64 serialNumber) :
            base(ln, hdlc)
        {
            if (flagID == null || flagID.Length != 3)
            {
                throw new ArgumentOutOfRangeException("Invalid FLAG ID.");
            }
            if (flagID == null || flagID.Length != 3)
            {
                throw new ArgumentOutOfRangeException("Invalid FLAG ID.");
            }
            ln.XDLMSContextInfo.settings = Settings;
            GXByteBuffer bb = new GXByteBuffer();
            bb.Add(flagID);
            GXByteBuffer serial = new GXByteBuffer();
            serial.SetUInt64(serialNumber);
            bb.Set(serial.Data, 3, 5);
            Ciphering = new GXCiphering(bb.Array());
            Settings.Cipher = Ciphering;
        }

        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="sn">Short name settings. </param>
        ///<param name="hdlc">
        /// Interface type.
        ///</param>
        ///<param name="flagID">
        /// Three letters FLAG ID.
        ///</param>
        ///<param name="serialNumber">
        /// Meter serial number. Size of serial number is 5 bytes.
        ///</param>
        public GXDLMSSecureServer(GXDLMSAssociationShortName sn, GXDLMSHdlcSetup hdlc, string flagID, UInt64 serialNumber) :
            base(sn, hdlc)
        {
            if (flagID == null || flagID.Length != 3)
            {
                throw new ArgumentOutOfRangeException("Invalid FLAG ID.");
            }
            if (flagID == null || flagID.Length != 3)
            {
                throw new ArgumentOutOfRangeException("Invalid FLAG ID.");
            }
            GXByteBuffer bb = new GXByteBuffer();
            bb.Add(flagID);
            GXByteBuffer serial = new GXByteBuffer();
            serial.SetUInt64(serialNumber);
            bb.Set(serial.Data, 3, 5);
            Ciphering = new GXCiphering(bb.Array());
            Settings.Cipher = Ciphering;
        }

        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="ln">Logical name settings. </param>
        ///<param name="hdlc">
        /// Interface type.
        ///</param>
        ///<param name="flagID">
        /// Three letters FLAG ID.
        ///</param>
        ///<param name="serialNumber">
        /// Meter serial number. Size of serial number is 5 bytes.
        ///</param>
        public GXDLMSSecureServer(GXDLMSAssociationLogicalName ln, GXDLMSTcpUdpSetup wrapper, string flagID, UInt64 serialNumber) :
            base(ln, wrapper)
        {
            if (flagID == null || flagID.Length != 3)
            {
                throw new ArgumentOutOfRangeException("Invalid FLAG ID.");
            }
            if (flagID == null || flagID.Length != 3)
            {
                throw new ArgumentOutOfRangeException("Invalid FLAG ID.");
            }
            ln.XDLMSContextInfo.settings = Settings;
            GXByteBuffer bb = new GXByteBuffer();
            bb.Add(flagID);
            GXByteBuffer serial = new GXByteBuffer();
            serial.SetUInt64(serialNumber);
            bb.Set(serial.Data, 3, 5);
            Ciphering = new GXCiphering(bb.Array());
            Settings.Cipher = Ciphering;
        }

        ///<summary>
        /// Constructor.
        ///</summary>
        ///<param name="sn">Short name settings. </param>
        ///<param name="hdlc">
        /// Interface type.
        ///</param>
        ///<param name="flagID">
        /// Three letters FLAG ID.
        ///</param>
        ///<param name="serialNumber">
        /// Meter serial number. Size of serial number is 5 bytes.
        ///</param>
        public GXDLMSSecureServer(GXDLMSAssociationShortName sn, GXDLMSTcpUdpSetup wrapper, string flagID, UInt64 serialNumber) :
            base(sn, wrapper)
        {
            if (flagID == null || flagID.Length != 3)
            {
                throw new ArgumentOutOfRangeException("Invalid FLAG ID.");
            }
            if (flagID == null || flagID.Length != 3)
            {
                throw new ArgumentOutOfRangeException("Invalid FLAG ID.");
            }
            GXByteBuffer bb = new GXByteBuffer();
            bb.Add(flagID);
            GXByteBuffer serial = new GXByteBuffer();
            serial.SetUInt64(serialNumber);
            bb.Set(serial.Data, 3, 5);
            Ciphering = new GXCiphering(bb.Array());
            Settings.Cipher = Ciphering;
        }

        /// <summary>
        /// Ciphering settings.
        /// </summary>
        public GXCiphering Ciphering
        {
            get;
            private set;
        }

        /// <summary>
        /// Key Encrypting Key, also known as Master key.
        /// </summary>
        public byte[] Kek
        {
            get
            {
                return Settings.Kek;
            }
            set
            {
                Settings.Kek = value;
            }
        }
    }
}
