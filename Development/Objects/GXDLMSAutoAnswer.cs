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
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSAutoAnswer : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSAutoAnswer()
        : base(ObjectType.AutoAnswer, "0.0.2.2.0.255", 0)
        {
            ListeningWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSAutoAnswer(string ln)
        : base(ObjectType.AutoAnswer, ln, 0)
        {
            ListeningWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSAutoAnswer(string ln, ushort sn)
        : base(ObjectType.AutoAnswer, ln, sn)
        {
            ListeningWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
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

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Mode, ListeningWindow, Status,
                              NumberOfCalls, NumberOfRingsInListeningWindow + "/" + NumberOfRingsOutListeningWindow
                            };
        }

        #region IGXDLMSBase Members

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //Mode is static and read only once.
            if (!base.IsRead(2))
            {
                attributes.Add(2);
            }
            //ListeningWindow is static and read only once.
            if (!base.IsRead(3))
            {
                attributes.Add(3);
            }
            //Status is not static.
            if (CanRead(4))
            {
                attributes.Add(4);
            }

            //NumberOfCalls is static and read only once.
            if (!base.IsRead(5))
            {
                attributes.Add(5);
            }
            //NumberOfRingsInListeningWindow is static and read only once.
            if (!base.IsRead(6))
            {
                attributes.Add(6);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Gurux.DLMS.Properties.Resources.LogicalNameTxt,
                             "Mode",
                             "Listening Window",
                             "Status",
                             "Number Of Calls",
                             "Number Of Rings In Listening Window"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
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
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return this.LogicalName;
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
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                if (e.Value is string)
                {
                    LogicalName = e.Value.ToString();
                }
                else
                {
                    LogicalName = GXDLMSClient.ChangeType((byte[])e.Value, DataType.OctetString, false).ToString();
                }
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
                    foreach (Object[] item in (Object[])e.Value)
                    {
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
                    NumberOfRingsInListeningWindow = Convert.ToInt32(((Object[])e.Value)[0]);
                    NumberOfRingsOutListeningWindow = Convert.ToInt32(((Object[])e.Value)[1]);
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

        #endregion
    }
}
