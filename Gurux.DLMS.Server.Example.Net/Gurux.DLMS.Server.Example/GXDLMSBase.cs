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
using Gurux.DLMS.Objects;
using Gurux.DLMS;
using Gurux.Net;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects.Enums;

namespace GuruxDLMSServerExample
{
    /// <summary>
    /// All example servers are using same objects.
    /// </summary>
    class GXDLMSBase : GXDLMSServer
    {
        public GXDLMSBase(bool logicalNameReferencing, InterfaceType type)
            : base(logicalNameReferencing, type)
        {

        }

        Gurux.Common.IGXMedia Media = null;

        public void Initialize(string port)
        {
            Media = new Gurux.Serial.GXSerial(port);
            Init();
        }
        /// <summary>
        /// Generic initialize for all servers.
        /// </summary>
        /// <param name="server"></param>
        public void Initialize(int port)
        {
            Media = new GXNet(NetworkType.Tcp, port);            
            Init();
        }

        void Init()
        {
            Media.OnReceived += new Gurux.Common.ReceivedEventHandler(OnReceived);
            Media.OnClientConnected += new Gurux.Common.ClientConnectedEventHandler(OnClientConnected);
            Media.OnClientDisconnected += new Gurux.Common.ClientDisconnectedEventHandler(OnClientDisconnected);
            Media.OnError += new Gurux.Common.ErrorEventHandler(OnError);
            Media.Open();
            ///////////////////////////////////////////////////////////////////////
            //Add Logical Device Name. 123456 is meter serial number.
            GXDLMSData d = new GXDLMSData("0.0.42.0.0.255");
            d.Value = "Gurux123456";
            //Set access right. Client can't change Device name.
            d.SetAccess(2, AccessMode.ReadWrite);
            //Value is get as Octet String.
            d.SetDataType(2, DataType.OctetString);
            d.SetUIDataType(2, DataType.String);
            Items.Add(d);
            //Add Last average.
            GXDLMSRegister r = new GXDLMSRegister("1.1.21.25.0.255");
            //Set access right. Client can't change average value.
            r.SetAccess(2, AccessMode.Read);
            Items.Add(r);
            //Add default clock. Clock's Logical Name is 0.0.1.0.0.255.
            GXDLMSClock clock = new GXDLMSClock();
            clock.Begin = new GXDateTime(-1, 9, 1, -1, -1, -1, -1);
            clock.End = new GXDateTime(-1, 3, 1, -1, -1, -1, -1);
            clock.Deviation = 0;
            Items.Add(clock);
            //Add Tcp Udp setup. Default Logical Name is 0.0.25.0.0.255.
            GXDLMSTcpUdpSetup tcp = new GXDLMSTcpUdpSetup();
            Items.Add(tcp);
            ///////////////////////////////////////////////////////////////////////
            //Add Load profile.
            GXDLMSProfileGeneric pg = new GXDLMSProfileGeneric("1.0.99.1.0.255");
            //Set capture period to 60 second.
            pg.CapturePeriod = 60;
            //Maximum row count.
            pg.ProfileEntries = 100;
            pg.SortMethod = SortMethod.FiFo;
            pg.SortObject = clock;
            //Add columns.
            //Set saved attribute index.
            pg.CaptureObjects.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(clock, new GXDLMSCaptureObject(2, 0)));
            //Set saved attribute index.
            pg.CaptureObjects.Add(new GXKeyValuePair<GXDLMSObject, GXDLMSCaptureObject>(r, new GXDLMSCaptureObject(2, 0)));
            Items.Add(pg);
            //Add initial rows.
            pg.Buffer.Add(new object[] { DateTime.Now, (int)10 });
            ///////////////////////////////////////////////////////////////////////
            //Add Auto connect object.
            GXDLMSAutoConnect ac = new GXDLMSAutoConnect();
            ac.Mode = AutoConnectMode.AutoDiallingAllowedAnytime;
            ac.Repetitions = 10;
            ac.RepetitionDelay = 60;
            //Calling is allowed between 1am to 6am.
            ac.CallingWindow.Add(new KeyValuePair<GXDateTime, GXDateTime>(new GXDateTime(-1, -1, -1, 1, 0, 0, -1), new GXDateTime(-1, -1, -1, 6, 0, 0, -1)));
            ac.Destinations = new string[] { "www.gurux.org" };
            Items.Add(ac);
            ///////////////////////////////////////////////////////////////////////
            //Add Activity Calendar object.
            GXDLMSActivityCalendar activity = new GXDLMSActivityCalendar();
            activity.CalendarNameActive = "Active";
            activity.SeasonProfileActive = new GXDLMSSeasonProfile[] { new GXDLMSSeasonProfile("Summer time", new GXDateTime(-1, 3, 31, -1, -1, -1, -1), "") };
            activity.WeekProfileTableActive = new GXDLMSWeekProfile[] { new GXDLMSWeekProfile("Monday", 1, 1, 1, 1, 1, 1, 1) };
            activity.DayProfileTableActive = new GXDLMSDayProfile[] { new GXDLMSDayProfile(1, new GXDLMSDayProfileAction[] { new GXDLMSDayProfileAction(new GXDateTime(DateTime.Now), "test", 1) }) };
            activity.CalendarNamePassive = "Passive";
            activity.SeasonProfilePassive = new GXDLMSSeasonProfile[] { new GXDLMSSeasonProfile("Winter time", new GXDateTime(-1, 10, 30, -1, -1, -1, -1), "") };
            activity.WeekProfileTablePassive = new GXDLMSWeekProfile[] { new GXDLMSWeekProfile("Tuesday", 1, 1, 1, 1, 1, 1, 1) };
            activity.DayProfileTablePassive = new GXDLMSDayProfile[] { new GXDLMSDayProfile(1, new GXDLMSDayProfileAction[] { new GXDLMSDayProfileAction(new GXDateTime(DateTime.Now), "0.0.1.0.0.255", 1) }) };
            activity.Time = new GXDateTime(DateTime.Now);
            Items.Add(activity);
            ///////////////////////////////////////////////////////////////////////
            //Add Optical Port Setup object.
            GXDLMSIECOpticalPortSetup optical = new GXDLMSIECOpticalPortSetup();
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
            dr.LogicalName = "0.0.1.0.0.255";
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
            rm.LogicalName = "0.0.1.0.0.255";
            rm.Thresholds = new object[] { (int)0x1234, (int)0x5678 };
            GXDLMSActionSet set = new GXDLMSActionSet();
            set.ActionDown.LogicalName = rm.LogicalName;
            set.ActionDown.ScriptSelector = 1;
            set.ActionUp.LogicalName = rm.LogicalName;
            set.ActionUp.ScriptSelector = 2;
            rm.Actions = new GXDLMSActionSet[] { set };
            rm.MonitoredValue.Update(r, 2);
            Items.Add(rm);
            ///////////////////////////////////////////////////////////////////////
            //Add action schedule object.
            GXDLMSActionSchedule actionS = new GXDLMSActionSchedule();
            actionS.LogicalName = "0.0.1.0.0.255";
            actionS.ExecutedScriptLogicalName = "1.2.3.4.5.6";
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
            aa.Mode = AutoConnectMode.EmailSending;
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

