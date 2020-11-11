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
using System.Linq;
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSArbitrator
    /// </summary>
    public class GXDLMSArbitrator : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSArbitrator()
        : this("0.0.96.3.20.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSArbitrator(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSArbitrator(string ln, ushort sn)
        : base(ObjectType.Arbitrator, ln, sn)
        {
        }

        /// <summary>
        /// Requested actions.
        /// </summary>
        [XmlIgnore()]
        public GXDLMSActionItem[] Actions
        {
            get;
            set;
        }

        /// <summary>
        /// Permissions for each actor to request actions.
        /// </summary>
        [XmlIgnore()]
        public string[] PermissionsTable
        {
            get;
            set;
        }

        /// <summary>
        /// Weight allocated for each actor and to each possible action of that actor.
        /// </summary>
        [XmlIgnore()]
        public UInt16[][] WeightingsTable
        {
            get;
            set;
        }

        /// <summary>
        /// The most recent requests of each actor.
        /// </summary>
        [XmlIgnore()]
        public string[] MostRecentRequestsTable
        {
            get;
            set;
        }


        /// <summary>
        /// The number identifies a bit in the Actions
        /// </summary>
        [XmlIgnore()]
        public byte LastOutcome
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Actions, PermissionsTable, WeightingsTable, MostRecentRequestsTable, LastOutcome };
        }

        #region IGXDLMSBase Members

        private string FillWithZero(string value)
        {
            char[] tmp = new char[value.Length];
            return new string(tmp);
        }

        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                //RequestAction
                object[] args = (object[]) e.Parameters;
                LastOutcome = (byte)args[0];
            }
            else if (e.Index == 2)
            {
                //Reset
                if (PermissionsTable != null)
                {
                    for (int pos = 0; pos != PermissionsTable.Length; ++pos)
                    {
                        PermissionsTable[pos] = "";
                    }
                }
                if (WeightingsTable != null)
                {
                    for (int pos = 0; pos != PermissionsTable.Length; ++pos)
                    {
                        PermissionsTable[pos] = FillWithZero(PermissionsTable[pos]);
                    }
                }
                if (MostRecentRequestsTable != null)
                {
                    for (int pos = 0; pos != MostRecentRequestsTable.Length; ++pos)
                    {
                        MostRecentRequestsTable[pos] = "";
                    }
                }
                LastOutcome = 0;
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            return null;
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //Actions
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //Permissions table
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            //Weightings table
            if (all || CanRead(4))
            {
                attributes.Add(4);
            }
            //Most recent requests table
            if (all || CanRead(5))
            {
                attributes.Add(5);
            }
            //Last outcome
            if (all || CanRead(6))
            {
                attributes.Add(6);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "Actions", "Permissions table", "Weightings table", "Most recent requests table", "Last outcome" };
        }

        /// <inheritdoc cref="IGXDLMSBase.GetMethodNames"/>
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "Request Action", "Reset" };
        }

        /// <summary>
        /// Request Action.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] RequestAction(GXDLMSClient client, byte actor, string actions)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Structure);
            bb.SetUInt8(2);
            bb.SetUInt8(DataType.UInt8);
            bb.SetUInt8(actor);
            GXCommon.SetData(null, bb, DataType.BitString, actions);
            return client.Method(this, 1, bb.Array(), DataType.Structure);
        }

        /// <summary>
        /// Reset value.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <returns>Action bytes.</returns>
        public byte[][] Reset(GXDLMSClient client)
        {
            return client.Method(this, 2, (sbyte)0);
        }


        int IGXDLMSBase.GetAttributeCount()
        {
            return 6;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 2;
        }

        /// <inheritdoc cref="IGXDLMSBase.GetDataType"/>
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                case 3:
                case 4:
                case 5:
                    return DataType.Array;
                case 6:
                    return DataType.UInt8;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            object ret;
            switch (e.Index)
            {
                case 1:
                    ret = GXCommon.LogicalNameToBytes(LogicalName);
                    break;
                case 2:
                    {
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Array);
                        if (Actions == null)
                        {
                            GXCommon.SetObjectCount(0, data);
                        }
                        else
                        {
                            GXCommon.SetObjectCount(Actions.Length, data);
                            foreach (GXDLMSActionItem it in Actions)
                            {
                                data.SetUInt8((byte)DataType.Structure);
                                //Count
                                data.SetUInt8((byte)2);
                                GXCommon.SetData(settings, data, DataType.OctetString, GXCommon.LogicalNameToBytes(it.LogicalName));
                                GXCommon.SetData(settings, data, DataType.UInt16, it.ScriptSelector);
                            }
                        }
                        ret = data.Array();
                    }
                    break;
                case 3:
                    {
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Array);
                        if (PermissionsTable == null)
                        {
                            GXCommon.SetObjectCount(0, data);
                        }
                        else
                        {
                            GXCommon.SetObjectCount(PermissionsTable.Length, data);
                            foreach (string it in PermissionsTable)
                            {
                                GXCommon.SetData(settings, data, DataType.BitString, it);
                            }
                        }
                        ret = data.Array();
                    }
                    break;
                case 4:
                    {
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Array);
                        if (WeightingsTable == null)
                        {
                            GXCommon.SetObjectCount(0, data);
                        }
                        else
                        {
                            GXCommon.SetObjectCount(WeightingsTable.Length, data);
                            foreach (UInt16[] it in WeightingsTable)
                            {
                                data.SetUInt8((byte)DataType.Array);
                                GXCommon.SetObjectCount(it.Length, data);
                                foreach (UInt16 it2 in it)
                                {
                                    GXCommon.SetData(settings, data, DataType.UInt16, it2);
                                }
                            }
                        }
                        ret = data.Array();
                    }
                    break;
                case 5:
                    {
                        GXByteBuffer data = new GXByteBuffer();
                        data.SetUInt8((byte)DataType.Array);
                        if (MostRecentRequestsTable == null)
                        {
                            GXCommon.SetObjectCount(0, data);
                        }
                        else
                        {
                            GXCommon.SetObjectCount(MostRecentRequestsTable.Length, data);
                            foreach (string it in MostRecentRequestsTable)
                            {
                                GXCommon.SetData(settings, data, DataType.BitString, it);
                            }
                        }
                        ret = data.Array();
                    }
                    break;
                case 6:
                    ret = LastOutcome;
                    break;
                default:
                    ret = null;
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Create a new action.
        /// </summary>
        private GXDLMSActionItem CreateAction(GXDLMSSettings settings, List<object> it)
        {
            GXDLMSActionItem item = new GXDLMSActionItem();
            item.LogicalName = GXCommon.ToLogicalName(it[0]);
            item.ScriptSelector = (UInt16)(it[1]);
            return item;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    {
                        List<GXDLMSActionItem> list = new List<GXDLMSActionItem>();
                        if (e.Value != null)
                        {
                            IEnumerable<object> arr = (IEnumerable<object>)e.Value;
                            foreach (List<object> it in arr)
                            {
                                list.Add(CreateAction(settings, it));
                            }
                        }
                        Actions = list.ToArray();
                    }
                    break;
                case 3:
                    {
                        List<string> list = new List<string>();
                        if (e.Value != null)
                        {
                            IEnumerable<object> arr = (IEnumerable<object>)e.Value;
                            foreach (object it in arr)
                            {
                                list.Add(it.ToString());
                            }
                        }
                        PermissionsTable = list.ToArray();
                    }
                    break;
                case 4:
                    {
                        List<UInt16[]> list = new List<UInt16[]>();
                        if (e.Value != null)
                        {
                            IEnumerable<object> arr = (IEnumerable<object>)e.Value;
                            foreach (List<object> it in arr)
                            {
                                List<UInt16> list2 = new List<UInt16>();
                                foreach (UInt16 it2 in it)
                                {
                                    list2.Add(it2);
                                }
                                list.Add(list2.ToArray());
                            }
                        }
                        WeightingsTable = list.ToArray();
                    }
                    break;
                case 5:
                    {
                        List<string> list = new List<string>();
                        if (e.Value != null)
                        {
                            IEnumerable<object> arr = (IEnumerable<object>)e.Value;
                            foreach (object it in arr)
                            {
                                list.Add(it.ToString());
                            }
                        }
                        MostRecentRequestsTable = list.ToArray();
                    }
                    break;
                case 6:
                    LastOutcome = (byte)(e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            List<GXDLMSActionItem> actions = new List<GXDLMSActionItem>();
            if (reader.IsStartElement("Actions", true))
            {
                while (reader.IsStartElement("Item", true))
                {
                    GXDLMSActionItem it = new GXDLMSActionItem();
                    it.LogicalName = reader.ReadElementContentAsString("LN");
                    it.ScriptSelector = (byte)reader.ReadElementContentAsInt("ScriptSelector");
                    actions.Add(it);
                }
                reader.ReadEndElement("Actions");
            }
            Actions = actions.ToArray();

            List<string> permissions = new List<string>();
            if (reader.IsStartElement("PermissionTable", true))
            {
                while (reader.IsStartElement("Item", false))
                {
                    permissions.Add(reader.ReadElementContentAsString("Item"));
                }
                reader.ReadEndElement("PermissionTable");
            }
            PermissionsTable = permissions.ToArray();

            List<UInt16[]> weightings = new List<UInt16[]>();
            if (reader.IsStartElement("WeightingTable", true))
            {
                while (reader.IsStartElement("Weightings", true))
                {
                    List<UInt16> list = new List<UInt16>();
                    while (reader.IsStartElement("Item", false))
                    {
                        list.Add((UInt16)reader.ReadElementContentAsInt("Item"));
                    }
                    weightings.Add(list.ToArray());
                }
                reader.ReadEndElement("WeightingTable");
            }
            WeightingsTable = weightings.ToArray();

            List<string> mostRecentRequests = new List<string>();
            if (reader.IsStartElement("MostRecentRequestsTable", true))
            {
                while (reader.IsStartElement("Item", false))
                {
                    permissions.Add(reader.ReadElementContentAsString("Item"));
                }
                reader.ReadEndElement("MostRecentRequestsTable");
            }
            MostRecentRequestsTable = mostRecentRequests.ToArray();
            LastOutcome = (byte)reader.ReadElementContentAsInt("LastOutcome");
        }

        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            if (Actions != null)
            {
                writer.WriteStartElement("Actions", 2);
                foreach (GXDLMSActionItem it in Actions)
                {
                    writer.WriteStartElement("Item", 2);
                    writer.WriteElementString("LN", it.LogicalName, 2);
                    writer.WriteElementString("ScriptSelector", it.ScriptSelector, 1);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();//Actions
            }

            if (PermissionsTable != null)
            {
                writer.WriteStartElement("PermissionTable", 3);
                foreach (string it in PermissionsTable)
                {
                    writer.WriteElementString("Item", it, 3);
                }
                writer.WriteEndElement();//PermissionTable
            }

            if (WeightingsTable != null)
            {
                writer.WriteStartElement("WeightingTable", 4);
                foreach (UInt16[] it in WeightingsTable)
                {
                    writer.WriteStartElement("Weightings", 4);
                    foreach (UInt16 it2 in it)
                    {
                        writer.WriteElementString("Item", it2, 4);
                    }
                    writer.WriteEndElement();//Weightings
                }
                writer.WriteEndElement();//WeightingTable
            }

            if (MostRecentRequestsTable != null)
            {
                writer.WriteStartElement("MostRecentRequestsTable", 5);
                foreach (string it in MostRecentRequestsTable)
                {
                    writer.WriteElementString("Item", it, 5);
                }
                writer.WriteEndElement();//MostRecentRequestsTable
            }
            writer.WriteElementString("LastOutcome", LastOutcome, 6);
        }

        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
