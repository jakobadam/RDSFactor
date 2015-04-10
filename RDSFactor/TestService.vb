Imports CICRadarR

Public Class TestService

    Private Radius_Service As New RDSFactor
    Private Sub btnStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStart.Click
        btnStart.Enabled = False
        Call Radius_Service.OnstartTest()
    End Sub

   
    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        Call Radius_Service.OnStopTest()
        End
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Button1.Enabled = False
        Dim SessionTimeOut As Integer = 1
        Dim TSGWSessionIdTimeStampHash As New Hashtable
        Dim TSGWSessionIdHash As New Hashtable
        TSGWSessionIdTimeStampHash.Add("ged", Now)
        Threading.Thread.Sleep(65000)
        Dim timeValid As Long = 0
        Dim hashTime As DateTime = DirectCast(TSGWSessionIdTimeStampHash("ged"), DateTime)
        timeValid = DateDiff(DateInterval.Second, hashTime, Now)
        MsgBox(timeValid)


        Dim Item As DictionaryEntry


        For Each Item In TSGWSessionIdTimeStampHash
            Dim hTime As DateTime = DirectCast(Item.Value, DateTime)
            Dim tValid = DateDiff(DateInterval.Minute, hTime, Now)
            If tValid >= SessionTimeOut Then
                TSGWSessionIdTimeStampHash.Remove(Item.Key)
                If TSGWSessionIdHash.Contains(Item.Key) Then
                    TSGWSessionIdHash.Remove(Item.Key)
                End If
            End If
        Next
    End Sub
End Class