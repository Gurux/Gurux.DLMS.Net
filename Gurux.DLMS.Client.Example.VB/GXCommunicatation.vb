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

Imports System.Text
Imports Gurux.Common
Imports Gurux.Net
Imports Gurux.Serial
Imports System.IO.Ports
Imports Gurux.DLMS.ManufacturerSettings
Imports Gurux.DLMS.Objects
Imports System.IO
Imports Gurux.DLMS.Enums
Imports Gurux.DLMS.Objects.Enums
Imports System.Threading
Imports Gurux.DLMS.Secure

Class GXCommunicatation
    Public Trace As Boolean = False
    Public Client As GXDLMSSecureClient
    Private WaitTime As Integer = 5000
    Private Media As IGXMedia
    Private InitializeIEC As Boolean
    Private Manufacturer As GXManufacturer
    Private HDLCAddressing As HDLCAddressType

    ''' <summary>
    ''' Constructor.
    ''' </summary>
    Public Sub New(dlms As GXDLMSSecureClient, media__1 As IGXMedia, initializeIEC__2 As Boolean, authentication As Authentication, password As String)
        Client = dlms
        Media = media__1
        InitializeIEC = initializeIEC__2
        Client.Authentication = authentication
        Client.Password = ASCIIEncoding.ASCII.GetBytes(password)
        'Delete trace file if exists.
        If File.Exists("trace.txt") Then
            File.Delete("trace.txt")
        End If
    End Sub

    Public Sub Close()
        If Media IsNot Nothing AndAlso Client IsNot Nothing Then
            Try
                Console.WriteLine("Disconnecting from the meter.")
                Dim reply As New GXReplyData()
                ReadDLMSPacket(Client.DisconnectRequest(), reply)
                Media.Close()

            Catch
            End Try
            Media = Nothing
            Client = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Destructor.
    ''' </summary>
    ''' <remarks>
    ''' Close connection to the media and send disconnect message.
    ''' </remarks>
    Protected Overrides Sub Finalize()
        Try
            Close()
        Finally
            MyBase.Finalize()
        End Try
    End Sub

    Public Sub UpdateManufactureSettings(id As String)
        If Manufacturer IsNot Nothing AndAlso String.Compare(Manufacturer.Identification, id, True) <> 0 Then
            Throw New Exception(String.Format("Manufacturer type does not match. Manufacturer is {0} and it should be {1}.", id, Manufacturer.Identification))
        End If
        If TypeOf Me.Media Is GXNet AndAlso Manufacturer.UseIEC47 Then
            Client.InterfaceType = InterfaceType.WRAPPER
        Else
            Client.InterfaceType = InterfaceType.HDLC
        End If
        Client.UseLogicalNameReferencing = Manufacturer.UseLogicalNameReferencing
        'If network media is used check is manufacturer supporting IEC 62056-47
        Dim server As GXServerAddress = Manufacturer.GetServer(HDLCAddressing)
        Client.ClientAddress = Manufacturer.GetAuthentication(Client.Authentication).ClientAddress
        If Client.InterfaceType = InterfaceType.WRAPPER Then
            If HDLCAddressing = HDLCAddressType.SerialNumber Then
                Client.ServerAddress = GXDLMSClient.GetServerAddress(server.PhysicalAddress, server.Formula)
            Else
                Client.ServerAddress = server.PhysicalAddress
            End If
            Client.ServerAddress = InlineAssignHelper(Client.ClientAddress, 1)
        Else
            If HDLCAddressing = HDLCAddressType.SerialNumber Then
                Client.ServerAddress = GXDLMSClient.GetServerAddress(server.PhysicalAddress, server.Formula)
            Else
                Client.ServerAddress = GXDLMSClient.GetServerAddress(server.LogicalAddress, server.PhysicalAddress)
            End If
        End If
    End Sub

    Private Sub InitSerial()
        Dim serial As GXSerial = TryCast(Media, GXSerial)
        Dim Terminator As Byte = CByte(&HA)
        If serial IsNot Nothing AndAlso InitializeIEC Then
            serial.BaudRate = 300
            serial.DataBits = 7
            serial.Parity = Parity.Even
            serial.StopBits = StopBits.One
        End If
        Media.Open()
        'Query device information.
        If Media IsNot Nothing AndAlso InitializeIEC Then
            Dim data As String = "/?!" & vbCr & vbLf
            If Trace Then
                Console.WriteLine(Convert.ToString("IEC sending:") & data)
            End If
            Dim p As New ReceiveParameters(Of String)()
            p.Eop = Terminator
            p.WaitTime = WaitTime

            SyncLock Media.Synchronous
                WriteTrace("<- " + DateTime.Now.ToLongTimeString() + vbTab + GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(data), True))
                Media.Send(data, Nothing)
                If Not Media.Receive(p) Then
                    'Try to move away from mode E.
                    Try
                        Dim reply As New GXReplyData()
                        ReadDLMSPacket(Client.DisconnectRequest(), reply)
                    Catch generatedExceptionName As Exception
                    End Try
                    data = &H1 + "B0" + &H3
                    Media.Send(data, Nothing)
                    p.Count = 1
                    If Not Media.Receive(p) Then
                    End If
                    data = "Failed to receive reply from the device in given time."
                    Console.WriteLine(data)
                    Throw New Exception(data)
                End If
                WriteTrace("-> " + DateTime.Now.ToLongTimeString() + vbTab + GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(p.Reply), True))
                'If echo is used.
                If p.Reply = data Then
                    p.Reply = Nothing
                    If Not Media.Receive(p) Then
                        'Try to move away from mode E.
                        Dim reply As New GXReplyData()
                        ReadDLMSPacket(Client.DisconnectRequest(), reply)
                        If serial IsNot Nothing Then
                            data = &H1 + "B0" + &H3
                            Media.Send(data, Nothing)
                            p.Count = 1
                            Media.Receive(p)
                            serial.BaudRate = 9600
                            data = &H1 + "B0" + &H3 + vbCr & vbLf
                            Media.Send(data, Nothing)
                            p.Count = 1
                            Media.Receive(p)
                        End If

                        data = "Failed to receive reply from the device in given time."
                        Console.WriteLine(data)
                        Throw New Exception(data)
                    End If
                    WriteTrace("-> " + DateTime.Now.ToLongTimeString() + vbTab + GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(p.Reply), True))
                End If
            End SyncLock
            Console.WriteLine("IEC received: " + p.Reply)
            If p.Reply(0) <> "/"c Then
                p.WaitTime = 100
                Media.Receive(p)
                Throw New Exception("Invalid responce.")
            End If
            Dim manufactureID As String = p.Reply.Substring(1, 3)
            UpdateManufactureSettings(manufactureID)
            Dim baudrate = p.Reply(4)
            Dim bRate As Integer = 0
            Select Case baudrate
                Case "0"c
                    bRate = 300
                    Exit Select
                Case "1"c
                    bRate = 600
                    Exit Select
                Case "2"c
                    bRate = 1200
                    Exit Select
                Case "3"c
                    bRate = 2400
                    Exit Select
                Case "4"c
                    bRate = 4800
                    Exit Select
                Case "5"c
                    bRate = 9600
                    Exit Select
                Case "6"c
                    bRate = 19200
                    Exit Select
                Case Else
                    Throw New Exception("Unknown baud rate.")
            End Select
            Console.WriteLine("BaudRate is : " + bRate.ToString())
            'Send ACK
            'Send Protocol control character
            Dim controlCharacter As Byte = "2"
            ' "2" HDLC protocol procedure (Mode E)
            'Send Baudrate character
            'Mode control character
            Dim ModeControlCharacter As Byte = "2"
            '"2" //(HDLC protocol procedure) (Binary mode)
            'Set mode E.
            Dim arr As Byte() = New Byte() {&H6, controlCharacter, Convert.ToByte(baudrate), ModeControlCharacter, 13, 10}
            Console.WriteLine("Moving to mode E.", arr)
            SyncLock Media.Synchronous
                WriteTrace("<- " + DateTime.Now.ToLongTimeString() + vbTab + GXCommon.ToHex(arr, True))
                Media.Send(arr, Nothing)
                p.Reply = Nothing

                p.WaitTime = 2000
                'Note! All meters do not echo this.
                Media.Receive(p)
                If p.Reply IsNot Nothing Then
                    WriteTrace("-> " + DateTime.Now.ToLongTimeString() + vbTab + GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(p.Reply), True))
                    Console.WriteLine("Received: " + p.Reply)
                End If
                If serial IsNot Nothing Then
                    Media.Close()
                    serial.BaudRate = bRate
                    serial.DataBits = 8
                    serial.Parity = Parity.None
                    serial.StopBits = StopBits.One
                    Media.Open()
                    'Some meters need this sleep. Do not remove.
                    Thread.Sleep(1000)
                End If
            End SyncLock
        End If
    End Sub

    Private Sub InitNet()
        Media.Open()
    End Sub

    Public Sub InitializeConnection(man As GXManufacturer)
        Manufacturer = man
        UpdateManufactureSettings(man.Identification)
        If TypeOf Media Is GXSerial Then
            Console.WriteLine("Initializing serial connection.")
            InitSerial()
        ElseIf TypeOf Media Is GXNet Then
            Console.WriteLine("Initializing Network connection.")
            InitNet()
            'Some Electricity meters need some time before first message can be send.
            System.Threading.Thread.Sleep(500)
        Else
            Throw New Exception("Unknown media type.")
        End If
        Dim reply As New GXReplyData()
        Dim data As Byte()
        data = Client.SNRMRequest()
        If data IsNot Nothing Then
            If Trace Then
                Console.WriteLine("Send SNRM request." + GXCommon.ToHex(data, True))
            End If
            ReadDLMSPacket(data, reply)
            If Trace Then
                Console.WriteLine("Parsing UA reply." + reply.ToString())
            End If
            'Has server accepted client.
            Client.ParseUAResponse(reply.Data)
            Console.WriteLine("Parsing UA reply succeeded.")
        End If
        'Generate AARQ request.
        'Split requests to multiple packets if needed.
        'If password is used all data might not fit to one packet.
        reply.Clear()
        ReadDataBlock(Client.AARQRequest(), reply)
        Try
            'Parse reply.
            Client.ParseAAREResponse(reply.Data)
            reply.Clear()
        Catch Ex As Exception
            reply.Clear()
            ReadDLMSPacket(Client.DisconnectRequest(), reply)
            Throw Ex
        End Try

        'Get challenge Is HLS authentication is used.
        If Client.IsAuthenticationRequired Then
            For Each it As Byte() In Client.GetApplicationAssociationRequest()
                reply.Clear()
                ReadDLMSPacket(it, reply)
            Next
            Client.ParseApplicationAssociationResponse(reply.Data)
        End If
        Console.WriteLine("Parsing AARE reply succeeded.")
    End Sub

    ''' <summary>
    ''' Read attribute value.
    ''' </summary>
    Public Function Read(it As GXDLMSObject, attributeIndex As Integer) As Object
        Dim reply As New GXReplyData()
        If Not ReadDataBlock(Client.Read(it, attributeIndex), reply) Then
            reply.Clear()
            Thread.Sleep(1000)
            If Not ReadDataBlock(Client.Read(it, attributeIndex), reply) Then
                If reply.[Error] <> 0 Then
                    Throw New GXDLMSException(reply.[Error])
                End If
            End If
        End If
        'Update data type.
        If it.GetDataType(attributeIndex) = DataType.None Then
            it.SetDataType(attributeIndex, reply.DataType)
        End If
        Return Client.UpdateValue(it, attributeIndex, reply.Value)
    End Function

    ''' <summary>
    ''' Read list of attributes.
    ''' </summary>
    Public Sub ReadList(list As List(Of KeyValuePair(Of GXDLMSObject, Integer)))
        Dim data As Byte()() = Client.ReadList(list)
        Dim reply As New GXReplyData()
        Dim values As New List(Of Object)()
        For Each it As Byte() In data
            ReadDataBlock(it, reply)
            If TypeOf reply.Value Is Object() Then
                values.AddRange(DirectCast(reply.Value, Object()))
            ElseIf reply.Value IsNot Nothing Then
                'Value is null if data is send multiple frames.
                values.Add(reply.Value)
            End If
            reply.Clear()
        Next
        If values.Count <> list.Count Then
            Throw New Exception("Invalid reply. Read items count do not match.")
        End If
        Client.UpdateValues(list, values)
    End Sub

    ''' <summary>
    ''' Write attribute value.
    ''' </summary>
    Public Sub Write(it As GXDLMSObject, attributeIndex As Integer)
        Dim reply As New GXReplyData()
        ReadDataBlock(Client.Write(it, attributeIndex), reply)
    End Sub

    ''' <summary>
    ''' Method attribute value.
    ''' </summary>
    Public Sub Method(it As GXDLMSObject, attributeIndex As Integer, value As Object, type As DataType)
        Dim reply As New GXReplyData()
        ReadDataBlock(Client.Method(it, attributeIndex, value, type), reply)
    End Sub

    ''' <summary>
    ''' Read Profile Generic Columns by entry.
    ''' </summary>
    Public Function ReadRowsByEntry(it As GXDLMSProfileGeneric, index As Integer, count As Integer) As Object()
        Dim reply As New GXReplyData()
        ReadDataBlock(Client.ReadRowsByEntry(it, index, count), reply)
        Return DirectCast(Client.UpdateValue(it, 2, reply.Value), Object())
    End Function

    ''' <summary>
    ''' Read Profile Generic Columns by range.
    ''' </summary>
    Public Function ReadRowsByRange(it As GXDLMSProfileGeneric, start As DateTime, [end] As DateTime) As Object()
        Dim reply As New GXReplyData()
        ReadDataBlock(Client.ReadRowsByRange(it, start, [end]), reply)
        Return DirectCast(Client.UpdateValue(it, 2, reply.Value), Object())
    End Function

    ''' <summary>
    ''' Read Association View from the meter.
    ''' </summary>
    Public Function GetAssociationView() As GXDLMSObjectCollection
        Dim reply As New GXReplyData()
        ReadDataBlock(Client.GetObjectsRequest(), reply)
        Return Client.ParseObjects(reply.Data, True)
    End Function

    Private Sub WriteTrace(line As String)
        If Trace Then
            Console.WriteLine(line)
        End If
        Using writer As TextWriter = New StreamWriter(File.Open("trace.txt", FileMode.Append))
            writer.WriteLine(line)
        End Using
    End Sub

    ''' <summary>
    ''' Read DLMS Data from the device.
    ''' </summary>
    ''' <param name="data">Data to send.</param>
    Public Sub ReadDLMSPacket(data As Byte(), reply As GXReplyData)
        If data Is Nothing Then
            Return
        End If
        reply.[Error] = 0
        Dim eop As Object = CByte(&H7E)
        'In network connection terminator is not used.
        If Client.InterfaceType = InterfaceType.WRAPPER AndAlso TypeOf Media Is GXNet Then
            eop = Nothing
        End If
        Dim pos As Integer = 0
        Dim succeeded As Boolean = False
        Dim p As New ReceiveParameters(Of Byte())()
        p.Eop = eop
        p.Count = 5
        p.WaitTime = WaitTime
        SyncLock Media.Synchronous
            While Not succeeded AndAlso pos <> 3
                WriteTrace("<- " + DateTime.Now.ToLongTimeString() + vbTab + GXCommon.ToHex(data, True))
                Media.Send(data, Nothing)
                succeeded = Media.Receive(p)
                If Not succeeded Then
                    'If Eop is not set read one byte at time.
                    If p.Eop Is Nothing Then
                        p.Count = 1
                    End If
                    'Try to read again...
                    If System.Threading.Interlocked.Increment(pos) <> 3 Then
                        System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3")
                        Continue While
                    End If
                    Throw New Exception("Failed to receive reply from the device in given time.")
                End If
            End While
            Try
                'Loop until whole COSEM packet is received.
                While Not Client.GetData(p.Reply, reply)
                    'If Eop is not set read one byte at time.
                    If p.Eop Is Nothing Then
                        p.Count = 1
                    End If
                    While Not Media.Receive(p)
                        'If echo.
                        If p.Reply.Length = data.Length Then
                            Media.Send(data, Nothing)
                        End If
                        'Try to read again...
                        If System.Threading.Interlocked.Increment(pos) <> 3 Then
                            System.Diagnostics.Debug.WriteLine("Data send failed. Try to resend " + pos.ToString() + "/3")
                            Continue While
                        End If
                        Throw New Exception("Failed to receive reply from the device in given time.")
                    End While
                End While
            Catch ex As Exception
                WriteTrace("-> " + DateTime.Now.ToLongTimeString() + vbTab + GXCommon.ToHex(p.Reply, True))
                Throw ex
            End Try
        End SyncLock
        WriteTrace("-> " + DateTime.Now.ToLongTimeString() + vbTab + GXCommon.ToHex(p.Reply, True))
        If reply.[Error] <> 0 Then
            If reply.[Error] = CShort(ErrorCode.Rejected) Then
                Thread.Sleep(1000)
                ReadDLMSPacket(data, reply)
            Else
                Throw New GXDLMSException(reply.[Error])
            End If
        End If
    End Sub

    Public Sub UpdateImage(target As GXDLMSImageTransfer, data As Byte(), Identification As String)
        Dim reply As New GXReplyData()
        'Check that image transfer ia enabled.
        ReadDataBlock(Client.Read(target, 5), reply)
        Client.UpdateValue(target, 5, reply)
        If Not target.ImageTransferEnabled Then
            Throw New Exception("Image transfer is not enabled")
        End If

        'Step 1: The client gets the ImageBlockSize.
        reply.Clear()
        ReadDataBlock(Client.Read(target, 2), reply)
        Client.UpdateValue(target, 2, reply)

        ' Step 2: The client initiates the Image transfer process.
        reply.Clear()
        ReadDataBlock(target.ImageTransferInitiate(Client, Identification, data.Length), reply)
        ' Step 3: The client transfers ImageBlocks.
        reply.Clear()
        Dim ImageBlockCount As Integer
        ReadDataBlock(target.ImageBlockTransfer(Client, data, ImageBlockCount), reply)
        'Step 4: The client checks the completeness of the Image in
        'each server individually and transfers any ImageBlocks not (yet) transferred;
        reply.Clear()
        Client.UpdateValue(target, 2, reply)

        ' Step 5: The Image is verified;
        reply.Clear()
        ReadDataBlock(target.ImageVerify(Client), reply)
        ' Step 6: Before activation, the Image is checked;

        'Get list to imaages to activate.
        reply.Clear()
        ReadDataBlock(Client.Read(target, 7), reply)
        Client.UpdateValue(target, 7, reply)
        Dim bFound As Boolean = False
        For Each it As GXDLMSImageActivateInfo In target.ImageActivateInfo
            If it.Identification = Identification Then
                bFound = True
                Exit For
            End If
        Next

        'Read image transfer status.
        reply.Clear()
        ReadDataBlock(Client.Read(target, 6), reply)
        Client.UpdateValue(target, 6, reply.Value)
        If target.ImageTransferStatus <> ImageTransferStatus.VerificationSuccessful Then
            Throw New Exception("Image transfer status is " + target.ImageTransferStatus.ToString())
        End If

        If Not bFound Then
            Throw New Exception("Image not found.")
        End If

        'Step 7: Activate image.
        reply.Clear()
        ReadDataBlock(target.ImageActivate(Client), reply)
    End Sub

    Public Function ReadDataBlock(data As Byte()(), reply As GXReplyData) As Boolean
        For Each it As Byte() In data
            reply.Clear()
            ReadDataBlock(it, reply)
        Next
        Return True
    End Function

    ''' <summary>
    ''' Read data block from the device.
    ''' </summary>
    ''' <param name="data">data to send</param>
    ''' <param name="text">Progress text.</param>
    ''' <param name="multiplier"></param>
    ''' <returns>Received data.</returns>
    Public Sub ReadDataBlock(data As Byte(), reply As GXReplyData)
        ReadDLMSPacket(data, reply)
        While reply.IsMoreData
            data = Client.ReceiverReady(reply.MoreData)
            ReadDLMSPacket(data, reply)
            If Not Trace Then
                'If data block is read.
                If (reply.MoreData And RequestTypes.Frame) = 0 Then
                    Console.Write("+")
                Else
                    Console.Write("-")
                End If
            End If
        End While
    End Sub
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function
End Class
