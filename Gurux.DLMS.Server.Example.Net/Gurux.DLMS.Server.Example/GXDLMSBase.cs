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
using Gurux.DLMS.Objects;
using Gurux.DLMS;
using Gurux.Net;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;
using Gurux.DLMS.Secure;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Threading;

namespace GuruxDLMSServerExample
{
    /// <summary>
    /// All example servers are using same objects.
    /// </summary>
    class GXDLMSBase : GXDLMSSecureServer
    {
        /// <summary>
        /// Serial number of the meter.
        /// </summary>
        UInt32 serialNumber = 12345678;

        /// <summary>
        /// Is data saved to ring buffer.
        /// </summary>
        bool UseRingBuffer = false;

        //Image to update.
        string ImageUpdate = null;
        //When image activation started.
        DateTime imageActionStartTime;
        //What is expected image size.
        UInt32 ImageSize = 0;
        static readonly object FileLock = new object();
        static string GetdataFile()
        {
            return Path.Combine(Path.GetDirectoryName(typeof(GXDLMSBase).Assembly.Location), "data.csv");
        }
        TraceLevel Trace = TraceLevel.Error;

        /// <summary>
        /// List of connections. This is used to close connection if meter is leave without diconnect.
        /// </summary>
        public static Dictionary<object, GXDLMSBase> connections = new Dictionary<object, GXDLMSBase>();

        /// <summary>
        /// Add low level security object and set parameters.
        /// </summary>
        private void AddLowLevelAssociation()
        {
            GXDLMSAssociationLogicalName obj = new GXDLMSAssociationLogicalName("0.0.40.0.2.255");
            obj.ClientSAP = 17;
            obj.XDLMSContextInfo.MaxSendPduSize = obj.XDLMSContextInfo.MaxReceivePduSize = 1024;
            byte[] secret = ASCIIEncoding.ASCII.GetBytes("Gurux");
            obj.Secret = secret;
            obj.AuthenticationMechanismName.MechanismId = Authentication.Low;
            // Only get, set, multiple references and parameterized access services
            // are allowed. https://www.gurux.fi/Gurux.DLMS.Conformance
            obj.XDLMSContextInfo.Conformance = Conformance.Get | Conformance.MultipleReferences | Conformance.Set | Conformance.SelectiveAccess;
            Items.Add(obj);
        }

        private void AddHighLevelAssociation()
        {
            GXDLMSAssociationLogicalName obj = new GXDLMSAssociationLogicalName("0.0.40.0.3.255");
            obj.ClientSAP = 18;
            obj.XDLMSContextInfo.MaxSendPduSize = obj.XDLMSContextInfo.MaxReceivePduSize = 1024;
            byte[] secret = ASCIIEncoding.ASCII.GetBytes("Gurux");
            obj.Secret = secret;
            obj.AuthenticationMechanismName.MechanismId = Authentication.High;
            // Add supported services.
            // https://www.gurux.fi/Gurux.DLMS.Conformance
            obj.XDLMSContextInfo.Conformance = GXDLMSClient.GetInitialConformance(true);
            Items.Add(obj);
            //Update access modes so user can invoke all methods and read/write values as default.
            if (obj.Version < 3)
            {
                obj.SetDefaultAccess(AccessMode.ReadWrite);
                obj.SetDefaultMethodAccess(MethodAccessMode.Access);
            }
            else
            {
                obj.SetDefaultAccess3(AccessMode3.Read | AccessMode3.Write);
                obj.SetDefaultMethodAccess3(MethodAccessMode3.Access);
            }
        }

        private void AddHighLevelAssociationMd5()
        {
            GXDLMSAssociationLogicalName obj = new GXDLMSAssociationLogicalName("0.0.40.0.4.255");
            obj.ClientSAP = 19;
            obj.XDLMSContextInfo.MaxSendPduSize = obj.XDLMSContextInfo.MaxReceivePduSize = 1024;
            byte[] secret = ASCIIEncoding.ASCII.GetBytes("Gurux");
            obj.Secret = secret;
            obj.AuthenticationMechanismName.MechanismId = Authentication.HighMD5;
            // Add supported services.
            // https://www.gurux.fi/Gurux.DLMS.Conformance
            obj.XDLMSContextInfo.Conformance = GXDLMSClient.GetInitialConformance(true);
            Items.Add(obj);
            //Update access modes so user can invoke all methods and read/write values as default.
            if (obj.Version < 3)
            {
                obj.SetDefaultAccess(AccessMode.ReadWrite);
                obj.SetDefaultMethodAccess(MethodAccessMode.Access);
            }
            else
            {
                obj.SetDefaultAccess3(AccessMode3.Read | AccessMode3.Write);
                obj.SetDefaultMethodAccess3(MethodAccessMode3.Access);
            }
        }

        private void AddHighLevelAssociationSha1()
        {
            GXDLMSAssociationLogicalName obj = new GXDLMSAssociationLogicalName("0.0.40.0.5.255");
            obj.ClientSAP = 20;
            obj.XDLMSContextInfo.MaxSendPduSize = obj.XDLMSContextInfo.MaxReceivePduSize = 1024;
            byte[] secret = ASCIIEncoding.ASCII.GetBytes("Gurux");
            obj.Secret = secret;
            obj.AuthenticationMechanismName.MechanismId = Authentication.HighSHA1;
            // Add supported services.
            // https://www.gurux.fi/Gurux.DLMS.Conformance
            obj.XDLMSContextInfo.Conformance = GXDLMSClient.GetInitialConformance(true);
            Items.Add(obj);
            //Update access modes so user can invoke all methods and read/write values as default.
            if (obj.Version < 3)
            {
                obj.SetDefaultAccess(AccessMode.ReadWrite);
                obj.SetDefaultMethodAccess(MethodAccessMode.Access);
            }
            else
            {
                obj.SetDefaultAccess3(AccessMode3.Read | AccessMode3.Write);
                obj.SetDefaultMethodAccess3(MethodAccessMode3.Access);
            }
        }

        private void AddHighLevelAssociationGmac()
        {
            GXDLMSAssociationLogicalName obj = new GXDLMSAssociationLogicalName("0.0.40.0.6.255");
            obj.ClientSAP = 21;
            obj.XDLMSContextInfo.MaxSendPduSize = obj.XDLMSContextInfo.MaxReceivePduSize = 1024;
            obj.AuthenticationMechanismName.MechanismId = Authentication.HighGMAC;
            // Add supported services.
            // https://www.gurux.fi/Gurux.DLMS.Conformance
            obj.XDLMSContextInfo.Conformance = GXDLMSClient.GetInitialConformance(true);
            Items.Add(obj);
        }

        private void AddHighLevelAssociationSha256()
        {
            GXDLMSAssociationLogicalName obj = new GXDLMSAssociationLogicalName("0.0.40.0.7.255");
            byte[] secret = ASCIIEncoding.ASCII.GetBytes("Gurux");
            obj.Secret = secret;
            obj.ClientSAP = 22;
            obj.XDLMSContextInfo.MaxSendPduSize = obj.XDLMSContextInfo.MaxReceivePduSize = 1024;
            obj.AuthenticationMechanismName.MechanismId = Authentication.HighSHA256;
            // Add supported services.
            // https://www.gurux.fi/Gurux.DLMS.Conformance
            obj.XDLMSContextInfo.Conformance = GXDLMSClient.GetInitialConformance(true);
            Items.Add(obj);
        }

