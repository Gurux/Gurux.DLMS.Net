//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
// Filename:    $HeadURL$
//
// Version:     $Revision$,
//      $Date$
//      $Author$
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

using Gurux.DLMS.Internal;
using System.Collections.Generic;
using System.Text;

namespace Gurux.DLMS.ASN
{
    internal sealed class GXAsn1Settings
    {
        private int count;

        /// <summary>
        /// Are comments used.
        /// </summary>
        public bool Comments
        {
            get;
            set;
        }

        private readonly StringBuilder sb = new StringBuilder();
        private Dictionary<short, string> tags = new Dictionary<short, string>();
        private Dictionary<string, short> tagbyName = new Dictionary<string, short>();

        private void AddTag(short key, string value)
        {
            tags.Add(key, value);
            tagbyName.Add(value.ToLower(), key);
        }

        /// <summary>Constructor.
        /// </summary>
        internal GXAsn1Settings()
        {
            AddTag((short)BerType.Application, "Application");
            AddTag((short)(BerType.Constructed | BerType.Context), "Context");
            AddTag((short)(((short)BerType.Constructed | (short)BerType.Context | 1)), "Constructed-Context-Specific-1");
            AddTag((short)(((short)BerType.Constructed | (short)BerType.Context | 2)), "Constructed-Context-Specific-2");
            AddTag((short)(((short)BerType.Constructed | (short)BerType.Context | 3)), "Constructed-Context-Specific-3");
            AddTag((short)(BerType.Constructed | BerType.Sequence), "Sequence");
            AddTag((short)(BerType.Constructed | BerType.Set), "Set");
            AddTag((short)BerType.ObjectIdentifier, "ObjectIdentifier");
            AddTag((short)BerType.PrintableString, "String");
            AddTag((short)BerType.Utf8StringTag, "UTF8");
            AddTag((short)BerType.Ia5String, "IA5");
            AddTag((short)BerType.Integer, "Integer");
            AddTag((short)BerType.Null, "Null");
            AddTag((short)BerType.BitString, "BitString");
            AddTag((short)BerType.UtcTime, "UtcTime");
            AddTag((short)BerType.GeneralizedTime, "GeneralizedTime");
            AddTag((short)BerType.OctetString, "OctetString");
            AddTag((short)BerType.Boolean, "Bool");
            AddTag(-1, "Byte");
            AddTag(-2, "Short");
            AddTag(-4, "Int");
            AddTag(-8, "Long");
        }

        public string GetTag(short value)
        {
            return tags[value];
        }

        public short GetTag(string value)
        {
            return tagbyName[value];
        }

        public int XmlLength
        {
            get
            {
                return sb.Length;
            }
        }

        /// <summary>
        /// Add comment.
        /// </summary>
        /// <param name="offset">Offset. </param>
        /// <param name="value">Comment value. </param>
        public void AppendComment(int offset, string value)
        {
            if (Comments)
            {
                bool empty = sb.Length == 0;
                StringBuilder tmp;
                if (empty)
                {
                    tmp = sb;
                }
                else
                {
                    tmp = new StringBuilder();
                }
                for (int pos = 0; pos < count - 1; ++pos)
                {
                    tmp.Append(' ');
                }
                tmp.Append("<!--");
                tmp.Append(value);
                tmp.Append("-->\r\n");
                if (!empty)
                {
                    sb.Insert(offset, tmp);
                }
            }
        }

        /// <summary>
        /// Append spaces to the buffer.
        /// </summary>
        public void AppendSpaces()
        {
            for (int pos = 0; pos != count; ++pos)
            {
                sb.Append(' ');
            }
        }

        public void Append(string value)
        {
            sb.Append(value);
        }

        public void Increase()
        {
            ++count;
            Append("\r\n");
        }

        public void Decrease()
        {
            --count;
            AppendSpaces();
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}