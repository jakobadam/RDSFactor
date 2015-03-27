
Imports System
Imports System.Security.Cryptography


Namespace Security
    Public Enum RandomPasswordOptions
        Alpha = 1
        Numeric = 2
        AlphaNumeric = Alpha + Numeric
        AlphaNumericSpecial = 4
    End Enum

    Public Class RandomPasswordGenerator
        ' Define default password length.
        Private Shared DEFAULT_PASSWORD_LENGTH As Integer = 2

        'No characters that are confusing: i, I, l, L, o, O, 0, 1, u, v

        Private Shared PASSWORD_CHARS_Alpha As String = "abcdefghjkmnpqrstwxyzABCDEFGHJKMNPQRSTWXYZ"
        Private Shared PASSWORD_CHARS_NUMERIC As String = "23456789"
        Private Shared PASSWORD_CHARS_SPECIAL As String = ""

#Region "Overloads"

        ''' <summary>
        ''' Generates a random password with the default length.
        ''' </summary>
        ''' <returns>Randomly generated password.</returns>
        Public Shared Function Generate() As String
            Return Generate(DEFAULT_PASSWORD_LENGTH, RandomPasswordOptions.AlphaNumericSpecial)
        End Function

        ''' <summary>
        ''' Generates a random password with the default length.
        ''' </summary>
        ''' <returns>Randomly generated password.</returns>
        Public Shared Function Generate(ByVal [option] As RandomPasswordOptions) As String
            Return Generate(DEFAULT_PASSWORD_LENGTH, [option])
        End Function

        ''' <summary>
        ''' Generates a random password with the default length.
        ''' </summary>
        ''' <returns>Randomly generated password.</returns>
        Public Shared Function Generate(ByVal passwordLength As Integer) As String
            Return Generate(DEFAULT_PASSWORD_LENGTH, RandomPasswordOptions.AlphaNumericSpecial)
        End Function

        ''' <summary>
        ''' Generates a random password.
        ''' </summary>
        ''' <returns>Randomly generated password.</returns>
        Public Shared Function Generate(ByVal passwordLength As Integer, ByVal [option] As RandomPasswordOptions) As String
            Return GeneratePassword(passwordLength, [option])
        End Function

#End Region


        ''' <summary>
        ''' Generates the password.
        ''' </summary>
        ''' <returns></returns>
        Private Shared Function GeneratePassword(ByVal passwordLength As Integer, ByVal [option] As RandomPasswordOptions) As String
            If passwordLength < 0 Then
                Return Nothing
            End If

            Dim passwordChars = GetCharacters([option])

            If String.IsNullOrEmpty(passwordChars) Then
                Return Nothing
            End If

            Dim password = New Char(passwordLength - 1) {}

            Dim random = GetRandom()

            For i As Integer = 0 To passwordLength - 1
                Dim index = random.[Next](passwordChars.Length)
                Dim passwordChar = passwordChars(index)

                password(i) = passwordChar
            Next

            Return New String(password)
        End Function



        ''' <summary>
        ''' Gets the characters selected by the option
        ''' </summary>
        ''' <returns></returns>
        Private Shared Function GetCharacters(ByVal [option] As RandomPasswordOptions) As String
            Select Case [option]
                Case RandomPasswordOptions.Alpha
                    Return PASSWORD_CHARS_Alpha
                Case RandomPasswordOptions.Numeric
                    Return PASSWORD_CHARS_NUMERIC
                Case RandomPasswordOptions.AlphaNumeric
                    Return PASSWORD_CHARS_Alpha + PASSWORD_CHARS_NUMERIC
                Case RandomPasswordOptions.AlphaNumericSpecial
                    Return PASSWORD_CHARS_Alpha + PASSWORD_CHARS_NUMERIC + PASSWORD_CHARS_SPECIAL
                Case Else
                    Exit Select
            End Select
            Return String.Empty
        End Function

        ''' <summary>
        ''' Gets a random object with a real random seed
        ''' </summary>
        ''' <returns></returns>
        Private Shared Function GetRandom() As Random
            ' Use a 4-byte array to fill it with random bytes and convert it then
            ' to an integer value.
            Dim randomBytes As Byte() = New Byte(3) {}

            ' Generate 4 random bytes.
            Dim rng As New RNGCryptoServiceProvider()
            rng.GetBytes(randomBytes)

            ' Convert 4 bytes into a 32-bit integer value.
            Dim seed As Integer = (randomBytes(0) And &H7F) << 24 Or randomBytes(1) << 16 Or randomBytes(2) << 8 Or randomBytes(3)

            ' Now, this is real randomization.
            Return New Random(seed)
        End Function


    End Class
End Namespace

