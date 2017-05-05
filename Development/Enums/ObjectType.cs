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

namespace Gurux.DLMS.Enums
{
    /// <summary>
    /// ObjectType enumerates the usable types of DLMS objects in GuruxDLMS.
    /// </summary>
    public enum ObjectType : int
    {
        ///<summary>
        ///Default value, no object type is set.
        ///</summary>
        [XmlEnum("0")]
        None = 0,

        ///<summary>
        ///When communicating with a meter, the application may demand periodical
        ///actions. If these actions are not linked to tariffication  = ActivityCalendar
        ///or Schedule, use an object of type ActionSchedule  = 0x16.
        ///</summary>
        [XmlEnum("22")]
        ActionSchedule = 22,

        ///<summary>
        ///When handling tariffication structures, you can use an object of type
        ///ActivityCalendar. It determines, when to activate specified scripts to
        ///perform certain activities in the meter. The activities, simply said,
        ///scheduled actions, are operations that are carried out on a specified day,
        ///at a specified time.
        ///ActivityCalendar can be used together with a more general object type,
        ///Schedule, and they can even overlap. If multiple actions are timed to the
        ///same moment, the actions determined in the Schedule are executed first,
        ///and then the ones determined in the ActivityCalendar. If using object
        ///type SpecialDaysTable, with ActivityCalendar, simultaneous actions determined
        ///in SpecialDaysTable are executed over the ones determined in ActivityCalendar.
        ///<p /><b>Note: </b>To make sure that tariffication is correct after a
        ///power failure, only the latest missed action from ActivityCalendar is
        ///executed, with a delay. In a case of power failure, if a Schedule object
        ///coexists, the latest missed action from ActivityCalendar has to be executed
        ///at the correct time, sequentially with actions determined in the Schedule.
        ///</summary>
        [XmlEnum("20")]
        ActivityCalendar = 20,

        ///<summary>
        ///AssociationLogicalName object type is used with meters that utilize
        ///Logical Name associations within a COSEM.
        ///</summary>
        [XmlEnum("15")]
        AssociationLogicalName = 15,

        ///<summary>
        ///AssociationShortName object type is used with meters that utilize Short
        ///Name associations within a COSEM.
        ///</summary>
        [XmlEnum("12")]
        AssociationShortName = 12,

        ///<summary>
        ///To determine auto answering settings  = for data transfer between device
        ///and modem = s to answer incoming calls, use AutoAnswer object.
        ///</summary>
        [XmlEnum("28")]
        AutoAnswer = 28,

        ///<summary>
        ///To determine auto connecting settings  = for data transfer from the meter
        ///to defined destinations, use AutoConnect  = previously known as AutoDial
        ///object.
        ///</summary>
        [XmlEnum("29")]
        AutoConnect = 29,

        ///<summary>
        ///An object of type Clock is used to handle the information of a date  = day,
        ///month and year and/or a time  = hundredths of a second, seconds, minutes
        ///and hours.
        ///</summary>
        [XmlEnum("8")]
        Clock = 8,

        ///<summary>
        ///An object of type Data typically stores manufacturer specific information
        ///of the meter, for example configuration data and logical name.
        ///</summary>
        [XmlEnum("1")]
        Data = 1,

        ///<summary>
        ///An object of type DemandRegister stores a value, information of the item,
        ///which the value belongs to, the status of the item, and the time of the value.
        ///DemandRegister object type enables both current, and last average, it
        ///supports both block, and sliding demand calculation, and it also provides
        ///resetting the value average, and periodic averages.
        ///</summary>
        [XmlEnum("5")]
        DemandRegister = 5,

        ///<summary>
        ///MAC address of the physical device.
        ///</summary>
        ///<remarks>
        ///The name and the use of this interface class has been changed from “Ethernet setup” to “MAC address setup” to
        ///allow a more general use.
        ///</remarks>
        [XmlEnum("43")]
        MacAddressSetup = 43,

        ///<summary>
        ///ExtendedRegister stores a value, and understands the type of the value.
        ///Refer to an object of this type by its logical name, using the OBIS
        ///identification code.
        ///</summary>
        [XmlEnum("4")]
        ExtendedRegister = 4,

        ///<summary>
        ///To determine the GPRS settings, use GprsSetup object.
        ///</summary>
        [XmlEnum("45")]
        GprsSetup = 45,

        ///<summary>
        ///To determine the HDLC = High-level Data Link Control settings, use the
        ///IecHdlcSetup object.
        ///</summary>
        [XmlEnum("23")]
        IecHdlcSetup = 23,

