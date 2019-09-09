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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Enums;
using System.Globalization;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Auto Connect implements data transfer from the device to one or several destinations.
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAutoConnect
    /// </summary>
    public class GXDLMSAutoConnect : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAutoConnect()
        : this("0.0.2.1.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSAutoConnect(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSAutoConnect(string ln, ushort sn)
        : base(ObjectType.AutoConnect, ln, sn)
        {
            CallingWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
        }

        /// <summary>
        /// Defines the mode controlling the auto dial functionality concerning the
        /// timing, the message type to be sent and the infrastructure to be used.
        /// </summary>
        [XmlIgnore()]
        public AutoConnectMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum number of trials in the case of unsuccessful dialling attempts.
        /// </summary>
        [XmlIgnore()]
        public int Repetitions
        {
            get;
            set;
        }

        /// <summary>
        /// The time delay, expressed in seconds until an unsuccessful dial attempt can be repeated.
        /// </summary>
        [XmlIgnore()]
        public int RepetitionDelay
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the start and end date/time stamp when the window becomes active.
        /// </summary>
        [XmlIgnore()]
        public List<KeyValuePair<GXDateTime, GXDateTime>> CallingWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the list of destinations (for example phone numbers, email
        /// addresses or their combinations) where the message(s) have to be sent
        /// under certain conditions. The conditions and their link to the elements of
        /// the array are not defined here.
        /// </summary>
        [XmlIgnore()]
        public string[] Destinations
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Mode, Repetitions, RepetitionDelay,
                              CallingWindow.ToArray(), Destinations
                            };
        }

        #region IGXDLMSBase Members

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //Mode
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //Repetitions
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //RepetitionDelay
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //CallingWindow
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //Destinations
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Internal.GXCommon.GetLogicalNameString(),
                             "Mode",
                             "Repetitions",
                             "Repetition Delay",
                             "Calling Window",
                             "Destinations"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 6;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 1;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            if (index == 1)
            {
                return DataType.OctetString;
            }
            if (index == 2)
            {
                return DataType.Enum;
            }
            if (index == 3)
            {
                return DataType.UInt8;
            }
            if (index == 4)
            {
                return DataType.UInt16;
            }
            if (index == 5)
            {
                return DataType.Array;
            }
            if (index == 6)
            {
                return DataType.Array;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }


        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return GXCommon.LogicalNameToBytes(LogicalName);
            }
            if (e.Index == 2)
            {
                return (byte)Mode;
            }
            if (e.Index == 3)
            {
                return Repetitions;
            }
            if (e.Index == 4)
            {
                return RepetitionDelay;
            }
            if (e.Index == 5)
            {
                int cnt = CallingWindow.Count;
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                //Add count
                GXCommon.SetObjectCount(cnt, data);
                if (cnt != 0)
                {
                    foreach (var it in CallingWindow)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8((byte)2); //Count
                        GXCommon.SetData(settings, data, DataType.OctetString, it.Key); //start_time
                        GXCommon.SetData(settings, data, DataType.OctetString, it.Value); //end_time
                    }
                }
                return data.Array();
            }
            if (e.Index == 6)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (Destinations == null)
                {
                    //Add count
                    GXCommon.SetObjectCount(0, data);
                }
                else
                {
                    int cnt = Destinations.Length;
                    //Add count
                    GXCommon.SetObjectCount(cnt, data);
                    foreach (string it in Destinations)
                    {
                        if (DestinationsAsString)
                        {
                            GXCommon.SetData(settings, data, DataType.String, it); //destination
                        }
                        else
                        {
                            GXCommon.SetData(settings, data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it)); //destination
                        }
                    }
                }
                return data.Array();
            }
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                LogicalName = GXCommon.ToLogicalName(e.Value);
            }
            else if (e.Index == 2)
            {
                Mode = (AutoConnectMode)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 3)
            {
                Repetitions = Convert.ToInt32(e.Value);
            }
            else if (e.Index == 4)
            {
                RepetitionDelay = Convert.ToInt32(e.Value);
            }
            else if (e.Index == 5)
            {
                CallingWindow.Clear();
                if (e.Value != null)
                {
                    foreach (List<object> item in (List<object>)e.Value)
                    {
                        GXDateTime start = (GXDateTime)GXDLMSClient.ChangeType((byte[])item[0], DataType.DateTime, settings.UseUtc2NormalTime);
                        GXDateTime end = (GXDateTime)GXDLMSClient.ChangeType((byte[])item[1], DataType.DateTime, settings.UseUtc2NormalTime);
                        CallingWindow.Add(new KeyValuePair<GXDateTime, GXDateTime>(start, end));
                    }
                }
            }
            else if (e.Index == 6)
            {
                Destinations = null;
                if (e.Value != null)
                {
                    List<string> items = new List<string>();
                    foreach (object item in (List<object>)e.Value)
                    {
                        string it;
                        if (item is byte[])
                        {
                            it = GXDLMSClient.ChangeType((byte[]) item, DataType.String, false).ToString();
                        }
                        else
                        {
                            it = Convert.ToString(item);
                            DestinationsAsString = true;
                        }
                        items.Add(it);
                    }
                    Destinations = items.ToArray();
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        bool DestinationsAsString = false;

        /// <summary>
        /// Initiates the connection process.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Connect(GXDLMSClient client)
        {
            return client.Method(this, 1, (sbyte)0);
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index != 1)
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            return null;
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            Mode = (AutoConnectMode)reader.ReadElementContentAsInt("Mode");
            Repetitions = reader.ReadElementContentAsInt("Repetitions");
            RepetitionDelay = reader.ReadElementContentAsInt("RepetitionDelay");
            CallingWindow.Clear();
            if (reader.IsStartElement("CallingWindow", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDateTime start = new GXDateTime(reader.ReadElementContentAsString("Start"), CultureInfo.InvariantCulture);
                    GXDateTime end = new GXDateTime(reader.ReadElementContentAsString("End"), CultureInfo.InvariantCulture);
                    CallingWindow.Add(new KeyValuePair<DLMS.GXDateTime, DLMS.GXDateTime>(start, end));
                }
                reader.ReadEndElement("CallingWindow");
            }
            Destinations = reader.ReadElementContentAsString("Destinations", "").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Mode", (int)Mode);
            writer.WriteElementString("Repetitions", Repetitions);
            writer.WriteElementString("RepetitionDelay", RepetitionDelay);
            if (CallingWindow != null)
            {
                writer.WriteStartElement("CallingWindow");
                foreach (KeyValuePair<GXDateTime, GXDateTime> it in CallingWindow)
                {
                    writer.WriteStartElement("Item");
                    writer.WriteElementString("Start", it.Key.ToFormatString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteElementString("End", it.Value.ToFormatString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            if (Destinations != null)
            {
                writer.WriteElementString("Destinations", string.Join(";", Destinations));
            }
        }
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
