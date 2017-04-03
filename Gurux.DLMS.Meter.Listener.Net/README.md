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

Problem
=========================== 
When meters are using dynamic IP addresses or firewall blocks access to the meter, 
it's possible that meter is scheduled to make connection to the server using TCP/IP connection.
Note! All meters do not support this.
This simple example server waits TCP/IP connections from the meters and when new TCP/IP connection is 
established it will start to act like DLMS Client.


Build
=========================== 

If you want to build source codes you need Nuget package manager for Visual Studio.
You can get it here:
https://visualstudiogallery.msdn.microsoft.com/27077b70-9dad-4c64-adcf-c7cf6bc9970c

Simple example
=========================== 

First we create TCP/IP server what listens given port. We are using GXNet as TCP/IP server. 

```c#

GXNet server = new GXNet(NetworkType.Tcp, 1234);
server.OnClientConnected += OnClientConnected;
server.Open();

```

Main idea is listen incoming connections. When connection is established, we attach connection from server and 
use connection what meter has made.
 
```c#

/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private static void OnClientConnected(object sender, Common.ConnectionEventArgs e)
{
    Console.WriteLine("Client {0} is connected.", e.Info);
    GXNet server = (GXNet)sender;
    try
    {
        using (GXNet cl = server.Attach(e.Info))
        {
            ReadMeter(cl);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

```

After this you can access meter as normal client. You can find more information how to read your meter from Gurux web page [Gurux web page](http://www.gurux.fi/index.php?q=Gurux.DLMS).
