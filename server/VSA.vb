'Copyright (C) 2008-2011  Nikolay Semov

'    This program is free software: you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation, either version 3 of the License, or
'    (at your option) any later version.

'    This program is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.

'    You should have received a copy of the GNU General Public License
'    along with this program.  If not, see <http://www.gnu.org/licenses/>.

Imports CICRadarR.Conversion

Public Class CiscoAVPair

    Private mVendorType As CiscoAVPairType
    Private mVendorName As String
    Private mVendorValue As String

    Public ReadOnly Property VendorType() As CiscoAVPairType
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
        mVendorType = CiscoAVPairType.Invalid
        mVendorName = ""
        mVendorValue = ""
        If value.Length < 6 Then Exit Sub
        If Not (value(0) = 0 And value(1) = 0 And value(2) = 0 And value(3) = 9) Then Exit Sub
        If value.Length <> value(5) + 4 Then Exit Sub
        mVendorType = value(4)
        mVendorName = "generic"
        Dim v() As Byte = {}
        Array.Resize(v, value.Length - 6)
        Array.Copy(value, 6, v, 0, v.Length)
        mVendorValue = ConvertToString(v)
        If VendorValue.Contains("=") Then
            mVendorName = Left(VendorValue, InStr(VendorValue, "=") - 1)
            mVendorValue = Right(VendorValue, VendorValue.Length - VendorName.Length - 1)
        End If
        If VendorName = "h323-ivr-in" Then mVendorType = CiscoAVPairType.IVR_In
        If VendorName = "h323-ivr-out" Then mVendorType = CiscoAVPairType.IVR_Out
    End Sub

    Public Sub New(ByVal type As CiscoAVPairType, ByVal value As String)
        mVendorType = type
        If type = CiscoAVPairType.Invalid Then
            mVendorName = ""
            mVendorValue = ""
        ElseIf type = CiscoAVPairType.Generic Then
            mVendorName = "generic"
            mVendorValue = value
        Else
            mVendorName = "h323-" & Replace(LCase(type.ToString), "_", "-")
            mVendorValue = value
        End If
    End Sub

    Public Sub New(ByVal name As String, ByVal value As String)
        mVendorType = CiscoAVPairType.Generic
        mVendorName = name
        mVendorValue = value
    End Sub

    Public Sub GetRADIUSAttribute(ByRef attributes As RADIUSAttributes)
        If attributes Is Nothing Then Exit Sub
        If mVendorType = CiscoAVPairType.Invalid Then Exit Sub
        Dim data() As Byte = {}
        Dim len As Byte = 6
        Dim lvt As Byte = mVendorType
        If lvt = CiscoAVPairType.IVR_In Or lvt = CiscoAVPairType.IVR_Out Then lvt = 1
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

End Class

Public Enum CiscoAVPairType As Byte
    Invalid = 0
    Generic = 1
    Remote_Address = 23
    Conf_Id = 24
    Setup_Time = 25
    Call_Origin = 26
    Call_Type = 27
    Connect_Time = 28
    Disconnect_Time = 29
    Disconnect_Cause = 30
    Voice_Quality = 31
    GW_Id = 33
    IVR_In = 201
    IVR_Out = 202
End Enum