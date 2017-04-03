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

Build
=========================== 

If you want to build source codes you need Nuget package manager for Visual Studio.
You can get it here:
https://visualstudiogallery.msdn.microsoft.com/27077b70-9dad-4c64-adcf-c7cf6bc9970c

Simple example
=========================== 
First you must create server class and derive it from GXDLMSServerBase to add support for DLMS/COSEM protocol.
Then you must create server media to listen incoming DLMS messages.


```c#

public class GXDLMSExampleServer : GXDLMSServerBase

```

First you must tell what objects meter offers.
You can also set default or static values here.
Create media component here and start listen incoming data.

```c#
public void Initialize(int port) throws IOException
{
	Media = new GXNet(NetworkType.Tcp, port);
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
    d.SetAccess(2, AccessMode.READ);
    Items.Add(d);
}

```

Read method is called when client wants to read some data from the meter.
If you want that framework returns current value just set Handled = false. 
Otherwice you can just set value that you want to return. 
In this example we will return current time for the clock.
Otherwise we will return attribute value of the object.

```c#

public override void Read(ValueEventArgs e)
{
    if(e.Target is GXDLMSClock)
    {
        //Implement spesific clock handling here.    
        //Otherwice initial values are used.      
        if (e.Index == 2)
        {
            e.Value = DateTime.Now;
            e.Handled = true;
            return;
        }
    }
    e.Handled = false;
}

```

Write method is called when client wants to write some data to the meter.
You can handle write by yourself or let the framework handle it.

```c#

public override void Write(ValueEventArgs e)
{    
}

```

Action method is called when client performs action like reset.
You can handle actions by yourself or let the framework handle it.

```c#

public override void Action(ValueEventArgs e)
{
        
}

```

Main functionality is happening here. When client sends DLMS byte packet to the server
media component receives it and sends it to the onReceived method.
You should only send received data to the handleRequest. This method parses the data and 
handles all nesessary actions. In result method returns data that is send to the client.

Note! If clientID and server ID (Server and Client Address) do not match, we do not return anything.
This is for the security. Client can't try to find meters just polling differents IP addresses.

```

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

```

On error is called if media causes error.

```c#

void OnError(object sender, Exception ex)
{
    System.Diagnostics.Debug.WriteLine(ex.Message);
}

```

When client is making connection onClientConnected method is called. 
You can example write log here.

```c#

/// <summary>
/// Client has made connection.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void OnClientConnected(object sender, Gurux.Common.ConnectionEventArgs e)
{
    Console.WriteLine("Client Connected.");
}

```

When client is closing connection onClientDisconnected method is called. 
It is important that yo call reset method here to reset all connection settings.


```c#

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
```