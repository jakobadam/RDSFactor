Imports System.Net
Imports System.Net.Sockets

Friend Class UDPServer

    Private mSocket As UdpClient
    Private mAsyncResult As IAsyncResult

    Friend Delegate Sub UDPPacketHandler(ByRef data() As Byte, ByRef endPoint As IPEndPoint)

    Private HandlePacket As UDPPacketHandler

    Friend Sub New(ByVal portNumber As Integer, ByVal onDataArrived As UDPPacketHandler)
        mSocket = New UdpClient(portNumber)
        HandlePacket = onDataArrived
        commonNew()
    End Sub

    Friend Sub New(ByVal ipAddress As String, ByVal portNumber As Integer, ByVal onDataArrived As UDPPacketHandler)
        Dim ep As New IPEndPoint(System.Net.IPAddress.Parse(ipAddress), portNumber)
        mSocket = New UdpClient(ep)
        HandlePacket = onDataArrived
        commonNew()
    End Sub

    Private Sub commonNew()
        mAsyncResult = mSocket.BeginReceive(New AsyncCallback(AddressOf DataReceived), Nothing)
    End Sub

  

    Private Sub DataReceived(ByVal ar As IAsyncResult)
        Dim ep As New IPEndPoint(0, 0)
        Dim ef As Boolean = False
        Dim data() As Byte = {}
        Try
            data = mSocket.EndReceive(mAsyncResult, ep)
        Catch ex As System.Net.Sockets.SocketException
            If ex.SocketErrorCode = 10054 Then  ' Client killed connection
                ef = False
            Else
                ef = True
            End If

        End Try
        If Not ef Then
            mAsyncResult = mSocket.BeginReceive(New AsyncCallback(AddressOf DataReceived), Nothing)
            HandlePacket(data, ep)

        End If
    End Sub

    Friend Sub Send(ByRef data() As Byte, ByRef endPoint As IPEndPoint)
        mSocket.Send(data, data.Length, endPoint)
    End Sub

End Class