        private void AddHighLevelAssociationEcdsa()
        {
            GXDLMSAssociationLogicalName obj = new GXDLMSAssociationLogicalName("0.0.40.0.8.255");
            obj.ClientSAP = 23;
            obj.XDLMSContextInfo.MaxSendPduSize = obj.XDLMSContextInfo.MaxReceivePduSize = 1024;
            obj.AuthenticationMechanismName.MechanismId = Authentication.HighECDSA;
            // Add supported services.
            // https://www.gurux.fi/Gurux.DLMS.Conformance
            obj.XDLMSContextInfo.Conformance = GXDLMSClient.GetInitialConformance(true);
            Items.Add(obj);
            GXDLMSSecuritySetup s = new GXDLMSSecuritySetup("0.0.43.0.7.255");
            s.SecuritySuite = SecuritySuite.Suite0;
            obj.SecuritySetupReference = s.LogicalName;
            s.ServerSystemTitle = Ciphering.SystemTitle;
            Items.Add(s);
        }

        /// <summary>
        /// Add ciphered High level association.
        /// </summary>
        private void AddSecuredHighLevelAssociation()
        {
            GXDLMSAssociationLogicalName obj = new GXDLMSAssociationLogicalName("0.0.40.0.9.255");
            obj.ClientSAP = 24;
            obj.XDLMSContextInfo.MaxSendPduSize = obj.XDLMSContextInfo.MaxReceivePduSize = 1024;
            byte[] secret = ASCIIEncoding.ASCII.GetBytes("Gurux");
            obj.Secret = secret;
            obj.AuthenticationMechanismName.MechanismId = Authentication.High;
            // Add supported services.
            // https://www.gurux.fi/Gurux.DLMS.Conformance
            obj.XDLMSContextInfo.Conformance = GXDLMSClient.GetInitialConformance(true);
            Items.Add(obj);
            GXDLMSSecuritySetup s = new GXDLMSSecuritySetup("0.0.43.0.8.255");
            s.SecuritySuite = SecuritySuite.Suite0;
            obj.SecuritySetupReference = s.LogicalName;
            s.ServerSystemTitle = Ciphering.SystemTitle;
            obj.ApplicationContextName.ContextId = ApplicationContextName.LogicalNameWithCiphering;
            Items.Add(s);
            // Add invocation counter.
            GXDLMSData d = new GXDLMSData("0.0.43.1.8.255");
            d.Value = 0;
            d.SetDataType(2, DataType.UInt32);
            Items.Add(d);
            //Add invocation counter for the public association as well so it can be read.
            GXDLMSAssociationLogicalName publicAssociation = Items.FindByLN(ObjectType.AssociationLogicalName, "0.0.40.0.1.255") as GXDLMSAssociationLogicalName;
            publicAssociation.ObjectList.Add(d);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical name settings.</param>
        /// <param name="hdlc">HDLC settings.</param>
        public GXDLMSBase(GXDLMSAssociationLogicalName ln, GXDLMSHdlcSetup hdlc)
        : base(ln, hdlc)
        {
            Conformance = Conformance.None;
            ln.LogicalName = "0.0.40.0.1.255";
            ln.ClientSAP = 16;
            // Only get is allowed.
            ln.XDLMSContextInfo.Conformance = Conformance.Get;
            PushClientAddress = 64;
            MaxReceivePDUSize = 1024;
            ln.XDLMSContextInfo.MaxReceivePduSize = ln.XDLMSContextInfo.MaxSendPduSize = 1024;
            //Set max TX and RX sizes for the HDLC frame.
            hdlc.MaximumInfoLengthReceive = hdlc.MaximumInfoLengthTransmit = 2030;
            //Set max TX and RX window sizes for the HDLC frame.
            hdlc.WindowSizeReceive = hdlc.WindowSizeTransmit = 3;
            // Add only three object for this association.
            ln.ObjectList.Clear();
            ln.ObjectList.Add(ln);
            //Add other objects.
            AddObjects();
            // Add Logical Device Name
            GXDLMSObject obj = Items.FindByLN(ObjectType.Data, "0.0.42.0.0.255");
            if (obj != null)
            {
                ln.ObjectList.Add(obj);
            }
            // Add invocation counter.
            obj = Items.FindByLN(ObjectType.Data, "0.0.43.1.0.255");
            if (obj != null)
            {
                ln.ObjectList.Add(obj);
            }
            //Update access modes so user can only read objects as default.
            if (ln.Version < 3)
            {
                ln.SetDefaultAccess(AccessMode.Read);
            }
            else
            {
                ln.SetDefaultAccess3(AccessMode3.Read);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sn">Short name settings.</param>
        /// <param name="hdlc">HDLC settings.</param>
        public GXDLMSBase(GXDLMSAssociationShortName sn, GXDLMSHdlcSetup hdlc)
        : base(sn, hdlc, "GRX", 12345678)
        {
            PushClientAddress = 64;
            MaxReceivePDUSize = 1024;
            //Default secret.
            sn.Secret = ASCIIEncoding.ASCII.GetBytes("Gurux");
            //Add security setup object.
            sn.SecuritySetupReference = "0.0.43.0.0.255";
            //Add other objects.
            AddObjects();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ln">Logical name settings.</param>
        /// <param name="wrapper">Wrapper settings.</param>
        public GXDLMSBase(GXDLMSAssociationLogicalName ln, GXDLMSTcpUdpSetup wrapper)
        : base(ln, wrapper, "GRX", 12345678)
        {
            Conformance = Conformance.None;
            ln.LogicalName = "0.0.40.0.1.255";
            ln.ClientSAP = 16;
            PushClientAddress = 64;
            MaxReceivePDUSize = 1024;
            ln.XDLMSContextInfo.Conformance = Conformance.Get;
            ln.XDLMSContextInfo.MaxReceivePduSize = ln.XDLMSContextInfo.MaxSendPduSize = 1024;
            //Default secret.
            ln.Secret = ASCIIEncoding.ASCII.GetBytes("Gurux");
            //Add other objects.
            AddObjects();
            //Update access modes so user can only read objects as default.
            if (ln.Version < 3)
            {
                ln.SetDefaultAccess(AccessMode.Read);
            }
            else
            {
                ln.SetDefaultAccess3(AccessMode3.Read);
            }
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sn">Short name settings.</param>
        /// <param name="wrapper">Wrapper settings.</param>
        public GXDLMSBase(GXDLMSAssociationShortName sn, GXDLMSTcpUdpSetup wrapper)
        : base(sn, wrapper, "GRX", 12345678)
        {
            PushClientAddress = 64;
            MaxReceivePDUSize = 1024;
            //Default secret.
            sn.Secret = ASCIIEncoding.ASCII.GetBytes("Gurux");
            //Add other objects.
            AddObjects();
        }

        Gurux.Common.IGXMedia Media = null;

        public void Initialize(string port, TraceLevel trace)
        {
            Media = new Gurux.Serial.GXSerial(port);
            Trace = trace;
            Init();
        }
        /// <summary>
        /// Generic initialize for all servers.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="trace"></param>
        public void Initialize(int port, TraceLevel trace)
        {
            Media = new GXNet(NetworkType.Tcp, port);
            Trace = trace;
            Init();
        }

        //Add Logical Device Name. 123456 is meter serial number.
        GXDLMSData AddLogicalDeviceName()
        {
            GXDLMSData ldn = new GXDLMSData("0.0.42.0.0.255");
            ldn.Value = "Gurux123456";
            //Value is get as Octet String.
            ldn.SetDataType(2, DataType.OctetString);
            ldn.SetUIDataType(2, DataType.String);
            Items.Add(ldn);
            return ldn;
        }

        void AddFirmwareVersion()
        {
            GXDLMSData fw = new GXDLMSData("1.0.0.2.0.255");
            fw.Value = "Gurux FW 0.0.1";
            Items.Add(fw);
        }

        /// <summary>
        /// Add invocation counter.
        /// </summary>
        void AddInvocationCounter()
        {
            GXDLMSData d = new GXDLMSData("0.0.43.1.0.255");
            d.Value = 0;
            d.SetDataType(2, DataType.UInt32);
            //Set initial value for invocation counter.
            d.Value = (UInt32)100;
            Items.Add(d);
        }

        private void AddObjects()
        {
            if (UseLogicalNameReferencing)
            {
                // Add all possible associations for demo purposes.
                AddLowLevelAssociation();
                AddHighLevelAssociation();
                AddHighLevelAssociationMd5();
                AddHighLevelAssociationSha1();
                AddHighLevelAssociationGmac();
                AddHighLevelAssociationSha256();
                AddHighLevelAssociationEcdsa();
                AddSecuredHighLevelAssociation();
            }


            ///////////////////////////////////////////////////////////////////////
            //Add Logical Device Name. 123456 is meter serial number.
            GXDLMSData ldn = AddLogicalDeviceName();
            //Add firmware version.
            AddFirmwareVersion();
            //Add invocation counter.
            AddInvocationCounter();

            //Add Last average.
            GXDLMSRegister r = new GXDLMSRegister("1.1.21.25.0.255");
            //Set access right. Client can't change average value.
            Items.Add(r);
            //Add default clock. Clock's Logical Name is 0.0.1.0.0.255.
            GXDLMSClock clock = new GXDLMSClock();
            clock.Begin = new GXDateTime(-1, 9, 1, -1, -1, -1, -1);
            clock.End = new GXDateTime(-1, 3, 1, -1, -1, -1, -1);
            clock.TimeZone = -(int)TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes;
            clock.Deviation = 60;
            Items.Add(clock);
            ///////////////////////////////////////////////////////////////////////
            //Add Load profile.
            GXDLMSProfileGeneric pg = new GXDLMSProfileGeneric("1.0.99.1.0.255");
            //Set capture period to 60 second.
            pg.CapturePeriod = 60;
            //Maximum row count.
            pg.ProfileEntries = 100000;
            pg.SortMethod = SortMethod.FiFo;
            pg.SortObject = clock;
            //Add columns.
            //Set saved attribute index.
            pg.CaptureObjects.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(clock, new GXDLMSCaptureObject(2, 0)));
            //Set saved attribute index.
            pg.CaptureObjects.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(r, new GXDLMSCaptureObject(2, 0)));
            Items.Add(pg);
            //Add initial rows.
            //Generate Profile Generic data file
            lock (FileLock)
            {
                //Create 10 000 rows for profile generic file.
                //In example profile generic we have two columns.
                //Date time and integer value.
                int rowCount = 10000;
                DateTime dt = DateTime.Now;
                //Reset minutes and seconds to Zero.
                dt = dt.AddSeconds(-dt.Second);
                dt = dt.AddMinutes(-dt.Minute);
                dt = dt.AddHours(-(rowCount - 1));
                StringBuilder sb = new StringBuilder();
                for (int pos = 0; pos != rowCount; ++pos)
                {
                    sb.Append(dt.ToString(CultureInfo.InvariantCulture));
                    sb.Append(';');
                    sb.AppendLine(Convert.ToString(pos + 1));
                    dt = dt.AddHours(1);
                }
                using (var writer = File.CreateText(GetdataFile()))
                {
                    sb.Length -= 2;
                    writer.Write(sb.ToString());
                }
            }
            ///////////////////////////////////////////////////////////////////////
            //Add Auto connect object.
            GXDLMSAutoConnect ac = new GXDLMSAutoConnect();
            ac.Mode = AutoConnectMode.AutoDiallingAllowedAnytime;
            ac.Repetitions = 10;
            ac.RepetitionDelay = 60;
            //Calling is allowed for every hour.
            ac.CallingWindow.Add(new KeyValuePair<GXDateTime, GXDateTime>(new GXDateTime(-1, -1, -1, -1, 0, 0, 0), new GXDateTime(-1, -1, -1, -1, 0, 0, 0)));
            ac.Destinations = new string[] { "localhost:4059" };
            Items.Add(ac);
            ///////////////////////////////////////////////////////////////////////
            //Add Activity Calendar object.
            GXDLMSActivityCalendar activity = new GXDLMSActivityCalendar();
            activity.CalendarNameActive = "Active";
            activity.WeekProfileTableActive = new GXDLMSWeekProfile[] { new GXDLMSWeekProfile("Monday", 1, 1, 1, 1, 1, 1, 1) };
            activity.DayProfileTableActive = new GXDLMSDayProfile[] { new GXDLMSDayProfile(1, new GXDLMSDayProfileAction[] { new GXDLMSDayProfileAction(new GXTime(DateTime.Now), "0.1.10.1.101.255", 1) }) };
            activity.SeasonProfileActive = new GXDLMSSeasonProfile[] { new GXDLMSSeasonProfile("Summer time", new GXDateTime(-1, 3, 31, -1, -1, -1, -1), activity.WeekProfileTableActive[0]) };

            activity.CalendarNamePassive = "Passive";
            activity.WeekProfileTablePassive = new GXDLMSWeekProfile[] { new GXDLMSWeekProfile("Tuesday", 1, 1, 1, 1, 1, 1, 1) };
            activity.DayProfileTablePassive = new GXDLMSDayProfile[] { new GXDLMSDayProfile(1, new GXDLMSDayProfileAction[] { new GXDLMSDayProfileAction(new GXTime(DateTime.Now), "0.1.10.1.101.255", 1) }) };
            activity.SeasonProfilePassive = new GXDLMSSeasonProfile[] { new GXDLMSSeasonProfile("Winter time", new GXDateTime(-1, 10, 30, -1, -1, -1, -1), activity.WeekProfileTablePassive[0]) };
            activity.Time = new GXDateTime(DateTime.Now);
            Items.Add(activity);
            ///////////////////////////////////////////////////////////////////////
            //Add local port setup object.
            GXDLMSIECLocalPortSetup optical = new GXDLMSIECLocalPortSetup();
            optical.DefaultMode = OpticalProtocolMode.Default;
            optical.ProposedBaudrate = BaudRate.Baudrate9600;
            optical.DefaultBaudrate = BaudRate.Baudrate300;
            optical.ResponseTime = LocalPortResponseTime.ms200;
            optical.DeviceAddress = "Gurux";
            optical.Password1 = "Gurux1";
            optical.Password2 = "Gurux2";
            optical.Password5 = "Gurux5";
            Items.Add(optical);
            ///////////////////////////////////////////////////////////////////////
            //Add Demand Register object.
            GXDLMSDemandRegister dr = new GXDLMSDemandRegister();
            dr.LogicalName = "1.0.31.4.0.255";
            dr.CurrentAverageValue = (uint)10;
            dr.LastAverageValue = (uint)20;
            dr.Status = (byte)1;
            dr.StartTimeCurrent = dr.CaptureTime = new GXDateTime(DateTime.Now);
            dr.Period = 10;
            dr.NumberOfPeriods = 1;
            Items.Add(dr);
            ///////////////////////////////////////////////////////////////////////
            //Add Register Monitor object.
            GXDLMSRegisterMonitor rm = new GXDLMSRegisterMonitor();
            rm.LogicalName = "0.0.16.1.0.255";
            rm.Thresholds = new object[] { (int)0x1234, (int)0x5678 };
            GXDLMSActionSet set = new GXDLMSActionSet();
            set.ActionDown.LogicalName = rm.LogicalName;
            set.ActionDown.ScriptSelector = 1;
            set.ActionUp.LogicalName = rm.LogicalName;
            set.ActionUp.ScriptSelector = 2;
            rm.Actions = new GXDLMSActionSet[] {
            set
            };
            rm.MonitoredValue.Update(r, 2);
            Items.Add(rm);
            ///////////////////////////////////////////////////////////////////////
            //Add Activate test mode Script table object.
            GXDLMSScriptTable st = new GXDLMSScriptTable("0.1.10.1.101.255");
            GXDLMSScript s = new GXDLMSScript();
            s.Id = 1;
            GXDLMSScriptAction a = new GXDLMSScriptAction();
            a.Target = null;

            s.Actions.Add(a);
            st.Scripts.Add(s);
            Items.Add(st);
            ///////////////////////////////////////////////////////////////////////
            //Add action schedule object.
            GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
            actionS.Target = st;
            actionS.ExecutedScriptSelector = 1;
            actionS.Type = SingleActionScheduleType.SingleActionScheduleType1;
            actionS.ExecutionTime = new GXDateTime[] { new GXDateTime(DateTime.Now) };
            Items.Add(actionS);
            ///////////////////////////////////////////////////////////////////////
            //Add SAP Assignment object.
            GXDLMSSapAssignment sap = new GXDLMSSapAssignment();
            sap.SapAssignmentList.Add(new KeyValuePair<UInt16, string>(1, "Gurux"));
            sap.SapAssignmentList.Add(new KeyValuePair<UInt16, string>(16, "Gurux-2"));
            Items.Add(sap);
            ///////////////////////////////////////////////////////////////////////
            //Add Auto Answer object.
            GXDLMSAutoAnswer aa = new GXDLMSAutoAnswer();
            aa.Mode = AutoAnswerMode.Connected;
            aa.ListeningWindow.Add(new KeyValuePair<GXDateTime, GXDateTime>(new GXDateTime(-1, -1, -1, 6, -1, -1, -1), new GXDateTime(-1, -1, -1, 8, -1, -1, -1)));
            aa.Status = AutoAnswerStatus.Inactive;
            aa.NumberOfCalls = 0;
            aa.NumberOfRingsInListeningWindow = 1;
            aa.NumberOfRingsOutListeningWindow = 2;
            Items.Add(aa);
            ///////////////////////////////////////////////////////////////////////
            //Add Modem Configuration object.
            GXDLMSModemConfiguration mc = new GXDLMSModemConfiguration();
            mc.CommunicationSpeed = BaudRate.Baudrate57600;
            GXDLMSModemInitialisation init = new GXDLMSModemInitialisation();
            init.Request = "AT";
            init.Response = "OK";
            init.Delay = 0;
            mc.InitialisationStrings = new GXDLMSModemInitialisation[] { init };
            Items.Add(mc);

            ///////////////////////////////////////////////////////////////////////
            //Add Mac Address Setup object.
            GXDLMSMacAddressSetup mac = new GXDLMSMacAddressSetup();
            mac.MacAddress = "00:11:22:33:44:55:66";
            Items.Add(mac);

            ///////////////////////////////////////////////////////////////////////
            //Add Image transfer object.
            GXDLMSImageTransfer i = new GXDLMSImageTransfer();
            Items.Add(i);
            ///////////////////////////////////////////////////////////////////////
            //Add IP4 Setup object.
            GXDLMSIp4Setup ip4 = new GXDLMSIp4Setup();
            Items.Add(ip4);

            ///////////////////////////////////////////////////////////////////////
            //Add IP6 Setup object.
            GXDLMSIp6Setup ip6 = new GXDLMSIp6Setup();

            Items.Add(ip6);

            //Get local IP address.
            var host = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> unicastIPAddress = new List<IPAddress>();
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip4.IPAddress = ip;
                }
                if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    unicastIPAddress.Add(ip);
                }
            }
            //This is just random IP6 address to show how to work.
            ip6.UnicastIPAddress = unicastIPAddress.ToArray();
            GXNeighborDiscoverySetup ss = new GXNeighborDiscoverySetup();
            ss.MaxRetry = 1;
            ss.RetryWaitTime = 2;
            ss.SendPeriod = 3;
            ip6.NeighborDiscoverySetup = new GXNeighborDiscoverySetup[] { ss };
            //Add Push Setup. (On Connectivity)
            GXDLMSPushSetup push = new GXDLMSPushSetup("0.0.25.9.0.255");
            //Send Push messages to this address as default.
            push.Destination = ip4.IPAddress + ":7000";
            Items.Add(push);
            //Add push object itself. This is needed to tell structure of data to the Push listener.
            push.PushObjectList.Add(new KeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(push, new GXDLMSCaptureObject(2, 0)));
            //Add logical device name.
            push.PushObjectList.Add(new KeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(ldn, new GXDLMSCaptureObject(2, 0)));
            //Add .0.0.25.1.0.255 Ch. 0 IPv4 setup IP address.
            push.PushObjectList.Add(new KeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(ip4, new GXDLMSCaptureObject(3, 0)));

            Items.Add(new GXDLMSSpecialDaysTable());
            //Add  S-FSK objects
            Items.Add(new GXDLMSSFSKPhyMacSetUp());
            Items.Add(new GXDLMSSFSKActiveInitiator());
            Items.Add(new GXDLMSSFSKMacCounters());
            Items.Add(new GXDLMSSFSKMacSynchronizationTimeouts());
            ///Add G3-PLC objects.
            Items.Add(new GXDLMSG3Plc6LoWPan());
            Items.Add(new GXDLMSG3PlcMacLayerCounters());
            Items.Add(new GXDLMSG3PlcMacSetup());

            Items.Add(new GXDLMSAccount());
            Items.Add(new GXDLMSCredit());
            Items.Add(new GXDLMSCharge());
            Items.Add(new GXDLMSTokenGateway());
            Items.Add(new GXDLMSCompactData());
            Items.Add(new GXDLMSDisconnectControl());

            Items.Add(new GXDLMSLlcSscsSetup());
            Items.Add(new GXDLMSPrimeNbOfdmPlcPhysicalLayerCounters());
            Items.Add(new GXDLMSPrimeNbOfdmPlcMacSetup());
            Items.Add(new GXDLMSPrimeNbOfdmPlcMacFunctionalParameters());
            Items.Add(new GXDLMSPrimeNbOfdmPlcMacCounters());
            Items.Add(new GXDLMSPrimeNbOfdmPlcMacNetworkAdministrationData());
            Items.Add(new GXDLMSPrimeNbOfdmPlcApplicationsIdentification());

            Items.Add(new GXDLMSSchedule());

            ///////////////////////////////////////////////////////////////////////
            //Server must initialize after all objects are added.
            Initialize();
        }

