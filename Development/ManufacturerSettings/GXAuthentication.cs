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
using System.ComponentModel;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.ManufacturerSettings
{
    /// <summary>
    /// Authentication class is used to give authentication iformation to the server.
    /// </summary>
#if !WINDOWS_UWP
    [Serializable]
#endif
    public class GXAuthentication
    {
        public GXAuthentication()
        {
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">Authentication type</param>
        /// <param name="clientAddress">Client address.</param>
        public GXAuthentication(Authentication type, int clientAddress) :
            this(type, "", clientAddress)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="auth">Authentication type</param>
        /// <param name="pw">Used password.</param>
        /// <param name="clientAddress">Client address.</param>
        public GXAuthentication(Authentication type, string pw, int clientAddress)
        {
            Type = type;
            if (type == Authentication.None)
            {
                Password = "";
            }
            else
            {
                Password = pw;
            }
            ClientAddress = clientAddress;
        }

        /// <summary>
        /// Is authentication selected.
        /// </summary>
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [DefaultValue(false)]
        public bool Selected
        {
            get;
            set;
        }

        /// <summary>
        /// Authentication type
        /// </summary>
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public Authentication Type
        {
            get;
            set;
        }

        /// <summary>
        /// Name of authentication.
        /// </summary>
        [DefaultValue(null)]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Is connection pre-established.
        /// </summary>
        [DefaultValue(false)]
        public bool PreEstablished
        {
            get;
            set;
        }

        /// <summary>
        /// Client address.
        /// </summary>
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [DefaultValue(0)]
        public int ClientAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Client address has replace this. This is obsolete. Use ClientAddress instead.
        /// </summary>
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [DefaultValue(0)]
        public object ClientID
        {
            get
            {
                return ClientAddress;
            }
            set
            {
                ClientAddress = Convert.ToInt32(value);
            }
        }

        /// <summary>
        /// Used password.
        /// </summary>
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [DefaultValue(null)]
        public string Password
        {
            get;
            set;
        }
    }
}
