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

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Activity Calendar's Day profile is defined on the standard.
    /// </summary>
    public class GXDLMSDayProfile
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSDayProfile()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSDayProfile(int dayId, GXDLMSDayProfileAction[] schedules)
        {
            DayId = dayId;
            DaySchedules = schedules;
        }

        /// <summary>
        /// User defined identifier, identifying the currentday_profile.
        /// </summary>
        public int DayId
        {
            get;
            set;
        }

        public GXDLMSDayProfileAction[] DaySchedules
        {
            get;
            set;
        }

        public override string ToString()
        {
            string str = DayId.ToString();
            foreach (GXDLMSDayProfileAction it in DaySchedules)
            {
                str += " " + it.ToString();
            }
            return str;
        }
    }

    /// <summary>
    /// Activity Calendar's Day Profile Action is defined on the standard.
    /// </summary>
    public class GXDLMSDayProfileAction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSDayProfileAction()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSDayProfileAction(GXTime startTime, string scriptLogicalName, UInt16 scriptSelector)
        {
            StartTime = startTime;
            ScriptLogicalName = scriptLogicalName;
            ScriptSelector = scriptSelector;
        }

        /// <summary>
        /// Defines the time when the script is to be executed.
        /// </summary>
        public GXTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the logical name of the “Script table” object;
        /// </summary>
        public string ScriptLogicalName
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the script_identifier of the script to be executed.
        /// </summary>
        public UInt16 ScriptSelector
        {
            get;
            set;
        }

        public override string ToString()
        {
            return StartTime.ToString() + " " + ScriptLogicalName + " " + ScriptSelector;
        }
    }
}
