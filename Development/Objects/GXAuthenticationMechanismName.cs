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
using System.Xml.Serialization;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    public class GXAuthenticationMechanismName
    {       
        /// <summary>
        /// Constuctor.
        /// </summary>
        public GXAuthenticationMechanismName()
        {
            JointIsoCtt = 2;
            Country = 16;
            CountryName = 756;
            IdentifiedOrganization = 5;
            DlmsUA = 8;
            AuthenticationMechanismName = 2;
            MechanismId = Authentication.None;
        }

        [XmlIgnore()]
        public byte JointIsoCtt
        {
            get;
            set;
        }
        [XmlIgnore()]
        public byte Country
        {
            get;
            set;
        }
        [XmlIgnore()]
        public UInt16 CountryName
        {
            get;
            set;
        }
        [XmlIgnore()]
        public byte IdentifiedOrganization
        {
            get;
            set;
        }

        [XmlIgnore()]
        public byte DlmsUA
        {
            get;
            set;
        }
        [XmlIgnore()]
        public byte AuthenticationMechanismName
        {
            get;
            set;
        }
        public Authentication MechanismId
        {
            get;
            set;
        }

        public override string ToString()
        {
            return JointIsoCtt.ToString() + " " + Country.ToString() + " " + 
                CountryName.ToString() + " " + IdentifiedOrganization.ToString() + " " + 
                DlmsUA.ToString() + " " + AuthenticationMechanismName.ToString() + " " + 
                MechanismId.ToString();
        }
    }
}
