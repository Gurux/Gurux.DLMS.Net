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

namespace Gurux.DLMS.Objects
{
    public class GXDLMSTarget
    {
        /// <summary>
        /// Target COSEM object.
        /// </summary>
        public GXDLMSObject Target
        {
            get;
            set;
        }

        /// <summary>
        /// Attribute Index of COSEM object.
        /// </summary>
        public byte AttributeIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Data index of COSEM object. 
        /// </summary>
        /// <remarks>
        /// All targets don't use this.
        /// </remarks>
        public int DataIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Data value of COSEM object. 
        /// </summary>
        /// <remarks>
        /// All targets don't use this.
        /// </remarks>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public GXDLMSTarget()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <param name="attributeIndex">Attribute index.</param>
        public GXDLMSTarget(GXDLMSObject target, byte attributeIndex)
        {
            Target = target;
            AttributeIndex = attributeIndex;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <param name="attributeIndex">Attribute index.</param>
        /// <param name="dataIndex">Data Index.</param>
        public GXDLMSTarget(GXDLMSObject target, byte attributeIndex, int dataIndex)
        {
            Target = target;
            AttributeIndex = attributeIndex;
            DataIndex = dataIndex;
        }
    }

}