        ///<summary>
        ///To determine the Local Port settings, use the IecLocalPortSetup object.
        ///</summary>
        [XmlEnum("19")]
        IecLocalPortSetup = 19,

        ///<summary>
        ///To determine the Twisted Pair settings, use the IecTwistedPairSetup object.
        ///</summary>
        [XmlEnum("24")]
        IecTwistedPairSetup = 24,

        ///<summary>
        ///To determine the IP 4 settings, use the Ip4Setup object.
        ///</summary>
        [XmlEnum("42")]
        Ip4Setup = 42,

        /// <summary>
        ///GSM diagnostic settings.
        /// </summary>
        [XmlEnum("47")]
        GSMDiagnostic = 47,

        ///<summary>
        ///To determine the IP 6 settings, use the Ip6Setup object.
        ///</summary>
        [XmlEnum("48")]
        Ip6Setup = 48,

        ///<summary>
        ///To determine the M-BUS settings, use the MbusSetup object.
        ///</summary>
        [XmlEnum("25")]
        MBusSlavePortSetup = 25,

        ///<summary>
        ///To determine modem settings, use ModemConfiguration object.
        ///</summary>
        [XmlEnum("27")]
        ModemConfiguration = 27,

        [XmlEnum("40")]
        PushSetup = 40,

        ///<summary>
        ///To determine PPP  = Point-to-Point Protocol settings, use the PppSetup object.
        ///</summary>
        [XmlEnum("44")]
        PppSetup = 44,

        ///<summary>
        ///ProfileGeneric determines a general way of gathering values from a profile.
        ///The data is retrieved either by a period of time, or by an occuring event.
        ///When gathering values from a profile, you need to understand the concept
        ///of the profile buffer, in which the profile data is stored. The buffer may
        ///be sorted by a register, or by a clock, within the profile, or the data
        ///can be just piled in it, in order: last in, first out.
        ///You can retrieve a part of the buffer, within a certain range of values,
        ///or by a range of entry numbers. You can also determine objects, whose
        ///values are to be retained. To determine, what to retrieve, and what to
        ///retain, you need to assign the objects to the profile. You can use static
        ///assignments, as all entries in a buffer are alike  = same size, same structure.
        ///<p /><b>Note: </b>When you modify any assignment, the buffer of the
        ///corresponding profile is cleared, and all other profiles, using the
        ///modified one, will be cleared too. This is to make sure that their
        ///entries stay alike by size and structure.
        ///</summary>
        [XmlEnum("7")]
        ProfileGeneric = 7,

        ///<summary>
        ///Register stores a value, and understands the type of the value. Refer to
        ///an object of this type by its logical name, using the OBIS identification
        ///code.
        ///</summary>
        [XmlEnum("3")]
        Register = 3,

        ///<summary>
        ///When handling tariffication structures, you can use RegisterActivation to
        ///determine, what objects to enable, when activating a certain activation mask.
        ///The objects, assigned to the register, but not determined in the mask,
        ///are disabled.
        ///<p /><b>Note: </b>If an object is not assigned to any register, it is,
        ///by default, enabled.
        ///</summary>
        [XmlEnum("6")]
        RegisterActivation = 6,

        ///<summary>
        ///RegisterMonitor allows you to determine scripts to execute, when a register
        ///value crosses a specified threshold. To use RegisterMonitor, also ScriptTable
        ///needs to be instantiated in the same logical device.
        ///</summary>
        [XmlEnum("21")]
        RegisterMonitor = 21,

        /// <summary>
        /// Instances of the Disconnect control IC manage an internal or external disconnect unit
        /// of the meter (e.g. electricity breaker, gas valve) in order to connect or disconnect
        /// – partly or entirely – the premises of the consumer to / from the supply.
        /// </summary>
        [XmlEnum("70")]
        DisconnectControl = 70,

        [XmlEnum("71")]
        Limiter = 71,

        [XmlEnum("72")]
        MBusClient = 72,


        [XmlEnum("65")]
        ParameterMonitor = 65,

        ///<summary>
        ///RegisterTable stores identical attributes of objects, in a selected
        ///collection of objects. All the objects in the collection need to be of
        ///the same type. Also, the value in value groups A to D and F in their
        ///logical name  = OBIS identification code needs to be identical.
        ///<p />Clause 5 determines the possible values in value group E, as a table,
        ///where header = the common part, and each cell = a possible E value,
        ///of the OBIS code.
        ///</summary>
        [XmlEnum("61")]
        RegisterTable = 61,

