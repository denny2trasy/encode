Imports System.Net
Imports System.IO
Imports System.Text

Public Class AsynFTP

#Region "Properties"
    Dim ftp As Net.FtpWebRequest
    Private _hostname As String
    ''' <summary>
    ''' Hostname
    ''' </summary>
    ''' <value></value>
    ''' <remarks>Hostname can be in either the full URL format
    ''' ftp://ftp.myhost.com or just ftp.myhost.com
    ''' </remarks>
    Public Property Hostname() As String
        Get
            If _hostname.StartsWith("ftp://") Then
                Return _hostname
            Else
                Return "ftp://" & _hostname
            End If
        End Get
        Set(ByVal value As String)
            _hostname = value
        End Set
    End Property
    Private _username As String
    ''' <summary>
    ''' Username property
    ''' </summary>
    ''' <value></value>
    ''' <remarks>Can be left blank, in which case 'anonymous' is returned</remarks>
    Public Property Username() As String
        Get
            Return IIf(_username = "", "anonymous", _username)
        End Get
        Set(ByVal value As String)
            _username = value
        End Set
    End Property
    Private _password As String
    Public Property Password() As String
        Get
            Return _password
        End Get
        Set(ByVal value As String)
            _password = value
        End Set
    End Property

    ''' <summary>
    ''' The CurrentDirectory value
    ''' </summary>
    ''' <remarks>Defaults to the root '/'</remarks>
    Private _currentDirectory As String = "/"
    Public Property CurrentDirectory() As String
        Get
            'return directory, ensure it ends with /
            Return _currentDirectory & CStr(IIf(_currentDirectory.EndsWith("/"), "", "/"))
        End Get
        Set(ByVal value As String)
            If Not value.StartsWith("/") Then Throw New ApplicationException("Directory should start with /")
            _currentDirectory = value
        End Set
    End Property


#End Region

#Region "CONSTRUCTORS"
    ''' <summary>
    ''' Blank constructor
    ''' </summary>
    ''' <remarks>Hostname, username and password must be set manually</remarks>
    Sub New()
    End Sub

    ''' <summary>
    ''' Constructor just taking the hostname
    ''' </summary>
    ''' <param name="Hostname">in either ftp://ftp.host.com or ftp.host.com form</param>
    ''' <remarks></remarks>
    Sub New(ByVal Hostname As String)
        _hostname = Hostname
    End Sub

    ''' <summary>
    ''' Constructor taking hostname, username and password
    ''' </summary>
    ''' <param name="Hostname">in either ftp://ftp.host.com or ftp.host.com form</param>
    ''' <param name="Username">Leave blank to use 'anonymous' but set password to your email</param>
    ''' <param name="Password"></param>
    ''' <remarks></remarks>
    Sub New(ByVal Hostname As String, ByVal Username As String, ByVal Password As String)
        _hostname = Hostname
        _username = Username
        _password = Password
    End Sub
#End Region


#Region "Directory functions"
    ''' <summary>
    ''' Return a simple directory listing
    ''' </summary>
    ''' <param name="directory">Directory to list, e.g. /pub</param>
    ''' <returns>A list of filenames and directories as a List(of String)</returns>
    ''' <remarks>For a detailed directory listing, use ListDirectoryDetail</remarks>
    Public Function ListDirectory(Optional ByVal directory As String = "")
        'return a simple list of filenames in directory
        Dim ftp As Net.FtpWebRequest = GetRequest(GetDirectory(directory))
        'Set request to do simple list
        ftp.Method = Net.WebRequestMethods.Ftp.ListDirectory

        Dim rs As RequestState = New RequestState()
        rs.Request = ftp
        ftp.BeginGetResponse(New AsyncCallback(AddressOf MainForm.ListDirectoryCallBack), rs)

    End Function

    Public Function CreateDirectory(ByVal dirpath As String)
        'perform create
        Dim URI As String = Me.Hostname & AdjustDir(dirpath)
        Debug.Print("AsynFTP Create Directory : " & URI)
        Dim ftp As Net.FtpWebRequest = GetRequest(URI)
        'Set request to MkDir
        ftp.Method = Net.WebRequestMethods.Ftp.MakeDirectory

        Dim rs As RequestState = New RequestState()
        rs.Request = ftp
        ftp.BeginGetResponse(New AsyncCallback(AddressOf MainForm.CreateDirectoryCallBack), rs)

    End Function


#End Region