            GXDLMSImageTransfer i = new GXDLMSImageTransfer();
            Items.Add(i);
            ///////////////////////////////////////////////////////////////////////
            //Server must initialize after all objects are added.
            Initialize();
        }

        void OnError(object sender, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }        

        /// <summary>
        /// Generic read handle for all servers.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="e"></param>
        public override void Read(ValueEventArgs e)
        {
            //Framework will handle Association objects automatically.
            if (e.Target is GXDLMSAssociationLogicalName ||
                e.Target is GXDLMSAssociationShortName ||
                //Framework will handle profile generic automatically.
                e.Target is GXDLMSProfileGeneric)
            {
                return;
            }            
            Console.WriteLine(string.Format("Client Read value from {0} attribute: {1}.", e.Target.Name, e.Index));
            if (e.Target.GetUIDataType(e.Index) == DataType.DateTime ||
                e.Target.GetDataType(e.Index) == DataType.DateTime)
            {
                e.Value = DateTime.Now;
                e.Handled = true;
            }
            else if (e.Target is GXDLMSClock)
            {
                //Implement specific clock handling here.
                //Otherwise initial values are used.                
            }
            else if (e.Target is GXDLMSRegisterMonitor && e.Index == 2)
            {
                //Update Register Monitor Thresholds values.
                e.Value = new object[]{new Random().Next(0, 1000)};
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

        public override void Write(ValueEventArgs e)
        {
        }

        public override void InvalidConnection(ConnectionEventArgs e)
        {
        }

        public override void Action(ValueEventArgs e)
        {
        }

        /// <summary>
        /// Our example server accept all connections.
        /// </summary>
        public override bool IsTarget(int serverAddress, int clientAddress)
        {
            return true;
        }

        /// <summary>
        /// Our example server accept all authentications.        
        /// </summary>
        public override SourceDiagnostic ValidateAuthentication(Authentication authentication, byte[] password)
        {
            return SourceDiagnostic.None;
        }

        /// <summary>
        /// All objects are static in our example.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="sn"></param>
        /// <param name="ln"></param>
        /// <returns></returns>
        public override GXDLMSObject FindObject(ObjectType objectType, int sn, string ln)
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
            //Reset server settings when connection closed.
            this.Reset();
            Console.WriteLine("Client Disconnected.");
        }

        /// <summary>
        /// Client has made connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClientConnected(object sender, Gurux.Common.ConnectionEventArgs e)
        {
            Console.WriteLine("Client Connected.");
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
                    Console.WriteLine("<- " + Gurux.Common.GXCommon.ToHex((byte[])e.Data, true));
                    byte[] reply = HandleRequest((byte[])e.Data);
                    //Reply is null if we do not want to send any data to the client.
                    //This is done if client try to make connection with wrong device ID.
                    if (reply != null)
                    {
                        Console.WriteLine("-> " + Gurux.Common.GXCommon.ToHex(reply, true));
                        Media.Send(reply, e.SenderInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }       
    }
}
