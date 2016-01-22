See An [Gurux](http://www.gurux.org/ "Gurux") for an overview.

Join the Gurux Community or follow [@Gurux](https://twitter.com/guruxorg "@Gurux") for project updates.

Gurux.DLMS library for Java is a high-performance Java component that helps you to read DLMS/COSEM compatible electricity, gas or water meters. We have try to make component so easy to use that you do not need understand protocol at all.

For more info check out [Gurux.DLMS](http://www.gurux.fi/index.php?q=Gurux.DLMS "Gurux.DLMS").

We are updating documentation on Gurux web page. 

Read should read [DLMS/COSEM FAQ](http://www.gurux.org/index.php?q=DLMSCOSEMFAQ) first to get started. Read Instructions for making your own [meter reading application](http://www.gurux.org/index.php?q=DLMSIntro) or build own 
DLMS/COSEM [meter/simulator/proxy](http://www.gurux.org/index.php?q=OwnDLMSMeter).

If you have problems you can ask your questions in Gurux [Forum](http://www.gurux.org/forum).

You do not nesessary need to use Gurux media component like Gurux.Net. 
You can use any connection library you want to.
Gurux.DLMS classes only parse the data.


Simple example
=========================== 
Before use you must set following device parameters. 
Parameters are manufacturer spesific.


```c#

GXDLMSClient client = new GXDLMSClient();

// Is used Logican Name or Short Name referencing.
client.UseLogicalNameReferencing = True;

// Is used HDLC or COSEM transport layers for IPv4 networks
client.InterfaceType = InterfaceType.General;

// Read http://www.gurux.fi/index.php?q=dlmsAddress
// to find out how Client and Server addresses are counted.
// Some manufacturers might use own Server and Client addresses.

client.ClientAddress = 0x21;
client.ServerAddress = 0x3;

```


After you have set parameters you can try to connect to the meter.
First you should send SNRM request and handle UA response.
After that you will send AARQ request and handle AARE response.


```c#

GXReplyData reply = new GXReplyData();
byte[] data;
data = client.SNRMRequest();
if (data != null)
{
    ReadDLMSPacket(data, reply);
    //Has server accepted client.
    client.ParseUAResponse(reply.Data);
}

//Generate AARQ request.
//Split requests to multiple packets if needed. 
//If password is used all data might not fit to one packet.
foreach (byte[] it in client.AARQRequest())          {
{
    reply.Clear();
    ReadDLMSPacket(it, reply);
}
//Parse reply.
client.ParseAAREResponse(reply.Data);

```

If parameters are right connection is made.
Next you can read Association view and show all objects that meter can offer.

```c#
/// Read Association View from the meter.
GXReplyData reply = new GXReplyData();
ReadDataBlock(client.GetObjectsRequest(), reply);
GXDLMSObjectCollection objects = client.ParseObjects(reply.Data, True);

```
Now you can read wanted objects. After read you must close the connection by sending
disconnecting request.

```c#
GXReplyData reply = new GXReplyData();
ReadDLMSPacket(client.DisconnectRequest(), reply);
Media.Close();

```

```c#

/// <summary>
/// Read DLMS Data from the device.
/// </summary>
/// <param name="data">Data to send.</param>
/// <returns>Received data.</returns>
public void ReadDLMSPacket(byte[] data, GXReplyData reply)
{
    if (data == null)
    {
        return;
    }
    object eop = (byte)0x7E;
    //In network connection terminator is not used.
    if (Client.InterfaceType == InterfaceType.WRAPPER && Media is GXNet)
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
            WriteTrace("<- " + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(data, true));
            Media.Send(data, null);
            succeeded = Media.Receive(p);
            if (!succeeded)
            {
                //If Eop is not set read one byte at time.
                if (p.Eop == null)
                {
                    p.Count = 1;
                }
                //Try to read again...
                if (++pos != 3)
                {
                    System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                    continue;
                }                        
                throw new Exception("Failed to receive reply from the device in given time.");
            }
        }
        //Loop until whole COSEM packet is received.                
        while (!Client.GetData(p.Reply, reply))
        {
            //If Eop is not set read one byte at time.
            if (p.Eop == null)
            {
                p.Count = 1;
            }
            if (!Media.Receive(p))
            {
                //Try to read again...
                if (pos != 3)
                {
                    System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3");
                    continue;
                }
                throw new Exception("Failed to receive reply from the device in given time.");
            }
        }
    }
    WriteTrace("-> " + DateTime.Now.ToLongTimeString() + "\t" + GXCommon.ToHex(p.Reply, true));
    if (reply.Error != 0)
    {
        throw new GXDLMSException(reply.Error);
    }
}

```

Using authentication

When authentication (Access security) is used server(meter) can allow different rights to  the client.
Example without authentication (None) only read is allowed.
Gurux DLMS component supports five different authentication level:

+ None
+ Low
+ High
+ HighMD5
+ HighSHA1

In default Authentication level None is used. If other level is used password must also give.
Used password depends from the meter.

```csharp
client.Authentication = Authentication.HighMD5;
client.Password = ASCIIEncoding.ASCII.GetBytes("12345678");
``` 

When authentication is High or above High Level security (HLS) is used.
After connection is made client must send challenge to the server and server must accept this challenge.
This is done checking is Is Authentication Required after AARE message is parsed.
If authentication is required client sends challenge to the server and if everything succeeded
server returns own challenge that client checks.

```csharp
//Parse reply.
Client.ParseAAREResponse(reply.Data);
//Get challenge Is HSL authentication is used.
if (Client.IsAuthenticationRequired)
{
    reply.Clear();
    ReadDLMSPacket(Client.GetApplicationAssociationRequest(), reply);
    Client.ParseApplicationAssociationResponse(reply.Data);
}
``` 

Writing values

Writing values to the meter is very simple. There are two ways to do this. 
First is using Write -method of GXDLMSClient.

```csharp
ReadDLMSPacket(Client.Write("0.0.1.0.0.255", DateTime.Now, 2, DataType.OctetString, ObjectType.Clock, 2), reply);
``` 


Note!
 Data type must be correct or meter returns usually error.
 If you are reading byte value you can't write UIn16.

It is easy to write simple data types like this. If you want to write complex data types like arrays there
is also another way to do this. You can Update Object's propery and then write it.
In this example we want to update listening window of GXDLMSAutoAnswer object.

```csharp
//Read Association view and find GXDLMSAutoAnswer object first.
GXDLMSAutoAnswer item = Client.Object.FindByLN("0.0.2.2.0.255", ObjectType.AutoAnswer);
//Window time is from 6am to 8am.
item.ListeningWindow.Add(new KeyValuePair<GXDateTime, GXDateTime>(new GXDateTime(-1, -1, -1, 6, -1, -1, -1), new GXDateTime(-1, -1, -1, 8, -1, -1, -1)));
ReadDLMSPacket(Client.Write(item, 3));
``` 

Transport security

DLMS supports tree different transport security. When transport security is used each packet is secured using GMAC security. Security level are:
* Authentication
* Encryption
* AuthenticationEncryption

Using secured messages is easy. Before security can be used following properties must set:
* Security
* SystemTitle
* AuthenticationKey
* BlockCipherKey
* FrameCounter

If we want communicate with Gurux DLMS server you just need to set the following settings.


```csharp
GXDLMSSecureClient sc = new GXDLMSSecureClient();
sc.Ciphering.Security = Security.ENCRYPTION;
//Default security when using Gurux test server.
sc.Ciphering.SystemTitle = ASCIIEncoding.ASCII.GetBytes("GRX12345");

```