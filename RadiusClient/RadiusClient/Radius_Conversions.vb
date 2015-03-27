
Friend Class Conversion
    Friend Shared Function ConvertToString(ByRef bytes() As Byte) As String
        Dim k As New System.Text.StringBuilder
        Dim i As Integer
        For i = 0 To bytes.Length - 1 : k.Append(Chr(bytes(i))) : Next
        Return k.ToString
    End Function

    Friend Shared Function ConvertToBytes(ByVal str As String) As Byte()
        Dim res() As Byte = {}
        Array.Resize(Of Byte)(res, str.Length)
        Dim i As Integer
        For i = 0 To res.Length - 1
            res(i) = Convert.ToByte(str.Chars(i))
        Next
        Return res
    End Function

    Friend Shared Function ConvertToDateTime(ByVal value As String) As DateTime
        Dim ret As DateTime
        value = LCase(value)
        Try
            value = Replace(value, "utc", "")
            value = Replace(value, "mon", "")
            value = Replace(value, "tue", "")
            value = Replace(value, "wed", "")
            value = Replace(value, "thu", "")
            value = Replace(value, "fri", "")
            value = Replace(value, "sat", "")
            value = Replace(value, "sun", "")
            value = Replace(value, "jan", "1/")
            value = Replace(value, "feb", "2/")
            value = Replace(value, "mar", "3/")
            value = Replace(value, "apr", "4/")
            value = Replace(value, "may", "5/")
            value = Replace(value, "jun", "6/")
            value = Replace(value, "jul", "7/")
            value = Replace(value, "aug", "8/")
            value = Replace(value, "sep", "9/")
            value = Replace(value, "oct", "10/")
            value = Replace(value, "nov", "11/")
            value = Replace(value, "dec", "12/")
            Do While InStr(value, "  ") <> 0
                value = Replace(value, "  ", " ")
            Loop
            value = Replace(value, "/ ", "/")

            ret = Convert.ToDateTime(value)
        Catch ex As Exception
            ret = Nothing
        End Try
        Return ret
    End Function
End Class

