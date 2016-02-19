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


namespace Gurux.DLMS.Internal
{
    /// <summary>
    /// BER encoding enumeration values.
    /// </summary>
    enum BerType
    {
        /// <summary>
        /// End of Content.
        /// </summary>
        EOC = 0x00,
        /// <summary>
        /// Boolean.
        /// </summary>
        Boolean = 0x1,
        /// <summary>
        /// Integer.
        /// </summary>
        Integer = 0x2,
        /// <summary>
        /// Bit String.
        /// </summary>
        BitString = 0x3,
        /// <summary>
        /// Octet string.
        /// </summary>
        OctetString = 0x4,
        /// <summary>
        /// Null value.
        /// </summary>
        Null = 0x5,
        /// <summary>
        /// Object identifier.
        /// </summary>
        ObjectIdentifier = 0x6,
        /// <summary>
        /// Object Descriptor.
        /// </summary>
        ObjectDescriptor = 7,
        /// <summary>
        /// External
        /// </summary>
        External = 8,
        /// <summary>
        /// Real (float).
        /// </summary>
        Real = 9,
        /// <summary>
        /// Enumerated.
        /// </summary>
        Enumerated = 10,
        /// <summary>
        /// Utf8 String.
        /// </summary>
        Utf8StringTag = 12,
        /// <summary>
        /// Numeric string.
        /// </summary>
        NumericString = 18,
        /// <summary>
        /// Printable string.
        /// </summary>
        PrintableString = 19,
        /// <summary>
        /// Teletex string.
        /// </summary>
        TeletexString = 20,
        /// <summary>
        /// Videotex string.
        /// </summary>
        VideotexString = 21,        
        /// <summary>
        /// Ia5 string
        /// </summary>
        Ia5String = 22,
        /// <summary>
        /// Utc time.
        /// </summary>
        UtcTime = 23,
        /// <summary>
        /// Generalized time.
        /// </summary>
        GeneralizedTime = 24,
        /// <summary>
        /// Graphic string.
        /// </summary>
        GraphicString = 25,
        /// <summary>
        /// Visible string.
        /// </summary>
        VisibleString = 26,
        /// <summary>
        /// General string.
        /// </summary>
        GeneralString = 27,
        /// <summary>
        /// Universal string.
        /// </summary>
        UniversalString = 28,
        /// <summary>
        /// Bmp string.
        /// </summary>
        BmpString = 30,
        /// <summary>
        /// Application class.
        /// </summary>
        Application = 0x40,
        /// <summary>
        /// Context class.
        /// </summary>
        Context = 0x80,
        /// <summary>
        /// Private class.
        /// </summary>
        Private = 0xc0,
        /// <summary>
        /// Constructed.
        /// </summary>
        Constructed = 0x20         
    }
}
