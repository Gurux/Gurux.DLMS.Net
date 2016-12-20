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
using System.Text;
using Gurux.DLMS;
using System.ComponentModel;
using System.Xml.Serialization;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Internal;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSMessageHandler : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSMessageHandler()
        : base(ObjectType.MessageHandler)
        {
            ListeningWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
            SendersAndActions = new List<KeyValuePair<string, KeyValuePair<int, GXDLMSScriptAction>>>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSMessageHandler(string ln)
        : base(ObjectType.MessageHandler, ln, 0)
        {
            ListeningWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
            SendersAndActions = new List<KeyValuePair<string, KeyValuePair<int, GXDLMSScriptAction>>>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSMessageHandler(string ln, ushort sn)
        : base(ObjectType.MessageHandler, ln, sn)
        {
            ListeningWindow = new List<KeyValuePair<GXDateTime, GXDateTime>>();
            SendersAndActions = new List<KeyValuePair<string, KeyValuePair<int, GXDLMSScriptAction>>>();
        }

        /// <summary>
        /// Listening Window.
        /// </summary>
        [XmlIgnore()]
        public List<KeyValuePair<GXDateTime, GXDateTime>> ListeningWindow
        {
            get;
            set;
        }

        /// <summary>
        /// List of allowed Senders.
        /// </summary>
        [XmlIgnore()]
        public string[] AllowedSenders
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the logical name of a �Script table� object and the script selector of the
        /// script to be executed if an empty message is received from a match-ing sender.
        /// </summary>
        [XmlIgnore()]
        public List<KeyValuePair<string, KeyValuePair<int, GXDLMSScriptAction>>> SendersAndActions
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, ListeningWindow, AllowedSenders, SendersAndActions };
        }

        #region IGXDLMSBase Members

        /// <summary>
        /// Data interface do not have any methods.
        /// </summary>
        /// <param name="index"></param>
        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            e.Error = ErrorCode.ReadWriteDenied;
            return null;
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //ListeningWindow
            if (CanRead(2))
            {
                attributes.Add(2);
            }
            //AllowedSenders
            if (CanRead(3))
            {
                attributes.Add(3);
            }
            //SendersAndActions
            if (CanRead(4))
            {
                attributes.Add(4);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Gurux.DLMS.Properties.Resources.LogicalNameTxt, "Listening Window",
                              "Allowed Senders", "Senders And Actions"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 4;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 0;
        }

        public override DataType GetDataType(int index)
        {
            if (index == 1)
            {
                return DataType.OctetString;
            }
            //ListeningWindow
            if (index == 2)
            {
                return DataType.Array;
            }
            //AllowedSenders
            if (index == 3)
            {
                return DataType.Array;
            }
            //SendersAndActions
            if (index == 4)
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
                GXByteBuffer buff = new GXByteBuffer();
                buff.Add((byte)DataType.Array);
                GXCommon.SetObjectCount(ListeningWindow.Count, buff);
                foreach (KeyValuePair<GXDateTime, GXDateTime> it in ListeningWindow)
                {
                    buff.Add((byte)DataType.Structure);
                    buff.Add(2);
                    GXCommon.SetData(settings, buff, DataType.OctetString, it.Key);
                    GXCommon.SetData(settings, buff, DataType.OctetString, it.Value);
                }
                return buff.Array();
            }
            if (e.Index == 3)
            {
                GXByteBuffer buff = new GXByteBuffer();
                buff.Add((byte)DataType.Array);
                GXCommon.SetObjectCount(AllowedSenders.Length, buff);
                foreach (string it in AllowedSenders)
                {
                    GXCommon.SetData(settings, buff, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it));
                }
                return buff.Array();
            }
            if (e.Index == 4)
            {
                GXByteBuffer buff = new GXByteBuffer();
                buff.Add((byte)DataType.Array);
                GXCommon.SetObjectCount(SendersAndActions.Count, buff);
                foreach (KeyValuePair<string, KeyValuePair<int, GXDLMSScriptAction>> it in SendersAndActions)
                {
                    buff.Add((byte)DataType.Structure);
                    buff.Add(2);
                    GXCommon.SetData(settings, buff, DataType.OctetString, ASCIIEncoding.ASCII.GetBytes(it.Key));
                    //TODO: GXCommon.SetData(buff, DataType.OctetString, (it.Value.));
                }
                return buff.Array();
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
                    LogicalName = GXDLMSClient.ChangeType((byte[])e.Value, DataType.OctetString).ToString();
                }
            }
            else if (e.Index == 2)
            {
                ListeningWindow.Clear();
                if (e.Value is Object[])
                {
                    foreach (object it in e.Value as Object[])
                    {
                        Object[] tmp = it as Object[];
                        GXDateTime start = GXDLMSClient.ChangeType((byte[])tmp[0], DataType.DateTime) as GXDateTime;
                        GXDateTime end = GXDLMSClient.ChangeType((byte[])tmp[1], DataType.DateTime) as GXDateTime;
                        ListeningWindow.Add(new KeyValuePair<GXDateTime, GXDateTime>(start, end));
                    }
                }

            }
            else if (e.Index == 3)
            {
                if (e.Value is Object[])
                {
                    List<string> tmp = new List<string>();
                    foreach (object it in e.Value as Object[])
                    {
                        tmp.Add(ASCIIEncoding.ASCII.GetString((byte[])it));
                    }
                    AllowedSenders = tmp.ToArray();
                }
                else
                {
                    AllowedSenders = new string[0];
                }
            }
            else if (e.Index == 4)
            {
                SendersAndActions.Clear();
                if (e.Value is Object[])
                {
                    foreach (object it in e.Value as Object[])
                    {
                        Object[] tmp = it as Object[];
                        string id = ASCIIEncoding.ASCII.GetString((byte[])tmp[0]);
                        Object[] tmp2 = tmp[1] as Object[];
                        /*TODO:
                        KeyValuePair<int, GXDLMSScriptAction> executed_script = new KeyValuePair<int, GXDLMSScriptAction>(Convert.ToInt32(tmp2[1], tmp2[2]));
                        SendersAndActions.Add(new KeyValuePair<string, KeyValuePair<int, GXDLMSScriptAction>>(id, tmp[1] as GXDateTime));
                         * */
                    }
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }
        #endregion
    }
}
