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

using Gurux.DLMS.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Save COSEM object to the file.
    /// </summary>
    public class GXXmlWriter : IDisposable
    {
        XmlWriter writer = null;
        Stream stream = null;
        GXXmlWriterSettings Settings = null;

        private bool IgnoreDefaultValues
        {
            get
            {
                if (Settings == null)
                {
                    return false;

                }
                return Settings.IgnoreDefaultValues;
            }
        }

        private bool UseMeterTime
        {
            get
            {
                if (Settings == null)
                {
                    return false;
                }
                return Settings.UseMeterTime;
            }
        }


        public void Dispose()
        {
            if (writer != null)
            {
#if !WINDOWS_UWP
                writer.Close();
#else
                writer.Dispose();
#endif
                writer = null;
            }
            if (stream != null)
            {
#if !WINDOWS_UWP
                stream.Close();
#endif
                stream.Dispose();
                stream = null;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filename">File name.</param>
        public GXXmlWriter(string filename) : this(filename, null)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filename">File name.</param>
        /// <param name="settings">Serialization settings.</param>
        public GXXmlWriter(string filename, GXXmlWriterSettings settings)
        {
            Settings = settings;
            XmlWriterSettings s = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "  ",
                NewLineOnAttributes = false
            };
            if (File.Exists(filename))
            {
                stream = File.Open(filename, FileMode.Truncate);
            }
            else
            {
                stream = File.Open(filename, FileMode.CreateNew);
            }
            writer = XmlWriter.Create(stream, s);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="s">Stream.</param>
        public GXXmlWriter(Stream s) : this(s, null)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="s">Stream.</param>
        /// <param name="settings">Serialization settings.</param>
        public GXXmlWriter(Stream s, GXXmlWriterSettings settings)
        {
            Settings = settings;
            XmlWriterSettings s2 = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "  ",
                NewLineOnAttributes = false
            };
            writer = XmlWriter.Create(s, s2);
        }

        public void WriteStartDocument()
        {
            writer.WriteStartDocument();
        }

        public void WriteStartElement(string name, int index)
        {
            writer.WriteStartElement(name);
        }
        public void WriteAttributeString(string name, string value, int index)
        {
            writer.WriteAttributeString(name, value);
        }

        public void WriteElementString(string name, UInt64 value, int index)
        {
            if (!IgnoreDefaultValues || value != 0)
            {
                writer.WriteElementString(name, value.ToString());
            }
        }

        public void WriteElementString(string name, double value, int index)
        {
            WriteElementString(name, value, 0, index);
        }

        public void WriteElementString(string name, double value, double defaultValue, int index)
        {
            if (!IgnoreDefaultValues || value != defaultValue)
            {
                writer.WriteElementString(name, value.ToString(CultureInfo.InvariantCulture));
            }
        }

        public void WriteElementString(string name, int value, int index)
        {
            if (!IgnoreDefaultValues || value != 0)
            {
                writer.WriteElementString(name, value.ToString());
            }
        }

        public void WriteElementString(string name, string value, int index)
        {
            if (!string.IsNullOrEmpty(value))
            {
                int eof = value.IndexOf('\0');
                if (eof != -1)
                {
                    value = value.Substring(0, eof);
                }
                writer.WriteElementString(name, value);
            }
            else if (!IgnoreDefaultValues)
            {
                writer.WriteElementString(name, "");
            }
        }

        public void WriteElementString(string name, bool value, int index)
        {
            if (!IgnoreDefaultValues || value)
            {
                writer.WriteElementString(name, value ? "1" : "0");
            }
        }

        public void WriteElementString(string name, GXDateTime value, int index)
        {
            if (value != null && value != DateTime.MinValue)
            {
                string str;
                if (UseMeterTime)
                {
                    str = value.ToFormatMeterString(System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    str = value.ToFormatString(System.Globalization.CultureInfo.InvariantCulture);
                }
                writer.WriteElementString(name, str);
            }
            else if (!IgnoreDefaultValues)
            {
                writer.WriteElementString(name, "");
            }
        }

        void WriteArray(object data)
        {
            if (data is List<object>)
            {
                foreach (object tmp in (List<object>)data)
                {
                    if (tmp is byte[])
                    {
                        WriteElementObject("Item", tmp, 0);
                    }
                    else if (tmp is GXArray)
                    {
                        writer.WriteStartElement("Item");
                        writer.WriteAttributeString("Type", ((int)DataType.Array).ToString());
                        WriteArray(tmp);
                        writer.WriteEndElement();
                    }
                    else if (tmp is GXStructure)
                    {
                        writer.WriteStartElement("Item");
                        writer.WriteAttributeString("Type", ((int)DataType.Structure).ToString());
                        WriteArray(tmp);
                        writer.WriteEndElement();
                    }
                    else if (tmp is Enum)
                    {
                        WriteElementObject("Item", Convert.ToInt32(tmp), 0);
                    }
                    else
                    {
                        WriteElementObject("Item", tmp, 0);
                    }
                }
            }
        }

        public void WriteElementObject(string name, object value, int index)
        {
            if (value != null || !IgnoreDefaultValues)
            {
                DataType dt = GXDLMSConverter.GetDLMSDataType(value);
                WriteElementObject(name, value, dt, DataType.None, index);
            }
        }

        /// <summary>
        /// Write object value to file.
        /// </summary>
        /// <param name="name">Object name.</param>
        /// <param name="value">Object value.</param>
        /// <param name="skipDefaultValue">Is default value serialized.</param>
        public void WriteElementObject(string name, object value, DataType dt, DataType uiType, int index)
        {
            if (value != null)
            {
                if (IgnoreDefaultValues && value is DateTime && ((DateTime)value == DateTime.MinValue || (DateTime)value == DateTime.MaxValue))
                {
                    return;
                }

                writer.WriteStartElement(name);
                writer.WriteAttributeString("Type", ((int)dt).ToString());
                if (uiType != DataType.None && dt != uiType && (uiType != DataType.String || dt == DataType.OctetString))
                {
                    writer.WriteAttributeString("UIType", ((int)uiType).ToString());
                }
                else if (value is float || value is double)
                {
                    if (value is double)
                    {
                        writer.WriteAttributeString("UIType", ((int)DataType.Float64).ToString());
                    }
                    else
                    {
                        writer.WriteAttributeString("UIType", ((int)DataType.Float32).ToString());
                    }
                }
                if (dt == DataType.Array || dt == DataType.Structure)
                {
                    WriteArray(value);
                }
                else
                {
                    if (value is GXDateTime)
                    {
                        if (UseMeterTime)
                        {
                            writer.WriteString(((GXDateTime)value).ToFormatMeterString(CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            writer.WriteString(((GXDateTime)value).ToFormatString(CultureInfo.InvariantCulture));
                        }
                    }
                    else if (value is DateTime)
                    {
                        writer.WriteString(((DateTime)value).ToString(CultureInfo.InvariantCulture));
                    }
                    else if (value is byte[])
                    {
                        writer.WriteString(GXDLMSTranslator.ToHex((byte[])value));
                    }
                    else
                    {
                        writer.WriteString(Convert.ToString(value, CultureInfo.InvariantCulture));
                    }
                }
                writer.WriteEndElement();
            }
            else if (!IgnoreDefaultValues)
            {
                writer.WriteStartElement(name);
                writer.WriteAttributeString("Type", "0");
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Write End Element tag.
        /// </summary>
        public void WriteEndElement()
        {
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write End document tag.
        /// </summary>
        public void WriteEndDocument()
        {
            writer.WriteEndDocument();
        }

        public void Flush()
        {
            writer.Flush();
        }
    }
}