#Region "Upload: File transfer TO ftp server"
    ''' <summary>
    ''' Copy a local file to the FTP server
    ''' </summary>
    ''' <param name="localFilename">Full path of the local file</param>
    ''' <param name="targetFilename">Target filename, if required</param>
    ''' <returns></returns>
    ''' <remarks>If the target filename is blank, the source filename is used
    ''' (assumes current directory). Otherwise use a filename to specify a name
    ''' or a full path and filename if required.</remarks>
    Public Function Upload(ByVal localFilename As String, ByVal targetFilename As String, ByVal currentCount As Integer, ByVal totalCount As Integer)
        '1. check source
        If Not File.Exists(localFilename) Then
            MainForm.LabelUploadStatus.Text = "File " & localFilename & " not found"
        Else
            Dim URI As String = Hostname & targetFilename

            'Dim ftp As Net.FtpWebRequest = GetRequest(URI)
            ftp = GetRequest(URI)
            ftp.Method = WebRequestMethods.Ftp.UploadFile


            Dim rs As RequestState = New RequestState()

            rs.Request = ftp

            rs.FileName = localFilename

            rs.currentCount = currentCount

            rs.totalFileCount = totalCount

            'Dim sourceStream As New StreamReader(localFilename)
            'Dim fileContents() As Byte = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd)
            'sourceStream.Close()

            'rs.ContentLength = fileContents.Length
            'rs.fileContents = fileContents

            Dim r As IAsyncResult = CType(ftp.BeginGetRequestStream(New AsyncCallback(AddressOf MainForm.UploadFileRequestCallBack), rs), IAsyncResult)

            'Do While Not r.IsCompleted
            '    'Dim input As String = Console.ReadLine()
            '    'Debug.Print("Console get : " & input)
            '    If Console.ReadLine() = "a" Then
            '        Debug.Print("Coming here, it is ok")
            '        ftp.Abort()
            '        Debug.Print(localFilename & "Canceled")
            '        Debug.Print("Coming here, it is ok")
            '    Else
            '        Debug.Print("what is this?")
            '    End If
            'Loop

            'Dim repRS As RequestState = New RequestState
            'repRS.Request = ftp


            'Dim responseAR As IAsyncResult = ftp.BeginGetResponse(New AsyncCallback(AddressOf MainForm.UploadFileResponseCallBack), repRS)

            'Do While Not responseAR.IsCompleted
            '    Dim input As String = Console.ReadLine()
            '    Debug.Print("Console get : " & input)
            '    If input = "a" Then
            '        MainForm.LabelUploadStatus.Text = "I am canceling from write file ..."
            '        ftp.Abort()
            '        MainForm.LabelUploadStatus.Text = localFilename & "Canceled"
            '    End If
            'Loop

        End If



    End Function

    Public Function abort()
        ftp.Abort()
        Debug.Print("Aborted....")
    End Function

#End Region



#Region "private supporting fns"
    'Get the basic FtpWebRequest object with the
    'common settings and security
    Private Function GetRequest(ByVal URI As String) As FtpWebRequest
        'create request
        Dim result As FtpWebRequest = CType(FtpWebRequest.Create(URI), FtpWebRequest)
        'Set the login details
        result.Credentials = GetCredentials()
        'Do not keep alive (stateless mode)
        result.KeepAlive = False
        Return result
    End Function


    ''' <summary>
    ''' Get the credentials from username/password
    ''' </summary>
    Private Function GetCredentials() As Net.ICredentials
        Return New Net.NetworkCredential(Username, Password)
    End Function

    ''' <summary>
    ''' returns a full path using CurrentDirectory for a relative file reference
    ''' </summary>
    Private Function GetFullPath(ByVal file As String) As String
        If file.Contains("/") Then
            Return AdjustDir(file)
        Else
            Return Me.CurrentDirectory & file
        End If
    End Function

    ''' <summary>
    ''' Amend an FTP path so that it always starts with /
    ''' </summary>
    ''' <param name="path">Path to adjust</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AdjustDir(ByVal path As String) As String
        Return CStr(IIf(path.StartsWith("/"), "", "/")) & path
    End Function

    Private Function GetDirectory(Optional ByVal directory As String = "") As String
        Dim URI As String
        If directory = "" Then
            'build from current
            URI = Hostname & Me.CurrentDirectory
            _lastDirectory = Me.CurrentDirectory
        Else
            If Not directory.StartsWith("/") Then Throw New ApplicationException("Directory should start with /")
            URI = Me.Hostname & directory
            _lastDirectory = directory
        End If
        Return URI
    End Function

    'stores last retrieved/set directory
    Private _lastDirectory As String = ""

    ''' <summary>
    ''' Obtains a response stream as a string
    ''' </summary>
    ''' <param name="ftp">current FTP request</param>
    ''' <returns>String containing response</returns>
    ''' <remarks>FTP servers typically return strings with CR and
    ''' not CRLF. Use respons.Replace(vbCR, vbCRLF) to convert
    ''' to an MSDOS string</remarks>
    Private Function GetStringResponse(ByVal ftp As FtpWebRequest) As String
        'Get the result, streaming to a string
        Dim result As String = ""
        Using response As FtpWebResponse = CType(ftp.GetResponse, FtpWebResponse)
            Dim size As Long = response.ContentLength
            Using datastream As Stream = response.GetResponseStream
                Using sr As New StreamReader(datastream)
                    result = sr.ReadToEnd()
                    sr.Close()
                End Using
                datastream.Close()
            End Using
            response.Close()
        End Using
        Return result
    End Function

    ''' <summary>
    ''' Gets the size of an FTP request
    ''' </summary>
    ''' <param name="ftp"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetSize(ByVal ftp As FtpWebRequest) As Long
        Dim size As Long
        Using response As FtpWebResponse = CType(ftp.GetResponse, FtpWebResponse)
            size = response.ContentLength
            response.Close()
        End Using
        Return size
    End Function
#End Region



End Class
