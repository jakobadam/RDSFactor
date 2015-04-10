Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading
Imports System.IO.Ports
Imports System.Windows.Forms

Public Class SMSModem

    Private serialPort As SerialPort

    Public Sub New(ByVal comPort As String)
        Me.serialPort = New SerialPort()
        Me.serialPort.PortName = comPort
        Me.serialPort.BaudRate = 38400
        Me.serialPort.Parity = Parity.None
        Me.serialPort.DataBits = 8
        Me.serialPort.StopBits = StopBits.One
        Me.serialPort.Handshake = Handshake.RequestToSend
        Me.serialPort.DtrEnable = True
        Me.serialPort.RtsEnable = True
        Me.serialPort.NewLine = System.Environment.NewLine
    End Sub

    Public Function send(ByVal cellNo As String, ByVal sms As String, ByVal SMSC As String) As Boolean
        Dim messages As String = Nothing
        messages = sms
        If Me.serialPort.IsOpen = True Then
            Try
                Me.serialPort.WriteLine("AT" + Chr(13))
                Thread.Sleep(4)
                Me.serialPort.WriteLine("AT+CSCA=""" + SMSC + """" + Chr(13))
                Thread.Sleep(30)
                Me.serialPort.WriteLine(Chr(13))
                Thread.Sleep(30)
                Me.serialPort.WriteLine("AT+CMGS=""" + cellNo + """")

                Thread.Sleep(30)
                Me.serialPort.WriteLine(messages + Chr(26))
            Catch ex As Exception
                MessageBox.Show(ex.Source)
            End Try
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub Opens()

        If Me.serialPort.IsOpen = False Then
            Try
                'bool ok =this.serialPort.IsOpen //does not work between 2 treads

                Me.serialPort.Open()
            Catch
                Thread.Sleep(1000)
                'wait for the port to get ready if 
                Opens()
            End Try
        End If
    End Sub
    Public Sub Closes()
        If Me.serialPort.IsOpen = True Then
            Me.serialPort.Close()
        End If
    End Sub
End Class

