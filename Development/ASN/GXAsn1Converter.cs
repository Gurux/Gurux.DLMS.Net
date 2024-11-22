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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Gurux.DLMS.Internal;
using Gurux.DLMS.ASN.Enums;
using System.Xml;
using System.Globalization;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Ecdsa.Enums;
using System.IO;

namespace Gurux.DLMS.ASN
{
    /// <summary>
    /// ASN1 converter. This class is used to convert public and private keys to byte array and vice verse.
    /// </summary>
    public sealed class GXAsn1Converter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        private GXAsn1Converter()
        {

        }

        /// <summary>
        /// Returns default file path.
        /// </summary>
        /// <param name="scheme">Used scheme.</param>
        /// <param name="certificateType">Certificate type.</param>
        /// <param name="systemTitle"> System title.</param>
        /// <returns>File path.</returns>
        [ObsoleteAttribute]
        public static string GetFilePath(Ecc scheme, CertificateType certificateType, byte[] systemTitle)
        {
            string path;
            switch (certificateType)
            {
                case CertificateType.DigitalSignature:
                    path = "D";
                    break;
                case CertificateType.KeyAgreement:
                    path = "A";
                    break;
                case CertificateType.TLS:
                    path = "T";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown certificate type.");
            }
            path += GXDLMSTranslator.ToHex(systemTitle, false) + ".pem";
            if (scheme == Ecc.P256)
            {
                path = Path.Combine("Keys", path);
            }
            else
            {
                path = Path.Combine("Keys384", path);
            }
            return path;
        }

