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

namespace Gurux.DLMS.ManufacturerSettings
{
    /// <summary>
    /// Authentication class is used to give authentication iformation to the server.
    /// </summary>
    [Serializable]
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
        /// <param name="auth">Authentication type</param>
        /// <param name="clientID">Client Id.</param>
        public GXAuthentication(Authentication type, object clientID) :
            this(type, null, clientID)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="auth">Authentication type</param>
        /// <param name="pw">Used password.</param>
        /// <param name="clientID">Client Id.</param>
        public GXAuthentication(Authentication type, string pw, object clientID)
        {
            Type = type;
            Password = pw;
            ClientID = clientID;
        }

        /// <summary>
        /// Is authentication selected.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        public bool Selected
        {
            get;
            set;
        }

        /// <summary>
        /// Authentication type
        /// </summary>
        [Browsable(false)]
        public Authentication Type
        {
            get;
            set;
        }

        /// <summary>
        /// Client address.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        public object ClientID
        {
            get;
            set;
        }

        /// <summary>
        /// Used password.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        public string Password
        {
            get;
            set;
        } 
    }
}
