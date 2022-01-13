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

using System.Xml.Serialization;

namespace Gurux.DLMS.Enums
{
    /// <summary>
    /// Unit describes available COSEM unit types.
    /// </summary>
    public enum Unit : int
    {
        /// <summary>
        /// No unit is used.
        /// </summary>
        [XmlEnum("0")]
        None = 0,
        /// <summary>
        /// Unit is yer.
        /// </summary>
        [XmlEnum("1")]
        Year = 1,
        /// <summary>
        /// Unit is month.
        /// </summary>
        [XmlEnum("2")]
        Month,
        /// <summary>
        /// Unit is week.
        /// </summary>
        [XmlEnum("3")]
        Week,
        /// <summary>
        /// Unit is day.
        /// </summary>
        [XmlEnum("4")]
        Day,
        /// <summary>
        /// Unit is hour.
        /// </summary>
        [XmlEnum("5")]
        Hour,
        /// <summary>
        /// Unit is minute.
        /// </summary>
        [XmlEnum("6")]
        Minute,
        /// <summary>
        /// Unit is second.
        /// </summary>
        [XmlEnum("7")]
        Second,
        /// <summary>
        /// Unit is phase angle degree rad*180/p
        /// </summary>
        [XmlEnum("8")]
        PhaseAngleDegree,
        /// <summary>
        /// Unit is temperature T degree centigrade
        /// </summary>
        [XmlEnum("9")]
        Temperature,
        /// <summary>
        /// Local currency is used as unit.
        /// </summary>
        [XmlEnum("10")]
        LocalCurrency,
        /// <summary>
        /// Length l meter m is used as an unit.
        /// </summary>
        [XmlEnum("11")]
        Length,
        /// <summary>
        /// Unit is Speed v m/s.
        /// </summary>
        [XmlEnum("12")]
        Speed,
        /// <summary>
        /// Unit is Volume V m3.
        /// </summary>
        [XmlEnum("13")]
        VolumeCubicMeter,
        /// <summary>
        /// Unit is Corrected volume m3.
        /// </summary>
        [XmlEnum("14")]
        CorrectedVolume,
        /// <summary>
        /// Unit is Volume flux m3/60*60s.
        /// </summary>
        [XmlEnum("15")]
        VolumeFluxHour,
        /// <summary>
        /// Unit is Corrected volume flux m3/60*60s.
        /// </summary>
        [XmlEnum("16")]
        CorrectedVolumeFluxHour,
        /// <summary>
        /// Unit is Volume flux m3/24*60*60s.
        /// </summary>
        [XmlEnum("17")]
        VolumeFluxDay,
        /// <summary>
        /// Unit is Corrected volume flux m3/24*60*60s.
        /// </summary>
        [XmlEnum("18")]
        CorrectedVolumeFluxDay,
        /// <summary>
        /// Unit is Volume 10-3 m3.
        /// </summary>
        [XmlEnum("19")]
        VolumeLiter,
        /// <summary>
        /// Unit is Mass m kilogram kg.
        /// </summary>
        [XmlEnum("20")]
        MassKg,
        /// <summary>
        /// Unit is Force F newton N.
        /// </summary>
        [XmlEnum("21")]
        Force,
        /// <summary>
        /// Unit is Energy newtonmeter J = Nm = Ws.
        /// </summary>
        [XmlEnum("22")]
        Energy,
        /// <summary>
        /// Unit is Pressure p pascal N/m2.
        /// </summary>
        [XmlEnum("23")]
        PressurePascal,
        /// <summary>
        /// Unit is Pressure p bar 10-5 N/m2.
        /// </summary>
        [XmlEnum("24")]
        PressureBar,
        /// <summary>
        /// Unit is Energy joule J = Nm = Ws.
        /// </summary>
        [XmlEnum("25")]
        EnergyJoule,
        /// <summary>
        /// Unit is Thermal power J/60*60s.
        /// </summary>
        [XmlEnum("26")]
        ThermalPower,
        /// <summary>
        /// Unit is Active power P watt W = J/s.
        /// </summary>
        [XmlEnum("27")]
        ActivePower,
        /// <summary>
        /// Unit is Apparent power S.
        /// </summary>
        [XmlEnum("28")]
        ApparentPower,
        /// <summary>
        /// Unit is Reactive power Q.
        /// </summary>
        [XmlEnum("29")]
        ReactivePower,
        /// <summary>
        /// Unit is Active energy W*60*60s.
        /// </summary>
        [XmlEnum("30")]
        ActiveEnergy,
        /// <summary>
        /// Unit is Apparent energy VA*60*60s.
        /// </summary>
        [XmlEnum("31")]
        ApparentEnergy,
        /// <summary>
        /// Unit is Reactive energy var*60*60s.
        /// </summary>
        [XmlEnum("32")]
        ReactiveEnergy,
        /// <summary>
        /// Unit is Current I ampere A.
        /// </summary>
        [XmlEnum("33")]
        Current,
        /// <summary>
        /// Unit is Electrical charge Q coulomb C = As.
        /// </summary>
        [XmlEnum("34")]
        ElectricalCharge,
        /// <summary>
        /// Unit is Voltage.
        /// </summary>
        [XmlEnum("35")]
        Voltage,
        /// <summary>
        /// Unit is Electrical field strength E V/m.
        /// </summary>
        [XmlEnum("36")]
        ElectricalFieldStrength,
        /// <summary>
        /// Unit is Capacity C farad C/V = As/V.
        /// </summary>
        [XmlEnum("37")]
        Capacity,
        /// <summary>
        /// Unit is Resistance R ohm = V/A.
        /// </summary>
        [XmlEnum("38")]
        Resistance,
        /// <summary>
        /// Unit is Resistivity.
        /// </summary>
        [XmlEnum("39")]
        Resistivity,
        /// <summary>
        /// Unit is Magnetic flux F weber Wb = Vs.
        /// </summary>
        [XmlEnum("40")]
        MagneticFlux,
        /// <summary>
        /// Unit is Induction T tesla Wb/m2.
        /// </summary>
        [XmlEnum("41")]
        Induction,
        /// <summary>
        /// Unit is Magnetic field strength H A/m.
        /// </summary>
        [XmlEnum("42")]
        Magnetic,
        /// <summary>
        /// Unit is Inductivity L henry H = Wb/A.
        /// </summary>
        [XmlEnum("43")]
        Inductivity,
        /// <summary>
        /// Unit is Frequency f.
        /// </summary>
        [XmlEnum("44")]
        Frequency,
        /// <summary>
        /// Unit is Active energy meter constant 1/Wh.
        /// </summary>
        [XmlEnum("45")]
        Active,
        /// <summary>
        /// Unit is Reactive energy meter constant.
        /// </summary>
        [XmlEnum("46")]
        Reactive,
        /// <summary>
        /// Unit is Apparent energy meter constant.
        /// </summary>
        [XmlEnum("47")]
        Apparent,
        /// <summary>
        /// Unit is V260*60s.
        /// </summary>
        [XmlEnum("48")]
        V260,
        /// <summary>
        /// Unit is A260*60s.
        /// </summary>
        [XmlEnum("49")]
        A260,
        /// <summary>
        /// Unit is Mass flux kg/s.
        /// </summary>
        [XmlEnum("50")]
        MassKgPerSecond,
        /// <summary>
        /// Unit is Conductance siemens 1/ohm.
        /// </summary>
        [XmlEnum("51")]
        Conductance,
        /// <summary>
        /// Temperature in Kelvin.
        /// </summary>
        [XmlEnum("52")]
        Kelvin,
        /// <summary>
        /// 1/(V2h) RU2h , volt-squared hour meter constant or pulse value.
        /// </summary>
        [XmlEnum("53")]
        RU2h,
        /// <summary>
        /// 1/(A2h) RI2h , ampere-squared hour meter constant or pulse value.
        /// </summary>
        [XmlEnum("54")]
        RI2h,
        /// <summary>
        /// 1/m3 RV , meter constant or pulse value (volume).
        /// </summary>
        [XmlEnum("55")]
        CubicMeterRV,
        /// <summary>
        /// Percentage
        /// </summary>
        [XmlEnum("56")]
        Percentage,
        // Ah ampere-hours 
        [XmlEnum("57")]
        AmpereHour,
        /// <summary>
        /// Wh/m3 energy per volume 3,6*103 J/m3.
        /// </summary>
        [XmlEnum("60")]
        EnergyPerVolume = 60,
        /// <summary>
        /// J/m3 calorific value, wobbe.
        /// </summary>
        [XmlEnum("61")]
        Wobbe = 61,
        /// <summary>
        /// Mol % molar fraction of gas composition mole percent (Basic gas composition unit)
        /// </summary>
        [XmlEnum("62")]
        MolePercent = 62,
        /// <summary>
        /// g/m3 mass density, quantity of material.
        /// </summary>
        [XmlEnum("63")]
        MassDensity = 63,
        /// <summary>
        /// Pa s dynamic viscosity pascal second (Characteristic of gas stream).
        /// </summary>
        [XmlEnum("64")]
        PascalSecond = 64,
        /// <summary>
        /// J/kg Specific energy 
        /// NOTE The amount of energy per unit of mass of a 
        /// substance Joule / kilogram m2 . kg . s -2 / kg = m2 . s �2
        /// </summary>
        [XmlEnum("65")]
        JouleKilogram = 65,
        /// <summary>
        /// Pressure, gram per square centimeter.
        /// </summary>
        [XmlEnum("66")]
        PressureGramPerSquareCentimeter = 66,
        /// <summary>
        /// Pressure, atmosphere.
        /// </summary>
        [XmlEnum("67")]
        PressureAtmosphere = 67,
        /// <summary>
        /// Signal strength, dB milliwatt (e.g. of GSM radio systems).
        /// </summary>
        [XmlEnum("70")]
        SignalStrengthMilliWatt = 70,
        /// <summary>
        /// Signal strength, dB microvolt.
        /// </summary>
        [XmlEnum("71")]
        SignalStrengthMicroVolt = 71,
        /// <summary>
        /// Logarithmic unit that expresses the ratio between two values of a physical quantity
        /// </summary>
        [XmlEnum("72")]
        dB = 72,
        /// <summary>
        /// Length in inches.
        /// </summary>
        [XmlEnum("128")]
        Inch = 128,
        /// <summary>
        /// Foot (Length).
        /// </summary>
        [XmlEnum("129")]
        Foot = 129,
        /// <summary>
        /// Pound (mass).
        /// </summary>
        [XmlEnum("130")]
        Pound = 130,
        /// <summary>
        /// Fahrenheit
        /// </summary>
        [XmlEnum("131")]
        Fahrenheit = 131,
        /// <summary>
        /// Rankine
        /// </summary>
        [XmlEnum("132")]
        Rankine = 132,
        /// <summary>
        /// Square inch.
        /// </summary>
        [XmlEnum("133")]
        SquareInch = 133,
        /// <summary>
        /// Square foot.
        /// </summary>
        [XmlEnum("134")]
        SquareFoot = 134,
        /// <summary>
        /// Acre
        /// </summary>
        [XmlEnum("135")]
        Acre = 135,
        /// <summary>
        /// Cubic inch.
        /// </summary>
        [XmlEnum("136")]
        CubicInch = 136,
        /// <summary>
        /// Cubic foot.
        /// </summary>
        [XmlEnum("137")]
        CubicFoot = 137,
        /// <summary>
        /// Acre-foot.
        /// </summary>
        [XmlEnum("138")]
        AcreFoot = 138,
        /// <summary>
        /// Gallon (imperial).
        /// </summary>
        [XmlEnum("139")]
        GallonImperial = 139,
        /// <summary>
        /// Gallon (US).
        /// </summary>
        [XmlEnum("140")]
        GallonUS = 140,
        /// <summary>
        /// Pound force.
        /// </summary>
        [XmlEnum("141")]
        PoundForce = 141,
        /// <summary>
        /// Pound force per square inch
        /// </summary>
        [XmlEnum("142")]
        PoundForcePerSquareInch = 142,
        /// <summary>
        /// Pound per cubic foot.
        /// </summary>
        [XmlEnum("143")]
        PoundPerCubicFoot = 143,
        /// <summary>
        /// Pound per (foot second) 
        /// </summary>
        [XmlEnum("144")]
        PoundPerFootSecond = 144,
        /// <summary>
        /// Square foot per second.
        /// </summary>
        [XmlEnum("145")]
        SquareFootPerSecond = 145,
        /// <summary>
        /// British thermal unit.
        /// </summary>
        [XmlEnum("146")]
        BritishThermalUnit = 146,
        /// <summary>
        /// Therm EU.
        /// </summary>
        [XmlEnum("147")]
        ThermEU = 147,
        /// <summary>
        /// Therm US.
        /// </summary>
        [XmlEnum("148")]
        ThermUS = 148,
        /// <summary>
        /// British thermal unit per pound.
        /// </summary>
        [XmlEnum("149")]
        BritishThermalUnitPerPound = 149,
        /// <summary>
        /// British thermal unit per cubic foot.
        /// </summary>
        [XmlEnum("150")]
        BritishThermalUnitPerCubicFoot = 150,
        /// <summary>
        /// Cubic feet.
        /// </summary>
        [XmlEnum("151")]
        CubicFeet = 151,
        /// <summary>
        /// Foot per second.
        /// </summary>
        [XmlEnum("152")]
        FootPerSecond = 152,
        /// <summary>
        /// Cubic foot per second. 
        /// </summary>
        [XmlEnum("153")]
        CubicFootPerSecond = 153,
        /// <summary>
        /// Cubic foot per min.
        /// </summary>
        [XmlEnum("154")]
        CubicFootPerMin = 154,
        /// <summary>
        /// Cubic foot per hour.
        /// </summary>
        [XmlEnum("155")]
        CubicFootPerhour = 155,
        /// <summary>
        /// Cubic foot per day
        /// </summary>
        [XmlEnum("156")]
        CubicFootPerDay = 156,
        /// <summary>
        /// Acre foot per second.
        /// </summary>
        [XmlEnum("157")]
        AcreFootPerSecond = 157,
        /// <summary>
        /// Acre foot per min.
        /// </summary>
        [XmlEnum("158")]
        AcreFootPerMin = 158,
        /// <summary>
        /// Acre foot per hour. 
        /// </summary>
        [XmlEnum("159")]
        AcreFootPerHour = 159,
        /// <summary>
        /// Acre foot per day.
        /// </summary>
        [XmlEnum("160")]
        AcreFootPerDay = 160,
        /// <summary>
        /// Imperial gallon.
        /// </summary>
        [XmlEnum("161")]
        ImperialGallon = 161,
        /// <summary>
        /// Imperial gallon per second.
        /// </summary>
        [XmlEnum("162")]
        ImperialGallonPerSecond = 162,
        /// <summary>
        /// Imperial gallon per min.
        /// </summary>
        [XmlEnum("163")]
        ImperialGallonPerMin = 163,
        /// <summary>
        /// Imperial gallon per hour.
        /// </summary>
        [XmlEnum("164")]
        ImperialGallonPerHour = 164,
        /// <summary>
        /// Imperial gallon per day.
        /// </summary>
        [XmlEnum("165")]
        ImperialGallonPerDay = 165,
        /// <summary>
        /// US gallon.
        /// </summary>
        [XmlEnum("166")]
        USGallon = 166,
        /// <summary>
        /// US gallon per second.
        /// </summary>
        [XmlEnum("167")]
        USGallonPerSecond = 167,
        /// <summary>
        /// US gallon per min.
        /// </summary>
        [XmlEnum("168")]
        USGallonPerMin = 168,
        /// <summary>
        /// US gallon per hour.
        /// </summary>
        [XmlEnum("169")]
        USGallonPerHour = 169,
        /// <summary>
        /// US gallon per day.
        /// </summary>
        [XmlEnum("170")]
        USGallonPerDay = 170,
        /// <summary>
        /// British thermal unit per second.
        /// </summary>
        [XmlEnum("171")]
        BritishThermalUnitPerSecond = 171,
        /// <summary>
        /// British thermal unit per minute.
        /// </summary>
        [XmlEnum("172")]
        BritishThermalUnitPerMinute = 172,
        /// <summary>
        /// British thermal unit per hour.
        /// </summary>
        [XmlEnum("173")]
        BritishThermalUnitPerHour = 173,
        /// <summary>
        /// British thermal unit per day.
        /// </summary>
        [XmlEnum("174")]
        BritishThermalUnitPerDay = 174,
        /// <summary>
        /// Other Unit is used.
        /// </summary>
        [XmlEnum("254")]
        OtherUnit = 254,
        /// <summary>
        /// No Unit is used.
        /// </summary>
        [XmlEnum("255")]
        NoUnit = 255
    }
}
