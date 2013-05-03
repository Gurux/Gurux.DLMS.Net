using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.DLMS.Objects;
using Gurux.DLMS;
using Gurux.Net;

namespace GuruxDLMSServerExample
{
    /// <summary>
    /// All example servers are using same objects.
    /// </summary>
    class GXDLMSBase : GXDLMSServerBase
    {
        public GXDLMSBase(bool logicalNameReferencing) : base(logicalNameReferencing)
        {

        }

        GXNet Media = null;
        /// <summary>
        /// Generic initialize for all servers.
        /// </summary>
        /// <param name="server"></param>
        public void Initialize(int port)
        {
            Media = new GXNet(NetworkType.Tcp, port);
            Media.OnReceived += new Gurux.Common.ReceivedEventHandler(OnReceived);
            Media.OnClientConnected += new Gurux.Common.ClientConnectedEventHandler(OnClientConnected);
            Media.OnClientDisconnected += new Gurux.Common.ClientDisconnectedEventHandler(OnClientDisconnected);
            Media.Open();
            ///////////////////////////////////////////////////////////////////////
            //Add Logical Device Name. 123456 is meter serial number.
            GXDLMSData d = new GXDLMSData("0.0.42.0.0.255");
            d.Value = "Gurux123456";
            //Set access right. Client can't change Device name.
            d.SetAccess(2, AccessMode.Read);
            Items.Add(d);
            //Add Last avarage.
            GXDLMSRegister r = new GXDLMSRegister("1.1.21.25.0.255");
            //Set access right. Client can't change average value.
            r.SetAccess(2, AccessMode.Read);
            Items.Add(r);
            //Add default clock. Clock's Logical Name is 0.0.1.0.0.255.
            GXDLMSClock clock = new GXDLMSClock();
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
            //Add colums.
            //Set saved attribute index.
            clock.SelectedAttributeIndex = 2;
            pg.CaptureObjects.Add(clock);
            //Set saved attribute index.
            r.SelectedAttributeIndex = 2;
            pg.CaptureObjects.Add(r);
            Items.Add(pg);             
            ///////////////////////////////////////////////////////////////////////
            //Server must initialize after all objects are added.
            Initialize();
            //Add rows after Initialize.
            pg.Buffer.Columns[0].DataType = typeof(DateTime);
            pg.Buffer.Columns[1].DataType = typeof(int);
            //pg.Buffer.Rows.Add(new object[] { DateTime.Now, (int)10 });

            Object[] row = new Object[] { DateTime.Now, 10 };
            Object[][] rows = new Object[140][];
            for (int pos = 0; pos != 140; ++pos)
            {
                rows[pos] = row;
                pg.Buffer.Rows.Add(row);
            }            
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
                //Implement spesific clock handling here.
                //Otherwice initial values are used.                
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

        public override void UpdateItems()
        {
        }

        public override void InvalidConnection(ConnectionEventArgs e)
        {
        }

        public override void Action(ValueEventArgs e)
        {
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
                    byte[] reply = HandleRequest((byte[])e.Data);
                    //Reply is null if we do not want to send any data to the client.
                    //This is done if client try to make connection with wrong device ID.
                    if (reply != null)
                    {
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
