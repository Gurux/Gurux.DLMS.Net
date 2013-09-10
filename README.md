See An [Gurux](http://www.gurux.org/ "Gurux") for an overview.

Join the Gurux Community or follow [@Gurux](https://twitter.com/guruxorg "@Gurux") for project updates.

Gurux.DLMS library is a high-performance .NET component that helps you to read you DLMS/COSEM compatible electricity, gas or water meters. We have try to make component so easy to use that you do not need understand protocol at all.

For more info check out [Gurux.DLMS](http://www.gurux.fi/index.php?q=Gurux.DLMS "Gurux.DLMS").

We are updating documentation on Gurux web page. 

Read should read [DLMS/COSEM FAQ](http://www.gurux.org/index.php?q=DLMSCOSEMFAQ) first to get started. Read Instructions for making your own [meter reading application](http://www.gurux.org/index.php?q=DLMSIntro) or build own 
DLMS/COSEM [meter/simulator/proxy](http://www.gurux.org/index.php?q=OwnDLMSMeter).

If you have problems you can ask your questions in Gurux [Forum](http://www.gurux.org/forum).

You do not nesessary need to use Gurux.Serial or Gurux.Net. 
You can use any connection library you want to.
Gurux.DLMS classes only parse the data.


Simple example
=========================== 
Before use you must set following device parameters. 
Parameters are manufacturer spesific.


```csharp

GXDLMSClient client = new GXDLMSClient();

// Is used Logican Name or Short Name referencing.
client.UseLogicalNameReferencing = true;

// Is used HDLC or COSEM transport layers for IPv4 networks
client.InterfaceType = InterfaceType.General;

// Read http://www.gurux.fi/index.php?q=node/336 
// to find out how Client and Server addresses are counted.
// Some manufacturers might use own Server and Client addresses.

client.ClientID = (byte) 0x21;
client.ServerID = (byte) 0x3;

```

If you are using IEC handshake you must first send identify command and move to mode E.

```csharp
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
```

After you have set parameters you can try to connect to the meter.
First you should send SNRM request and handle UA response.
After that you will send AARQ request and handle AARE response.


```csharp

byte[] data, reply = null;
data = client.SNRMRequest();
if (data != null)
{
    reply = ReadDLMSPacket(data);
    //Has server accepted client.
    client.ParseUAResponse(reply);
    Console.WriteLine("Parsing UA reply succeeded.");
}

//Generate AARQ request.
//Split requests to multiple packets if needed. 
//If password is used all data might not fit to one packet.
foreach (byte[] it in client.AARQRequest(null))
{
    reply = ReadDLMSPacket(it);
}
//Parse reply.
client.ParseAAREResponse(reply);
Console.WriteLine("Connection succeeded.");

```

If parameters are right connection is made.
Next you can read Association view and show all objects that meter can offer.

```csharp
/// Read Association View from the meter.
byte[] reply = ReadDataBlock(client.GetObjects());
GXDLMSObjectCollection objects = client.ParseObjects(reply, true);

```
Now you can read wanted objects. After read you must close the connection by sending
disconnecting request.

```csharp
ReadDLMSPacket(m_Parser.DisconnectRequest());
Media.Close();

```

```csharp

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
    if (client.InterfaceType == InterfaceType.Net && Media is GXNet)
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
        while (!client.IsDLMSPacketComplete(p.Reply))
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
    object errors = client.CheckReplyErrors(data, p.Reply);
    if (errors != null)
    {
        object[,] arr = (object[,])errors;
        int error = (int)arr[0, 0];
        throw new GXDLMSException(error);
    }
    return p.Reply;
}
```