        ///<summary>
        ///Configure a ZigBee PRO device with information necessary
        ///to create or join the network.
        ///</summary>
        [XmlEnum("101")]
        ZigBeeSasStartup = 101,

        ///<summary>
        ///Configure the behaviour of a ZigBee PRO device on
        ///joining or loss of connection to the network.
        ///</summary>
        [XmlEnum("102")]
        ZigBeeSasJoin = 102,

        ///<summary>
        ///Configure the fragmentation feature of ZigBee PRO transport layer.
        ///</summary>
        [XmlEnum("103")]
        ZigBeeSasApsFragmentation = 103,

        ///<summary>
        ///SapAssigment stores information of assignment of the logical devices to
        ///their SAP  = Service Access Points.
        ///</summary>
        [XmlEnum("17")]
        SapAssignment = 17,

        /// <summary>
        /// Instances of the Image transfer IC model the mechanism of
        /// transferring binary files, called firmware Images to COSEM servers.
        /// </summary>
        [XmlEnum("18")]
        ImageTransfer = 18,

        ///<summary>
        ///To handle time and date driven actions, use Schedule, with an object of
        ///type SpecialDaysTable.
        ///</summary>
        [XmlEnum("10")]
        Schedule = 10,

        ///<summary>
        ///To trigger a set of actions with an execute method, use object type
        ///ScriptTable. Each table entry  = script includes a unique identifier, and
        ///a set of action specifications, which either execute a method, or modify
        ///the object attributes, within the logical device. The script can be
        ///triggered by other objects  = within the same logical device, or from the
        ///outside.
        ///</summary>
        [XmlEnum("9")]
        ScriptTable = 9,

        ///<summary>
        ///With SpecialDaysTable you can determine dates to override a preset behaviour,
        ///for specific days  = data item day_id. SpecialDaysTable works together with
        ///objects of Schedule, or Activity Calendar.
        ///</summary>
        [XmlEnum("11")]
        SpecialDaysTable = 11,

        ///<summary>
        ///StatusMapping object stores status words with mapping. Each bit in the
        ///status word is mapped to position = s in referencing status table.
        ///</summary>
        [XmlEnum("63")]
        StatusMapping = 63,

        [XmlEnum("64")]
        SecuritySetup = 64,

        ///<summary>
        ///To determine Internet TCP/UDP protocol settings, use the TcpUdpSetup object.
        ///</summary>
        [XmlEnum("41")]
        TcpUdpSetup = 41,

        ///<summary>
        ///In an object of type UtilityTables each "Table"  = ANSI C12.19:1997 table data
        ///is represented as an instance, and identified by its logical name.
        ///</summary>
        [XmlEnum("26")]
        UtilityTables = 26,

        [XmlEnum("115")]
        Token = 115,

        /// <summary>
        /// S-FSK Phy MAC Setup
        /// </summary>
        [XmlEnum("50")]
        SFSKPhyMacSetUp = 50,

        /// <summary>
        /// S-FSK Active initiator.
        /// </summary>
        [XmlEnum("51")]
        SFSKActiveInitiator = 51,
        /// <summary>
        /// S-FSK MAC synchronization timeouts
        /// </summary>
        [XmlEnum("52")]
        SFSKMacSynchronizationTimeouts = 52,

        /// <summary>
        /// S-FSK MAC Counters.
        /// </summary>
        [XmlEnum("53")]
        SFSKMacCounters = 53,

        /// <summary>
        ///G3-PLC MAC layer counters
        /// </summary>
        [XmlEnum("90")]
        G3PlcMacLayerCounters = 90,

        /// <summary>
        /// G3-PLC MAC setup.
        /// </summary>
        [XmlEnum("91")]
        G3PlcMacSetup = 91,

        /// <summary>
        /// G3-PLC 6LoWPAN.
        /// </summary>
        [XmlEnum("92")]
        G3Plc6LoWPan = 92,

        /// <summary>
        /// IEC 14908 Identification.
        /// </summary>
        [XmlEnum("150")]
        IEC14908Identification = 150,

        /// <summary>
        /// IEC 14908 Physical Setup.
        /// </summary>
        [XmlEnum("151")]
        IEC14908PhysicalSetup = 151,

        /// <summary>
        /// IEC 14908 Physical Status.
        /// </summary>
        [XmlEnum("152")]
        IEC14908PhysicalStatus = 152,

        /// <summary>
        /// IEC 14908 Diagnostic.
        /// </summary>
        [XmlEnum("153")]
        IEC14908Diagnostic = 153
    }
}
