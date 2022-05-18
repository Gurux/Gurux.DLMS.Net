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
using System.Xml.Serialization;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Enums;
using System.Globalization;
using System.Text;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSAutoAnswer
    /// </summary>
    public class GXDLMSAutoAnswer : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAutoAnswer()
        : this("0.0.2.2.0.255")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSAutoAnswer(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSAutoAnswer(string ln, ushort sn)
        : base(ObjectType.AutoAnswer, ln, sn)
        {
            Version = 2;
            ListeningWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
            AllowedCallers = new List<KeyValuePair<string, CallType>>();
        }

        [XmlIgnore()]
        public AutoAnswerMode Mode
        {
            get;
            set;
        }

        [XmlIgnore()]
        public List<KeyValuePair<GXDateTime, GXDateTime>> ListeningWindow
        {
            get;
            set;
        }

        [XmlIgnore()]
        public AutoAnswerStatus Status
        {
            get;
            set;
        }

        [XmlIgnore()]
        public int NumberOfCalls
        {
            get;
            set;
        }

        /// <summary>
        /// Number of rings within the window defined by ListeningWindow.
        /// </summary>
        [XmlIgnore()]
        public int NumberOfRingsInListeningWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Number of rings outside the window defined by ListeningWindow.
        /// </summary>
        [XmlIgnore()]
        public int NumberOfRingsOutListeningWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Number of rings outside the window defined by ListeningWindow.
        /// </summary>
        [XmlIgnore()]
        public List<KeyValuePair<string, CallType>> AllowedCallers
        {
            get;
            set;
        }



        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Mode, ListeningWindow.ToArray(), Status,
                              NumberOfCalls, NumberOfRingsInListeningWindow + "/" + NumberOfRingsOutListeningWindow,
                              AllowedCallers};
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
            //Mode is static and read only once.
            if (all || !base.IsRead(2))
            {
                attributes.Add(2);
            }
            //ListeningWindow is static and read only once.
            if (all || !base.IsRead(3))
            {
                attributes.Add(3);
            }
            //Status is not static.
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }

            //NumberOfCalls is static and read only once.
            if (all || !base.IsRead(5))
            {
                attributes.Add(5);
            }
            //NumberOfRingsInListeningWindow is static and read only once.
            if (all || !base.IsRead(6))
            {
                attributes.Add(6);
            }
            if (Version > 1)
            {
                //Allowed callers.
                if (all || !base.IsRead(7))
                {
                    attributes.Add(7);
                }
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            if (Version > 1)
            {
                return new string[] {Internal.GXCommon.GetLogicalNameString(),
                             "Mode",
                             "Listening Window",
                             "Status",
                             "Number Of Calls",
                             "Number Of Rings In Listening Window",
                             "Allowed callers"};
            }
            return new string[] {Internal.GXCommon.GetLogicalNameString(),
                             "Mode",
                             "Listening Window",
                             "Status",
                             "Number Of Calls",
                             "Number Of Rings In Listening Window"};
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[0];
        }

        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 2;
        }


        int IGXDLMSBase.GetAttributeCount()
        {
            if (Version > 1)
            {
                return 7;
            }
            return 6;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
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
                return DataType.Array;
            }
            if (index == 4)
            {
                return DataType.Enum;
            }
            if (index == 5)
            {
                return DataType.UInt8;
            }
            if (index == 6)
            {
                return DataType.Array;
            }
            if (Version > 1)
            {
                if (index == 7)
                {
                    return DataType.Array;
                }
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
                int cnt = ListeningWindow.Count;
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                //Add count
                GXCommon.SetObjectCount(cnt, data);
                if (cnt != 0)
                {
                    foreach (var it in ListeningWindow)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8((byte)2); //Count
                        GXCommon.SetData(settings, data, DataType.OctetString, it.Key); //start_time
                        GXCommon.SetData(settings, data, DataType.OctetString, it.Value); //end_time
                    }
                }
                return data.Array();
            }
            if (e.Index == 4)
            {
                return Status;
            }
            if (e.Index == 5)
            {
                return NumberOfCalls;
            }
            if (e.Index == 6)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                //Add count
                GXCommon.SetObjectCount(2, data);
                GXCommon.SetData(settings, data, DataType.UInt8, NumberOfRingsInListeningWindow);
                GXCommon.SetData(settings, data, DataType.UInt8, NumberOfRingsOutListeningWindow);
                return data.Array();
            }
            if (e.Index == 7)
            {
                int cnt = AllowedCallers.Count;
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                //Add count
                GXCommon.SetObjectCount(cnt, data);
                if (cnt != 0)
                {
                    foreach (var it in AllowedCallers)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        data.SetUInt8((byte)2); //Count
                        //Caller_id.
                        GXCommon.SetData(settings, data, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Key));
                        //Call type.
                        GXCommon.SetData(settings, data, DataType.Enum, it.Value);
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
                Mode = (AutoAnswerMode)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 3)
            {
                ListeningWindow.Clear();
                if (e.Value != null)
                {
                    foreach (object tmp in (IEnumerable<object>)e.Value)
                    {
                        List<object> item;
                        if (tmp is List<object>)
                        {
                            item = (List<object>)tmp;
                        }
                        else
                        {
                            item = new List<object>((object[])tmp);
                        }
                        GXDateTime start = (GXDateTime)GXDLMSClient.ChangeType((byte[])item[0], DataType.DateTime, settings.UseUtc2NormalTime);
                        GXDateTime end = (GXDateTime)GXDLMSClient.ChangeType((byte[])item[1], DataType.DateTime, settings.UseUtc2NormalTime);
                        ListeningWindow.Add(new KeyValuePair<GXDateTime, GXDateTime>(start, end));
                    }
                }
            }
            else if (e.Index == 4)
            {
                Status = (AutoAnswerStatus)Convert.ToInt32(e.Value);
            }
            else if (e.Index == 5)
            {
                NumberOfCalls = Convert.ToInt32(e.Value);
            }
            else if (e.Index == 6)
            {
                NumberOfRingsInListeningWindow = NumberOfRingsOutListeningWindow = 0;
                if (e.Value != null)
                {
                    List<object> arr;
                    if (e.Value is List<object>)
                    {
                        arr = (List<object>)e.Value;
                    }
                    else
                    {
                        arr = new List<object>((object[])e.Value);
                    }
                    NumberOfRingsInListeningWindow = Convert.ToInt32(arr[0]);
                    NumberOfRingsOutListeningWindow = Convert.ToInt32(arr[1]);
                }
            }
            else if (e.Index == 7)
            {
                AllowedCallers.Clear();
                if (e.Value != null)
                {
                    foreach (object tmp in (IEnumerable<object>)e.Value)
                    {
                        List<object> item;
                        if (tmp is List<object>)
                        {
                            item = (List<object>)tmp;
                        }
                        else
                        {
                            item = new List<object>((object[])tmp);
                        }
                        string callerId = ASCIIEncoding.ASCII.GetString((byte[])item[0]);
                        CallType callType = (CallType)Convert.ToInt32(item[1]);
                        AllowedCallers.Add(new KeyValuePair<string, CallType>(callerId, callType));
                    }
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            Mode = (AutoAnswerMode)reader.ReadElementContentAsInt("Mode");
            ListeningWindow.Clear();
            if (reader.IsStartElement("ListeningWindow", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDateTime start = new GXDateTime(reader.ReadElementContentAsString("Start"), CultureInfo.InvariantCulture);
                    GXDateTime end = new GXDateTime(reader.ReadElementContentAsString("End"), CultureInfo.InvariantCulture);
                    ListeningWindow.Add(new KeyValuePair<DLMS.GXDateTime, DLMS.GXDateTime>(start, end));
                }
                reader.ReadEndElement("ListeningWindow");
            }
            Status = (AutoAnswerStatus)reader.ReadElementContentAsInt("Status");
            NumberOfCalls = reader.ReadElementContentAsInt("NumberOfCalls");
            NumberOfRingsInListeningWindow = reader.ReadElementContentAsInt("NumberOfRingsInListeningWindow");
            NumberOfRingsOutListeningWindow = reader.ReadElementContentAsInt("NumberOfRingsOutListeningWindow");
            if (reader.IsStartElement("AllowedCallers", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    string callerId = reader.ReadElementContentAsString("Id");
                    CallType callType = (CallType)reader.ReadElementContentAsInt("Type");
                    AllowedCallers.Add(new KeyValuePair<string, CallType>(callerId, callType));
                }
                reader.ReadEndElement("AllowedCallers");
            }
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteElementString("Mode", (int)Mode, 2);
            if (ListeningWindow != null)
            {
                writer.WriteStartElement("ListeningWindow", 3);
                foreach (KeyValuePair<GXDateTime, GXDateTime> it in ListeningWindow)
                {
                    writer.WriteStartElement("Item", 0);
                    writer.WriteElementString("Start", it.Key, 0);
                    writer.WriteElementString("End", it.Value, 0);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteElementString("Status", (int)Status, 4);
            writer.WriteElementString("NumberOfCalls", NumberOfCalls, 5);
            writer.WriteElementString("NumberOfRingsInListeningWindow", NumberOfRingsInListeningWindow, 6);
            writer.WriteElementString("NumberOfRingsOutListeningWindow", NumberOfRingsOutListeningWindow, 6);
            if (AllowedCallers != null)
            {
                writer.WriteStartElement("AllowedCallers", 3);
                foreach (KeyValuePair<string, CallType> it in AllowedCallers)
                {
                    writer.WriteStartElement("Item", 0);
                    writer.WriteElementString("Id", it.Key, 0);
                    writer.WriteElementString("Type", (int)it.Value, 0);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }

        #endregion
    }
}
