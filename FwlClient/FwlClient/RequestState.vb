Imports System.Text
Imports System.Net
Imports System.IO

Public Class RequestState
    Public RequestData As New StringBuilder("")
    Public BufferRead(1024) As Byte
    Public Request As FtpWebRequest
    Public ResponseStream As Stream

    ' 记录上传文件的个数
    Public currentCount As Integer
    Public totalFileCount As Integer

    Public StreamDecode As Decoder = Encoding.UTF8.GetDecoder
    Friend FileName As Object


    Public Sub New()
        Request = Nothing
        ResponseStream = Nothing
    End Sub


End Class
