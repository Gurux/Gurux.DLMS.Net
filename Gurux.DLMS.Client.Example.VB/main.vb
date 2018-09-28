'
' --------------------------------------------------------------------------
'  Gurux Ltd
'
'
'
' Filename:        $HeadURL$
'
' Version:         $Revision$,
'                  $Date$
'                  $Author$
'
' Copyright (c) Gurux Ltd
'
'---------------------------------------------------------------------------
'
'  DESCRIPTION
'
' This file is a part of Gurux Device Framework.
'
' Gurux Device Framework is Open Source software; you can redistribute it
' and/or modify it under the terms of the GNU General Public License
' as published by the Free Software Foundation; version 2 of the License.
' Gurux Device Framework is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
' See the GNU General Public License for more details.
'
' More information of Gurux products: http://www.gurux.org
'
' This code is licensed under the GNU General Public License v2.
' Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
'---------------------------------------------------------------------------

Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Gurux.Common
Imports Gurux.DLMS.Enums
Imports Gurux.DLMS.ManufacturerSettings
Imports Gurux.DLMS.Objects
Imports Gurux.DLMS.Secure
Imports Gurux.Net
Imports Gurux.Serial

Module main

    Private Sub ShowHelp()
        Console.WriteLine("GuruxDlmsSample reads data from the DLMS/COSEM device.")
        Console.WriteLine("Note! Before first use update initial settings with /u.")
        Console.WriteLine("")
        Console.WriteLine("GuruxDlmsSample /m=lgz /h=[Meter IP Address] /p=[Meter Port No] [/s=] [/u]")
        Console.WriteLine(" /u" & vbTab & " Update meter settings from Gurux web portal.")
        Console.WriteLine(" /m=" & vbTab & " manufacturer identifier.")
        Console.WriteLine(" /sp=" & vbTab & " serial port.")
        Console.WriteLine(" /h=" & vbTab & " host name or IP address.")
        Console.WriteLine(" /p=" & vbTab & " port number or name (Example: 1000).")
        Console.WriteLine(" /s=" & vbTab & " start protocol (IEC or DLMS).")
        Console.WriteLine(" /a=" & vbTab & " Authentication (None, Low, High).")
        Console.WriteLine(" /pw=" & vbTab & " Password for authentication.")
        Console.WriteLine(" /client=" & vbTab & " Client address.")
        Console.WriteLine(" /server=" & vbTab & " Server address.")
        Console.WriteLine(" /r=[sn, LN]" & vbTab & " Short name or Logican Name referencing is used.")
        Console.WriteLine(" /wrapper" & vbTab & " wrapper is used.")
        Console.WriteLine("Example:")
        Console.WriteLine("Read LG device using TCP/IP connection.")
        Console.WriteLine("GuruxDlmsSample /m=lgz /h=[Meter IP Address] /p=[Meter Port No]")
        Console.WriteLine("Read LG device using serial port connection.")
        Console.WriteLine("GuruxDlmsSample /m=lgz /sp=COM1 /s=DLMS")
        Console.WriteLine("GuruxDlmsSample /sp=COM1 /s=DLMS /client=16 /server=1 /sn /wrapper")
    End Sub

    Private Sub Trace(writer As TextWriter, text As String)
        writer.Write(text)
        Console.Write(text)
    End Sub

    Private Sub TraceLine(writer As TextWriter, text As String)
        writer.WriteLine(text)
        Console.WriteLine(text)
    End Sub


    Sub Main()
        Dim args As New List(Of String)(Environment.GetCommandLineArgs())
        args.RemoveAt(0)
        Dim media As IGXMedia = Nothing
        Dim comm As GXCommunicatation = Nothing
        Try
            Dim logFile As TextWriter = New StreamWriter(File.Open("LogFile.txt", FileMode.Create))
            '////////////////////////////////////////
            'Handle command line parameters.
            Dim sn As [String], client As [String] = "", server As [String] = "", id As [String] = "", host As [String] = "", port As [String] = "",
            pw As [String] = ""
            Dim trace__1 As Boolean = False, iec As Boolean = True
            Dim hdlc As Boolean = True, ln As Boolean = True
            Dim auth As Authentication = Authentication.None
            For Each it As String In args
                Dim item As [String] = it.Trim().ToLower()
                If String.Compare(item, "/u", True) = 0 Then
                    'Update
                    'Get latest manufacturer settings from Gurux web server.
                    GXManufacturerCollection.UpdateManufactureSettings()
                ElseIf item.StartsWith("/sn=") Then
                    'Serial number.
                    sn = item.Replace("/sn=", "")
                ElseIf String.Compare(item, "/wrapper", True) = 0 Then
                    'Wrapper is used.
                    hdlc = False
                ElseIf item.StartsWith("/r=") Then
                    'referencing
                    id = item.Replace("/r=", "")
                ElseIf item.StartsWith("/m=") Then
                    'Manufacturer
                    id = item.Replace("/m=", "")
                ElseIf item.StartsWith("/client=") Then
                    'Client address
                    client = item.Replace("/client=", "")
                ElseIf item.StartsWith("/server=") Then
                    'Server address
                    server = item.Replace("/server=", "")
                ElseIf item.StartsWith("/h=") Then
                    'Host
                    host = item.Replace("/h=", "")
                ElseIf item.StartsWith("/p=") Then
                    ' TCP/IP Port
                    media = New Gurux.Net.GXNet()
                    port = item.Replace("/p=", "")
                ElseIf item.StartsWith("/sp=") Then
                    'Serial Port
                    port = item.Replace("/sp=", "")
                    media = New GXSerial()
                ElseIf item.StartsWith("/t") Then
                    'Are messages traced.
                    trace__1 = True
                ElseIf item.StartsWith("/s=") Then
                    'Start
                    Dim tmp As [String] = item.Replace("/s=", "")
                    iec = String.Compare(tmp, "dlms", True) <> 0
                ElseIf item.StartsWith("/a=") Then
                    'Authentication
                    auth = DirectCast([Enum].Parse(GetType(Authentication), it.Trim().Replace("/a=", "")), Authentication)
                ElseIf item.StartsWith("/pw=") Then
                    'Password
                    pw = it.Trim().Replace("/pw=", "")
                Else
                    ShowHelp()
                    Return
                End If
            Next
            If String.IsNullOrEmpty(id) OrElse String.IsNullOrEmpty(port) OrElse (TypeOf media Is GXNet AndAlso String.IsNullOrEmpty(host)) Then
                ShowHelp()
                Return
            End If
            '/////////////////////////////////////
            'Initialize connection settings.
            If TypeOf media Is GXSerial Then
                Dim serial As GXSerial = TryCast(media, GXSerial)
                serial.PortName = port
                If iec Then
                    serial.BaudRate = 300
                    serial.DataBits = 7
                    serial.Parity = System.IO.Ports.Parity.Even
                    serial.StopBits = System.IO.Ports.StopBits.One
                Else
                    serial.BaudRate = 9600
                    serial.DataBits = 8
                    serial.Parity = System.IO.Ports.Parity.None
                    serial.StopBits = System.IO.Ports.StopBits.One
                End If
            ElseIf TypeOf media Is GXNet Then
                Dim net As Gurux.Net.GXNet = TryCast(media, GXNet)
                net.Port = Convert.ToInt32(port)
                net.HostName = host
                net.Protocol = Gurux.Net.NetworkType.Tcp
            Else
                Throw New Exception("Unknown media type.")
            End If
            '/////////////////////////////////////
            'Update manufacturer depended settings.
            Dim Manufacturers As New GXManufacturerCollection()
            GXManufacturerCollection.ReadManufacturerSettings(Manufacturers)
            Dim man As GXManufacturer = Manufacturers.FindByIdentification(id)
            If man Is Nothing Then
                Throw New Exception("Unknown manufacturer: " + id)
            End If
            Dim dlms As New GXDLMSSecureClient()
            'Update Obis code list so we can get right descriptions to the objects.
            dlms.CustomObisCodes = man.ObisCodes
            comm = New GXCommunicatation(dlms, media, iec, auth, pw)
            comm.Trace = trace__1
            comm.InitializeConnection(man)
            Dim objects As GXDLMSObjectCollection = Nothing
            Dim path As String = host.Replace("."c, "_"c) + "_" + port.ToString() + ".xml"

            Dim extraTypes As New List(Of Type)(Gurux.DLMS.GXDLMSClient.GetObjectTypes())
            extraTypes.Add(GetType(GXDLMSAttributeSettings))
            extraTypes.Add(GetType(GXDLMSAttribute))

            Console.WriteLine("Get available objects from the device.")
            objects = comm.GetAssociationView()
            Dim objs As GXDLMSObjectCollection = objects.GetObjects(New ObjectType() {ObjectType.Register, ObjectType.ExtendedRegister, ObjectType.DemandRegister})
            Console.WriteLine("Read scalers and units from the device.")
            If (dlms.NegotiatedConformance And Conformance.MultipleReferences) <> 0 Then
                Dim list As New List(Of KeyValuePair(Of GXDLMSObject, Integer))()
                For Each it As GXDLMSObject In objs
                    If TypeOf it Is GXDLMSRegister Then
                        list.Add(New KeyValuePair(Of GXDLMSObject, Integer)(it, 3))
                    End If
                    If TypeOf it Is GXDLMSDemandRegister Then
                        list.Add(New KeyValuePair(Of GXDLMSObject, Integer)(it, 4))
                    End If
                Next
                comm.ReadList(list)
            Else
                'Read values one by one.
                For Each it As GXDLMSObject In objs
                    Try
                        If TypeOf it Is GXDLMSRegister Then
                            Console.WriteLine(it.Name)
                            comm.Read(it, 3)
                        End If
                        If TypeOf it Is GXDLMSDemandRegister Then
                            Console.WriteLine(it.Name)
                            comm.Read(it, 4)
                        End If
                        'Actaric SL7000 can return error here. Continue reading.
                    Catch
                    End Try
                Next
            End If
            'Read Profile Generic columns first.
            For Each it As GXDLMSObject In objects.GetObjects(ObjectType.ProfileGeneric)
                Try
                    Console.WriteLine(it.Name)
                    comm.Read(it, 3)
                    Dim cols As GXDLMSObject() = TryCast(it, GXDLMSProfileGeneric).GetCaptureObject()
                    TraceLine(logFile, "Profile Generic " + Convert.ToString(it.Name) + " Columns:")
                    Dim sb As New StringBuilder()
                    Dim First As Boolean = True
                    For Each col As GXDLMSObject In cols
                        If Not First Then
                            sb.Append(" | ")
                        End If
                        First = False
                        sb.Append(col.Name)
                        sb.Append(" ")
                        sb.Append(col.Description)
                    Next
                    TraceLine(logFile, sb.ToString())
                Catch ex As Exception
                    'Continue reading.
                    TraceLine(logFile, "Err! Failed to read columns:" + ex.Message)
                End Try
            Next
            Try
            Catch ex As Exception
                If File.Exists(path) Then
                    File.Delete(path)
                End If
                Throw ex
            End Try
            Console.WriteLine("--- Available objects ---")
            For Each it As GXDLMSObject In objects
                Console.WriteLine(Convert.ToString(it.Name) + " " + it.Description)
            Next

            For Each it As GXDLMSObject In objects
                ' Profile generics are read later because they are special cases.
                ' (There might be so lots of data and we so not want waste time to read all the data.)
                If TypeOf it Is GXDLMSProfileGeneric Then
                    Continue For
                End If
                If Not (TypeOf it Is IGXDLMSBase) Then
                    'If interface is not implemented.
                    'Example manufacturer spesific interface.
                    Console.WriteLine("Unknown Interface: " + it.ObjectType.ToString())
                    Continue For
                End If
                TraceLine(logFile, "-------- Reading " + it.[GetType]().Name + " " + Convert.ToString(it.Name) + " " + it.Description)
                For Each pos As Integer In TryCast(it, IGXDLMSBase).GetAttributeIndexToRead(True)
                    Try
                        Dim val As Object = comm.Read(it, pos)
                        'If data is array.
                        If TypeOf val Is Byte() Then
                            val = GXCommon.ToHex(DirectCast(val, Byte()), True)
                        ElseIf TypeOf val Is Array Then
                            Dim str As String = ""
                            Dim pos2 As Integer = 0
                            While pos2 <> TryCast(val, Array).Length
                                If str <> "" Then
                                    str += ", "
                                End If
                                If TypeOf TryCast(val, Array).GetValue(pos2) Is Byte() Then
                                    str += GXCommon.ToHex(DirectCast(TryCast(val, Array).GetValue(pos2), Byte()), True)
                                Else
                                    str += TryCast(val, Array).GetValue(pos2).ToString()
                                End If
                                pos2 += 1
                            End While
                            val = str
                        ElseIf TypeOf val Is System.Collections.IList Then
                            Dim str As String = "["
                            Dim empty As Boolean = True
                            For Each it2 As Object In TryCast(val, System.Collections.IList)
                                If Not empty Then
                                    str += ", "
                                End If
                                empty = False
                                If TypeOf it2 Is Byte() Then
                                    str += GXCommon.ToHex(DirectCast(it2, Byte()), True)
                                Else
                                    str += it2.ToString()
                                End If
                            Next
                            str += "]"
                            val = str
                        End If
                        TraceLine(logFile, "Index: " + Convert.ToString(pos) + " Value: " + Convert.ToString(val))
                    Catch ex As Exception
                        TraceLine(logFile, "Error! Index: " + Convert.ToString(pos) + " " + ex.Message)
                    End Try
                Next
            Next
            'Find profile generics and read them.
            For Each it As GXDLMSObject In objects.GetObjects(ObjectType.ProfileGeneric)
                TraceLine(logFile, "-------- Reading " + it.[GetType]().Name + " " + Convert.ToString(it.Name) + " " + it.Description)
                Dim entriesInUse As Long = Convert.ToInt64(comm.Read(it, 7))
                Dim entries As Long = Convert.ToInt64(comm.Read(it, 8))
                TraceLine(logFile, "Entries: " + Convert.ToString(entriesInUse) + "/" + Convert.ToString(entries))
                'If there are no columns or rows.
                If entriesInUse = 0 OrElse TryCast(it, GXDLMSProfileGeneric).CaptureObjects.Count = 0 Then
                    Continue For
                End If
                'All meters are not supporting parameterized read.
                If (dlms.NegotiatedConformance And (Conformance.ParameterizedAccess Or Conformance.SelectiveAccess)) <> 0 Then
                    Try
                        'Read first row from Profile Generic.
                        Dim rows As Object() = comm.ReadRowsByEntry(TryCast(it, GXDLMSProfileGeneric), 1, 1)
                        Dim sb As New StringBuilder()
                        For Each row As Object() In rows
                            For Each cell As Object In row
                                If TypeOf cell Is Byte() Then
                                    sb.Append(GXCommon.ToHex(DirectCast(cell, Byte()), True))
                                Else
                                    sb.Append(Convert.ToString(cell))
                                End If
                                sb.Append(" | ")
                            Next
                            sb.Append(vbCr & vbLf)
                        Next
                        Trace(logFile, sb.ToString())
                    Catch ex As Exception
                        'Continue reading.
                        TraceLine(logFile, "Error! Failed to read first row: " + ex.Message)
                    End Try
                End If
                'All meters are not supporting parameterized read.
                If (dlms.NegotiatedConformance And (Conformance.ParameterizedAccess Or Conformance.SelectiveAccess)) <> 0 Then
                    Try
                        'Read last day from Profile Generic.
                        Dim rows As Object() = comm.ReadRowsByRange(TryCast(it, GXDLMSProfileGeneric), DateTime.Now.[Date], DateTime.MaxValue)
                        Dim sb As New StringBuilder()
                        For Each row As Object() In rows
                            For Each cell As Object In row
                                If TypeOf cell Is Byte() Then
                                    sb.Append(GXCommon.ToHex(DirectCast(cell, Byte()), True))
                                Else
                                    sb.Append(Convert.ToString(cell))
                                End If
                                sb.Append(" | ")
                            Next
                            sb.Append(vbCr & vbLf)
                        Next
                        Trace(logFile, sb.ToString())
                    Catch ex As Exception
                        'Continue reading.
                        TraceLine(logFile, "Error! Failed to read last day: " + ex.Message)
                    End Try
                End If
            Next
            logFile.Flush()
            logFile.Close()
        Catch ex As Exception
            If comm IsNot Nothing Then
                comm.Close()
            End If
            Console.WriteLine(ex.Message)
            If Not System.Diagnostics.Debugger.IsAttached Then
                Console.ReadKey()
            End If
        Finally
            If comm IsNot Nothing Then
                comm.Close()
            End If
            If System.Diagnostics.Debugger.IsAttached Then
                Console.WriteLine("Ended. Press any key to continue.")
                Console.ReadKey()
            End If
        End Try
    End Sub

End Module
