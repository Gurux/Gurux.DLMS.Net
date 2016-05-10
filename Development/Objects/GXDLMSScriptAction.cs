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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSScriptAction
    {

        /// <summary>
        /// Defines which action to be applied to the referenced object.
        /// </summary>
        public ScriptActionType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Executed object type.
        /// </summary>        
        public ObjectType ObjectType
        {
            get;
            set;
        }

        /// <summary>
        /// Logical name of executed object.
        /// </summary>
        public string LogicalName
        {
            get;
            set;
        }
        /// <summary>
        /// Defines which attribute of the selected object is affected; 
        /// or which specific method is to be executed.
        /// </summary>        
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Parameter is service spesific.
        /// </summary>        
        public Object Parameter
        {
            get;
            set;
        }
        /// <summary>
        /// Parameter data type can be used to tell spesific data type.
        /// </summary>        
        public DataType ParameterDataType
        {
            get;
            set;
        }

        public override string ToString()
        {
            string tmp;
            if (Parameter is byte[])
            {
                tmp = BitConverter.ToString((byte[])Parameter).Replace("-", " ");
            }
            else
            {
                tmp = Convert.ToString(Parameter);
            }
            return Type.ToString() + " " + ObjectType.ToString()  + " " + LogicalName  + " " + 
                Index.ToString()  + " " + tmp;
        }
    }
}
