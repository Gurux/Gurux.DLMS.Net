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
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

// Gurux.Common class implements interfaces that are needed for 
// Gurux Media components and Gurux Device Framework.
namespace Gurux.Common
{    
    [CollectionDataContract()]
    internal class GXAddInList : List<GXAddIn> 
    { 
        public GXAddIn FindByName(string name)
        {
            name = name.Trim();
            foreach (GXAddIn it in this)
            {
                if (string.Compare(it.Name.Trim(), name, true) == 0)
                {
                    return it;
                }
            }
            return null;
        }
    }

    [CollectionDataContract(ItemName = "Name")]
    internal class DependenciesList : List<string> { }

    [CollectionDataContract(ItemName = "Name")]
    internal class ResourcesList : List<string> { }

    [Flags]
    internal enum AddInStates
    {
        /// <summary>
        /// New AddIn is available.
        /// </summary>
        Available = 0x0,
        /// <summary>
        /// New AddIn is marked to download.
        /// </summary>
        Download = 0x1,
        /// <summary>
        /// AddIn is installed.
        /// </summary>
        Installed = 0x2,
        /// <summary>
        /// AddIn is disabled.
        /// </summary>
        Disabled = 0x4,
        /// <summary>
        /// Version is update to installed one.
        /// </summary>
        Update = 0x8
    }

    enum GXAddInType
    {
        None = 0,        
        AddIn,
        Application
    };

    [DataContract()]
    internal class GXAddIn
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal GXAddIn()
        {
            Type = GXAddInType.None;
            State = AddInStates.Available;
        }

        [DataMember]
        public string Name
        {
            get;
            internal set;
        }

        [DataMember]
        public string File
        {
            get;
            internal set;
        }

        public int ProgressPercentage
        {
            get;
            set;
        }

        [DataMember(IsRequired=false, EmitDefaultValue=true)]
        public GXAddInType Type
        {
            get;
            internal set;
        }
        
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public AddInStates State
        {
            get;
            set;
        }
        

        /// <summary>
        /// Installed version number.
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string InstalledVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Update version number.
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Version
        {
            get;
            set;
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public DependenciesList Dependencies
        {
            get;
            internal set;
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ResourcesList Resources
        {
            get;
            internal set;
        }
    }    
}
