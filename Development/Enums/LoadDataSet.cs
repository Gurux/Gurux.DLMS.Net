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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Gurux.DLMS.Enums
{
    /// <summary>
    /// LoadDataSet describes load dataset errors.
    /// </summary>
    public enum LoadDataSet
    {
        /// <summary>
        /// Other error.
        /// </summary>
        [XmlEnum("0")]
        Other = 0,
        /// <summary>
        /// Primitive out of sequence.
        /// </summary>
        [XmlEnum("1")]
        PrimitiveOutOfSequence,
        /// <summary>
        /// Not loadable.
        /// </summary>
        [XmlEnum("2")]
        NotLoadable,
        /// <summary>
        /// Dataset size is too large.
        /// </summary>
        [XmlEnum("3")]
        DatasetSizeTooLarge,
        /// <summary>
        /// Not awaited segment.
        /// </summary>
        [XmlEnum("4")]
        NotAwaitedSegment,
        /// <summary>
        /// Interpretation failure.
        /// </summary>
        [XmlEnum("5")]
        InterpretationFailure,
        /// <summary>
        /// Storage failure.
        /// </summary>
        [XmlEnum("6")]
        StorageFailure,
        /// <summary>
        /// Dataset not ready.
        /// </summary>
        [XmlEnum("7")]
        DatasetNotReady
    }
}