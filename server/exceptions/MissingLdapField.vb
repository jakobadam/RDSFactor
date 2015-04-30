Public Class MissingLdapField

    Inherits Exception

    Public Sub New(field As String, username As String)
        MyBase.New("No " & field & " entry in LDAP for " & username)
    End Sub

End Class
