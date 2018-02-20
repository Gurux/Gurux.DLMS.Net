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

using Gurux.DLMS.Internal;
using System.Text;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSSeasonProfile
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSeasonProfile()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSeasonProfile(string name, GXDateTime start, string weekName)
        {
            if (name != null)
            {
                Name = ASCIIEncoding.ASCII.GetBytes(name);
            }
            Start = start;
            if (weekName != null)
            {
                WeekName = ASCIIEncoding.ASCII.GetBytes(weekName);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSeasonProfile(string name, GXDateTime start, GXDLMSWeekProfile weekProfile)
        {
            if (name != null)
            {
                Name = ASCIIEncoding.ASCII.GetBytes(name);
            }
            Start = start;
            if (weekProfile != null)
            {
                WeekName = weekProfile.Name;
            }
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSeasonProfile(byte[] name, GXDateTime start, byte[] weekName)
        {
            Name = name;
            Start = start;
            WeekName = weekName;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSSeasonProfile(byte[] name, GXDateTime start, GXDLMSWeekProfile weekProfile)
        {
            Name = name;
            Start = start;
            WeekName = weekProfile.Name;
        }


        /// <summary>
        /// Name of season profile.
        /// </summary>
        /// <remarks>
        /// Some manufacturers are using non ASCII names.
        /// </remarks>
        public byte[] Name
        {
            get;
            set;
        }

        /// <summary>
        /// Season Profile start time.
        /// </summary>
        public GXDateTime Start
        {
            get;
            set;
        }

        /// <summary>
        /// Week name of season profile.
        /// </summary>
        /// <remarks>
        /// Some manufacturers are using non ASCII names.
        /// </remarks>
        public byte[] WeekName
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (GXCommon.IsAscii(Name))
            {
                sb.Append(ASCIIEncoding.ASCII.GetString(Name));
            }
            else
            {
                sb.Append(GXDLMSTranslator.ToHex(Name));
            }
            sb.Append(' ');
            sb.Append(Start.ToFormatString());
            sb.Append(' ');
            if (GXCommon.IsAscii(WeekName))
            {
                sb.Append(ASCIIEncoding.ASCII.GetString(WeekName));
            }
            else
            {
                sb.Append(GXDLMSTranslator.ToHex(WeekName));
            }
            return sb.ToString();
        }

    }
}
