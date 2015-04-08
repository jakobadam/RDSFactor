Public Class MissingRadiusSecret

    Inherits Exception

    Public Sub New(ByVal ip As String)
        MyBase.New("No shared secret for ip: " & ip & ". This MUST be inserted in the config file.")
    End Sub

End Class
