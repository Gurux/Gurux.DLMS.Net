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
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Internal;
using Gurux.DLMS.ManufacturerSettings;
using System.Globalization;

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
        Standard Standard;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSConverter() : this(Standard.DLMS)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSConverter(Standard standard)
        {
            Standard = standard;
        }

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
                    ReadStandardObisInfo(Standard, codes);
                }
            }
            List<String> list = new List<String>();
            bool all = string.IsNullOrEmpty(logicalName);
            foreach (GXStandardObisCode it in codes.Find(logicalName, type, Standard))
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
                    if (Standard == Standard.SaudiArabia)
                    {
                        list.Add(it.Description.Replace("U(", "V("));
                    }
                    else
                    {
                        list.Add(it.Description);
                    }
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
                    ReadStandardObisInfo(Standard, codes);
                }
            }
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            foreach (GXStandardObisCode it in codes.Find(null, type, Standard))
            {
                if (it.Interfaces != "*")
                {
                    string obis = GetCode(it.OBIS[0]) + "." + GetCode(it.OBIS[1]) + "." +
                        GetCode(it.OBIS[2]) + "." + GetCode(it.OBIS[3]) + "." + GetCode(it.OBIS[4]) + "." + GetCode(it.OBIS[5]);
                    list.Add(new KeyValuePair<string, string>(obis, codes.Find(obis, type, Standard)[0].Description));
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
                    ReadStandardObisInfo(Standard, codes);
                }
                UpdateOBISCodeInfo(codes, target, Standard);
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
                    ReadStandardObisInfo(Standard, codes);
                }
                foreach (GXDLMSObject it in targets)
                {
                    UpdateOBISCodeInfo(codes, it, Standard);
                }
            }
        }

        /// <summary>
        /// Read standard OBIS code information from the file.
        /// </summary>
        /// <param name="codes">Collection of standard OBIS codes.</param>
        private static void ReadStandardObisInfo(Standard standard, GXStandardObisCodeCollection codes)
        {
#if !WINDOWS_UWP
            if (standard != Standard.DLMS)
            {
                foreach (GXObisCode it in GetObjects(standard))
                {
                    GXStandardObisCode tmp = new GXStandardObisCode()
                    {
                        Interfaces = ((int)it.ObjectType).ToString(),
                        OBIS = it.LogicalName.Split(new char[] { '.' }),
                        Description = it.Description
                    };
                    codes.Add(tmp);
                }
            }
#if __MOBILE__
            string[] rows = Resources.OBISCodes.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
#else
            string[] rows = Gurux.DLMS.Properties.Resources.OBISCodes.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
#endif //__MOBILE__

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

        private static void UpdateOBISCodeInfo(GXStandardObisCodeCollection codes, GXDLMSObject it, Standard standard)
        {
            GXStandardObisCode code = codes.Find(it.LogicalName, it.ObjectType, standard)[0];
            if (string.IsNullOrEmpty(it.Description))
            {
                it.Description = code.Description;
                if (standard == Standard.SaudiArabia)
                {
                    it.Description = it.Description.Replace("U(", "V(");
                }
            }
            //If string is used
            string datatype = code.DataType;
            if (datatype == null)
            {
                datatype = "";
            }
            if (datatype.Contains("10"))
            {
                code.UIDataType = "10";
            }
            //If date time is used.
            else if (datatype.Contains("25") || datatype.Contains("26"))
            {
                code.UIDataType = code.DataType = "25";
            }
            //Time stamps of the billing periods objects (first scheme if there are two)
            else if (datatype.Contains("9"))
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
            //Unix time
            else if (it.ObjectType == ObjectType.Data && GXStandardObisCodeCollection.EqualsMask("0.0.1.1.0.255", it.LogicalName))
            {
                code.UIDataType = "25";
            }

            if (code.DataType != "*" && !string.IsNullOrEmpty(code.DataType) && !code.DataType.Contains(","))
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
#if !WINDOWS_UWP && !__MOBILE__
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
#if __MOBILE__
            switch (value)
            {
                case Unit.Year:
                    return Resources.UnitYearTxt;
                case Unit.Month:
                    return Resources.UnitMonthTxt;
                case Unit.Week:
                    return Resources.UnitWeekTxt;
                case Unit.Day:
                    return Resources.UnitDayTxt;
                case Unit.Hour:
                    return Resources.UnitHourTxt;
                case Unit.Minute:
                    return Resources.UnitMinuteTxt;
                case Unit.Second:
                    return Resources.UnitSecondTxt;
                case Unit.PhaseAngleDegree:
                    return Resources.UnitPhasAngleDegreeTxt;
                case Unit.Temperature:
                    return Resources.UnitTemperatureTxt;
                case Unit.LocalCurrency:
                    return Resources.UnitLocalCurrencyTxt;
                case Unit.Length:
                    return Resources.UnitLengthTxt;
                case Unit.Speed:
                    return Resources.UnitSpeedTxt;
                case Unit.VolumeCubicMeter:
                    return Resources.UnitVolumeCubicMeterTxt;
                case Unit.CorrectedVolume:
                    return Resources.UnitCorrectedVolumeTxt;
                case Unit.VolumeFluxHour:
                    return Resources.UnitVolumeFluxHourTxt;
                case Unit.CorrectedVolumeFluxHour:
                    return Resources.UnitCorrectedVolumeFluxHourTxt;
                case Unit.VolumeFluxDay:
                    return Resources.UnitVolumeFluxDayTxt;
                case Unit.CorrecteVolumeFluxDay:
                    return Resources.UnitCorrecteVolumeFluxDayTxt;
                case Unit.VolumeLiter:
                    return Resources.UnitVolumeLiterTxt;
                case Unit.MassKg:
                    return Resources.UnitMassKgTxt;
                case Unit.Force:
                    return Resources.UnitForceTxt;
                case Unit.Energy:
                    return Resources.UnitEnergyTxt;
                case Unit.PressurePascal:
                    return Resources.UnitPressurePascalTxt;
                case Unit.PressureBar:
                    return Resources.UnitPressureBarTxt;
                case Unit.EnergyJoule:
                    return Resources.UnitEnergyJouleTxt;
                case Unit.ThermalPower:
                    return Resources.UnitThermalPowerTxt;
                case Unit.ActivePower:
                    return Resources.UnitActivePowerTxt;
                case Unit.ApparentPower:
                    return Resources.UnitApparentPowerTxt;
                case Unit.ReactivePower:
                    return Resources.UnitReactivePowerTxt;
                case Unit.ActiveEnergy:
                    return Resources.UnitActiveEnergyTxt;
                case Unit.ApparentEnergy:
                    return Resources.UnitApparentEnergyTxt;
                case Unit.ReactiveEnergy:
                    return Resources.UnitReactiveEnergyTxt;
                case Unit.Current:
                    return Resources.UnitCurrentTxt;
                case Unit.ElectricalCharge:
                    return Resources.UnitElectricalChargeTxt;
                case Unit.Voltage:
                    return Resources.UnitVoltageTxt;
                case Unit.ElectricalFieldStrength:
                    return Resources.UnitElectricalFieldStrengthTxt;
                case Unit.Capacity:
                    return Resources.UnitCapacityTxt;
                case Unit.Resistance:
                    return Resources.UnitResistanceTxt;
                case Unit.Resistivity:
                    return Resources.UnitResistivityTxt;
                case Unit.MagneticFlux:
                    return Resources.UnitMagneticFluxTxt;
                case Unit.Induction:
                    return Resources.UnitInductionTxt;
                case Unit.Magnetic:
                    return Resources.UnitMagneticTxt;
                case Unit.Inductivity:
                    return Resources.UnitInductivityTxt;
                case Unit.Frequency:
                    return Resources.UnitFrequencyTxt;
                case Unit.Active:
                    return Resources.UnitActiveTxt;
                case Unit.Reactive:
                    return Resources.UnitReactiveTxt;
                case Unit.Apparent:
                    return Resources.UnitApparentTxt;
                case Unit.V260:
                    return Resources.UnitV260Txt;
                case Unit.A260:
                    return Resources.UnitA260Txt;
                case Unit.MassKgPerSecond:
                    return Resources.UnitMassKgPerSecondTxt;
                case Unit.Conductance:
                    return Resources.UnitConductanceTxt;
                case Unit.OtherUnit:
                    return Resources.UnitOtherTxt;
                case Unit.NoUnit:
                    return Resources.UnitNoneTxt;
            }
            return "";
#else
            return value.ToString();
#endif
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
                    return typeof(GXArray);
                case DataType.CompactArray:
                case DataType.Structure:
                    return typeof(GXStructure);
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
                case DataType.Enum:
                    return typeof(GXEnum);
                default:
                    throw new Exception("Invalid DLMS data type.");
            }
        }

        /// <summary>
        /// Get DLMS data type.
        /// </summary>
        /// <param name="value">Object</param>
        /// <returns>DLMS data type.</returns>
        static public DataType GetDLMSDataType(object value)
        {
            if (value == null)
            {
                return DataType.None;
            }
            return GXCommon.GetDLMSDataType(value.GetType());
        }

        /// <summary>
        /// Get DLMS data type.
        /// </summary>
        /// <param name="type">Data type.</param>
        /// <returns>DLMS data type.</returns>
        static public DataType GetDLMSDataType(Type type)
        {
            return GXCommon.GetDLMSDataType(type);
        }

        static public byte[] GetBytes(object value, DataType type)
        {
            GXByteBuffer bb = new GXByteBuffer();
            GXCommon.SetData(null, bb, type, value);
            return bb.Array();
        }

        /// <summary>
        /// Get country spesific OBIS codes.
        /// </summary>
        /// <param name="standard">Used standard.</param>
        /// <returns>Collection for special OBIC codes.</returns>
        public static GXObisCode[] GetObjects(Standard standard)
        {
            List<GXObisCode> codes = new List<GXObisCode>();
#if !WINDOWS_UWP
            string[] rows;
#if __MOBILE__
            if (standard == Standard.Italy)
            {
                rows = Resources.Italy.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (standard == Standard.India)
            {
                rows = Resources.India.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (standard == Standard.SaudiArabia)
            {
                rows = Resources.SaudiArabia.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                return new GXObisCode[0];
            }
#else
            if (standard == Standard.Italy)
            {
                rows = Gurux.DLMS.Properties.Resources.Italy.Split(new string[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (standard == Standard.India)
            {
                rows = Gurux.DLMS.Properties.Resources.India.Split(new string[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (standard == Standard.SaudiArabia)
            {
                rows = Gurux.DLMS.Properties.Resources.SaudiArabia.Split(new string[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                return new GXObisCode[0];
            }
#endif //!__MOBILE__
            foreach (string it in rows)
            {
                if (!it.StartsWith("#"))
                {
                    string[] items = it.Split(new char[] { ';' });
                    ObjectType ot = (ObjectType)int.Parse(items[0]);
                    string ln = GXCommon.ToLogicalName(GXCommon.LogicalNameToBytes(items[1]));
                    int version = int.Parse(items[2]);
                    string desc = items[3];
                    GXObisCode code = new GXObisCode(ln, ot, desc);
                    code.Version = version;
                    codes.Add(code);
                }
            }
#endif
            return codes.ToArray();
        }

        /// <summary>
        /// Convert byte array to logical name.
        /// </summary>
        /// <param name="value">Logical name as byte array.</param>
        /// <returns>Logical name as a string.</returns>
        public static string ToLogicalName(object value)
        {
            return GXCommon.ToLogicalName(value);
        }
        /// <summary>
        /// Convert logical name to byte array.
        /// </summary>
        /// <param name="value">Logical name as a string.</param>
        /// <returns>Logical name as byte array.</returns>
        public static byte[] LogicalNameToBytes(string value)
        {
            return GXCommon.LogicalNameToBytes(value);
        }


        static public object ChangeType(object value, DataType type, CultureInfo cultureInfo)
        {
            object ret;
            if (type == DataType.OctetString)
            {
                if (value is byte[])
                {
                    ret = value;
                }
                else
                {
                    ret = GXDLMSTranslator.HexToBytes((string)value);
                }
            }
            else if (type == DataType.DateTime)
            {
                if (value is GXDateTime)
                {
                    ret = value;
                }
                else
                {
                    ret = new GXDateTime((string)value, CultureInfo.InvariantCulture);
                }
            }
            else if (type == DataType.Date)
            {
                if (value is GXDateTime)
                {
                    ret = value;
                }
                else
                {
                    ret = new GXDate((string)value, CultureInfo.InvariantCulture);
                }
            }
            else if (type == DataType.Time)
            {
                if (value is GXDateTime)
                {
                    ret = value;
                }
                else
                {
                    ret = new GXTime((string)value, CultureInfo.InvariantCulture);
                }
            }
            else if (type == DataType.Enum)
            {
                if (value is GXEnum)
                {
                    ret = value;
                }
                else
                {
                    ret = new GXEnum((byte)Convert.ChangeType(value, typeof(byte)));
                }
            }
            else if (type == DataType.Structure || type == DataType.Array)
            {
                ret = GXDLMSTranslator.XmlToValue((string)value);
            }
            else
            {
                ret = Convert.ChangeType(value, GXDLMSConverter.GetDataType(type), cultureInfo);
            }
            return ret;
        }
    }
}

