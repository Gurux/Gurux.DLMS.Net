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
    public class GXDLMSLimiter : GXDLMSObject, IGXDLMSBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSLimiter()
        : base(ObjectType.Limiter)
        {
            EmergencyProfile = new GXDLMSEmergencyProfile();
            ActionOverThreshold = new GXDLMSActionItem();
            ActionUnderThreshold = new GXDLMSActionItem();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        public GXDLMSLimiter(string ln)
        : base(ObjectType.Limiter, ln, 0)
        {
            EmergencyProfile = new GXDLMSEmergencyProfile();
            ActionOverThreshold = new GXDLMSActionItem();
            ActionUnderThreshold = new GXDLMSActionItem();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical Name of the object.</param>
        /// <param name="sn">Short Name of the object.</param>
        public GXDLMSLimiter(string ln, ushort sn)
        : base(ObjectType.Limiter, ln, sn)
        {
            EmergencyProfile = new GXDLMSEmergencyProfile();
            ActionOverThreshold = new GXDLMSActionItem();
            ActionUnderThreshold = new GXDLMSActionItem();
        }

        /// <summary>
        /// Defines an attribute of an object to be monitored.
        /// </summary>
        [XmlIgnore()]
        public GXDLMSObject MonitoredValue
        {
            get;
            set;
        }

        /// <summary>
        /// Attribute index of monitored value.
        /// </summary>
        [XmlIgnore()]
        public int MonitoredAttributeIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Provides the active threshold value to which the attribute monitored is compared.
        /// </summary>
        public object ThresholdActive
        {
            get;
            set;
        }

        /// <summary>
        /// Provides the threshold value to which the attribute monitored
        /// is compared when in normal operation.
        /// </summary>
        public object ThresholdNormal
        {
            get;
            set;
        }

        /// <summary>
        /// Provides the threshold value to which the attribute monitored
        /// is compared when an emergency profile is active.
        /// </summary>
        public object ThresholdEmergency
        {
            get;
            set;
        }

        /// <summary>
        /// Defines minimal over threshold duration in seconds required
        /// to execute the over threshold action.
        /// </summary>
        public UInt32 MinOverThresholdDuration
        {
            get;
            set;
        }

        /// <summary>
        /// Defines minimal under threshold duration in seconds required to
        /// execute the under threshold action.
        /// </summary>
        public UInt32 MinUnderThresholdDuration
        {
            get;
            set;
        }

        public GXDLMSEmergencyProfile EmergencyProfile
        {
            get;
            set;
        }

        public UInt16[] EmergencyProfileGroupIDs
        {
            get;
            set;
        }

        /// <summary>
        /// Is Emergency Profile active.
        /// </summary>
        public bool EmergencyProfileActive
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the scripts to be executed when the monitored value
        /// crosses the threshold for minimal duration time.
        /// </summary>
        public GXDLMSActionItem ActionOverThreshold
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the scripts to be executed when the monitored value
        /// crosses the threshold for minimal duration time.
        /// </summary>
        public GXDLMSActionItem ActionUnderThreshold
        {
            get;
            set;
        }



        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, MonitoredValue, ThresholdActive, ThresholdNormal, ThresholdEmergency,
                              MinOverThresholdDuration, MinUnderThresholdDuration, EmergencyProfile, EmergencyProfileGroupIDs,
                              EmergencyProfileActive, new object[] {ActionOverThreshold, ActionUnderThreshold}
                            };
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
            //MonitoredValue
            if (CanRead(2))
            {
                attributes.Add(2);
            }

            //ThresholdActive
            if (CanRead(3))
            {
                attributes.Add(3);
            }

            //ThresholdNormal
            if (CanRead(4))
            {
                attributes.Add(4);
            }

            //ThresholdEmergency
            if (CanRead(5))
            {
                attributes.Add(5);
            }

            //MinOverThresholdDuration
            if (CanRead(6))
            {
                attributes.Add(6);
            }

            //MinUnderThresholdDuration
            if (CanRead(7))
            {
                attributes.Add(7);
            }

            //EmergencyProfile
            if (CanRead(8))
            {
                attributes.Add(8);
            }
            //EmergencyProfileGroup
            if (CanRead(9))
            {
                attributes.Add(9);
            }

            //EmergencyProfileActive
            if (CanRead(10))
            {
                attributes.Add(10);
            }
            //Actions
            if (CanRead(11))
            {
                attributes.Add(11);
            }
            return attributes.ToArray();
        }

        /// <inheritdoc cref="IGXDLMSBase.GetNames"/>
        string[] IGXDLMSBase.GetNames()
        {
            return new string[] {Gurux.DLMS.Properties.Resources.LogicalNameTxt, "Monitored Value",
                             "Active Threshold", "Normal Threshold", "Emergency Threshold", "Threshold Duration Min Over",
                             "Threshold Duration Min Under", "Emergency Profile", "Emergency Profile Group",
                             "Emergency Profile Active", "Actions"
                            };
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            return 11;
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
                return DataType.Structure;
            }
            if (index == 3)
            {
                return base.GetDataType(index);
            }
            if (index == 4)
            {
                return base.GetDataType(index);
            }
            if (index == 5)
            {
                return base.GetDataType(index);
            }
            if (index == 6)
            {
                return DataType.UInt32;
            }
            if (index == 7)
            {
                return DataType.UInt32;
            }
            if (index == 8)
            {
                return DataType.Structure;
            }
            if (index == 9)
            {
                return DataType.Array;
            }
            if (index == 10)
            {
                return DataType.Boolean;
            }
            if (index == 11)
            {
                return DataType.Structure;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(GXDLMSSettings settings, ValueEventArgs e)
        {
            if (e.Index == 1)
            {
                return this.LogicalName;
            }
            else if (e.Index == 2)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8(3);
                if (MonitoredValue == null)
                {
                    GXCommon.SetData(settings, data, DataType.Int16, 0);
                    GXCommon.SetData(settings, data, DataType.OctetString, "0.0.0.0.0.0");
                    GXCommon.SetData(settings, data, DataType.UInt8, 0);
                }
                else
                {
                    GXCommon.SetData(settings, data, DataType.Int16, MonitoredValue.ObjectType);
                    GXCommon.SetData(settings, data, DataType.OctetString, MonitoredValue.LogicalName);
                    GXCommon.SetData(settings, data, DataType.UInt8, MonitoredAttributeIndex);

                }
                return data.Array();
            }
            else if (e.Index == 3)
            {
                return ThresholdActive;
            }
            else if (e.Index == 4)
            {
                return ThresholdNormal;
            }
            else if (e.Index == 5)
            {
                return ThresholdEmergency;
            }
            else if (e.Index == 6)
            {
                return MinOverThresholdDuration;
            }
            else if (e.Index == 7)
            {
                return MinUnderThresholdDuration;
            }
            else if (e.Index == 8)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8(3);
                GXCommon.SetData(settings, data, DataType.UInt16, EmergencyProfile.ID);
                GXCommon.SetData(settings, data, DataType.OctetString, EmergencyProfile.ActivationTime);
                GXCommon.SetData(settings, data, DataType.UInt32, EmergencyProfile.Duration);
                return data.Array();
            }
            else if (e.Index == 9)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Array);
                if (EmergencyProfileGroupIDs == null)
                {
                    data.SetUInt8(0);
                }
                else
                {
                    data.SetUInt8((byte)EmergencyProfileGroupIDs.Length);
                    foreach (object it in EmergencyProfileGroupIDs)
                    {
                        GXCommon.SetData(settings, data, DataType.UInt16, it);
                    }
                }

                return data.Array();
            }
            else if (e.Index == 10)
            {
                return EmergencyProfileActive;
            }
            else if (e.Index == 11)
            {
                GXByteBuffer data = new GXByteBuffer();
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8(2);
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8(2);
                GXCommon.SetData(settings, data, DataType.OctetString, ActionOverThreshold.LogicalName);
                GXCommon.SetData(settings, data, DataType.UInt16, ActionOverThreshold.ScriptSelector);
                data.SetUInt8((byte)DataType.Structure);
                data.SetUInt8(2);
                GXCommon.SetData(settings, data, DataType.OctetString, ActionUnderThreshold.LogicalName);
                GXCommon.SetData(settings, data, DataType.UInt16, ActionUnderThreshold.ScriptSelector);
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
                ObjectType ot = (ObjectType)Convert.ToInt16(((object[])e.Value)[0]);
                string ln = GXDLMSClient.ChangeType((byte[])((object[])e.Value)[1], DataType.OctetString, settings.UseUtc2NormalTime).ToString();
                int attIndex = Convert.ToInt32(((object[])e.Value)[2]);
                MonitoredValue = settings.Objects.FindByLN(ot, ln);
                MonitoredAttributeIndex = attIndex;
            }
            else if (e.Index == 3)
            {
                ThresholdActive = e.Value;
            }
            else if (e.Index == 4)
            {
                ThresholdNormal = e.Value;
            }
            else if (e.Index == 5)
            {
                ThresholdEmergency = e.Value;
            }
            else if (e.Index == 6)
            {
                MinOverThresholdDuration = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 7)
            {
                MinUnderThresholdDuration = Convert.ToUInt32(e.Value);
            }
            else if (e.Index == 8)
            {
                object[] tmp = (object[])e.Value;
                EmergencyProfile.ID = (UInt16)tmp[0];
                EmergencyProfile.ActivationTime = (GXDateTime)GXDLMSClient.ChangeType((byte[])tmp[1], DataType.DateTime, settings.UseUtc2NormalTime);
                EmergencyProfile.Duration = (UInt32)tmp[2];
            }
            else if (e.Index == 9)
            {
                List<UInt16> list = new List<UInt16>();
                if (e.Value != null)
                {
                    foreach (object it in (object[])e.Value)
                    {
                        list.Add(Convert.ToUInt16(it));
                    }
                }
                EmergencyProfileGroupIDs = list.ToArray();
            }
            else if (e.Index == 10)
            {
                EmergencyProfileActive = Convert.ToBoolean(e.Value);
            }
            else if (e.Index == 11)
            {
                object[] tmp = (object[])e.Value;
                object[] tmp1 = (object[])tmp[0];
                object[] tmp2 = (object[])tmp[1];
                ActionOverThreshold.LogicalName = GXDLMSClient.ChangeType((byte[])tmp1[0], DataType.OctetString, settings.UseUtc2NormalTime).ToString();
                ActionOverThreshold.ScriptSelector = Convert.ToUInt16(tmp1[1]);
                ActionUnderThreshold.LogicalName = GXDLMSClient.ChangeType((byte[])tmp2[0], DataType.OctetString, settings.UseUtc2NormalTime).ToString();
                ActionUnderThreshold.ScriptSelector = Convert.ToUInt16(tmp2[1]);
            }
            else
            {
                e.Error = ErrorCode.ReadWriteDenied;
            }
        }
        #endregion
    }
}
