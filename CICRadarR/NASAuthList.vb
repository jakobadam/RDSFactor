Public Class NASAuthList
    Inherits System.Collections.Generic.Dictionary(Of String, String)

    Public Sub AddSharedSecret(ByVal nasIP As String, ByVal secret As String)
        If MyBase.ContainsKey(nasIP) Then
            MyBase.Item(nasIP) = secret
        Else
            MyBase.Add(nasIP, secret)
        End If
    End Sub

    Public Function GetSharedSecret(ByVal nasIP As String) As String
        Dim res As String = ""
        MyBase.TryGetValue(nasIP, res)
        Return res
    End Function
End Class