Module Utility
    Public Function RequestURL(address As String)
        If address.StartsWith("ftp") Then
            RequestURL = address
        Else
            RequestURL = "ftp://" & address
        End If
    End Function

    Public Function isEncodeFormat(format As String) As Boolean
        Dim l_format As String = format.ToLower
        If l_format.Equals(".mov") Or l_format.Equals(".avi") Or l_format.Equals(".ts") Or l_format.Equals(".mp4") Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function EncodeOutputFile(path As String, filename As String) As String
        Dim str As String
        If path.EndsWith("\") Then
            str = path & filename & ".mp4"
        Else
            str = path & "\" & filename & ".mp4"
        End If
        Return str
    End Function

    Public Function EncodeParams(input As String, output As String) As String
        Dim params As String = " -e x264 --x264-profile baseline -x level=3.0 -w 1280 -l 720 -r 15 -b 800 --two-pass --turbo --decomb -R 24 -B 32 "
        ' -e x264 -w 1280 -l 720 -r 15 -b 800 -R 24 -B 32
        Return " -e x264  --x264-profile baseline -x level=3.0 -w 1280 -l 720 -r 15 -b 800 --two-pass --turbo --decomb -R 24 -B 32  -i " & input & " -o " & output
    End Function

    Public Function isUploadFormat(format As String) As Boolean
        Dim l_format As String = format.ToLower
        If l_format.Equals(".mp4") Then
            Return True
        Else
            Return False
        End If
    End Function
End Module