        void Init()
        {
            //KEK is used when authentication keys are updated.
            Kek = ASCIIEncoding.ASCII.GetBytes("1111111111111111");

            //If pre-established connections are used.
            ClientSystemTitle = ASCIIEncoding.ASCII.GetBytes("ABCDEFGH");
            Ciphering.Security = Security.AuthenticationEncryption;
            this.Conformance |= Conformance.GeneralBlockTransfer;

            Media.OnReceived += new Gurux.Common.ReceivedEventHandler(OnReceived);
            Media.OnClientConnected += new Gurux.Common.ClientConnectedEventHandler(OnClientConnected);
            Media.OnClientDisconnected += new Gurux.Common.ClientDisconnectedEventHandler(OnClientDisconnected);
            Media.OnError += new Gurux.Common.ErrorEventHandler(OnError);
            Media.Open();
        }

        public override void Close()
        {
            base.Close();
            Media.Close();
        }

        void OnError(object sender, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }

        /// <summary>
        /// Get head position where next new item is inserted.
        /// </summary>
        /// <remarks>
        /// This is used with ring buffer.
        /// </remarks>
        /// <returns>Position where next item is inserted.</returns>
        UInt32 GetHead()
        {
            UInt16 head = 0;
            DateTime last = DateTime.MinValue;
            lock (FileLock)
            {
                using (var fs = File.OpenRead(GetdataFile()))
                {
                    using (var reader = new StreamReader(fs))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (line.Length != 0)
                            {
                                string[] values = line.Split(';');
                                DateTime tm = DateTime.Parse(values[0], CultureInfo.InvariantCulture);
                                if (last > tm)
                                {
                                    break;
                                }
                                last = tm;
                                ++head;
                            }
                        }
                    }
                }
            }
            return head;
        }

        /// <summary>
        /// Return data using start and end indexes.
        /// </summary>
        /// <param name="p">ProfileGeneric</param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns>Add data Rows</returns>
        void GetProfileGenericDataByEntry(GXDLMSProfileGeneric p, UInt32 index, UInt32 count)
        {
            if (count != 0)
            {
                lock (FileLock)
                {
                    using (var fs = File.OpenRead(GetdataFile()))
                    {
                        using (var reader = new StreamReader(fs))
                        {
                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine();
                                if (line.Length != 0)
                                {
                                    //Skip row
                                    if (index > 0)
                                    {
                                        --index;
                                    }
                                    else
                                    {
                                        string[] values = line.Split(';');
                                        p.Buffer.Add(new object[] { DateTime.Parse(values[0], CultureInfo.InvariantCulture), int.Parse(values[1]) });
                                    }
                                    if (p.Buffer.Count == count)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                //Read values from the begin if ring buffer is used.
                if (p.Buffer.Count != count)
                {
                    GetProfileGenericDataByEntry(p, index, count);
                }
            }
        }

        /// <summary>
        /// Find start index and row count using start and end date time.
        /// </summary>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time</param>
        /// <param name="index">Start index.</param>
        /// <param name="count">Item count.</param>
        void GetProfileGenericDataByRangeFromRingBuffer(ValueEventArgs e)
        {
            GXDateTime start = (GXDateTime)GXDLMSClient.ChangeType((byte[])((List<object>)e.Parameters)[1], DataType.DateTime);
            GXDateTime end = (GXDateTime)GXDLMSClient.ChangeType((byte[])((List<object>)e.Parameters)[2], DataType.DateTime);
            uint pos = 0;
            DateTime last = DateTime.MinValue;
            lock (FileLock)
            {
                using (var fs = File.OpenRead(GetdataFile()))
                {
                    using (var reader = new StreamReader(fs))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (line.Length != 0)
                            {
                                string[] values = line.Split(';');
                                DateTime tm = DateTime.Parse(values[0], CultureInfo.InvariantCulture);
                                //If value is inside of start and end time.
                                if (tm >= start && tm <= end)
                                {
                                    if (last == DateTime.MinValue)
                                    {
                                        e.RowBeginIndex = pos;
                                        //Save end position if we have only one row.
                                        e.RowEndIndex = pos + 1;
                                    }
                                    else
                                    {
                                        if (tm > last)
                                        {
                                            e.RowEndIndex = pos + 1;
                                        }
                                        else
                                        {
                                            GXDLMSProfileGeneric p = (GXDLMSProfileGeneric)e.Target;
                                            if (e.RowEndIndex == 0)
                                            {
                                                ++e.RowEndIndex;
                                            }
                                            e.RowEndIndex += GetProfileGenericDataCount(p);
                                            e.RowBeginIndex = pos;
                                            break;
                                        }
                                    }
                                    last = tm;

                                }
                                ++pos;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find start index and row count using start and end date time.
        /// </summary>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time</param>
        /// <param name="index">Start index.</param>
        /// <param name="count">Item count.</param>
        void GetProfileGenericDataByRange(ValueEventArgs e)
        {
            GXDateTime start = (GXDateTime)GXDLMSClient.ChangeType((byte[])((List<object>)e.Parameters)[1], DataType.DateTime);
            GXDateTime end = (GXDateTime)GXDLMSClient.ChangeType((byte[])((List<object>)e.Parameters)[2], DataType.DateTime);
            lock (FileLock)
            {
                using (var fs = File.OpenRead(GetdataFile()))
                {
                    using (var reader = new StreamReader(fs))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (line.Length != 0)
                            {
                                string[] values = line.Split(';');
                                DateTime tm = DateTime.Parse(values[0], CultureInfo.InvariantCulture);
                                if (tm > end)
                                {
                                    //If all data is read.
                                    break;
                                }
                                if (tm < start)
                                {
                                    //If we have not find first item.
                                    ++e.RowBeginIndex;
                                }
                                ++e.RowEndIndex;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get row count.
        /// </summary>
        /// <returns></returns>
        UInt16 GetProfileGenericDataCount(GXDLMSProfileGeneric pg)
        {
            UInt16 rows = 0;
            lock (FileLock)
            {
                using (var fs = File.OpenRead(GetdataFile()))
                {
                    using (var reader = new StreamReader(fs))
                    {
                        while (!reader.EndOfStream)
                        {
                            if (reader.ReadLine().Length != 0)
                            {
                                ++rows;
                            }
                        }
                    }
                }
            }
            return rows;
        }

        void HandleClock(ValueEventArgs e)
        {
            GXDLMSClock c = e.Target as GXDLMSClock;
            if (e.Index == 2)
            {
                c.Time = c.Now();
            }
        }

        /// <summary>
        /// Generic read handle for all servers.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="e"></param>
        protected override void PreRead(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs e in args)
            {
                //Framework will handle Association objects automatically.
                if (e.Target is GXDLMSAssociationLogicalName ||
                        e.Target is GXDLMSAssociationShortName ||
                        e.Target is GXDLMSIp4Setup)
                {
                    continue;
                }
                //Framework will handle profile generic automatically.
                if (e.Target is GXDLMSProfileGeneric)
                {
                    //If buffer is read and we want to save memory.
                    if (e.Index == 7)
                    {
                        //If client wants to know EntriesInUse.
                        GXDLMSProfileGeneric p = (GXDLMSProfileGeneric)e.Target;
                        p.EntriesInUse = GetProfileGenericDataCount(p);
                    }
                    if (e.Index == 2)
                    {
                        //Client reads buffer.
                        GXDLMSProfileGeneric p = (GXDLMSProfileGeneric)e.Target;
                        //If reading first time.
                        if (e.RowEndIndex == 0)
                        {
                            if (e.Selector == 0)
                            {
                                e.RowEndIndex = GetProfileGenericDataCount(p);
                            }
                            else if (e.Selector == 1)
                            {
                                //Read by entry.
                                if (UseRingBuffer)
                                {
                                    GetProfileGenericDataByRangeFromRingBuffer(e);
                                }
                                else
                                {
                                    GetProfileGenericDataByRange(e);
                                }
                            }
                            else if (e.Selector == 2)
                            {
                                //Read by range.
                                e.RowBeginIndex = (UInt32)((List<object>)e.Parameters)[0] - 1;
                                e.RowEndIndex = (UInt32)((List<object>)e.Parameters)[1];
                                //If client wants to read more data what we have.
                                UInt16 cnt = GetProfileGenericDataCount(p);
                                if (e.RowEndIndex > cnt)
                                {
                                    if (UseRingBuffer)
                                    {
                                        e.RowEndIndex = cnt;
                                    }
                                    else
                                    {
                                        e.RowEndIndex = cnt - e.RowBeginIndex;
                                    }
                                    if (e.RowEndIndex < 0)
                                    {
                                        e.RowEndIndex = 0;
                                    }
                                }
                            }
                        }
                        UInt32 count = e.RowEndIndex - e.RowBeginIndex;
                        //Read only rows that can fit to one PDU.
                        if (e.RowToPdu != 0 && e.RowEndIndex - e.RowBeginIndex > e.RowToPdu)
                        {
                            count = e.RowToPdu;
                        }
                        //Clear old data. It's already serialized.
                        p.Buffer.Clear();
                        if (e.Selector == 1)
                        {
                            GetProfileGenericDataByEntry(p, e.RowBeginIndex, count);
                        }
                        else
                        {
                            //Index where to start.
                            UInt32 index = e.RowBeginIndex;
                            if (UseRingBuffer)
                            {
                                index += GetHead();
                            }
                            GetProfileGenericDataByEntry(p, index, count);
                            e.RowEndIndex -= e.RowBeginIndex;
                            e.RowBeginIndex = 0;
                        }
                    }
                    continue;
                }
                if (Trace > TraceLevel.Warning)
                {
                    Console.WriteLine(string.Format("Client Read value from {0} attribute: {1}.", e.Target.Name, e.Index));
                }
                if (e.Target is GXDLMSClock)
                {
                    HandleClock(e);
                }
                else if (e.Target.GetUIDataType(e.Index) == DataType.DateTime ||
                         e.Target.GetDataType(e.Index) == DataType.DateTime)
                {
                    e.Value = DateTime.Now;
                    e.Handled = true;
                }

                else if (e.Target is GXDLMSRegisterMonitor && e.Index == 2)
                {
                    //Update Register Monitor Thresholds values.
                    e.Value = new object[] { new Random().Next(0, 1000) };
                    e.Handled = true;
                }
                else
                {
                    //If data is not assigned and value type is unknown return number.
                    object[] values = e.Target.GetValues();
                    if (e.Index <= values.Length)
                    {
                        if (values[e.Index - 1] == null)
                        {
                            DataType tp = e.Target.GetDataType(e.Index);
                            if (tp == DataType.None || tp == DataType.Int8 || tp == DataType.Int16 ||
                                    tp == DataType.Int32 || tp == DataType.Int64 || tp == DataType.UInt8 || tp == DataType.UInt16 ||
                                    tp == DataType.UInt32 || tp == DataType.UInt64)
                            {
                                e.Value = new Random().Next(0, 1000);
                                e.Handled = true;
                            }
                            if (tp == DataType.String)
                            {
                                e.Value = "Gurux";
                                e.Handled = true;
                            }
                        }
                    }
                }
            }
        }

        protected override void PostRead(ValueEventArgs[] args)
        {

        }

        protected override void PreWrite(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs it in args)
            {
                System.Diagnostics.Debug.WriteLine("Writing " + it.Target.LogicalName);
            }
        }

        protected override void PostWrite(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs it in args)
            {
                System.Diagnostics.Debug.WriteLine("Writing " + it.Target.LogicalName);
            }
        }

        protected override void InvalidConnection(GXDLMSConnectionEventArgs e)
        {
        }

        void SendPush(GXDLMSPushSetup target)
        {
            int pos = target.Destination.IndexOf(':');
            if (pos == -1)
            {
                throw new ArgumentException("Invalid destination.");
            }
            byte[][] data = this.GeneratePushSetupMessages(DateTime.MinValue, target);
            string host = target.Destination.Substring(0, pos);
            int port = int.Parse(target.Destination.Substring(pos + 1));
            GXNet net = new GXNet(NetworkType.Tcp, host, port);
            try
            {
                net.Open();
                foreach (byte[] it in data)
                {
                    net.Send(it, null);
                }
            }
            finally
            {
                net.Close();
            }
        }

        protected override void PreAction(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs it in args)
            {
                System.Diagnostics.Debug.WriteLine("Action " + it.Target.LogicalName);
                if (it.Target is GXDLMSPushSetup && it.Index == 1)
                {
                    SendPush(it.Target as GXDLMSPushSetup);
                    it.Handled = true;
                }
                if (it.Target is GXDLMSImageTransfer)
                {
                    GXDLMSImageTransfer i = it.Target as GXDLMSImageTransfer;
                    //Image name and size to transfer
                    if (it.Index == 1)
                    {
                        i.ImageTransferStatus = ImageTransferStatus.NotInitiated;
                        i.ImageActivateInfo = null;
                        ImageUpdate = ASCIIEncoding.ASCII.GetString((byte[])(it.Parameters as List<object>)[0]);
                        ImageSize = Convert.ToUInt32((it.Parameters as List<object>)[1]);
                        string file = Path.Combine(Path.GetDirectoryName(typeof(GXDLMSBase).Assembly.Location), ImageUpdate + ".exe");
                        System.Diagnostics.Debug.WriteLine("Updating image" + ImageUpdate + " Size:" + ImageSize);
                        using (var writer = File.Create(file))
                        {
                        }
                    }
                    //Transfers one block of the Image to the server
                    else if (it.Index == 2)
                    {
                        i.ImageTransferStatus = ImageTransferStatus.TransferInitiated;
                        string file = Path.Combine(Path.GetDirectoryName(typeof(GXDLMSBase).Assembly.Location), ImageUpdate + ".exe");
                        List<object> p = (List<object>)it.Parameters;
                        try
                        {
                            using (FileStream fs = new FileStream(file, FileMode.Append))
                            {
                                using (BinaryWriter writer = new BinaryWriter(fs))
                                {
                                    writer.Write((byte[])p[1]);
                                }
                                fs.Close();
                            }
                        }
                        catch (System.IO.IOException)
                        {
                            Thread.Sleep(1000);
                            using (FileStream fs = new FileStream(file, FileMode.Append))
                            {
                                using (BinaryWriter writer = new BinaryWriter(fs))
                                {
                                    writer.Write((byte[])p[1]);
                                }
                                fs.Close();
                            }
                        }
                        imageActionStartTime = DateTime.Now;
                    }
                    //Verifies the integrity of the Image before activation.
                    else if (it.Index == 3)
                    {
                        string file = Path.Combine(Path.GetDirectoryName(typeof(GXDLMSBase).Assembly.Location), ImageUpdate + ".exe");
                        i.ImageTransferStatus = ImageTransferStatus.VerificationInitiated;
                        //Check that size match.
                        uint size = (uint)new FileInfo(file).Length;
                        if (size != ImageSize)
                        {
                            i.ImageTransferStatus = ImageTransferStatus.VerificationFailed;
                            it.Error = ErrorCode.OtherReason;
                        }
                        else
                        {
                            //Wait 10 seconds before image is verified.
                            if ((DateTime.Now - imageActionStartTime).TotalSeconds < 10)
                            {
                                Console.WriteLine("Image verification is on progress.");
                                it.Error = ErrorCode.TemporaryFailure;
                            }
                            else
                            {
                                Console.WriteLine("Image is verificated");
                                i.ImageTransferStatus = ImageTransferStatus.VerificationSuccessful;
                                imageActionStartTime = DateTime.Now;
                            }
                        }
                    }
                    //Activates the Image.
                    else if (it.Index == 4)
                    {
                        i.ImageTransferStatus = ImageTransferStatus.ActivationInitiated;
                        //Wait 10 seconds before image is verified.
                        if ((DateTime.Now - imageActionStartTime).TotalSeconds < 10)
                        {
                            Console.WriteLine("Image activation is on progress.");
                            it.Error = ErrorCode.TemporaryFailure;
                        }
                        else
                        {
                            Console.WriteLine("Image is activated.");
                            i.ImageTransferStatus = ImageTransferStatus.ActivationSuccessful;
                            imageActionStartTime = DateTime.Now;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Capture data to the ring buffer.
        /// </summary>
        /// <param name="pg"></param>
        private void CaptureToRingBuffer(GXDLMSProfileGeneric pg)
        {
            lock (FileLock)
            {
                StringBuilder sb = new StringBuilder();
                UInt32 head = GetHead();
                foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in pg.CaptureObjects)
                {
                    if (sb.Length != 0)
                    {
                        sb.Append(';');
                    }
                    object value;
                    if (it.Key is GXDLMSClock && it.Value.AttributeIndex == 2)
                    {
                        value = (it.Key as GXDLMSClock).Now();
                    }
                    else
                    {
                        // TODO: Read value here example from the meter if it's not
                        // updated automatically.
                        value = it.Key.GetValues()[it.Value.AttributeIndex - 1];
                        if (value == null)
                        {
                            // Generate random value here.
                            value = GetProfileGenericDataCount(pg) + 1 + head;
                        }
                    }
                    if (value is DateTime)
                    {
                        sb.Append(((DateTime)value).ToString(CultureInfo.InvariantCulture));
                    }
                    else if (value is GXDateTime)
                    {
                        sb.Append(((GXDateTime)value).Value.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        sb.Append(Convert.ToString(value));
                    }
                }
                //Read all rows and write them back. This is not elegant sulution, but it works.
                List<string> rows = new List<string>();
                rows.AddRange(File.ReadAllLines(GetdataFile()));
                if (pg.ProfileEntries == rows.Count)
                {
                    head = head % pg.ProfileEntries;
                    rows[(int)head] = sb.ToString();
                }
                else
                {
                    rows.Add(sb.ToString());
                }
                using (var writer = File.CreateText(GetdataFile()))
                {
                    foreach (string it in rows)
                    {
                        writer.WriteLine(it);
                    }
                }
            }
        }

        private void Capture(GXDLMSProfileGeneric pg)
        {
            lock (FileLock)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("");
                foreach (GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject> it in pg.CaptureObjects)
                {
                    if (sb.Length != 2)
                    {
                        sb.Append(';');
                    }
                    object value;
                    if (it.Key is GXDLMSClock && it.Value.AttributeIndex == 2)
                    {
                        value = (it.Key as GXDLMSClock).Now();
                    }
                    else
                    {
                        // TODO: Read value here example from the meter if it's not
                        // updated automatically.
                        value = it.Key.GetValues()[it.Value.AttributeIndex - 1];
                        if (value == null)
                        {
                            // Generate random value here.
                            value = GetProfileGenericDataCount(pg) + 1;
                        }
                    }
                    if (value is DateTime)
                    {
                        sb.Append(((DateTime)value).ToString(CultureInfo.InvariantCulture));
                    }
                    else if (value is GXDateTime)
                    {
                        sb.Append(((GXDateTime)value).Value.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        sb.Append(Convert.ToString(value));
                    }
                }
                using (var writer = File.AppendText(GetdataFile()))
                {
                    writer.Write(sb.ToString());
                }
            }
        }

        private void HandleProfileGenericActions(ValueEventArgs it)
        {
            GXDLMSProfileGeneric pg = (GXDLMSProfileGeneric)it.Target;
            if (it.Index == 1)
            {
                //Profile generic clear is called. Clear data.
                lock (FileLock)
                {
                    using (var fs = File.CreateText(GetdataFile()))
                    {
                    }
                }
            }
            else if (it.Index == 2)
            {
                //Profile generic Capture is called.
            }
        }

        protected override void PostAction(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs it in args)
            {
                if (it.Target is GXDLMSProfileGeneric)
                {
                    lock (it)
                    {
                        HandleProfileGenericActions(it);
                    }
                }
                if (it.Target is GXDLMSSecuritySetup && it.Index == 2)
                {
                    Console.WriteLine("----------------------------------------------------------");
                    Console.WriteLine("Updated keys:");
                    Console.WriteLine("Server System title: {0}", GXDLMSTranslator.ToHex(Ciphering.SystemTitle));
                    Console.WriteLine("Authentication key: {0}", GXDLMSTranslator.ToHex(Ciphering.AuthenticationKey));
                    Console.WriteLine("Block cipher key: {0}", GXDLMSTranslator.ToHex(Ciphering.BlockCipherKey));
                    Console.WriteLine("Client System title: {0}", GXDLMSTranslator.ToHex(ClientSystemTitle));
                    Console.WriteLine("Master key (KEK) title: {0}", GXDLMSTranslator.ToHex(Kek));
                }
            }
        }

        /// <summary>
        /// Our example server accept all connections.
        /// </summary>
        protected override bool IsTarget(int serverAddress, int clientAddress)
        {
            //Only one connection per meter at the time is allowed.
            if (AssignedAssociation != null)
            {
                return false;
            }
            bool ret = false;
            //Check HDLC station address if it's used.
            if (InterfaceType == InterfaceType.HDLC &&
                    Hdlc != null && Hdlc.DeviceAddress != 0)
            {
                ret = Hdlc.DeviceAddress == serverAddress;
            }
            // Check server address using serial number.
            if (!((serverAddress & 0x3FFF) == 0x3FFF || (serverAddress & 0x7F) == 0x7F ||
                (serverAddress & 0x3FFF) == serialNumber % 10000 + 1000))
            {
                // Find address from the SAP table.
                GXDLMSObjectCollection saps = Items.GetObjects(ObjectType.SapAssignment);
                if (saps.Count != 0)
                {
                    foreach (GXDLMSSapAssignment sap in saps)
                    {
                        if (sap.SapAssignmentList.Count == 0)
                        {
                            ret = true;
                            break;
                        }
                        foreach (KeyValuePair<UInt16, string> e in sap.SapAssignmentList)
                        {
                            // Check server address with two bytes.
                            if ((serverAddress & 0xFFFF0000) == 0 && (serverAddress & 0x7FFF) == e.Key)
                            {
                                ret = true;
                                break;
                            }
                            // Check server address with one byte.
                            if ((serverAddress & 0xFFFFFF00) == 0 && (serverAddress & 0x7F) == e.Key)
                            {
                                ret = true;
                                break;
                            }
                        }
                        if (ret)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    //Accept all server addresses if there is no SAP table available.
                    ret = true;
                }
            }
            if (ret)
            {
                AssignedAssociation = null;
                foreach (GXDLMSAssociationLogicalName it in Items.GetObjects(ObjectType.AssociationLogicalName))
                {
                    if (it.ClientSAP == clientAddress)
                    {
                        AssignedAssociation = it;
                        break;
                    }
                }
                if (AssignedAssociation == null)
                {
                    ret = false;
                }
            }
            return ret;
        }

        /// <summary>
        /// Get access rights for LN version #3.
        /// </summary>
        protected override AccessMode3 GetAttributeAccess3(ValueEventArgs arg)
        {
            if (AssignedAssociation == null)
            {
                return arg.Target.GetAccess3(arg.Index);
            }
            return AssignedAssociation.GetAccess3(arg.Target, arg.Index);
        }

        protected override AccessMode GetAttributeAccess(ValueEventArgs arg)
        {
            if (AssignedAssociation == null)
            {
                return arg.Target.GetAccess(arg.Index);
            }
            //Only read is allowed for None or Low authentications.
            if (AssignedAssociation.AuthenticationMechanismName.MechanismId == Authentication.None ||
                AssignedAssociation.AuthenticationMechanismName.MechanismId == Authentication.Low)
            {
                return AccessMode.Read;
            }
            return AssignedAssociation.GetAccess(arg.Target, arg.Index);
        }

        /// <summary>
        /// Get method access mode.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Method access mode</returns>
        protected override MethodAccessMode GetMethodAccess(ValueEventArgs arg)
        {
            //Invoke is called when high level authentication is used.
            if (AssignedAssociation == null)
            {
                return arg.Target.GetMethodAccess(arg.Index);
            }
            //Actions are not allowed for None or Low authentications.
            if (AssignedAssociation.AuthenticationMechanismName.MechanismId == Authentication.None ||
                AssignedAssociation.AuthenticationMechanismName.MechanismId == Authentication.Low)
            {
                return MethodAccessMode.NoAccess;
            }
            return AssignedAssociation.GetMethodAccess(arg.Target, arg.Index);
        }

        /// <summary>
        /// Get method access mode.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Method access mode</returns>
        protected override MethodAccessMode3 GetMethodAccess3(ValueEventArgs arg)
        {
            //Invoke is called when high level authentication is used.
            if (AssignedAssociation == null)
            {
                return arg.Target.GetMethodAccess3(arg.Index);
            }
            //Actions are not allowed for None or Low authentications.
            if (AssignedAssociation.AuthenticationMechanismName.MechanismId == Authentication.None ||
                AssignedAssociation.AuthenticationMechanismName.MechanismId == Authentication.Low)
            {
                return MethodAccessMode3.NoAccess;
            }
            return AssignedAssociation.GetMethodAccess3(arg.Target, arg.Index);
        }

        /// <summary>
        /// Our example server accept all authentications.
        /// </summary>
        protected override SourceDiagnostic ValidateAuthentication(Authentication authentication, byte[] password)
        {
            //If low authentication fails.
            if (authentication == Authentication.Low)
            {
                byte[] expected;
                if (UseLogicalNameReferencing)
                {
                    GXDLMSAssociationLogicalName ln = AssignedAssociation;
                    if (ln == null)
                    {
                        ln = (GXDLMSAssociationLogicalName)Items.FindByLN(ObjectType.AssociationLogicalName, "0.0.40.0.0.255");
                    }
                    ln.AuthenticationMechanismName.MechanismId = authentication;
                    ln.AssociationStatus = AssociationStatus.Associated;
                    expected = ln.Secret;
                }
                else
                {
                    GXDLMSAssociationShortName sn = (GXDLMSAssociationShortName)Items.FindByLN(ObjectType.AssociationShortName, "0.0.40.0.0.255");
                    expected = sn.Secret;
                }
                if (Gurux.Common.GXCommon.EqualBytes(expected, password))
                {
                    return SourceDiagnostic.None;
                }
                return SourceDiagnostic.AuthenticationFailure;
            }
            if (UseLogicalNameReferencing)
            {
                GXDLMSAssociationLogicalName ln = AssignedAssociation;
                if (ln == null)
                {
                    ln = (GXDLMSAssociationLogicalName)Items.FindByLN(ObjectType.AssociationLogicalName, "0.0.40.0.0.255");
                }
                if (ln != null)
                {
                    ln.AuthenticationMechanismName.MechanismId = authentication;
                    ln.AssociationStatus = AssociationStatus.AssociationPending;
                }
            }
            //Other authentication levels are check later.
            return SourceDiagnostic.None;
        }

        /// <summary>
        /// All objects are static in our example.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="sn"></param>
        /// <param name="ln"></param>
        /// <returns></returns>
        protected override GXDLMSObject FindObject(ObjectType objectType, int sn, string ln)
        {
            return null;
        }


        /// <summary>
        /// Client has close connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClientDisconnected(object sender, Gurux.Common.ConnectionEventArgs e)
        {
            if (Trace > TraceLevel.Warning)
            {
                Console.WriteLine("Client Disconnected.");
            }
            if (connections.ContainsKey(e.Info))
            {
                connections.Remove(e.Info);
            }
        }

        /// <summary>
        /// Client has made connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClientConnected(object sender, Gurux.Common.ConnectionEventArgs e)
        {
            if (connections.ContainsKey(e.Info))
            {
                connections[e.Info].Reset();
            }
            connections[e.Info] = this;

            //Reset server settings when connection is established.
            //This is mandatory if Wrapper is used.
            this.Reset();
            if (Trace > TraceLevel.Warning)
            {
                Console.WriteLine("Client Connected.");
            }
        }

        /// <summary>
        /// Client has send data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnReceived(object sender, Gurux.Common.ReceiveEventArgs e)
        {
            try
            {
                lock (this)
                {
                    if (Trace > TraceLevel.Info)
                    {
                        Console.WriteLine("RX:\t" + Gurux.Common.GXCommon.ToHex((byte[])e.Data, true));
                    }
                    GXServerReply sr = new GXServerReply((byte[])e.Data);
                    do
                    {
                        HandleRequest(sr);
                        //Reply is null if we do not want to send any data to the client.
                        //This is done if client try to make connection with wrong device ID.
                        if (sr.Reply != null)
                        {
                            if (Trace > TraceLevel.Info)
                            {
                                Console.WriteLine("TX:\t" + Gurux.Common.GXCommon.ToHex(sr.Reply, true));
                            }
                            Media.Send(sr.Reply, e.SenderInfo);
                            sr.Data = null;
                        }
                    }
                    while (sr.IsStreaming);
                }
            }
            catch (Exception ex)
            {
                if (Trace > TraceLevel.Off)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Execute selected actions
        /// </summary>
        /// <param name="actions">List of actions to execute.</param>
        protected override void Execute(List<KeyValuePair<GXDLMSObject, int>> actions)
        {
            foreach (var it in actions)
            {
                Console.WriteLine(DateTime.Now + " Executing: " + it.Key.ObjectType + " " + it.Key);
            }
        }

        /// <summary>
        /// Generate random value for profile generic.
        /// </summary>
        /// <param name="args"></param>
        public override void PreGet(ValueEventArgs[] args)
        {
            foreach (ValueEventArgs it in args)
            {
                if (it.Target is GXDLMSProfileGeneric)
                {
                    GXDLMSProfileGeneric pg = (GXDLMSProfileGeneric)it.Target;
                    if (UseRingBuffer)
                    {
                        CaptureToRingBuffer(pg);
                    }
                    else
                    {
                        Capture(pg);
                    }
                }
            }
        }

        public override void PostGet(ValueEventArgs[] args)
        {
        }

        protected override void Connected(GXDLMSConnectionEventArgs e)
        {
            if (Trace > TraceLevel.Warning)
            {
                Console.WriteLine("Connected.");
            }
        }

        protected override void Disconnected(GXDLMSConnectionEventArgs e)
        {
            if (Trace > TraceLevel.Warning)
            {
                Console.WriteLine("Disconnected");
            }
        }
    }
}