        public static List<KeyValuePair<object, object>> EncodeSubject(string value)
        {
            X509Name name;
            object val;
            List<KeyValuePair<object, object>> list = new List<KeyValuePair<object, object>>();
            foreach (string tmp in value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] it = tmp.Split(new char[] { '=' });
                if (it.Length != 2)
                {
                    throw new ArgumentException("Invalid subject.");
                }
                name = (X509Name)Enum.Parse(typeof(X509Name), it[0].Trim());
                switch (name)
                {
                    case X509Name.C:
                        // Country code is printable string
                        val = it[1].Trim();
                        break;
                    case X509Name.E:
                        // email address in Verisign certificates
                        val = new GXAsn1Ia5String(it[1].Trim());
                        break;
                    default:
                        val = new GXAsn1Utf8String(it[1].Trim());
                        break;
                }
                string oid = X509NameConverter.GetString(name);
                list.Add(new KeyValuePair<object, object>(new GXAsn1ObjectIdentifier(oid), val));
            }
            return list;
        }

        internal static string GetSubject(GXAsn1Sequence values)
        {
            object value;
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<object, object> it in values)
            {
                sb.Append(X509NameConverter.FromString(it.Key.ToString()));
                sb.Append('=');
                value = it.Value;
                sb.Append(value);
                sb.Append(", ");
            }
            // Remove last comma.
            if (sb.Length != 0)
            {
                sb.Length = sb.Length - 2;
            }
            return sb.ToString();
        }

        private static void GetValue(GXByteBuffer bb, IList<object> objects, GXAsn1Settings s, bool getNext)
        {
            int len;
            short type;
            IList<object> tmp;
            byte[] tmp2;
            type = bb.GetUInt8();
            len = GXCommon.GetObjectCount(bb);
            if (len > bb.Available)
            {
                throw new OutOfMemoryException("GXAsn1Converter.GetValue");
            }
            int connectPos = 0;
            if (s != null)
            {
                connectPos = s.XmlLength;
            }
            int start = bb.Position;
            string tagString = null;
            if (s != null)
            {
                s.AppendSpaces();
                if (type == (byte)BerType.Integer)
                {
                    if (len == 1 || len == 2 || len == 4 || len == 8)
                    {
                        tagString = s.GetTag((short)-len);
                    }
                    else
                    {
                        tagString = s.GetTag((byte)BerType.Integer);
                    }
                }
                else
                {
                    tagString = s.GetTag(type);
                }
                s.Append("<" + tagString + ">");
            }

            switch (type)
            {
                case (byte)(BerType.Constructed | BerType.Context):
                case ((byte)(BerType.Constructed | BerType.Context) | 1):
                case ((byte)(BerType.Constructed | BerType.Context) | 2):
                case ((byte)(BerType.Constructed | BerType.Context) | 3):
                case ((byte)(BerType.Constructed | BerType.Context) | 4):
                case ((byte)(BerType.Constructed | BerType.Context) | 5):
                    if (s != null)
                    {
                        s.Increase();
                    }
                    tmp = new GXAsn1Context() { Index = type & 0xF };
                    objects.Add(tmp);
                    while (bb.Position < start + len)
                    {
                        GetValue(bb, tmp, s, false);
                    }
                    if (s != null)
                    {
                        s.Decrease();
                    }
                    break;
                case (byte)(BerType.Constructed | BerType.Sequence):
                    if (s != null)
                    {
                        s.Increase();
                    }
                    tmp = new GXAsn1Sequence();
                    objects.Add(tmp);
                    int cnt = 0;
                    while (bb.Position < start + len)
                    {
                        ++cnt;
                        GetValue(bb, tmp, s, false);
                        if (getNext)
                        {
                            break;
                        }
                    }
                    if (s != null)
                    {
                        // Append comment.
                        s.AppendComment(connectPos, Convert.ToString(cnt) + " elements.");
                        s.Decrease();
                    }
                    break;
                case (byte)(BerType.Constructed | BerType.Set):
                    if (s != null)
                    {
                        s.Increase();
                    }
                    tmp = new List<object>();
                    GetValue(bb, tmp, s, false);
                    if (tmp[0] is GXAsn1Sequence)
                    {
                        tmp = (GXAsn1Sequence)tmp[0];
                        objects.Add(new KeyValuePair<object, object>(tmp[0], tmp.Count == 1 ? null : tmp[1]));
                    }
                    else
                    {
                        KeyValuePair<object, object> e = new KeyValuePair<object, object>(tmp, null);
                        objects.Add(e);
                    }
                    if (s != null)
                    {
                        s.Decrease();
                    }
                    break;
                case (byte)BerType.ObjectIdentifier:
                    GXAsn1ObjectIdentifier oi = new GXAsn1ObjectIdentifier(bb, len);
                    objects.Add(oi);
                    if (s != null)
                    {
                        string str = oi.Description;
                        if (str != null)
                        {
                            s.AppendComment(connectPos, str);
                        }
                        s.Append(oi.ToString());
                    }

                    break;
                case (byte)BerType.PrintableString:
                    {
                        string str = bb.GetString(len);
                        objects.Add(str);
                        if (s != null)
                        {
                            s.Append(str);
                        }
                    }
                    break;
                case (byte)BerType.BmpString:
                    {
                        string str = bb.GetStringUnicode(len);
                        objects.Add(str);
                        if (s != null)
                        {
                            s.Append(str);
                        }
                    }
                    break;
                case (byte)BerType.Utf8StringTag:
                    objects.Add(new GXAsn1Utf8String(bb.GetString(bb.Position, len)));
                    bb.Position = bb.Position + len;
                    if (s != null)
                    {
                        s.Append(Convert.ToString(objects[objects.Count - 1]));
                    }

                    break;
                case (byte)BerType.Ia5String:
                    objects.Add(new GXAsn1Ia5String(bb.GetString(len)));
                    if (s != null)
                    {
                        s.Append(Convert.ToString(objects[objects.Count - 1]));
                    }
                    break;
                case (byte)BerType.Integer:
                    if (len == 1)
                    {
                        objects.Add(bb.GetInt8());
                    }
                    else if (len == 2)
                    {
                        objects.Add(bb.GetInt16());
                    }
                    else if (len == 4)
                    {
                        objects.Add(bb.GetInt32());
                    }
                    else
                    {
                        tmp2 = new byte[len];
                        bb.Get(tmp2);
                        objects.Add(new GXAsn1Integer(tmp2));
                    }
                    if (s != null)
                    {
                        s.Append(Convert.ToString(objects[objects.Count - 1]));
                    }
                    break;
                case (byte)BerType.Null:
                    objects.Add(null);
                    break;
                case (byte)BerType.BitString:
                    GXBitString tmp3 = new GXBitString(bb.SubArray(bb.Position, len));
                    objects.Add(tmp3);
                    bb.Position = bb.Position + len;
                    if (s != null)
                    {
                        // Append comment.
                        s.AppendComment(connectPos, Convert.ToString(tmp3.Length) + " bit.");
                        s.Append(tmp3.ToString());
                    }
                    break;
                case (byte)BerType.UtcTime:
                    tmp2 = new byte[len];
                    bb.Get(tmp2);
                    objects.Add(GetUtcTime(ASCIIEncoding.ASCII.GetString(tmp2)));
                    if (s != null)
                    {
                        s.Append(((DateTimeOffset)objects[objects.Count - 1]).UtcDateTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture));
                    }
                    break;
                case (byte)BerType.GeneralizedTime:
                    tmp2 = new byte[len];
                    bb.Get(tmp2);
                    objects.Add(GXCommon.GetGeneralizedTime(ASCIIEncoding.ASCII.GetString(tmp2)));
                    if (s != null)
                    {
                        s.Append(Convert.ToString(objects[objects.Count - 1]));
                    }
                    break;
                case (byte)BerType.Context:
                case (byte)BerType.Context | 1:
                case (byte)BerType.Context | 2:
                case (byte)BerType.Context | 3:
                case (byte)BerType.Context | 4:
                case (byte)BerType.Context | 5:
                case (byte)BerType.Context | 6:
                    tmp = new GXAsn1Context() { Constructed = false, Index = type & 0xF };
                    tmp2 = new byte[len];
                    bb.Get(tmp2);
                    tmp.Add(tmp2);
                    objects.Add(tmp);
                    if (s != null)
                    {
                        s.Append(GXCommon.ToHex(tmp2, false));
                    }
                    break;
                case (byte)BerType.OctetString:
                    int t = bb.GetUInt8(bb.Position);
                    switch (t)
                    {
                        case (byte)(BerType.Constructed | BerType.Sequence):
                        case (byte)BerType.BitString:
                            if (s != null)
                            {
                                s.Increase();
                            }
                            GetValue(bb, objects, s, false);
                            if (s != null)
                            {
                                s.Decrease();
                            }
                            break;
                        default:
                            tmp2 = new byte[len];
                            bb.Get(tmp2);
                            objects.Add(tmp2);
                            if (s != null)
                            {
                                s.Append(GXCommon.ToHex(tmp2, false));
                            }
                            break;
                    }
                    break;
                case (byte)BerType.Boolean:
                    bool b = bb.GetUInt8() != 0;
                    objects.Add(b);
                    if (s != null)
                    {
                        s.Append(Convert.ToString(b));
                    }
                    break;
                default:
                    throw new System.ArgumentException("Invalid type: " + type);
            }
            if (s != null)
            {
                s.Append("</" + tagString + ">\r\n");
            }
        }

        private static DateTime GetUtcTime(string dateString)
        {
            int year, month, day, hour, minute, second = 0;
            year = 2000 + Convert.ToInt32(dateString.Substring(0, 2));
            month = Convert.ToInt32(dateString.Substring(2, 2));
            day = Convert.ToInt32(dateString.Substring(4, 2));
            hour = Convert.ToInt32(dateString.Substring(6, 2));
            minute = Convert.ToInt32(dateString.Substring(8, 2));
            // If UTC time.
            if (dateString.EndsWith("Z"))
            {
                if (dateString.Length > 11)
                {
                    second = Convert.ToInt32(dateString.Substring(10, 2));
                }
                return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
            }
            if (dateString.Length > 15)
            {
                second = Convert.ToInt32(dateString.Substring(10, 2));
            }
            string tmp = dateString.Substring(dateString.Length - 6, dateString.Length - 1 - (dateString.Length - 6));
            return new DateTimeOffset(new DateTime(year, month, day, hour, minute, second), new TimeSpan(0, 0, 0)).UtcDateTime;
        }

        private static string DateToString(DateTime date)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((date.Year - 2000).ToString("00"));
            sb.Append(date.Month.ToString("00"));
            sb.Append(date.Day.ToString("00"));
            sb.Append(date.Hour.ToString("00"));
            sb.Append(date.Minute.ToString("00"));
            sb.Append(date.Second.ToString("00"));
            sb.Append("Z");
            return sb.ToString();
        }

        /// <summary>
        /// Convert byte array to ASN1 objects.
        /// </summary>
        /// <param name="data">ASN-1 bytes.</param>
        /// <returns> Parsed objects. </returns>
        public static object FromByteArray(byte[] data)
        {
            GXByteBuffer bb = new GXByteBuffer(data);
            IList<object> objects = new List<object>();
            while (bb.Position != bb.Size)
            {
                GetValue(bb, objects, null, false);
            }
            if (objects.Count == 0)
            {
                return null;
            }
            return objects[0];
        }

        /// <summary>
        /// Get next ASN1 value from the byte buffer.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object GetNext(GXByteBuffer data)
        {
            IList<object> objects = new List<object>();
            GetValue(data, objects, null, true);
            return objects[0];
        }


        /// <summary>
        /// Add ASN1 object to byte buffer.
        /// </summary>
        /// <param name="bb">Byte buffer where ANS1 object is serialized. </param>
        /// <param name="target">ANS1 object </param>
        /// <returns>Size of object. </returns>
        private static int GetBytes(GXByteBuffer bb, object target)
        {
            GXByteBuffer tmp;
            string str;
            int start = bb.Size;
            int cnt = 0;
            if (target is GXAsn1Context a)
            {
                tmp = new GXByteBuffer();
                foreach (object it in a)
                {
                    cnt += GetBytes(tmp, it);
                }
                start = bb.Size;
                if (a.Constructed)
                {
                    bb.SetUInt8((byte)((int)BerType.Constructed | (int)BerType.Context | a.Index));
                    GXCommon.SetObjectCount(cnt, bb);
                }
                else
                {
                    tmp.SetUInt8(0, (byte)((int)BerType.Context | a.Index));
                }
                cnt += bb.Size - start;
                bb.Set(tmp);
                return cnt;
            }
            else if (target is object[])
            {
                tmp = new GXByteBuffer();
                foreach (object it in (object[])target)
                {
                    cnt += GetBytes(tmp, it);
                }
                start = bb.Size;
                bb.SetUInt8(BerType.Constructed | BerType.Sequence);
                GXCommon.SetObjectCount(cnt, bb);
                cnt += bb.Size - start;
                bb.Set(tmp);
                return cnt;
            }
            else if (target is string)
            {
                bb.SetUInt8(BerType.PrintableString);
                GXCommon.SetObjectCount(((string)target).Length, bb);
                bb.Add(target);
            }
            else if (target is sbyte)
            {
                bb.SetUInt8((byte)BerType.Integer);
                GXCommon.SetObjectCount(1, bb);
                bb.Add(target);
            }
            else if (target is short)
            {
                bb.SetUInt8((byte)BerType.Integer);
                GXCommon.SetObjectCount(2, bb);
                bb.Add(target);
            }
            else if (target is int)
            {
                bb.SetUInt8((byte)BerType.Integer);
                GXCommon.SetObjectCount(4, bb);
                bb.Add(target);
            }
            else if (target is GXAsn1Integer)
            {
                bb.SetUInt8((byte)BerType.Integer);
                byte[] b = ((GXAsn1Integer)target).Value;
                GXCommon.SetObjectCount(b.Length, bb);
                bb.Add(b);
            }
            else if (target is long)
            {
                bb.SetUInt8((byte)BerType.Integer);
                GXCommon.SetObjectCount(8, bb);
                bb.Add(target);
            }
            else if (target is byte[])
            {
                bb.SetUInt8(BerType.OctetString);
                GXCommon.SetObjectCount(((byte[])target).Length, bb);
                bb.Add(target);
            }
            else if (target == null)
            {
                bb.SetUInt8(BerType.Null);
                GXCommon.SetObjectCount(0, bb);
            }
            else if (target is bool)
            {
                bb.SetUInt8(BerType.Boolean);
                bb.SetUInt8(1);
                if ((bool)target)
                {
                    bb.SetUInt8(255);
                }
                else
                {
                    bb.SetUInt8(0);
                }
            }
            else if (target is GXAsn1ObjectIdentifier)
            {
                bb.SetUInt8(BerType.ObjectIdentifier);
                byte[] t = ((GXAsn1ObjectIdentifier)target).Encoded;
                GXCommon.SetObjectCount(t.Length, bb);
                bb.Add(t);
            }
            else if (target is KeyValuePair<object, object>)
            {
                KeyValuePair<object, object> e = (KeyValuePair<object, object>)target;
                GXByteBuffer tmp2 = new GXByteBuffer();
                if (e.Value != null)
                {
                    tmp = new GXByteBuffer();
                    cnt += GetBytes(tmp2, e.Key);
                    cnt += GetBytes(tmp2, e.Value);
                    tmp.SetUInt8(BerType.Constructed | BerType.Sequence);
                    GXCommon.SetObjectCount(cnt, tmp);
                    tmp.Set(tmp2);
                }
                else
                {
                    GetBytes(tmp2, (e.Key as List<object>)[0]);
                    tmp = tmp2;
                }
                // Update len.
                cnt = bb.Size;
                bb.SetUInt8(BerType.Constructed | BerType.Set);
                GXCommon.SetObjectCount(tmp.Size, bb);
                bb.Set(tmp);
                return bb.Size - cnt;
            }
            else if (target is GXAsn1Utf8String)
            {
                bb.SetUInt8(BerType.Utf8StringTag);
                str = target.ToString();
                GXCommon.SetObjectCount(str.Length, bb);
                bb.Add(str);
            }
            else if (target is GXAsn1Ia5String)
            {
                bb.SetUInt8(BerType.Ia5String);
                str = target.ToString();
                GXCommon.SetObjectCount(str.Length, bb);
                bb.Add(str);
            }
            else if (target is GXBitString)
            {
                GXBitString bs = (GXBitString)target;
                bb.SetUInt8(BerType.BitString);
                GXCommon.SetObjectCount(1 + bs.Value.Length, bb);
                bb.SetUInt8((byte)bs.PadBits);
                bb.Add(bs.Value);
            }
            else if (target is GXAsn1PublicKey)
            {
                GXAsn1PublicKey bs = (GXAsn1PublicKey)target;
                bb.SetUInt8(BerType.BitString);
                // Size is 64 bytes + padding and uncompressed point indicator.
                GXCommon.SetObjectCount(66, bb);
                // Add padding.
                bb.SetUInt8(0);
                // prefixed with the uncompressed point indicator 04
                bb.SetUInt8(4);
                bb.Add(bs.Value);
                // Count is type + size + 64 bytes + padding + uncompressed point
                // indicator.
                return 68;
            }
            else if (target is DateTime)
            {
                // Save date time in UTC.
                bb.SetUInt8(BerType.UtcTime);
                str = DateToString((DateTime)target);
                bb.SetUInt8((byte)str.Length);
                bb.Add(str);
            }
            else if (target is GXAsn1Sequence || target is IList)
            {
                tmp = new GXByteBuffer();
                foreach (object it in (IList)target)
                {
                    cnt += GetBytes(tmp, it);
                }
                start = bb.Size;
                if (target is GXAsn1Context c)
                {
                    if (c.Constructed)
                    {
                        bb.SetUInt8((byte)((byte)BerType.Constructed | (byte)BerType.Sequence | c.Index));
                    }
                    else
                    {
                        bb.SetUInt8((byte)((byte)BerType.Sequence | c.Index));
                    }
                }
                else
                {
                    bb.SetUInt8(BerType.Constructed | BerType.Sequence);
                }
                GXCommon.SetObjectCount(cnt, bb);
                cnt += bb.Size - start;
                bb.Set(tmp);
                return cnt;
            }
            else
            {
                throw new ArgumentException("Invalid type: " + target.GetType().ToString());
            }
            return bb.Size - start;
        }

        /// <summary>
        /// Convert ASN1 objects to byte array.
        /// </summary>
        /// <param name="objects">ASN.1 objects. </param>
        /// <returns> ASN.1 objects as byte array. </returns>
        public static byte[] ToByteArray(object objects)
        {
            GXByteBuffer bb = new GXByteBuffer();
            GetBytes(bb, objects);
            return bb.Array();
        }

        /// <summary>
        /// Convert ASN1 PDU bytes to XML.
        /// </summary>
        /// <param name="value">Bytes to convert. </param>
        /// <returns> Converted XML. </returns>
        public static string PduToXml(string value)
        {
            if (value == null || value.Length == 0)
            {
                return "";
            }
            if (!GXCommon.IsHexString(value))
            {
                return PduToXml(GXCommon.FromBase64(value));
            }
            return PduToXml(new GXByteBuffer(GXCommon.HexToBytes(value)));
        }

        /// <summary>
        /// Convert ASN1 PDU bytes to XML.
        /// </summary>
        /// <param name="value">Bytes to convert. </param>
        /// <returns>Converted XML. </returns>
        public static string PduToXml(byte[] value)
        {
            return PduToXml(new GXByteBuffer(value), false);
        }

        /// <summary>
        /// Convert ASN1 PDU bytes to XML.
        /// </summary>
        /// <param name="value">Bytes to convert. </param>
        /// <param name="comments">Are comments added to generated XML. </param>
        /// <returns>Converted XML. </returns>
        public static string PduToXml(byte[] value, bool comments)
        {
            return PduToXml(new GXByteBuffer(value), comments);
        }

        /// <summary>
        /// Convert ASN.1 PDU bytes to XML.
        /// </summary>
        /// <param name="value">Bytes to convert. </param>
        /// <returns> Converted XML. </returns>
        public static string PduToXml(GXByteBuffer value)
        {
            return PduToXml(value, false);
        }

        /// <summary>
        /// Convert ASN.1 PDU bytes to XML.
        /// </summary>
        /// <param name="value">Bytes to convert. </param>
        /// <param name="comments">Are comments added to generated XML. </param>
        /// <returns>Converted XML. </returns>
        public static string PduToXml(GXByteBuffer value, bool comments)
        {
            GXAsn1Settings s = new GXAsn1Settings();
            s.Comments = comments;
            IList<object> objects = new List<object>();
            while (value.Position != value.Size)
            {
                GetValue(value, objects, s, false);
            }
            return s.ToString();
        }

        private static int ReadNode(XmlElement node, GXAsn1Settings s, IList<object> list)
        {
            IList<object> tmp;
            string str = node.Name.ToLower();
            int tag = s.GetTag(str);
            switch (tag)
            {
                case (byte)BerType.Application:
                    tmp = new List<object>();
                    foreach (XmlElement node2 in node.ChildNodes)
                    {
                        if (node2.NodeType == XmlNodeType.Element)
                        {
                            ReadNode(node2, s, tmp);
                        }
                    }
                    list.Add(tmp);
                    break;
                case (byte)(BerType.Constructed | BerType.Context):
                    tmp = new GXAsn1Context();
                    foreach (XmlElement node2 in node.ChildNodes)
                    {
                        if (node2.NodeType == XmlNodeType.Element)
                        {
                            ReadNode(node2, s, tmp);
                        }
                    }
                    list.Add(tmp);
                    break;
                case (byte)(BerType.Constructed | BerType.Sequence):
                    tmp = new GXAsn1Sequence();
                    foreach (XmlElement node2 in node.ChildNodes)
                    {
                        if (node2.NodeType == XmlNodeType.Element)
                        {
                            ReadNode(node2, s, tmp);
                        }
                    }
                    list.Add(tmp);
                    break;
                case (byte)(BerType.Constructed | BerType.Set):
                    tmp = new List<object>();
                    foreach (XmlElement node2 in node.ChildNodes)
                    {
                        if (node2.NodeType == XmlNodeType.Element)
                        {
                            ReadNode(node2, s, tmp);
                        }
                    }
                    foreach (object val in tmp)
                    {
                        KeyValuePair<object, object> e;
                        if (val is IList)
                        {
                            IList t = (IList)val;
                            e = new KeyValuePair<object, object>(t[0], t[1]);
                        }
                        else
                        {
                            e = new KeyValuePair<object, object>(tmp, null);
                        }
                        list.Add(e);
                    }
                    break;
                case (byte)BerType.ObjectIdentifier:
                    list.Add(new GXAsn1ObjectIdentifier(node.ChildNodes[0].Value));
                    break;
                case (byte)BerType.PrintableString:
                    list.Add(node.ChildNodes[0].Value);
                    break;
                case (byte)BerType.Utf8StringTag:
                    list.Add(new GXAsn1Utf8String(node.ChildNodes[0].Value));
                    break;
                case (byte)BerType.Ia5String:
                    list.Add(new GXAsn1Ia5String(node.ChildNodes[0].Value));
                    break;
                case (byte)BerType.Integer:
                    list.Add(new GXAsn1Integer(node.ChildNodes[0].Value));
                    break;
                case (byte)BerType.Null:
                    list.Add(null);
                    break;
                case (byte)BerType.BitString:
                    list.Add(new GXBitString(node.ChildNodes[0].Value));
                    break;
                case (byte)BerType.UtcTime:
                    DateTime d = DateTime.ParseExact(node.ChildNodes[0].Value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    list.Add(d);
                    break;
                case (byte)BerType.GeneralizedTime:
                    break;
                case (byte)BerType.OctetString:
                    break;
                case -1:
                    list.Add(Convert.ToSByte(node.ChildNodes[0].Value));
                    break;
                case -2:
                    list.Add(Convert.ToInt16(node.ChildNodes[0].Value));
                    break;
                case -4:
                    list.Add(Convert.ToInt32(node.ChildNodes[0].Value));
                    break;
                case -8:
                    list.Add(Convert.ToInt64(node.ChildNodes[0].Value));
                    break;
                default:
                    throw new System.ArgumentException("Invalid node: " + node.Name);
            }
            return 0;
        }

        /// <summary>
        /// Convert XML to ASN.1 PDU bytes.
        /// </summary>
        /// <param name="xml">XML. </param>
        /// <returns>ASN.1 PDU. </returns>
        public static byte[] XmlToPdu(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            IList<object> list = new List<object>();
            GXAsn1Settings s = new GXAsn1Settings();
            ReadNode(doc.DocumentElement, s, list);
            return ToByteArray(list[0]);
        }

        /// <summary>
        /// Convert system title to subject.
        /// </summary>
        /// <param name="systemTitle">System title.</param>
        /// <returns>Subject.</returns>
        public static string SystemTitleToSubject(byte[] systemTitle)
        {
            GXByteBuffer bb = new GXByteBuffer(systemTitle);
            return "CN=" + bb.ToHex(false, 0);
        }

        /// <summary>
        /// Get system title from the subject.
        /// </summary>
        /// <param name="subject">Subject.</param>
        /// <returns>System title.</returns>
        public static byte[] SystemTitleFromSubject(string subject)
        {
            return GXDLMSTranslator.HexToBytes(HexSystemTitleFromSubject(subject));
        }

        /// <summary>
        /// Get system title in hex string from the subject.
        /// </summary>
        /// <param name="subject">Subject.</param>
        /// <returns>System title.</returns>
        public static string HexSystemTitleFromSubject(string subject)
        {
            int index = subject.IndexOf("CN=");
            if (index == -1)
            {
                throw new Exception("System title not found from the subject.");
            }
            return subject.Substring(index + 3, 16);
        }

        /// <summary>
        /// Convert ASN1 certificate type to DLMS key usage.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static KeyUsage CertificateTypeToKeyUsage(CertificateType type)
        {
            KeyUsage k;
            switch (type)
            {
                case CertificateType.DigitalSignature:
                    k = KeyUsage.DigitalSignature;
                    break;
                case CertificateType.KeyAgreement:
                    k = KeyUsage.KeyAgreement;
                    break;
                case CertificateType.TLS:
                    k = KeyUsage.KeyCertSign;
                    break;
                case CertificateType.Other:
                    k = KeyUsage.CrlSign;
                    break;
                default:
                    // At least one bit must be used.
                    k = KeyUsage.None;
                    break;
            }
            return k;
        }

        /// <summary>
        /// Get certificate type from byte array.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static PkcsType GetCertificateType(byte[] data)
        {
            return GetCertificateType(data, null);
        }

        /// <summary>
        /// Get certificate type from byte array.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static PkcsType GetCertificateType(byte[] data, GXAsn1Sequence seq)
        {
            if (seq == null)
            {
                seq = (GXAsn1Sequence)GXAsn1Converter.FromByteArray(data);
            }
            if (seq[0] is GXAsn1Sequence)
            {
                try
                {
                    new GXx509Certificate(data);
                    return PkcsType.x509Certificate;
                }
                catch (Exception)
                {
                    //It's ok if this fails.
                }
            }
            if (seq[0] is GXAsn1Sequence)
            {
                try
                {
                    new GXPkcs10(data);
                    return PkcsType.Pkcs10;
                }
                catch (Exception)
                {
                    //It's ok if this fails.

                }
            }
            if (seq[0] is sbyte)
            {
                try
                {
                    new GXPkcs8(data);
                    return PkcsType.Pkcs8;
                }
                catch (Exception)
                {
                    //It's ok if this fails.
                }
            }
            return PkcsType.None;
        }

        /// <summary>
        /// Get certificate type from DER string.
        /// </summary>
        /// <param name="der">DER string</param>
        /// <returns></returns>
        public static PkcsType GetCertificateType(string der)
        {
            return GetCertificateType(Convert.FromBase64String(der));
        }
    }
}