Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Security.Cryptography


Partial Friend NotInheritable Class Crypto
    Private Sub New()
    End Sub
    Public Shared Function GeneratePAP_PW(ClearTextPW As String, SharedSecret As String, RequestAuthenticator As Byte()) As Byte()
        
        Dim pKeyRA As String = SharedSecret & Encoding.[Default].GetString(RequestAuthenticator)
        Dim md5 As MD5 = New MD5CryptoServiceProvider()
        Dim pMD5Sum As Byte() = md5.ComputeHash(System.Text.Encoding.[Default].GetBytes(pKeyRA))
        Dim pMD5String As String = Utils.ToHexString(pMD5Sum)

        ' Determine how many rounds are needed for authentication
        Dim pCrounds As Integer = ClearTextPW.Length \ 16
        If ClearTextPW.Length Mod 16 <> 0 Then
            pCrounds += 1
        End If


        Dim Result As Byte() = New Byte(pCrounds * 16 - 1) {}
        For j As Integer = 0 To pCrounds - 1
            Dim pm As Integer
            Dim pp As Integer

            'Split the password in 16byte 
            Dim pRoundPW As String = ""
            If ClearTextPW.Length < (j + 1) * 16 Then
                pRoundPW = ClearTextPW.Substring(j * 16, ClearTextPW.Length - j * 16)
            Else
                pRoundPW = ClearTextPW.Substring(j * 16, 16)
            End If

            For i As Integer = 0 To 15
                If 2 * i > pMD5String.Length Then
                    pm = 0
                Else
                    pm = System.Convert.ToInt32(pMD5String.Substring(2 * i, 2), 16)
                End If
                If i >= pRoundPW.Length Then
                    pp = 0
                Else
                    pp = AscW(pRoundPW(i))
                End If
                Dim pc As Integer = pm Xor pp
                Result((j * 16) + i) = CByte(pc)
            Next


            'Determine the next MD5 Sum MD5(S + cn-1)
            Dim pCN1 As Byte() = New Byte(15) {}
            Array.Copy(Result, j * 16, pCN1, 0, 16)
            Dim pKeyCN1 As String = SharedSecret & Encoding.[Default].GetString(pCN1)
            md5 = New MD5CryptoServiceProvider()
            pMD5Sum = md5.ComputeHash(System.Text.Encoding.[Default].GetBytes(pKeyCN1))
            pMD5String = Utils.ToHexString(pMD5Sum)
        Next

        Return Result
    End Function

    Public Shared Function CalcResponseAuth(ReceivedBytes As Byte()) As Byte()
        Dim temp As Byte() = New Byte(2) {}

        Return temp
    End Function


End Class





