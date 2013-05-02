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

namespace Gurux.DLMS
{
    public enum Unit : int
    {
        [XmlEnum("0")]
        None = 0,
        [XmlEnum("1")]
        Year = 1,
        [XmlEnum("2")]
        Month,
        [XmlEnum("3")]
        Week,
        [XmlEnum("4")]
        Day,
        [XmlEnum("5")]
        Hour,
        [XmlEnum("6")]
        Minute,
        [XmlEnum("7")]
        Second,
        [XmlEnum("8")]
        PhaseAngleGegree, // Phase angle degree rad*180/p
        [XmlEnum("9")]
        Temperature, // Temperature T degree centigrade
        [XmlEnum("10")]
        LocalCurrency, //Local currency
        [XmlEnum("11")]
        Length, // Length l meter m
        [XmlEnum("12")]
        Speed, // "Speed v m/s        
        [XmlEnum("13")]
        VolumeCubicMeter, //Volume V m3       
        [XmlEnum("14")]
        CorrectedVolume, // Corrected volume m3
        [XmlEnum("15")]
        VolumeFluxHour, //Volume flux m3/60*60s
        [XmlEnum("16")]
        CorrectedVolumeFluxHour, // Corrected volume flux m3/60*60s
        [XmlEnum("17")]
        VolumeFluxDay, // Volume flux m3/24*60*60s                
        [XmlEnum("18")]
        CorrecteVolumeFluxDay, // Corrected volume flux m3/24*60*60s        
        [XmlEnum("19")]
        VolumeLiter, //Volume 10-3 m3
        [XmlEnum("20")]
        MassKg, //Mass m kilogram kg
        [XmlEnum("21")]
        Force, // return "Force F newton N
        [XmlEnum("22")]
        Energy, // Energy newtonmeter J = Nm = Ws
        [XmlEnum("23")]
        PressurePascal, // Pressure p pascal N/m2
        [XmlEnum("24")]
        PressureBar, // Pressure p bar 10-5 N/m2
        [XmlEnum("25")]
        EnergyJoule, // Energy joule J = Nm = Ws
        [XmlEnum("26")]
        ThermalPower, // Thermal power J/60*60s
        [XmlEnum("27")]
        ActivePower, //Active power P watt W = J/s
        [XmlEnum("28")]
        ApparentPower, // Apparent power S
        [XmlEnum("29")]
        ReactivePower, //Reactive power Q
        [XmlEnum("30")]
        ActiveEnergy, // Active energy W*60*60s
        [XmlEnum("31")]
        ApparentEnergy, // Apparent energy VA*60*60s
        [XmlEnum("32")]
        ReactiveEnergy, // Reactive energy var*60*60s
        [XmlEnum("33")]
        Current, // Current I ampere A
        [XmlEnum("34")]
        ElectricalCharge, // Electrical charge Q coulomb C = As
        [XmlEnum("35")]
        Voltage, // Voltage
        [XmlEnum("36")]
        ElectricalFieldStrength, // Electrical field strength E V/m
        [XmlEnum("37")]
        Capacity, // Capacity C farad C/V = As/V
        [XmlEnum("38")]
        Resistance, // Resistance R ohm = V/A
        [XmlEnum("39")]
        Resistivity, // Resistivity
        [XmlEnum("40")]
        MagneticFlux, // Magnetic flux F weber Wb = Vs
        [XmlEnum("41")]
        Induction, // Induction T tesla Wb/m2
        [XmlEnum("42")]
        Magnetic, // Magnetic field strength H A/m
        [XmlEnum("43")]
        Inductivity, // Inductivity L henry H = Wb/A
        [XmlEnum("44")]
        Frequency, // Frequency f
        [XmlEnum("45")]
        Active, // Active energy meter constant 1/Wh
        [XmlEnum("46")]
        Reactive, // Reactive energy meter constant
        [XmlEnum("47")]
        Apparent, // Apparent energy meter constant
        [XmlEnum("48")]
        V260, // V260*60s
        [XmlEnum("49")]
        A260, // A260*60s
        [XmlEnum("50")]
        MassKgPerSecond, // Mass flux kg/s
        [XmlEnum("51")]
        Conductance, // Conductance siemens 1/ohm
        [XmlEnum("254")]
        OtherUnit = 254, // Other Unit
        [XmlEnum("255")]
        NoUnit = 255// No Unit                
    }
}
