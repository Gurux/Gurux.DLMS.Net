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

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Enumerates compact data restriction types.
    /// </summary>
    public enum CompactDataRestrictionType
    {
        /// <summary>
        /// None.
        /// </summary>
        None,
        /// <summary>
        /// Restriction by date.
        /// </summary>
        Date,
        /// <summary>
        /// Restriction by entry.
        /// </summary>
        Entry
    }

    public class GXDLMSCompactDataRestriction
    {
        /// <summary>
        /// Compact data restriction types.
        /// </summary>
        public CompactDataRestrictionType Type
        {
            get;
            set;
        }

        /// <summary>
        /// From date or entry.
        /// </summary>
        public object From
        {
            get;
            set;
        }

        /// <summary>
        /// To date or entry.
        /// </summary>
        public object To
        {
            get;
            set;
        }
    }

    public class GXDLMSCaptureObject
    {
        /// <summary>
        /// Attribute Index of DLMS object in the profile generic table.
        /// </summary>
        public int AttributeIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Data index of DLMS object in the profile generic table. 
        /// </summary>
        public int DataIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Restriction element for compact data.
        /// </summary>
        public GXDLMSCompactDataRestriction Restriction
        {
            get;
            set;
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public GXDLMSCaptureObject()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="attributeIndex">Attribute index.</param>
        /// <param name="dataIndex">Data Index.</param>
        public GXDLMSCaptureObject(int attributeIndex, int dataIndex)
        {
            AttributeIndex = attributeIndex;
            DataIndex = dataIndex;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="attributeIndex">Attribute index.</param>
        /// <param name="dataIndex">Data Index.</param>
        public GXDLMSCaptureObject(int attributeIndex, int dataIndex, GXDLMSCompactDataRestriction restriction)
        {
            AttributeIndex = attributeIndex;
            DataIndex = dataIndex;
            Restriction = restriction;
        }

    }

}
