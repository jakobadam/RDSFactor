

Imports System.Net
Imports System.Security.Cryptography


Public Class RADIUSPacket
    Private mCode As RadiusPacketCode
    Private mIdentifier As Byte
    Private mAuthenticator() As Byte = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    Private mAttributes As New RADIUSAttributes
    Private mEndPoint As IPEndPoint
    Private mIsValid As Boolean


    Friend Sub New(ByRef data() As Byte)
        'Check validity ...
        mIsValid = mAttributes.LoadAttributes(data)
        If mIsValid Then
            mCode = data(0)
            mIdentifier = data(1)
            Array.Copy(data, 4, mAuthenticator, 0, 16)
            '   mEndPoint = endPoint

        End If
    End Sub

    Public Sub New(ByVal code As RadiusPacketCode, ByVal identifier As Byte, ByVal attributes As RADIUSAttributes, authenticator As Byte())
        mCode = code
        mIdentifier = identifier
        If attributes Is Nothing Then
            mAttributes = New RADIUSAttributes
        Else
            mAttributes = attributes
        End If

        mAuthenticator = authenticator
        'If endPoint Is Nothing Then
        '    mIsValid = False
        'Else
        '    mEndPoint = endPoint
        '    mIsValid = True
        'End If
    End Sub

    Public ReadOnly Property IsValid() As Boolean
        Get
            Return mIsValid
        End Get
    End Property

    Public ReadOnly Property Code() As RadiusPacketCode
        Get
            Return mCode
        End Get



    End Property

    Public ReadOnly Property Identifier() As Byte
        Get
            Return mIdentifier
        End Get
    End Property

    Public ReadOnly Property Attributes() As RADIUSAttributes
        Get
            Return mAttributes
        End Get
    End Property

    Public ReadOnly Property Authenticator() As Byte()
        Get
            Return mAuthenticator
        End Get
    End Property

    Public ReadOnly Property EndPoint() As IPEndPoint
        Get
            Return mEndPoint
        End Get
    End Property

    Public ReadOnly Property UserName() As String
        Get
            If mCode <> RadiusPacketCode.AccessRequest Then Return Nothing
            If mAttributes.GetFirstAttribute(RadiusAttributeType.UserName) Is Nothing Then Return Nothing
            Return mAttributes.GetFirstAttribute(RadiusAttributeType.UserName).GetString
        End Get
    End Property

  

    Friend Function Bytes() As Byte()
        Dim mLength = 20 + mAttributes.Length
        Dim result() As Byte = {}
        Array.Resize(result, mLength)
        result(0) = mCode
        result(1) = mIdentifier
        result(2) = mLength \ 256
        result(3) = mLength Mod 256
        mAuthenticator.CopyTo(result, 4)
        If mLength > 20 Then mAttributes.Bytes.CopyTo(result, 20)
        Return result
    End Function

 


    Private Function XorBytes(ByVal oper1() As Byte, ByVal oper2() As Byte) As Byte()
        Dim res() As Byte = {}
        If oper1.Length <> oper2.Length Then Return res
        Dim i As Integer
        Array.Resize(res, oper1.Length)
        For i = 0 To oper1.Length - 1
            res(i) = oper1(i) Xor oper2(i)
        Next
        Return res
    End Function

End Class

Public Enum RadiusPacketCode As Byte
    AccessRequest = 1
    AccessAccept = 2
    AccessReject = 3
    AccountingRequest = 4
    AccountingResponse = 5
    AccessChallenge = 11
    StatusServer = 12
    StatusClient = 13
    Reserved = 255
End Enum
