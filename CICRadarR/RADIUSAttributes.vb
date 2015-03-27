Imports Microsoft.VisualBasic.Conversion
Imports CICRadarR.Conversion

Public Class RADIUSAttributes
    Inherits List(Of RADIUSAttribute)

    Friend Function LoadAttributes(ByRef data() As Byte) As Boolean
        Dim offset As Integer = 20
        Dim attr As RADIUSAttribute
        Dim result As Boolean = True

        Do While offset < data.Length And result
            If offset + 1 > data.Length Then result = False
            If result Then
                If data(offset + 1) < 3 Then result = False
            End If
            If result Then
                If offset + data(offset + 1) > data.Length Then result = False
            End If
            If result Then
                attr = New RADIUSAttribute(data, offset)
                Me.Add(attr)
                offset += data(offset + 1)
            End If
        Loop

        Return result
    End Function

    Public ReadOnly Property Length() As Integer
        Get
            Dim result As Integer = 0
            For Each attr In Me
                result += attr.Length
            Next
            Return result
        End Get
    End Property

    Friend ReadOnly Property Bytes() As Byte()
        Get
            Dim result() As Byte = {}
            Dim offset As Integer = 0
            Array.Resize(result, Me.Length)
            For Each attr In Me
                Array.Copy(attr.Bytes, 0, result, offset, attr.Length)
                offset += attr.Length
            Next
            Return result
        End Get
    End Property

    ' fixed function so it return correct value (CI)
    Public Function AttributeExists(ByVal type As RadiusAttributeType) As Boolean
        Dim attr As RADIUSAttribute
        Dim result As Boolean = False
        For Each attr In Me
            result = (attr.Type = type)

            If result = True Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function GetFirstAttribute(ByVal type As RadiusAttributeType) As RADIUSAttribute
        Dim atr As RADIUSAttribute
        For Each atr In Me
            If atr.Type = type Then Return atr
        Next
        Return Nothing
    End Function

    Public Function GetAllAttributes(ByVal type As RadiusAttributeType) As RADIUSAttributes
        Dim ret As New RADIUSAttributes
        Dim atr As RADIUSAttribute
        For Each atr In Me
            If atr.Type = type Then ret.Add(atr)
        Next
        Return ret
    End Function

    'Public Function GetAllVSAs() As RADIUSAttributes
    '    Dim ret As New RADIUSAttributes
    '    Dim atr As RADIUSAttribute
    '    For Each atr In Me
    '        If atr.Type = RadiusAttributeType.VendorSpecific Then ret.Add(atr)
    '    Next
    '    Return ret
    'End Function

    'Public Function GetAllCiscoAVPairs() As RADIUSAttributes
    '    Dim ret As New RADIUSAttributes
    '    Dim atr As RADIUSAttribute
    '    For Each atr In Me
    '        If atr.GetCiscoAVPair IsNot Nothing Then ret.Add(atr)
    '    Next
    '    Return ret
    'End Function

    'Public Function GetFirstCiscoAVPair(ByVal type As CiscoAVPairType) As CiscoAVPair
    '    Dim atr As RADIUSAttribute
    '    Dim avpair As CiscoAVPair
    '    For Each atr In Me
    '        If atr.Type = RadiusAttributeType.VendorSpecific Then
    '            avpair = atr.GetCiscoAVPair
    '            If avpair IsNot Nothing Then
    '                If avpair.VendorType = type Then Return avpair
    '            End If
    '        End If
    '    Next
    '    Return Nothing
    'End Function

    'Public Function GetFirstCiscoAVPair(ByVal name As String) As CiscoAVPair
    '    Dim atr As RADIUSAttribute
    '    Dim avpair As CiscoAVPair
    '    For Each atr In Me
    '        If atr.Type = RadiusAttributeType.VendorSpecific Then
    '            avpair = atr.GetCiscoAVPair
    '            If avpair IsNot Nothing Then
    '                If avpair.VendorName = name Then Return avpair
    '            End If
    '        End If
    '    Next
    '    Return Nothing
    'End Function
End Class

