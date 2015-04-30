Public Class SMSSendException

    Inherits Exception

    Public Sub New(ByVal message As String)
        MyBase.New("SMS send error: " & message)
    End Sub

End Class
