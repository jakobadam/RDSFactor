
Imports RadiusClient.Conversion

Public Class VendorSpecificAttribute

    Private mVendorType As VendorSpecificType
    Private mVendorName As String
    Private mVendorValue As String

    Public ReadOnly Property VendorType() As VendorSpecificType
        Get
            Return mVendorType
        End Get
    End Property

    Public ReadOnly Property VendorName() As String
        Get
            Return mVendorName
        End Get
    End Property

    Public ReadOnly Property VendorValue() As String
        Get
            Return mVendorValue
        End Get
    End Property

    Public Function GetTimeStamp() As DateTime
        Return ConvertToDateTime(mVendorValue)
    End Function

    Friend Sub New(ByRef value() As Byte)
        mVendorType = VendorSpecificType.Invalid
        mVendorName = ""
        mVendorValue = ""
        If value.Length < 6 Then Exit Sub
        If value.Length <> value(5) + 4 Then Exit Sub
        mVendorType = value(4)
        mVendorName = "generic"
        Dim v() As Byte = {}
        Array.Resize(v, value.Length - 6)
        Array.Copy(value, 6, v, 0, v.Length)
        mVendorValue = ConvertToString(v)
    
    End Sub

    Public Sub New(ByVal type As VendorSpecificType, ByVal value As String)
        mVendorType = type
        If type = VendorSpecificType.Invalid Then
            mVendorName = ""
            mVendorValue = ""
        ElseIf type = VendorSpecificType.Generic Then
            mVendorName = "generic"
            mVendorValue = value
       
        End If
    End Sub

    Public Sub New(ByVal name As String, ByVal value As String)
        mVendorType = VendorSpecificType.Generic
        mVendorName = name
        mVendorValue = value
    End Sub

    Public Sub GetRADIUSAttribute(ByRef attributes As RADIUSAttributes)
        If attributes Is Nothing Then Exit Sub
        If mVendorType = VendorSpecificType.Invalid Then Exit Sub
        Dim data() As Byte = {}
        Dim len As Byte = 6
        Dim lvt As Byte = mVendorType

        If VendorName = "generic" Then
            len += VendorValue.Length
            Array.Resize(data, len)
            ConvertToBytes(VendorValue).CopyTo(data, 6)
        Else
            len += VendorName.Length + 1 + VendorValue.Length
            Array.Resize(data, len)
            ConvertToBytes(VendorName & "=" & VendorValue).CopyTo(data, 6)
        End If

        data(4) = lvt
        data(5) = len - 4
        data(0) = 0
        data(1) = 0
        data(2) = 0
        data(3) = 9
        Dim attr As New RADIUSAttribute(RadiusAttributeType.VendorSpecific, data)
        attributes.Add(attr)
    End Sub

    Public Sub SetRADIUSAttribute(ByRef attributes As RADIUSAttributes)

        If mVendorType = VendorSpecificType.Invalid Then Exit Sub
        Dim data() As Byte = {}
        Dim len As Byte = 6
        Dim lvt As Byte = mVendorType


        len += VendorValue.Length
        Array.Resize(data, len)
        ConvertToBytes(VendorValue).CopyTo(data, 6)

        data(4) = lvt
        data(5) = len - 4
        data(0) = 0
        data(1) = 0
        data(2) = 0 ' Generic value 666. Not sure if this confilct with other vendor, but RFC does not list existing vendor id's
        data(3) = 9
        Dim attr As New RADIUSAttribute(RadiusAttributeType.VendorSpecific, data)
        attributes.Add(attr)
    End Sub

End Class

Public Enum VendorSpecificType As Byte
    Invalid = 0
    Generic = 1
   
End Enum