Public Class MissingEmail

    Inherits Exception

    Public Sub New(ByVal user As String)
        MyBase.New("User: " & user & " has no email")
    End Sub

End Class
