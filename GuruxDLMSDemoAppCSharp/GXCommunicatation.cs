using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.Common;
using Gurux.DLMS;
using Gurux.Net;
using Gurux.Serial;
using System.IO.Ports;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Objects;

namespace GuruxDLMSDemoAppCSharp
{
    class GXCommunicatation
    {
        public bool Trace = false;
        Gurux.DLMS.GXDLMSClient m_Parser;
        int WaitTime = 5000;
        IGXMedia Media;
        bool InitializeIEC;
        GXManufacturer Manufacturer;
        HDLCAddressType HDLCAddressing;        

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXCommunicatation(Gurux.DLMS.GXDLMSClient dlms, IGXMedia media, bool initializeIEC, Gurux.DLMS.Authentication authentication, string password)
        {
            m_Parser = dlms;
            Media = media;
            InitializeIEC = initializeIEC;
            m_Parser.Authentication = authentication;
            m_Parser.Password = password;
        }

        public void Close()
        {
            if (Media != null && m_Parser != null)
            {
                try
                {
                    Console.WriteLine("Disconnecting from the meter.");
                    ReadDLMSPacket(m_Parser.DisconnectRequest());
                    Media.Close();
                }
                catch
                {

                }
                Media = null;
                m_Parser = null;
            }
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        /// <remarks>
        /// Close connection to the media and send disconnect message.
        /// </remarks>
        ~GXCommunicatation()
        {
            Close();
        }

        public void UpdateManufactureSettings(string id)
        {
            if (Manufacturer != null && string.Compare(Manufacturer.Identification, id, true) != 0)
            {
                throw new Exception(string.Format("Manufacturer type does not match. Manufacturer is {0} and it should be {1}.", id, Manufacturer.Identification));
            }            
            m_Parser.InterfaceType = InterfaceType.General;
            m_Parser.UseLogicalNameReferencing = Manufacturer.UseLogicalNameReferencing;
            //If network media is used check is manufacturer supporting IEC 62056-47
            GXServerAddress server = Manufacturer.GetServer(HDLCAddressing);
            if (this.Media is GXNet && Manufacturer.UseIEC47)
            {
                m_Parser.InterfaceType = InterfaceType.Net;
                m_Parser.ClientID = Convert.ToUInt16(Manufacturer.GetAuthentication(m_Parser.Authentication).ClientID);
                m_Parser.ServerID = Convert.ToUInt16(server.PhysicalAddress);
            }
            else
            {
                if (HDLCAddressing == HDLCAddressType.Custom)
                {
                    m_Parser.ClientID = Manufacturer.GetAuthentication(m_Parser.Authentication).ClientID;
                }
                else
                {
                    m_Parser.ClientID = (byte)(Convert.ToByte(Manufacturer.GetAuthentication(m_Parser.Authentication).ClientID) << 1 | 0x1);
                }                                
                m_Parser.ServerID = GXManufacturer.CountServerAddress(HDLCAddressing, server.Formula, server.PhysicalAddress, server.LogicalAddress);
            }
        }

        void InitSerial()
        {
            GXSerial serial = Media as GXSerial;
            byte Terminator = (byte)0x0A;
            if (serial != null && InitializeIEC)
            {
                serial.BaudRate = 300;
                serial.DataBits = 7;
                serial.Parity = Parity.Even;
                serial.StopBits = StopBits.One;
            }
            Media.Open();
            //Query device information.
            if (Media != null && InitializeIEC)
            {
                string data = "/?!\r\n";
                if (Trace)
                {
                    Console.WriteLine("HDLC sending:" + data);
                }
                ReceiveParameters<string> p = new ReceiveParameters<string>()
                {
                    Eop = Terminator,
                    WaitTime = WaitTime
                };
                lock (Media.Synchronous)
                {
                    Media.Send(data, null);
                    if (!Media.Receive(p))
                    {
                        //Try to move away from mode E.                        
                        throw new Exception("Failed to receive reply from the device in given time.");
                    }
                    //If echo is used.
                    if (p.Reply == data)
                    {
                        p.Reply = null;
                        if (!Media.Receive(p))
                        {
                            //Try to move away from mode E.                            
                            throw new Exception("Failed to receive reply from the device in given time.");
                        }
                    }
                }                
                if (Trace)
                {
                    Console.WriteLine("HDLC received: " + p.Reply);
                }
                if (p.Reply[0] != '/')
                {
                    p.WaitTime = 100;
                    Media.Receive(p);
                    throw new Exception("Invalid responce.");
                }
                string manufactureID = p.Reply.Substring(1, 3);
                //UpdateManufactureSettings(manufactureID);
                char baudrate = p.Reply[4];
                int BaudRate = 0;
                switch (baudrate)
                {
                    case '0':
                        BaudRate = 300;
                        break;
                    case '1':
                        BaudRate = 600;
                        break;
                    case '2':
                        BaudRate = 1200;
                        break;
                    case '3':
                        BaudRate = 2400;
                        break;
                    case '4':
                        BaudRate = 4800;
                        break;
                    case '5':
                        BaudRate = 9600;
                        break;
                    case '6':
                        BaudRate = 19200;
                        break;
                    default:
                        throw new Exception("Unknown baud rate.");
                }
                Console.WriteLine("BaudRate is :", BaudRate.ToString());
                //Send ACK
                //Send Protocol control character
                byte controlCharacter = (byte)'2';// "2" HDLC protocol procedure (Mode E)
                //Send Baudrate character
                //Mode control character 
                byte ModeControlCharacter = (byte)'2';//"2" //(HDLC protocol procedure) (Binary mode)
                //Set mode E.
                byte[] arr = new byte[] { 0x06, controlCharacter, (byte)baudrate, ModeControlCharacter, 13, 10 };
                if (Trace)
                {
                    Console.WriteLine("Moving to mode E", BitConverter.ToString(arr));
                }
                lock (Media.Synchronous)
                {
                    Media.Send(arr, null);
                    p.Reply = null;
                    p.WaitTime = 500;
                    if (!Media.Receive(p))
                    {
                        //Try to move away from mode E.
                        this.ReadDLMSPacket(m_Parser.DisconnectRequest());                        
                        throw new Exception("Failed to receive reply from the device in given time.");
                    }
                }
                if (serial != null)
                {
                    serial.BaudRate = BaudRate;
                    serial.DataBits = 8;
                    serial.Parity = Parity.None;
                    serial.StopBits = StopBits.One;
                }
            }
        }

        void InitNet()
        {
            Media.Open();
        }

        public void InitializeConnection(GXManufacturer man)
        {
            Manufacturer = man;
            UpdateManufactureSettings(man.Identification);
            if (Media is GXSerial)
            {
                Console.WriteLine("Initializing serial connection.");
                InitSerial();                
            }
            else if (Media is GXNet)
            {
                Console.WriteLine("Initializing Network connection.");
                InitNet();
                //Some Electricity meters need some time before first message can be send.
                System.Threading.Thread.Sleep(500);
            }
            else
            {                
                throw new Exception("Unknown media type.");
            }
            byte[] data, reply = null;
            data = m_Parser.SNRMRequest();
            if (data != null)
            {
                if (Trace)
                {
                    Console.WriteLine("Send SNRM request." + BitConverter.ToString(data));
                }
                reply = ReadDLMSPacket(data);
                if (Trace)
                {
                    Console.WriteLine("Parsing UA reply." + BitConverter.ToString(reply));
                }
                //Has server accepted client.
                m_Parser.ParseUAResponse(reply);
                Console.WriteLine("Parsing UA reply succeeded.");
            }
            //Generate AARQ request.
            //Split requests to multible packets if needed. 
            //If password is used all data might not fit to one packet.
            foreach (byte[] it in m_Parser.AARQRequest(null))
            {
                if (Trace)
                {
                    Console.WriteLine("Send AARQ request", BitConverter.ToString(data));
                }
                reply = ReadDLMSPacket(it);
            }
            if (Trace)
            {
                Console.WriteLine("Parsing AARE reply" + BitConverter.ToString(reply));
            }
            //Parse reply.
            m_Parser.ParseAAREResponse(reply);
            Console.WriteLine("Parsing AARE reply succeeded.");
        }

        /// <summary>
        /// Read attribute value.
        /// </summary>
        public object Read(GXDLMSObject it, int attributeIndex)
        {
            byte[] reply = ReadDataBlock(m_Parser.Read(it.Name, it.ObjectType, attributeIndex)[0]);
            return m_Parser.GetValue(reply, it.ObjectType, it.LogicalName, attributeIndex);
        }
        
        /// <summary>
        /// Read Profile Generic Columns.
        /// </summary>
        public GXDLMSObjectCollection GetColumns(GXDLMSObject it)
        {
            byte[] reply = ReadDataBlock(m_Parser.Read(it.Name, it.ObjectType, 3)[0]);
            return m_Parser.ParseColumns(reply);
        }

        /// <summary>
        /// Read Profile Generic Columns by entry.
        /// </summary>
        public object[] ReadRowsByEntry(GXDLMSObject it, int index, int count, GXDLMSObjectCollection columns)
        {
            byte[] reply = ReadDataBlock(m_Parser.ReadRowsByEntry(it.Name, index, count));
            object[] rows = (object[])m_Parser.GetValue(reply);
            if (columns != null && rows.Length != 0 && m_Parser.ObisCodes.Count != 0)
            {
                Array row = (Array) rows[0];
                if (columns.Count != row.Length)
                {
                    throw new Exception("Columns count do not mach.");
                }
                for (int pos = 0; pos != columns.Count; ++pos)
                {
                    if (row.GetValue(pos) is byte[])
                    {
                        DataType type = DataType.None;
                        //Find Column type
                        GXObisCode code = m_Parser.ObisCodes.FindByLN(columns[pos].ObjectType, columns[pos].LogicalName, null);
                        if (code != null)
                        {
                            GXDLMSAttributeSettings att = code.Attributes.Find(columns[pos].SelectedAttributeIndex);
                            if (att != null)
                            {
                                type = att.UIType;
                            }
                        }
                        foreach (object[] cell in rows)
                        {
                            cell[pos] = GXDLMSClient.ChangeType((byte[])cell[pos], type);
                        }
                    }
                }
            }
            return rows;
        }

        /// <summary>
        /// Read Profile Generic Columns by range.
        /// </summary>
        public object[] ReadRowsByRange(GXDLMSObject it, DateTime start, DateTime end, GXDLMSObjectCollection columns)
        {
            GXDLMSObject col = columns[0];
            byte[] reply = ReadDataBlock(m_Parser.ReadRowsByRange(it.Name, col.LogicalName, col.ObjectType, col.Version, start, end));
            object[] rows = (object[])m_Parser.GetValue(reply);
            if (columns != null && rows.Length != 0 && m_Parser.ObisCodes.Count != 0)
            {
                Array row = (Array)rows[0];
                if (columns.Count != row.Length)
                {
                    throw new Exception("Columns count do not mach.");
                }
                for (int pos = 0; pos != columns.Count; ++pos)
                {
                    if (row.GetValue(pos) is byte[])
                    {
                        DataType type = DataType.None;
                        //Find Column type
                        GXObisCode code = m_Parser.ObisCodes.FindByLN(columns[pos].ObjectType, columns[pos].LogicalName, null);
                        if (code != null)
                        {
                            GXDLMSAttributeSettings att = code.Attributes.Find(columns[pos].SelectedAttributeIndex);
                            if (att != null)
                            {
                                type = att.UIType;
                            }
                        }
                        foreach (object[] cell in rows)
                        {
                            cell[pos] = GXDLMSClient.ChangeType((byte[])cell[pos], type);
                        }
                    }
                }
            }
            return rows;
        }

        /// <summary>
        /// Read data type of selected attribute index.
        /// </summary>
        public DataType GetDLMSDataType(GXDLMSObject it, int attributeIndex)
        {
            byte[] reply = ReadDataBlock(m_Parser.Read(it.Name, it.ObjectType, attributeIndex)[0]);
            return m_Parser.GetDLMSDataType(reply);
        }        

        /// <summary>
        /// Read Association View from the meter.
        /// </summary>
        public GXDLMSObjectCollection GetAssociationView()
        {
            byte[] reply = ReadDataBlock(m_Parser.GetObjects());
            return m_Parser.ParseObjects(reply, true);
        }

        /// <summary>
        /// Read DLMS Data from the device.
        /// </summary>
        /// <remarks>
        /// If access is denied return null.
        /// </remarks>
        /// <param name="data">Data to send.</param>
        /// <returns>Received data.</returns>
        public byte[] ReadDLMSPacket(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            object eop = (byte)0x7E;
            //In network connection terminator is not used.
            if (m_Parser.InterfaceType == InterfaceType.Net && Media is GXNet)
            {
                eop = null;
            }
            int pos = 0;
            bool succeeded = false;
            ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
            {
                AllData = true,
                Eop = eop,
                Count = 5,
                WaitTime = WaitTime,
            };
            lock (Media.Synchronous)
            {
                while (!succeeded && pos != 3)
                {
                    if (Trace)
                    {
                        Console.WriteLine("<- " + DateTime.Now.ToLongTimeString() + " " + BitConverter.ToString(data));
                    }
                    Media.Send(data, null);
                    succeeded = Media.Receive(p);
                    if (!succeeded)
                    {
                        //Try to read again...
                        if (++pos != 3)
                        {
                            System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                            continue;
                        }                        
                        throw new Exception("Failed to receive reply from the device in given time.");
                    }
                }
                //Loop until whole Cosem packet is received.                
                while (!m_Parser.IsDLMSPacketComplete(p.Reply))
                {
                    if (!Media.Receive(p))
                    {
                        //Try to read again...
                        if (++pos != 3)
                        {
                            System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                            continue;
                        }
                        throw new Exception("Failed to receive reply from the device in given time.");
                    }
                }
            }
            if (Trace)
            {
                Console.WriteLine("-> " + DateTime.Now.ToLongTimeString() + " "+ BitConverter.ToString(p.Reply));
            }
            object errors = m_Parser.CheckReplyErrors(data, p.Reply);
            if (errors != null)
            {
                object[,] arr = (object[,])errors;
                int error = (int)arr[0, 0];
                throw new GXDLMSException(error);
            }
            return p.Reply;
        }

        /// <summary>
        /// Read data block from the device.
        /// </summary>
        /// <param name="data">data to send</param>
        /// <param name="text">Progress text.</param>
        /// <param name="multiplier"></param>
        /// <returns>Received data.</returns>
        public byte[] ReadDataBlock(byte[] data)
        {
            byte[] reply = ReadDLMSPacket(data);
            byte[] allData = null;
            RequestTypes moredata = m_Parser.GetDataFromPacket(reply, ref allData);
            int maxProgress = m_Parser.GetMaxProgressStatus(allData);
            while (moredata != 0)
            {
                while ((moredata & RequestTypes.Frame) != 0)
                {
                    data = m_Parser.ReceiverReady(RequestTypes.Frame);                    
                    reply = ReadDLMSPacket(data);
                    RequestTypes tmp = m_Parser.GetDataFromPacket(reply, ref allData);
                    if (!Trace)
                    {
                        Console.Write("-");
                    }
                    //If this was last frame.
                    if ((tmp & RequestTypes.Frame) == 0)
                    {
                        moredata &= ~RequestTypes.Frame;
                        break;
                    }
                }
                if ((moredata & RequestTypes.DataBlock) != 0)
                {
                    //Send Receiver Ready.
                    data = m_Parser.ReceiverReady(RequestTypes.DataBlock);
                    reply = ReadDLMSPacket(data);
                    moredata = m_Parser.GetDataFromPacket(reply, ref allData);
                    if (!Trace)
                    {
                        Console.Write("+");
                    }
                }
            }            
            return allData;
        }
    }
}
