Public Class MissingNumber

    Inherits Exception

    Public Sub New(ByVal user As String)
        MyBase.New("User: " & user & " has no mobile number")
    End Sub

End Class