Public Class RADIUSAttribute
    Private mType As Byte
    Private mLength As Byte
    Private mValue() As Byte = {0}

    Friend Sub New(ByRef data() As Byte, ByVal offset As Integer)
        mLength = data(offset + 1)
        Array.Resize(mValue, mLength - 2)
        Array.Copy(data, offset + 2, mValue, 0, mLength - 2)
        mType = data(offset)
    End Sub

    Public Sub New(ByVal type As RadiusAttributeType, ByVal data() As Byte)
        CommonNew(type, data)
    End Sub

    Public Sub New(ByVal type As RadiusAttributeType, ByVal data As String)
        Dim newdata() As Byte = ConvertToBytes(data)
        CommonNew(type, newdata)
    End Sub

    Public Sub New(ByVal type As RadiusAttributeType, ByVal data As Long)
        Dim newdata() As Byte = {data \ 16777216, _
                                 (data Mod 16777216) \ 65536, _
                                 (data Mod 65536) \ 256, _
                                 (data Mod 256)}
        CommonNew(type, newdata)
    End Sub

    Private Sub CommonNew(ByVal type As Byte, ByRef data() As Byte)
        If data.Length > 253 Then
            mType = 0
            mLength = 3
        Else
            mType = type
            Array.Resize(mValue, data.Length)
            Array.Copy(data, 0, mValue, 0, data.Length)
            mLength = mValue.Length + 2
        End If
    End Sub

    Public ReadOnly Property Length() As Byte
        Get
            Return mLength
        End Get
    End Property

    Public ReadOnly Property Type() As RadiusAttributeType
        Get
            Return mType
        End Get
    End Property

    Friend ReadOnly Property Bytes() As Byte()
        Get
            Dim result() As Byte = {}
            Array.Resize(result, mLength)
            Array.Copy(mValue, 0, result, 2, mLength - 2)
            result(0) = mType
            result(1) = mLength
            Return result
        End Get
    End Property

    Public ReadOnly Property Value() As Byte()
        Get
            Return mValue
        End Get
    End Property

    Public Function GetString() As String
        Return ConvertToString(mValue)
    End Function

    Public Function GetLong() As Long
        If mLength <> 6 Then Return 0
        Return mValue(0) * 16777216 + _
               mValue(1) * 65536 + _
               mValue(2) * 256 + _
               mValue(3)
    End Function

    Public Function GetIPAddress() As String
        If mLength <> 6 Then Return "0.0.0.0"
        Return mValue(0) & "." & mValue(1) & "." & mValue(2) & "." & mValue(3)
    End Function

    Public Function GetHex() As String
        Dim i As Integer
        Dim result As String = ""
        Dim k As String
        For i = 0 To mLength - 3
            k = Hex(mValue(i))
            If k.Length = 1 Then k = "0" & k
            result = result & k & " "
        Next
        Return result
    End Function

    Public Function GetTrimHex() As String
        Return Replace(GetHex, " ", "")
    End Function

    Public Function GetVendorSpecific() As VendorSpecificAttribute
        Return New VendorSpecificAttribute(mValue)
    End Function

    'Public Function GetCiscoAVPair() As CiscoAVPair
    '    Return New CiscoAVPair(mValue)
    'End Function

    'Public Function GetVendorSpecific() As CiscoAVPair
    '    Return New CiscoAVPair(mValue)
    'End Function

    'Public Function CiscoAVPairTypeLookahead() As CiscoAVPairType
    '    If mLength < 8 Then Return CiscoAVPairType.Invalid
    '    If mValue(3) <> 9 Then Return CiscoAVPairType.Invalid
    '    If mValue(2) <> 0 Then Return CiscoAVPairType.Invalid
    '    If mValue(1) <> 0 Then Return CiscoAVPairType.Invalid
    '    If mValue(0) <> 0 Then Return CiscoAVPairType.Invalid
    '    Return mValue(4)
    'End Function
End Class

Public Enum RadiusAttributeType As Byte
    Invalid = 0
    UserName = 1
    UserPassword = 2
    CHAPPassword = 3
    NASIPAddress = 4
    NASPort = 5
    ServiceType = 6
    FramedProtocol = 7
    FramedIPAddress = 8
    FramedIPNetmask = 9
    FramedRouting = 10
    FilterId = 11
    FramedMTU = 12
    FramedCompression = 13
    LoginIPHost = 14
    LoginService = 15
    LoginTCPPort = 16
    ReplyMessage = 18
    CallbackNumber = 19
    CallbackId = 20
    FramedRoute = 22
    FramedIPXNetwork = 23
    State = 24
    [Class] = 25
    VendorSpecific = 26
    SessionTimeout = 27
    IdleTimeout = 28
    TerminationAction = 29
    CalledStationId = 30
    CallingStationId = 31
    NASIdentifier = 32
    ProxyState = 33
    LoginLATService = 34
    LoginLATNode = 35
    LoginLATGroup = 36
    FramedAppleTalkLink = 37
    FramedAppleTalkNetwork = 38
    FramedAppleTalkZone = 39
    AcctStatusType = 40
    AcctDelayTime = 41
    AcctInputOctets = 42
    AcctOutputOctets = 43
    AcctSessionId = 44
    AcctAuthentic = 45
    AcctSessionTime = 46
    AcctInputPackets = 47
    AcctOutputPackets = 48
    AcctTerminateCause = 49
    AcctMultiSessionId = 50
    AcctLinkCount = 51
    CHAPChallenge = 60
    NASPortType = 61
    PortLimit = 62
    LoginLATPort = 63
    MessageAuthenticator = 80
End Enum