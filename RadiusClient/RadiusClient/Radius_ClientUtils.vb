Imports System.Net
Imports System.Net.Dns

Partial Friend NotInheritable Class Utils
    Private Sub New()
    End Sub
    Public Shared Function GetCurrentIP() As IPAddress

        Dim IPBytes As Byte() = New Byte(3) {}
        Dim HostName As String = Dns.GetHostEntry("127.0.0.1").HostName
        Dim local As IPHostEntry = Dns.GetHostEntry(HostName)

        If local.AddressList.Length > 0 Then
            IPBytes = local.AddressList(0).GetAddressBytes()
        Else
            IPBytes = IPAddress.Loopback.GetAddressBytes()
        End If

        Dim returnIP As New IPAddress(IPBytes)
        Return returnIP
    End Function

    Public Shared Function ToHexString(bytes As Byte()) As String

        Dim hexDigits As Char() = {"0"c, "1"c, "2"c, "3"c, "4"c, "5"c, _
         "6"c, "7"c, "8"c, "9"c, "A"c, "B"c, _
         "C"c, "D"c, "E"c, "F"c}


        Dim chars As Char() = New Char(bytes.Length * 2 - 1) {}
        For i As Integer = 0 To bytes.Length - 1
            Dim b As Integer = bytes(i)
            chars(i * 2) = hexDigits(b >> 4)
            chars(i * 2 + 1) = hexDigits(b And &HF)
        Next
        Return New String(chars)
    End Function

    Public Shared Function intToByteArray(value As Integer) As Byte()
        Dim littleendian As Byte() = BitConverter.GetBytes(CShort(value))
        Return New Byte() {littleendian(1), littleendian(0)}
    End Function
    

End Class




