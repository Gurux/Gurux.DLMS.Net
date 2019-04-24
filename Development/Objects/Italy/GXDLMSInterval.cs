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

using Gurux.DLMS.Objects.Italy.Enums;
using System.Xml.Serialization;

namespace Gurux.DLMS.Objects.Italy
{
    public class GXDLMSInterval
    {
        /// <summary>
        /// Start hour of the interval.
        /// </summary>
        /// <remarks>
        /// Possible values 0-23.
        /// </remarks>
        [XmlIgnore()]
        public byte StartHour
        {
            get;
            set;
        }

        /// <summary>
        /// Tariff used in the current interval
        /// </summary>
        [XmlIgnore()]
        public DefaultTariffBand IntervalTariff
        {
            get;
            set;
        }

        /// <summary>
        /// Is interval used.
        /// </summary>
        [XmlIgnore()]
        public bool UseInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Weekly Activation.
        /// </summary>
        [XmlIgnore()]
        public WeeklyActivation WeeklyActivation
        {
            get;
            set;
        }

        /// <summary>
        /// Month of the year of the special day.
        /// </summary>
        [XmlIgnore()]
        public byte SpecialDayMonth
        {
            get;
            set;
        }
        /// <summary>
        /// Day of the month of the special day.
        /// </summary>
        [XmlIgnore()]
        public byte SpecialDay
        {
            get;
            set;
        }

        /// <summary>
        /// Day of the month of the special day.
        /// </summary>
        [XmlIgnore()]
        public bool SpecialDayEnabled
        {
            get;
            set;
        }
    }

}
