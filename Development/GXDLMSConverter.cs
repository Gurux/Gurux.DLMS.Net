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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS
{
    /// <summary>
    /// DLMS Converter is used to get string value for enumeration types.
    /// </summary>
    public class GXDLMSConverter
    {
        /// <summary>
        /// Collection of standard OBIS codes.
        /// </summary>
        private GXStandardObisCodeCollection codes = new GXStandardObisCodeCollection();

        /// <summary>
        /// Get OBIS code description.
        /// </summary>
        /// <param name="logicalName">Logical name (OBIS code).</param>
        /// <returns>Array of descriptions that match given OBIS code.</returns>
        public String[] GetDescription(String logicalName)
        {
            return GetDescription(logicalName, ObjectType.None);
        }

        /// <summary>
        /// Get OBIS code description.
        /// </summary>
        /// <param name="logicalName">Logical name (OBIS code).</param>
        /// <param name="description">Description filter.</param>
        /// <returns>Array of descriptions that match given OBIS code.</returns>
        public String[] GetDescription(String logicalName,
                String description)
        {
            return GetDescription(logicalName, ObjectType.None, description);
        }

        /// <summary>
        /// Get OBIS code description.
        /// </summary>
        /// <param name="logicalName">Logical name (OBIS code).</param>
        /// <param name="type">Object type.</param>
        /// <returns>Array of descriptions that match given OBIS code.</returns>
        public String[] GetDescription(String logicalName,
                ObjectType type)
        {
            return GetDescription(logicalName, type, null);
        }

        /// <summary>
        /// Get OBIS code description.
        /// </summary>
        /// <param name="logicalName">Logical name (OBIS code).</param>
        /// <param name="type">Object type.</param>
        /// <param name="description">Description filter.</param>
        /// <returns>Array of descriptions that match given OBIS code.</returns>
        public String[] GetDescription(String logicalName,
                ObjectType type, String description)
        {
            lock (codes)
            {
                if (codes.Count == 0)
                {
                    ReadStandardObisInfo(codes);
                }
            }
            List<String> list = new List<String>();
            bool all = string.IsNullOrEmpty(logicalName);
            foreach (GXStandardObisCode it in codes.Find(logicalName, type))
            {
                if (!string.IsNullOrEmpty(description)
                        && !it.Description.ToLower().Contains(description.ToLower()))
                {
                    continue;
                }
                if (all)
                {
                    list.Add("A=" + it.OBIS[0] + ", B=" + it.OBIS[1]
                            + ", C=" + it.OBIS[2] + ", D=" + it.OBIS[3]
                            + ", E=" + it.OBIS[4] + ", F=" + it.OBIS[5]
                            + "\r\n" + it.Description);
                }
                else
                {
                    list.Add(it.Description);
                }
            }
            return list.ToArray();
        }

        private string GetCode(string value)
        {
            int index = value.IndexOfAny(new char[] { '.', '-', ',' });
            if (index != -1)
            {
                return value.Substring(0, index);
            }
            return value;
        }

        /// <summary>
        /// Get example OBIS codes using object type as a filter.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <returns>Array of OBIS codes and descriptions that match given type.</returns>
        public KeyValuePair<string, string>[] GetObisCodesByType(ObjectType type)
        {
            lock (codes)
            {
                if (codes.Count == 0)
                {
                    ReadStandardObisInfo(codes);
                }
            }
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            foreach (GXStandardObisCode it in codes.Find(null, type))
            {
                if (it.Interfaces != "*")
                {
                    string obis = GetCode(it.OBIS[0]) + "." + GetCode(it.OBIS[1]) + "." +
                        GetCode(it.OBIS[2]) + "." + GetCode(it.OBIS[3]) + "." + GetCode(it.OBIS[4]) + "." + GetCode(it.OBIS[5]);
                    list.Add(new KeyValuePair<string, string>(obis, codes.Find(obis, type)[0].Description));
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Update standard OBIS codes description and type if defined.
        /// </summary>
        /// <param name="target"> COSEM object.</param>
        public void UpdateOBISCodeInformation(GXDLMSObject target)
        {
            lock (codes)
            {
                if (codes.Count == 0)
                {
                    ReadStandardObisInfo(codes);
                }
                UpdateOBISCodeInfo(codes, target);
            }
        }

        /// <summary>
        /// Update standard OBIS codes descriptions and type if defined.
        /// </summary>
        /// <param name="targets">Collection of COSEM objects to update.</param>
        public void UpdateOBISCodeInformation(GXDLMSObjectCollection targets)
        {
            lock (codes)
            {
                if (codes.Count == 0)
                {
                    ReadStandardObisInfo(codes);
                }
                foreach (GXDLMSObject it in targets)
                {
                    UpdateOBISCodeInfo(codes, it);
                }
            }
        }

        /// <summary>
        /// Read standard OBIS code information from the file.
        /// </summary>
        /// <param name="codes">Collection of standard OBIS codes.</param>
        private static void ReadStandardObisInfo(GXStandardObisCodeCollection codes)
        {
#if !WINDOWS_UWP
            string[] rows = Gurux.DLMS.Properties.Resources.OBISCodes.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string it in rows)
            {
                string[] items = it.Split(new char[] { ';' });
                string[] obis = items[0].Split(new char[] { '.' });
                GXStandardObisCode code = new GXStandardObisCode(obis, items[3] + "; " + items[4] + "; " +
                        items[5] + "; " + items[6] + "; " + items[7], items[1], items[2]);
                codes.Add(code);
            }
#else
            //TODO:
#endif
        }

        private static void UpdateOBISCodeInfo(GXStandardObisCodeCollection codes, GXDLMSObject it)
        {
            if (!string.IsNullOrEmpty(it.Description))
            {
                return;
            }
            GXStandardObisCode code = codes.Find(it.LogicalName, it.ObjectType)[0];
            it.Description = code.Description;
            //If string is used
            if (code.DataType.Contains("10"))
            {
                code.UIDataType = "10";
            }
            //If date time is used.
            else if (code.DataType.Contains("25") || code.DataType.Contains("26"))
            {
                code.UIDataType = code.DataType = "25";
            }
            //Time stamps of the billing periods objects (first scheme if there are two)
            else if (code.DataType.Contains("9"))
            {
                if ((GXStandardObisCodeCollection.EqualsMask("0.0-64.96.7.10-14.255", it.LogicalName) ||
                        //Time stamps of the billing periods objects (second scheme)
                        GXStandardObisCodeCollection.EqualsMask("0.0-64.0.1.5.0-99,255", it.LogicalName) ||
                        //Time of power failure
                        GXStandardObisCodeCollection.EqualsMask("0.0-64.0.1.2.0-99,255", it.LogicalName) ||
                        //Time stamps of the billing periods objects (first scheme if there are two)
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.1.2.0-99,255", it.LogicalName) ||
                        //Time stamps of the billing periods objects (second scheme)
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.1.5.0-99,255", it.LogicalName) ||
                        //Time expired since last end of billing period
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.0.255", it.LogicalName) ||
                        //Time of last reset
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.6.255", it.LogicalName) ||
                        //Date of last reset
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.7.255", it.LogicalName) ||
                        //Time expired since last end of billing period (Second billing period scheme)
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.13.255", it.LogicalName) ||
                        //Time of last reset (Second billing period scheme)
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.14.255", it.LogicalName) ||
                        //Date of last reset (Second billing period scheme)
                        GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.15.255", it.LogicalName)))
                {
                    code.UIDataType = "25";
                }
                //Local time
                else if (GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.1.255", it.LogicalName))
                {
                    code.UIDataType = "27";
                }
                //Local date
                else if (GXStandardObisCodeCollection.EqualsMask("1.0-64.0.9.2.255", it.LogicalName))
                {
                    code.UIDataType = "26";
                }
                //Active firmware identifier
                else if (GXStandardObisCodeCollection.EqualsMask("1.0.0.2.0.255", it.LogicalName))
                {
                    code.UIDataType = "10";
                }
            }
            if (code.DataType != "*" && code.DataType != string.Empty && !code.DataType.Contains(","))
            {
                DataType type = (DataType)int.Parse(code.DataType);
                switch (it.ObjectType)
                {
                    case ObjectType.Data:
                    case ObjectType.Register:
                    case ObjectType.RegisterActivation:
                    case ObjectType.ExtendedRegister:
                        it.SetDataType(2, type);
                        break;
                    default:
                        break;
                }
            }
            if (!string.IsNullOrEmpty(code.UIDataType))
            {
                DataType uiType = (DataType)int.Parse(code.UIDataType);
                switch (it.ObjectType)
                {
                    case ObjectType.Data:
                    case ObjectType.Register:
                    case ObjectType.RegisterActivation:
                    case ObjectType.ExtendedRegister:
                        it.SetUIDataType(2, uiType);
                        break;
                    default:
                        break;
                }
            }
        }


        /// <summary>
        /// Returns unit text.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetUnit(Unit value)
        {
#if !WINDOWS_UWP
            switch (value)
            {
                case Unit.Year:
                    return Gurux.DLMS.Properties.Resources.UnitYearTxt;
                case Unit.Month:
                    return Gurux.DLMS.Properties.Resources.UnitMonthTxt;
                case Unit.Week:
                    return Gurux.DLMS.Properties.Resources.UnitWeekTxt;
                case Unit.Day:
                    return Gurux.DLMS.Properties.Resources.UnitDayTxt;
                case Unit.Hour:
                    return Gurux.DLMS.Properties.Resources.UnitHourTxt;
                case Unit.Minute:
                    return Gurux.DLMS.Properties.Resources.UnitMinuteTxt;
                case Unit.Second:
                    return Gurux.DLMS.Properties.Resources.UnitSecondTxt;
                case Unit.PhaseAngleDegree:
                    return Gurux.DLMS.Properties.Resources.UnitPhasAngleDegreeTxt;
                case Unit.Temperature:
                    return Gurux.DLMS.Properties.Resources.UnitTemperatureTxt;
                case Unit.LocalCurrency:
                    return Gurux.DLMS.Properties.Resources.UnitLocalCurrencyTxt;
                case Unit.Length:
                    return Gurux.DLMS.Properties.Resources.UnitLengthTxt;
                case Unit.Speed:
                    return Gurux.DLMS.Properties.Resources.UnitSpeedTxt;
                case Unit.VolumeCubicMeter:
                    return Gurux.DLMS.Properties.Resources.UnitVolumeCubicMeterTxt;
                case Unit.CorrectedVolume:
                    return Gurux.DLMS.Properties.Resources.UnitCorrectedVolumeTxt;
                case Unit.VolumeFluxHour:
                    return Gurux.DLMS.Properties.Resources.UnitVolumeFluxHourTxt;
                case Unit.CorrectedVolumeFluxHour:
                    return Gurux.DLMS.Properties.Resources.UnitCorrectedVolumeFluxHourTxt;
                case Unit.VolumeFluxDay:
                    return Gurux.DLMS.Properties.Resources.UnitVolumeFluxDayTxt;
                case Unit.CorrecteVolumeFluxDay:
                    return Gurux.DLMS.Properties.Resources.UnitCorrecteVolumeFluxDayTxt;
                case Unit.VolumeLiter:
                    return Gurux.DLMS.Properties.Resources.UnitVolumeLiterTxt;
                case Unit.MassKg:
                    return Gurux.DLMS.Properties.Resources.UnitMassKgTxt;
                case Unit.Force:
                    return Gurux.DLMS.Properties.Resources.UnitForceTxt;
                case Unit.Energy:
                    return Gurux.DLMS.Properties.Resources.UnitEnergyTxt;
                case Unit.PressurePascal:
                    return Gurux.DLMS.Properties.Resources.UnitPressurePascalTxt;
                case Unit.PressureBar:
                    return Gurux.DLMS.Properties.Resources.UnitPressureBarTxt;
                case Unit.EnergyJoule:
                    return Gurux.DLMS.Properties.Resources.UnitEnergyJouleTxt;
                case Unit.ThermalPower:
                    return Gurux.DLMS.Properties.Resources.UnitThermalPowerTxt;
                case Unit.ActivePower:
                    return Gurux.DLMS.Properties.Resources.UnitActivePowerTxt;
                case Unit.ApparentPower:
                    return Gurux.DLMS.Properties.Resources.UnitApparentPowerTxt;
                case Unit.ReactivePower:
                    return Gurux.DLMS.Properties.Resources.UnitReactivePowerTxt;
                case Unit.ActiveEnergy:
                    return Gurux.DLMS.Properties.Resources.UnitActiveEnergyTxt;
                case Unit.ApparentEnergy:
                    return Gurux.DLMS.Properties.Resources.UnitApparentEnergyTxt;
                case Unit.ReactiveEnergy:
                    return Gurux.DLMS.Properties.Resources.UnitReactiveEnergyTxt;
                case Unit.Current:
                    return Gurux.DLMS.Properties.Resources.UnitCurrentTxt;
                case Unit.ElectricalCharge:
                    return Gurux.DLMS.Properties.Resources.UnitElectricalChargeTxt;
                case Unit.Voltage:
                    return Gurux.DLMS.Properties.Resources.UnitVoltageTxt;
                case Unit.ElectricalFieldStrength:
                    return Gurux.DLMS.Properties.Resources.UnitElectricalFieldStrengthTxt;
                case Unit.Capacity:
                    return Gurux.DLMS.Properties.Resources.UnitCapacityTxt;
                case Unit.Resistance:
                    return Gurux.DLMS.Properties.Resources.UnitResistanceTxt;
                case Unit.Resistivity:
                    return Gurux.DLMS.Properties.Resources.UnitResistivityTxt;
                case Unit.MagneticFlux:
                    return Gurux.DLMS.Properties.Resources.UnitMagneticFluxTxt;
                case Unit.Induction:
                    return Gurux.DLMS.Properties.Resources.UnitInductionTxt;
                case Unit.Magnetic:
                    return Gurux.DLMS.Properties.Resources.UnitMagneticTxt;
                case Unit.Inductivity:
                    return Gurux.DLMS.Properties.Resources.UnitInductivityTxt;
                case Unit.Frequency:
                    return Gurux.DLMS.Properties.Resources.UnitFrequencyTxt;
                case Unit.Active:
                    return Gurux.DLMS.Properties.Resources.UnitActiveTxt;
                case Unit.Reactive:
                    return Gurux.DLMS.Properties.Resources.UnitReactiveTxt;
                case Unit.Apparent:
                    return Gurux.DLMS.Properties.Resources.UnitApparentTxt;
                case Unit.V260:
                    return Gurux.DLMS.Properties.Resources.UnitV260Txt;
                case Unit.A260:
                    return Gurux.DLMS.Properties.Resources.UnitA260Txt;
                case Unit.MassKgPerSecond:
                    return Gurux.DLMS.Properties.Resources.UnitMassKgPerSecondTxt;
                case Unit.Conductance:
                    return Gurux.DLMS.Properties.Resources.UnitConductanceTxt;
                case Unit.OtherUnit:
                    return Gurux.DLMS.Properties.Resources.UnitOtherTxt;
                case Unit.NoUnit:
                    return Gurux.DLMS.Properties.Resources.UnitNoneTxt;
            }
            return "";
#else
            return value.ToString();
#endif
        }

        /// <summary>
        /// Convert DLMS data type to .Net data type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static public Type GetDataType(DataType type)
        {
            switch (type)
            {
                case DataType.None:
                    return null;
                case DataType.Array:
                case DataType.CompactArray:
                case DataType.Structure:
                    return typeof(object[]);
                case DataType.Bcd:
                    return typeof(string);
                case DataType.BitString:
                    return typeof(string);
                case DataType.Boolean:
                    return typeof(bool);
                case DataType.Date:
                    return typeof(DateTime);
                case DataType.DateTime:
                    return typeof(DateTime);
                case DataType.Float32:
                    return typeof(float);
                case DataType.Float64:
                    return typeof(double);
                case DataType.Int16:
                    return typeof(Int16);
                case DataType.Int32:
                    return typeof(Int32);
                case DataType.Int64:
                    return typeof(Int64);
                case DataType.Int8:
                    return typeof(sbyte);
                case DataType.OctetString:
                    return typeof(byte[]);
                case DataType.String:
                    return typeof(string);
                case DataType.Time:
                    return typeof(DateTime);
                case DataType.UInt16:
                    return typeof(UInt16);
                case DataType.UInt32:
                    return typeof(UInt32);
                case DataType.UInt64:
                    return typeof(UInt64);
                case DataType.UInt8:
                    return typeof(byte);
                default:
                case DataType.Enum:
                    break;
            }
            throw new Exception("Invalid DLMS data type.");
        }

        /// <summary>
        /// Get DLMS data type.
        /// </summary>
        /// <param name="value">Object</param>
        /// <returns>DLMS data type.</returns>
        static public DataType GetDLMSDataType(object value)
        {
            return GXCommon.GetValueType(value);
        }
    }
}
