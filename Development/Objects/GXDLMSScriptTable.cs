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
using Gurux.DLMS.Objects.Enums;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Script table objects contain a table of script entries. Each entry consists of a script identifier
    /// and a series of action specifications.
    /// </summary>
    public class GXDLMSScriptTable : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSScriptTable()
        : base(ObjectType.ScriptTable)
        {
            Scripts = new List<GXDLMSScript>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSScriptTable(string ln)
        : base(ObjectType.ScriptTable, ln, 0)
        {
            Scripts = new List<GXDLMSScript>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSScriptTable(string ln, ushort sn)
        : base(ObjectType.ScriptTable, ln, sn)
        {
            Scripts = new List<GXDLMSScript>();
        }

        [XmlIgnore()]
        public List<GXDLMSScript> Scripts
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, Scripts };
        }

        #region IGXDLMSBase Members


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
            //Scripts
            if (!base.IsRead(2))
            {
                attributes.Add(2);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] { Gurux.DLMS.Properties.Resources.LogicalNameTxt, "Scripts" };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 2;
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
                int cnt = Scripts.Count;
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                //Add count
                GXCommon.SetObjectCount(cnt, data);
                foreach (GXDLMSScript it in Scripts)
                {
                    data.SetUInt8((byte)DataType.Structure);
                    data.SetUInt8(2); //Count
                    GXCommon.SetData(settings, data, DataType.UInt16, it.Id); //Script_identifier:
                    data.SetUInt8((byte)DataType.Array);
                    data.SetUInt8((byte)it.Actions.Count); //Count
                    foreach (GXDLMSScriptAction a in it.Actions)
                    {
                        data.SetUInt8((byte)DataType.Structure);
                        //Count
                        data.SetUInt8(5);
                        //service_id
                        GXCommon.SetData(settings, data, DataType.Enum, a.Type);
                        if (a.Target == null)
                        {
#pragma warning disable CS0618
                            //class_id
                            GXCommon.SetData(settings, data, DataType.UInt16, a.ObjectType);
                            //logical_name
                            GXCommon.SetData(settings, data, DataType.OctetString, a.LogicalName);
#pragma warning restore CS0618
                        }
                        else
                        {
                            //class_id
                            GXCommon.SetData(settings, data, DataType.UInt16, a.Target.ObjectType);
                            //logical_name
                            GXCommon.SetData(settings, data, DataType.OctetString, a.Target.LogicalName);
                        }
                        //index
                        GXCommon.SetData(settings, data, DataType.Int8, a.Index);
                        //parameter
                        DataType tp = a.ParameterDataType;
                        if (tp == DataType.None)
                        {
                            tp = GXCommon.GetValueType(a.Parameter);
                        }
                        GXCommon.SetData(settings, data, tp, a.Parameter);
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
                if (e.Value is string)
                {
                    LogicalName = e.Value.ToString();
                }
                else
                {
                    LogicalName = GXDLMSClient.ChangeType((byte[])e.Value, DataType.OctetString, settings.UseUtc2NormalTime).ToString();
                }
            }
            else if (e.Index == 2)
            {
                Scripts.Clear();
                //Fix Xemex bug here.
                //Xemex meters do not return array as they shoul be according standard.
                if (e.Value is Object[] && ((Object[])e.Value).Length != 0)
                {
                    if (((Object[])e.Value)[0] is Object[])
                    {
                        foreach (Object[] item in (Object[])e.Value)
                        {
                            GXDLMSScript script = new GXDLMSScript();
                            script.Id = Convert.ToInt32(item[0]);
                            Scripts.Add(script);
                            foreach (Object[] arr in (Object[])item[1])
                            {
                                GXDLMSScriptAction it = new GXDLMSScriptAction();
                                it.Type = (ScriptActionType)Convert.ToInt32(arr[0]);
                                ObjectType ot = (ObjectType)Convert.ToInt32(arr[1]);
                                String ln = GXDLMSClient.ChangeType((byte[])arr[2], DataType.OctetString, settings.UseUtc2NormalTime).ToString();
                                it.Target = settings.Objects.FindByLN(ot, ln);
                                if (it.Target == null)
                                {
#pragma warning disable CS0618
                                    it.ObjectType = (ObjectType)Convert.ToInt32(arr[1]);
                                    it.LogicalName = GXDLMSClient.ChangeType((byte[])arr[2], DataType.OctetString, settings.UseUtc2NormalTime).ToString();
#pragma warning restore CS0618
                                }

                                it.Index = Convert.ToInt32(arr[3]);
                                it.Parameter = arr[4];
                                script.Actions.Add(it);
                            }
                        }
                    }
                    else //Read Xemex meter here.
                    {
                        GXDLMSScript script = new GXDLMSScript();
                        script.Id = Convert.ToInt32(((Object[])e.Value)[0]);
                        Scripts.Add(script);
                        Object[] arr = (Object[])((Object[])e.Value)[1];
                        GXDLMSScriptAction it = new GXDLMSScriptAction();
                        it.Type = (ScriptActionType)Convert.ToInt32(arr[0]);
                        ObjectType ot = (ObjectType)Convert.ToInt32(arr[1]);
                        String ln = GXDLMSClient.ChangeType((byte[])arr[2], DataType.OctetString, settings.UseUtc2NormalTime).ToString();
                        it.Target = settings.Objects.FindByLN(ot, ln);
                        if (it.Target == null)
                        {
#pragma warning disable CS0618
                            it.ObjectType = (ObjectType)Convert.ToInt32(arr[1]);
                            it.LogicalName = GXDLMSClient.ChangeType((byte[])arr[2], DataType.OctetString, settings.UseUtc2NormalTime).ToString();
#pragma warning restore CS0618
                        }

                        it.Index = Convert.ToInt32(arr[3]);
                        it.Parameter = arr[4];
                        script.Actions.Add(it);
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
