See An [Gurux](http://www.gurux.org/ "Gurux") for an overview.

Join the Gurux Community or follow [@Gurux](https://twitter.com/guruxorg "@Gurux") for project updates.

Gurux.DLMS library is a high-performance .NET component that helps you to read you DLMS/COSEM compatible electricity, gas or water meters. We have try to make component so easy to use that you do not need understand protocol at all.

For more info check out [Gurux.DLMS](http://www.gurux.fi/index.php?q=Gurux.DLMS "Gurux.DLMS").

We are updating documentation on Gurux web page. 

Read should read [DLMS/COSEM FAQ](http://www.gurux.org/index.php?q=DLMSCOSEMFAQ) first to get started. Read Instructions for making your own [meter reading application](http://www.gurux.org/index.php?q=DLMSIntro) or build own 
DLMS/COSEM [meter/simulator/proxy](http://www.gurux.org/index.php?q=OwnDLMSMeter).

If you have problems you can ask your questions in Gurux [Forum](http://www.gurux.org/forum).

Build
=========================== 

If you want to build source codes you need Nuget package manager for Visual Studio.
https://learn.microsoft.com/en-us/nuget/install-nuget-client-tools


Gurux.DLMS.XMLClient 
=========================== 

Purpose of Gurux DLMS Xml client is that you can describe DLMS commands using xml syntax and then you can communicate with your meter using xml.
This is great tool meter testing. You can add new command and you do not need to modify source code. Just create new xml file and thats it.

Structure of XML 
=========================== 
In xml you describe what you want to do. Example below makes Get (read) reguest to the meter.
You give (Interface type, OBIS code and attribute id).

<?xml version="1.0" encoding="utf-8"?>
<Messages>
  <GetRequest>
    <GetRequestNormal>
      <InvokeIdAndPriority Value="129" />
      <AttributeDescriptor>
        <!--CLOCK-->
        <ClassId Value="0008" />
        <!--0.0.1.0.0.255-->
        <InstanceId Value="0000010000FF" />
        <AttributeId Value="02" />
      </AttributeDescriptor>
    </GetRequestNormal>
  </GetRequest>
</Messages>

You can convert DLMS bytes to xml easily. First read your meter with GXDLMSDirector or some other app. Then open [GuruxDLMSTranslator](https://www.gurux.fi/GuruxDLMSTranslator) and 
select Messages tab. Paste sent bytes to the left side and press "To Conformance Test" -button. Create file and copy generated xml to file ("sample1.xml")
You can add comment field where you describe purpose of generated command.

You can try with this data:
7E A0 19 03 21 32 6F D8 E6 E6 00 C0 01 C1 00 01 00 00 2A 00 00 FF 02 00 12 80 7E
Generated xml should be same as xml above, without comment field.

Gurux XmlClient only shows data what is received from the meter. You can modify HandleReply method for your needs.

Command line parameters 
=========================== 

You can give command line parameters for the app. You can give example meter ip address and port number and input xml file or folder if you want to execute several messages.
Connection to the meter is closed after each file is executed. If you want to ask several objecs, you can add them to the one file.

You can read data from Gurux example server using HDLC framing and Logican name referencing like this:

Gurux.DLMS.XmlClient -h localhost -p 4061 -x "Path to the file to execute."

Parameters are:
-h host name or IP address.
-p port number or name (Example: 1000).
-S Serial port settings (Example: COM1:9600:8None1).
-i IEC is a start protocol.
-a \t Authentication (None, Low, High).
-P \t Password for authentication.
-c \t Client address. (Default: 16).
-s \t Server address. (Default: 1).
-n \t Server address as serial number.
-r [sn, sn]\t Short name or Logican Name (default) referencing is used.
-w WRAPPER profile is used. HDLC is default.
-t [Error, Warning, Info, Verbose] Trace messages.
-x input XML file.

Example:
Gurux DLMS Xml Client TCP/IP connection:
Gurux.DLMS.XmlClient -r LN -c 16 -s 1 -h [Meter IP Address] -p [Meter Port No]
Gurux DLMS Xml Client using serial port connection:
Gurux.DLMS.XmlClient -r SN -c 16 -s 1 -S COM1:9600:8None1 -i
Gurux.DLMS.XmlClient -S COM1:9600:8None1 -c 16 -s 1 -a Low -P [password]
