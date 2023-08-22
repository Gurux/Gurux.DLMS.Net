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
using System.Text;
using System.Xml.Serialization;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Online help:
    /// https://www.gurux.fi/Gurux.DLMS.Objects.GXDLMSFunctionControl
    /// </summary>
    public class GXDLMSFunctionControl : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSFunctionControl()
        : this("0.0.44.1.0.255", 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSFunctionControl(string ln)
        : this(ln, 0)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSFunctionControl(string ln, ushort sn)
        : base(ObjectType.FunctionControl, ln, sn)
        {
        }

        /// <summary>
        /// The current status of each functional block defined in the functions property.
        /// </summary>
        [XmlIgnore()]
        public List<GXKeyValuePair<string, bool>> ActivationStatus
        {
            get;
            set;
        } = new List<GXKeyValuePair<string, bool>>();

        /// <summary>
        /// List of modified functions.
        /// </summary>
        [XmlIgnore()]
        public List<KeyValuePair<string, List<GXDLMSObject>>> FunctionList
        {
            get;
            set;
        } = new List<KeyValuePair<string, List<GXDLMSObject>>>();


        /// <inheritdoc>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, ActivationStatus, FunctionList };
        }

        /// <summary>
        /// Adjusts the value of the current credit amount attribute.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="functions">Enabled or disabled functions.</param>
        /// <returns>Action bytes.</returns>
        /// <summary>
        public byte[][] SetFunctionStatus(
            GXDLMSClient client,
            List<GXKeyValuePair<string, bool>> functions)
        {
            return client.Method(this, 1,
                FunctionStatusToByteArray(functions), DataType.Array);
        }

        /// <summary>
        /// Adjusts the value of the current credit amount attribute.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="functions">Added functions.</param>
        /// <returns>Action bytes.</returns>
        /// <summary>
        public byte[][] AddFunction(
            GXDLMSClient client,
            List<KeyValuePair<string, List<GXDLMSObject>>> functions)
        {
            return client.Method(this, 2,
                FunctionListToByteArray(functions), DataType.Array);
        }

        /// <summary>
        /// Adjusts the value of the current credit amount attribute.
        /// </summary>
        /// <param name="client">DLMS client.</param>
        /// <param name="functions">Added functions.</param>
        /// <returns>Action bytes.</returns>
        /// <summary>
        public byte[][] RemoveFunction(
            GXDLMSClient client,
            List<KeyValuePair<string, List<GXDLMSObject>>> functions)
        {
            return client.Method(this, 3,
                FunctionListToByteArray(functions), DataType.Array);
        }

        /// <summary>
        /// Convert function states to byte array.
        /// </summary>
        /// <param name="functions">Functions.</param>
        /// <returns></returns>
        private static byte[] FunctionStatusToByteArray(
           List<GXKeyValuePair<string, bool>> functions)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Array);
            GXCommon.SetObjectCount(functions.Count, bb);
            foreach (var it in functions)
            {
                bb.SetUInt8(DataType.Structure);
                bb.SetUInt8(2);
                bb.SetUInt8(DataType.OctetString);
                GXCommon.SetObjectCount(it.Key.Length, bb);
                bb.Set(ASCIIEncoding.ASCII.GetBytes(it.Key));
                bb.SetUInt8(DataType.Boolean);
                bb.SetUInt8((byte)(it.Value ? 1 : 0));
            }
            return bb.Array();
        }

        /// <summary>
        /// Get function states from byte array.
        /// </summary>
        /// <param name="values">Byte buffer.</param>
        /// <returns>Function statuses.</returns>
        private static List<GXKeyValuePair<string, bool>> FunctionStatusFromByteArray(List<object> values)
        {
            List<GXKeyValuePair<string, bool>> functions = new List<GXKeyValuePair<string, bool>>();
            foreach (GXStructure it in values)
            {
                functions.Add(new GXKeyValuePair<string, bool>(ASCIIEncoding.ASCII.GetString((byte[])it[0]),
                    Convert.ToBoolean(it[1])));
            }
            return functions;
        }


        /// <summary>
        /// Convert function list to byte array.
        /// </summary>
        /// <param name="functions">Functions.</param>
        /// <returns>Action bytes.</returns>
        /// <summary>
        private static byte[] FunctionListToByteArray(
            List<KeyValuePair<string, List<GXDLMSObject>>> functions)
        {
            GXByteBuffer bb = new GXByteBuffer();
            bb.SetUInt8(DataType.Array);
            GXCommon.SetObjectCount(functions.Count, bb);
            foreach (var it in functions)
            {
                bb.SetUInt8(DataType.Structure);
                bb.SetUInt8(2);
                bb.SetUInt8(DataType.OctetString);
                GXCommon.SetObjectCount(it.Key.Length, bb);
                bb.Set(ASCIIEncoding.ASCII.GetBytes(it.Key));
                bb.SetUInt8(DataType.Array);
                GXCommon.SetObjectCount(it.Value.Count, bb);
                foreach (var obj in it.Value)
                {
                    bb.SetUInt8(DataType.Structure);
                    bb.SetUInt8(2);
                    //Object type.
                    bb.SetUInt8(DataType.UInt16);
                    bb.SetUInt16((UInt16)obj.ObjectType);
                    //LN
                    GXCommon.SetData(null, bb, DataType.OctetString,
                        GXCommon.LogicalNameToBytes(obj.LogicalName));
                }
            }
            return bb.Array();
        }

        /// <summary>
        /// Convert function list to byte array.
        /// </summary>
        /// <param name="functions">Functions.</param>
        /// <returns>Action bytes.</returns>
        /// <summary>
        private static List<KeyValuePair<string, List<GXDLMSObject>>> FunctionListFromByteArray(List<object> values)
        {
            List<KeyValuePair<string, List<GXDLMSObject>>> functions = new List<KeyValuePair<string, List<GXDLMSObject>>>();
            foreach (GXStructure it in values)
            {
                string fn = ASCIIEncoding.ASCII.GetString((byte[])it[0]);
                List<GXDLMSObject> objects = new List<GXDLMSObject>();
                foreach (GXStructure it2 in (GXArray)it[1])
                {
                    ObjectType ot = (ObjectType)Convert.ToInt32(it2[0]);
                    byte[] ln = (byte[])it2[1];
                    var obj = GXDLMSClient.CreateObject(ot);
                    obj.LogicalName = GXCommon.ToLogicalName(ln);
                    objects.Add(obj);
                }
                functions.Add(new KeyValuePair<string, List<GXDLMSObject>>(fn, objects));
            }

            return functions;
        }

        #region IGXDLMSBase Members

        /// <inheritdoc />
        byte[] IGXDLMSBase.Invoke(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                var functions = FunctionStatusFromByteArray((List<object>)e.Parameters);
                foreach (var f in functions)
                {
                    ActivationStatus.Where(w => w.Key == f.Key).ToList().ForEach(w => w.Value = f.Value);
                }
            }
            else if (e.Index == 2)
            {
                var functions = FunctionListFromByteArray((List<object>)e.Parameters);
                FunctionList.AddRange(functions);
            }
            else if (e.Index == 3)
            {
                var functions = FunctionListFromByteArray((List<object>)e.Parameters);
                foreach (var f in functions)
                {
                    FunctionList.RemoveAll(w => f.Key == w.Key);
                }
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
            return null;
        }

        /// <inheritdoc />
        int[] IGXDLMSBase.GetAttributeIndexToRead(bool all)
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (all || string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }
            //activation_status
            if (all || CanRead(2))
            {
                attributes.Add(2);
            }
            //function_list
            if (all || CanRead(3))
            {
                attributes.Add(3);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Internal.GXCommon.GetLogicalNameString(), "ActivationStatus", "FunctionList" };
        }

        /// <inheritdoc />
        string[] IGXDLMSBase.GetMethodNames()
        {
            return new string[] { "SetFunctionStatus", "AddFunction", "RemoveFunction" };
        }

        /// <inheritdoc />
        int IGXDLMSBase.GetMaxSupportedVersion()
        {
            return 0;
        }

        /// <inheritdoc />
        int IGXDLMSBase.GetAttributeCount()
        {
            return 3;
        }

        /// <inheritdoc />
        int IGXDLMSBase.GetMethodCount()
        {
            return 3;
        }

        /// <inheritdoc />
        public override DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.Array;
                case 3:
                    return DataType.Array;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    return GXCommon.LogicalNameToBytes(LogicalName);
                case 2:
                    return FunctionStatusToByteArray(ActivationStatus);
                case 3:
                    return FunctionListToByteArray(FunctionList);
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
            return null;
        }

        void IGXDLMSBase.SetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            switch (e.Index)
            {
                case 1:
                    LogicalName = GXCommon.ToLogicalName(e.Value);
                    break;
                case 2:
                    ActivationStatus = FunctionStatusFromByteArray((List<object>)e.Value);
                    break;
                case 3:
                    FunctionList = FunctionListFromByteArray((List<object>)e.Value);
                    break;
                default:
                    e.Error = ErrorCode.ReadWriteDenied;
                    break;
            }
        }

        /// <inheritdoc />
        void IGXDLMSBase.Load(GXXmlReader reader)
        {
            if (reader.IsStartElement("Activations", true))
            {
                ActivationStatus.Clear();
                while (reader.IsStartElement("Item", true))
                {
                    string name = reader.ReadElementContentAsString("Name");
                    bool status = reader.ReadElementContentAsInt("Status") != 0;
                    ActivationStatus.Add(new GXKeyValuePair<string, bool>(name, status));
                }
                reader.ReadEndElement("Activations");
            }
            if (reader.IsStartElement("Functions", true))
            {
                FunctionList.Clear();
                while (reader.IsStartElement("Item", true))
                {
                    string name = reader.ReadElementContentAsString("Name");
                    List<GXDLMSObject> objects = new List<GXDLMSObject>();
                    FunctionList.Add(new KeyValuePair<string, List<GXDLMSObject>>(name, objects));
                    if (reader.IsStartElement("Objects", true))
                    {
                        while (reader.IsStartElement("Object", true))
                        {
                            ObjectType ot = (ObjectType)reader.ReadElementContentAsInt("ObjectType");
                            string ln = reader.ReadElementContentAsString("LN");
                            var obj = GXDLMSClient.CreateObject(ot);
                            obj.LogicalName = ln;
                            objects.Add(obj);
                        }
                        reader.ReadEndElement("Objects");
                    }
                }
                reader.ReadEndElement("Functions");
            }
        }

        /// <inheritdoc />
        void IGXDLMSBase.Save(GXXmlWriter writer)
        {
            writer.WriteStartElement("Activations", 0);
            foreach (var it in ActivationStatus)
            {
                writer.WriteStartElement("Item", 0);
                writer.WriteElementString("Name", it.Key, 0);
                writer.WriteElementString("Status", it.Value, 0);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();//Activations

            writer.WriteStartElement("Functions", 0);
            foreach (var it in FunctionList)
            {
                writer.WriteStartElement("Item", 0);
                writer.WriteElementString("Name", it.Key, 0);
                writer.WriteStartElement("Objects", 0);
                foreach (var obj in it.Value)
                {
                    writer.WriteStartElement("Object", 0);
                    writer.WriteElementString("ObjectType", (int)obj.ObjectType, 0);
                    writer.WriteElementString("LN", obj.LogicalName, 0);
                    writer.WriteEndElement(); //Object
                }
                writer.WriteEndElement();//Objects
                writer.WriteEndElement();//Item
            }
            writer.WriteEndElement();//Functions
        }

        /// <inheritdoc />
        void IGXDLMSBase.PostLoad(GXXmlReader reader)
        {
        }
        #endregion
    }
}
