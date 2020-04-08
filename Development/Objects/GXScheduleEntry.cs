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

using Gurux.DLMS.Objects.Enums;
using System;

namespace Gurux.DLMS.Objects
{

    /// <summary>
    /// Executed scripts.
    /// </summary>
    public class GXScheduleEntry
    {
        /// <summary>
        /// Schedule entry index.
        /// </summary>
        public UInt16 Index
        {
            get;
            set;
        }

        /// <summary>
        /// Is Schedule entry enabled.
        /// </summary>
        public bool Enable
        {
            get;
            set;
        }

        /// <summary>
        /// Executed Script.
        /// </summary>
        public GXDLMSScriptTable Script
        {
            get;
            set;
        }

        /// <summary>
        /// Script identifier of the script to be executed.
        /// </summary>
        public UInt16 ScriptSelector
        {
            get;
            set;
        }

        /// <summary>
        /// Switch time.
        /// </summary>
        public GXTime SwitchTime
        {
            get;
            set;
        }

        /// <summary>
        /// Defines a period in minutes, in which an entry shall be processed after power fail.
        /// </summary>
        public UInt16 ValidityWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Days of the week on which the entry is valid.
        /// </summary>
        public Weekdays ExecWeekdays
        {
            get;
            set;
        }

        /// <summary>
        /// Perform the link to the IC "Special days table", day_id.
        /// </summary>
        ///<seealso cref="GXDLMSSpecialDaysTable"/>
        public String ExecSpecDays
        {
            get;
            set;
        }

        /// <summary>
        /// Date starting period in which the entry is valid.
        /// </summary>
        public GXDate BeginDate
        {
            get;
            set;
        }

        /// <summary>
        /// Date starting period in which the entry is valid.
        /// </summary>
        public GXDate EndDate
        {
            get;
            set;
        }
    }

}
