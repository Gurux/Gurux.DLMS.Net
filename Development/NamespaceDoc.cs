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
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

namespace Gurux.DLMS
{
    /// <summary>
    /// <p>With Gurux DLMS/COSEM protocol component you can easily build your own 
    /// AMR (Automatic Meter Reading) system for electricity consumption meters, 
    /// through either TCP/IP, or Serial connection.</p>
    /// <p>If you are no expert in DLMS/COSEM, use our UI application, GXDLMSDirector, 
    /// for the component to get started. Both the component, and its UI are OpenSource, 
    /// and therefor available to anyone interested.</p>
    /// Read <a href="http://www.gurux.org/index.php?q=DLMSCOSEMFAQ">FAQ</a> first. 
    /// After that you can do your own reader application using instructions that are available <a href = "http://www.gurux.org/index.php?q=DLMSIntro">here</a>.
    /// Instructions for server/proxy simulator creation you will find <a href="http://www.gurux.org/index.php?q=OwnDLMSMeter">here</a>.    
    /// If you have problems you can ask your questions in Gurux <a href="http://www.gurux.org/forum">Forum</a>.    
    /// 
    /// Gurux.DLMS library is a high-performance .NET component that helps you to read you DLMS/COSEM 
    /// compatible electricity, gas or water meters. We have try to make component so easy to 
    /// use that you do not need understand protocol at all.
    /// You do not nesessary need to use <a href="http://www.gurux.org/index.php?q=Gurux.Serial">Gurux.Serial</a>, <a href="http://www.gurux.org/index.php?q=Gurux.Terminal">Gurux.Terminal</a> or <a href="http://www.gurux.org/index.php?q=Gurux.Net">Gurux.Net</a>. 
    /// You can use any connection library you want to. Gurux.DLMS classes only parse the data.
    /// <a target="_blank" href="index.php?q=DLMSIntro">Read intraductions</a> and start building your own AMR system today!
    /// <a target="_blank" href="index.php?q=OwnDLMSMeter">Read intraductions</a> and build your own DLMS/COSEM meter.   
    /// <h2>Simple meter reading example</h2>
    /// Before use you must set following device parameters. 
    /// Parameters are manufacturer spesific.
    /// <example>
    /// <code>
    /// GXDLMSClient client = new GXDLMSClient();
    /// // Is used Logican Name or Short Name referencing.
    /// client.UseLogicalNameReferencing = true;
    /// // Is used HDLC or COSEM transport layers for IPv4 networks
    /// client.InterfaceType = InterfaceType.General;
    /// // Read http://www.gurux.fi/index.php?q=node/336 
    /// // to find out how Client and Server addresses are counted.
    /// // Some manufacturers might use own Server and Client addresses.    
    /// client.ClientID = (byte) 0x21;
    /// client.ServerID = (byte) 0x3;
    /// </code>
    /// </example>
    /// If you are using IEC handshake you must first send identify command and move to mode E.
    /// <example>
    /// <code>
    /// void InitSerial()
    /// {
    ///     GXSerial serial = Media as GXSerial;
    ///     byte Terminator = (byte)0x0A;
    ///     if (serial != null &amp;&amp; InitializeIEC)
    ///     {
    ///         serial.BaudRate = 300;
    ///         serial.DataBits = 7;
    ///         serial.Parity = Parity.Even;
    ///         serial.StopBits = StopBits.One;
    ///     }
    ///     Media.Open();
    ///     //Query device information.
    ///     if (Media != null &amp;&amp; InitializeIEC)
    ///     {
    ///         string data = "/?!\\r\\n";
    ///         if (Trace)
    ///         {
    ///             Console.WriteLine("HDLC sending:" + data);
    ///         }
    ///         ReceiveParameters&amp;lt;string&amp;gt; p = new ReceiveParameters&amp;lt;string&amp;gt;()
    ///         {
    ///             Eop = Terminator,
    ///             WaitTime = WaitTime
    ///         };
    ///         lock (Media.Synchronous)
    ///         {
    ///             Media.Send(data, null);
    ///             if (!Media.Receive(p))
    ///             {
    ///                 //Try to move away from mode E.                        
    ///                 throw new Exception("Failed to receive reply from the device in given time.");
    ///             }
    ///             //If echo is used.
    ///             if (p.Reply == data)
    ///             {
    ///                 p.Reply = null;
    ///                 if (!Media.Receive(p))
    ///                 {
    ///                     //Try to move away from mode E.                            
    ///                     throw new Exception("Failed to receive reply from the device in given time.");
    ///                 }
    ///             }
    ///         }                
    ///         if (p.Reply[0] != '/')
    ///         {
    ///             p.WaitTime = 100;
    ///             Media.Receive(p);
    ///             throw new Exception("Invalid responce.");
    ///         }
    ///         string manufactureID = p.Reply.Substring(1, 3);    
    ///         char baudrate = p.Reply[4];
    ///         int BaudRate = 0;
    ///         switch (baudrate)
    ///         {
    ///             case '0':
    ///                 BaudRate = 300;
    ///                 break;
    ///             case '1':
    ///                 BaudRate = 600;
    ///                 break;
    ///             case '2':
    ///                 BaudRate = 1200;
    ///                 break;
    ///             case '3':
    ///                 BaudRate = 2400;
    ///                 break;
    ///             case '4':
    ///                 BaudRate = 4800;
    ///                 break;
    ///             case '5':
    ///                 BaudRate = 9600;
    ///                 break;
    ///             case '6':
    ///                 BaudRate = 19200;
    ///                 break;
    ///             default:
    ///                 throw new Exception("Unknown baud rate.");
    ///         }
    ///         Console.WriteLine("BaudRate is :", BaudRate.ToString());
    ///         //Send ACK
    ///         //Send Protocol control character
    ///         byte controlCharacter = (byte)'2';// "2" HDLC protocol procedure (Mode E)
    ///         //Send Baudrate character
    ///         //Mode control character 
    ///         byte ModeControlCharacter = (byte)'2';//"2" //(HDLC protocol procedure) (Binary mode)
    ///         //Set mode E.
    ///         byte[] arr = new byte[] { 0x06, controlCharacter, (byte)baudrate, ModeControlCharacter, 13, 10 };
    ///         if (Trace)
    ///         {
    ///             Console.WriteLine("Moving to mode E", BitConverter.ToString(arr));
    ///         }
    ///         lock (Media.Synchronous)
    ///         {
    ///             Media.Send(arr, null);
    ///             p.Reply = null;
    ///             p.WaitTime = 500;
    ///             if (!Media.Receive(p))
    ///             {
    ///                 //Try to move away from mode E.
    ///                 this.ReadDLMSPacket(m_Parser.DisconnectRequest());                        
    ///                 throw new Exception("Failed to receive reply from the device in given time.");
    ///             }
    ///         }
    ///         if (serial != null)
    ///         {
    ///             serial.BaudRate = BaudRate;
    ///             serial.DataBits = 8;
    ///             serial.Parity = Parity.None;
    ///             serial.StopBits = StopBits.One;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    /// After you have set parameters you can try to connect to the meter.
    /// First you should send SNRM request and handle UA response.
    /// After that you will send AARQ request and handle AARE response.    
    /// <example>
    /// <code>
    /// byte[] data, reply = null;
    /// data = client.SNRMRequest();
    /// if (data != null)
    /// {
    ///     reply = ReadDLMSPacket(data);
    ///     //Has server accepted client.
    ///     client.ParseUAResponse(reply);
    ///     Console.WriteLine("Parsing UA reply succeeded.");
    /// }
    /// //Generate AARQ request.
    /// //Split requests to multiple packets if needed. 
    /// //If password is used all data might not fit to one packet.
    /// foreach (byte[] it in client.AARQRequest(null))
    /// {
    /// reply = ReadDLMSPacket(it);
    /// }
    /// //Parse reply.
    /// client.ParseAAREResponse(reply);
    /// Console.WriteLine("Connection succeeded.");
    /// </code>
    /// </example>
    /// If parameters are right connection is made.
    /// Next you can read Association view and show all objects that meter can offer.
    /// Note! COSEM Object descriptions are updated from the OBIS Code.
    /// <example>
    /// <code>
    /// // Read Association View from the meter.
    /// byte[] reply = ReadDataBlock(client.GetObjects());
    /// GXDLMSObjectCollection objects = client.ParseObjects(reply, true);
    /// </code>
    /// </example>
    /// Now you can read wanted objects. 
    /// In this example we read all atributes from all objects from the meter.
    /// Note! This might take some time.
    /// <example>
    /// <code> 
    /// foreach (GXDLMSObject it in objects)
    /// {
    ///     foreach (int pos in (it as IGXDLMSBase).GetAttributeIndexToRead())
    ///     {
    ///         try
    ///         {
    ///             object val = comm.Read(it, pos);
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    /// Writing is also very simple. Just update data to the object and write attribute index to the meter.    
    /// <example>
    /// <code> 
    /// GXDLMSActivityCalendar activity = it as GXDLMSActivityCalendar;
    /// activity.setDayProfileTableActive(new GXDLMSDayProfile[]{new GXDLMSDayProfile(1, new GXDLMSDayProfileAction[]{new GXDLMSDayProfileAction(new GXDateTime(now), "test", 1)})});
    /// ReadDataBlock(m_Parser.Write(it, attributeIndex)[0]);    
    /// </code>
    /// </example>    
    /// After read and/or writing you must close the connection by sending disconnecting request.    
    /// <example>
    /// <code>
    /// ReadDLMSPacket(m_Parser.DisconnectRequest());
    /// Media.Close();
    /// </code>
    /// </example>
    /// If you want to keep connection up you must send keep alive message. Keep alive time is meter spesific.
    /// <example>
    /// <code>
    /// byte[] allData = null, reply = null;
    /// reply = ReadDLMSPacket(m_Cosem.GetKeepAlive());
    /// m_Cosem.GetDataFromPacket(reply, ref allData);
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// // Read DLMS Data from the device.
    /// // If access is denied return null.
    /// /// &amp;lt;/remarks&amp;gt;
    /// /// &amp;lt;param name="data"&amp;gt;Data to send.&amp;lt;/param&amp;gt;
    /// /// &amp;lt;returns&amp;gt;Received data.&amp;lt;/returns&amp;gt;
    /// public byte[] ReadDLMSPacket(byte[] data)
    /// {
    ///     if (data == null)
    ///     {
    ///         return null;
    ///     }
    ///     object eop = (byte)0x7E;
    ///     //In network connection terminator is not used.
    ///     if (client.InterfaceType == InterfaceType.Net &amp;&amp; Media is GXNet)
    ///     {
    ///         eop = null;
    ///     }
    ///     int pos = 0;
    ///     bool succeeded = false;
    ///     ReceiveParameters&amp;lt;byte[]&amp;gt; p = new ReceiveParameters&amp;lt;byte[]&amp;gt;()
    ///     {
    ///         AllData = true,
    ///         Eop = eop,
    ///         Count = 5,
    ///         WaitTime = WaitTime,
    ///     };
    ///     lock (Media.Synchronous)
    ///     {
    ///         while (!succeeded &amp;&amp; pos != 3)
    ///         {            
    ///             Media.Send(data, null);
    ///             succeeded = Media.Receive(p);
    ///             if (!succeeded)
    ///             {
    ///                 //Try to read again...
    ///                 if (++pos != 3)
    ///                 {
    ///                     System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
    ///                     continue;
    ///                 }                        
    ///                 throw new Exception("Failed to receive reply from the device in given time.");
    ///             }
    ///         }
    ///         //Loop until whole Cosem packet is received.                
    ///         while (!client.IsDLMSPacketComplete(p.Reply))
    ///         {
    ///             if (!Media.Receive(p))
    ///             {
    ///                 //Try to read again...
    ///                 if (++pos != 3)
    ///                 {
    ///                     System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
    ///                     continue;
    ///                 }
    ///                 throw new Exception("Failed to receive reply from the device in given time.");
    ///             }
    ///         }
    ///     }    
    ///     object errors = client.CheckReplyErrors(data, p.Reply);
    ///     if (errors != null)
    ///     {
    ///         object[,] arr = (object[,])errors;
    ///         int error = (int)arr[0, 0];
    ///         throw new GXDLMSException(error);
    ///     }
    ///     return p.Reply;
    /// }
    /// </code>
    /// </example>
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }   
}