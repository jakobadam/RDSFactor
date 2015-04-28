Imports System
Imports System.IO
Imports System.Data

Namespace LogFile
    Public Class LogWriter


        Public filePath As String
        Private fileStream As FileStream
        Private streamWriter As StreamWriter

        Public Sub OpenFile()
            Try
                Dim strPath As String
                strPath = filePath
                If System.IO.File.Exists(strPath) Then
                    fileStream = New FileStream(strPath, FileMode.Append, FileAccess.Write)
                Else
                    fileStream = New FileStream(strPath, FileMode.Create, FileAccess.Write)
                End If
                streamWriter = New StreamWriter(fileStream)
            Catch
            End Try
        End Sub

        Public Sub WriteLog(ByVal strComments As String)
            Try
                OpenFile()
                streamWriter.WriteLine(strComments)
                CloseFile()
            Catch
            End Try
        End Sub

        Public Sub CloseFile()
            Try
                streamWriter.Close()
                fileStream.Close()
            Catch
            End Try
        End Sub
    End Class
End Namespace
