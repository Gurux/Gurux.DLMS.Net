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

namespace Gurux.DLMS.Objects.Italy
{
    /// <summary>
    /// Tariff plan.
    /// </summary>
    public class GXTariffPlan
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXTariffPlan()
        {
            WinterSeason = new GXBandDescriptor();
            SummerSeason = new GXBandDescriptor();
        }
        /// <summary>
        /// Default tariff band.
        /// </summary>
        [XmlIgnore()]
        public byte DefaultTariffBand
        {
            get;
            set;
        }

        /// <summary>
        /// Winter season.
        /// </summary>
        [XmlIgnore()]
        public GXBandDescriptor WinterSeason
        {
            get;
            set;
        }

        /// <summary>
        /// Summer season.
        /// </summary>
        [XmlIgnore()]
        public GXBandDescriptor SummerSeason
        {
            get;
            set;
        }

        /// <summary>
        /// Weekly activation.
        /// </summary>
        [XmlIgnore()]
        public string WeeklyActivation
        {
            get;
            set;
        }

        /// <summary>
        /// Special days.
        /// </summary>
        [XmlIgnore()]
        public UInt16[] SpecialDays
        {
            get;
            set;
        }
    }

}
