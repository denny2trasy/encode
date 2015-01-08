Imports System.Net
Imports System.IO
Imports System.Globalization
Imports System.Resources
Imports System.Threading
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports Caliburn.Micro.PropertyChangedBase
Imports fwl.FwlClient.Encode
Imports HandBrake.ApplicationServices

Public Class MainForm

    Public pDefaultPath As String
    Public pFtpUserName As String
    Public pFtpIP As String
    Const NO_PASSWORD As String = "Please input password first"

    Delegate Sub ChangeFolderListSafe(ByVal list As List(Of String))
    Delegate Sub CreateDirectoryResultSafe(ByVal result As String)
    Delegate Sub UploadFileProgressSafe(ByVal result As String)
    Delegate Sub EncodeResultSafe(ByVal result As String)

    Public tasks As List(Of AsynFTP)
    Public en_tasks As List(Of Process)



    ' Below is for HandBrake Service
    Dim queueProcessor As QueueProcess
    Dim outFiles As List(Of String)


#Region "Page Load"

    Public Sub New()

        Debug.Print("CurrentCulture := " & CultureInfo.CurrentCulture.Name)
        ' set i18n
        'Thread.CurrentThread.CurrentCulture = New CultureInfo("zh-Hans", True)
        'Thread.CurrentThread.CurrentUICulture = New CultureInfo("zh-Hans", True)

        Debug.Print("CurrentCulture := " & CultureInfo.CurrentCulture.Name)
        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub

    Private Sub MainForm_Load(sender As Object, e As System.EventArgs) Handles Me.Load



        Dim filename As String
        Dim str(3) As String
        Dim i As Integer = 0
        filename = Application.LocalUserAppDataPath & "/config.txt"

        If File.Exists(filename) Then
            FileOpen(1, filename, OpenMode.Input)
            Do While Not EOF(1)
                Input(1, str(i))
                i = i + 1
            Loop
            FileClose(1)
            pDefaultPath = str(0)
            pFtpUserName = str(1)
            pFtpIP = str(2)
        Else
            pDefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            pFtpUserName = "fwl_china"
            pFtpIP = "182.92.189.193"
            Dim sw As StreamWriter = File.CreateText(filename)
            sw.WriteLine(pDefaultPath)
            sw.WriteLine(pFtpUserName)
            sw.WriteLine(pFtpIP)
            sw.Flush()
            sw.Close()
        End If
        tasks = New List(Of AsynFTP)
        en_tasks = New List(Of Process)

        queueProcessor = New QueueProcess(New EncodeService)
        AddHandler queueProcessor.JobProcessingStarted, AddressOf QueueProcessorJobProcessingStarted
        AddHandler queueProcessor.QueueCompleted, AddressOf QueueCompleted
        AddHandler queueProcessor.QueueChanged, AddressOf QueueChanged
        AddHandler queueProcessor.EncodeService.EncodeStatusChanged, AddressOf EncodeStatusChanged


    End Sub

#End Region

#Region "Menu Button"


    Private Sub BtnHelp_Click(sender As Object, e As System.EventArgs) Handles BtnHelp.Click
        Process.Start("http://182.92.189.193:23888/box/helps/manual")
    End Sub

#End Region

#Region "Setting Page"
    Private Sub BtnSClear_Click(sender As Object, e As System.EventArgs) Handles BtnSClear.Click
        TxtDefaultPath.Text = ""
        TxtFtpUserName.Text = ""
        TxtFtpIP.Text = ""
    End Sub

    Private Sub BtnSLoad_Click(sender As Object, e As System.EventArgs) Handles BtnSLoad.Click
        Dim filename As String
        Dim str(3) As String
        Dim i As Integer = 0
        filename = Application.LocalUserAppDataPath & "/config.txt"
        FileOpen(1, filename, OpenMode.Input)
        Do While Not EOF(1)
            Input(1, str(i))
            i = i + 1
        Loop
        FileClose(1)
        TxtDefaultPath.Text = str(0)
        TxtFtpUserName.Text = str(1)
        TxtFtpIP.Text = str(2)
    End Sub

    Private Sub BtnSSave_Click(sender As Object, e As System.EventArgs) Handles BtnSSave.Click
        Dim filename As String
        filename = Application.LocalUserAppDataPath & "\config.txt"
        Debug.Print(filename)
        FileOpen(1, filename, OpenMode.Output)
        WriteLine(1, TxtDefaultPath.Text)
        WriteLine(1, TxtFtpUserName.Text)
        WriteLine(1, TxtFtpIP.Text)
        FileClose(1)
        pDefaultPath = TxtDefaultPath.Text
        pFtpUserName = TxtFtpUserName.Text
        pFtpIP = TxtFtpIP.Text
    End Sub
#End Region

#Region "Home Page"

#Region "Encode Part"

    Private Sub ListEncode_DragDrop(sender As Object, e As DragEventArgs) Handles ListEncode.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim MyFiles() As String
            Dim i As Integer

            'Assignthefilestoanarray.
            MyFiles = e.Data.GetData(DataFormats.FileDrop)
            'Loopthroughthearrayandaddthefilestothelist.
            For i = 0 To MyFiles.Length - 1
                ListEncode.Items.Add(MyFiles(i))
            Next
        End If
    End Sub

    Private Sub ListEncode_DragEnter(sender As Object, e As DragEventArgs) Handles ListEncode.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.All
        End If
    End Sub

    Private Sub BtnEncodeClear_Click(sender As Object, e As System.EventArgs) Handles BtnEncodeClear.Click
        ListEncode.Items.Clear()
        Dim LocRm As New ResourceManager("fwl.MainFormStrings", GetType(MainForm).Assembly)
        LabelEncodeStatus.Text = LocRm.GetString("statusEncode")
    End Sub

    Private Sub BtnEncodeCancel_Click(sender As Object, e As System.EventArgs) Handles BtnEncodeCancel.Click
        ' Method 3 Stop
        'LabelEncodeStatus.Text = "Start to cancel..."
        'For i = 0 To en_tasks.Count - 1
        '    Dim pro As Process = CType(en_tasks.Item(i), Process)
        '    If Not pro.HasExited Then
        '        pro.Kill()
        '        Debug.Print("Kill")
        '    End If
        'Next
        'en_tasks.Clear()
        'LabelEncodeStatus.Text = "Canceled"

        ' Method 4 Stop
        Me.queueProcessor.Clear()
        Me.queueProcessor.EncodeService.Cancel()



    End Sub

    Private Sub BtnEncodeStart_Click(sender As Object, e As System.EventArgs) Handles BtnEncodeStart.Click

        Dim count As Integer = ListEncode.Items.Count
        Me.outFiles = New List(Of String)
        Debug.Print("Encode File Count = " & count)
        If count > 0 Then
            Dim i As Integer
            Dim file As String
            For i = 0 To count - 1
                file = ListEncode.Items.Item(i)
                Debug.Print("File " & i & "  is = " & file)
                EncodeFile(file)
            Next
            queueProcessor.Start(True)
        Else
            LabelEncodeStatus.Text = "Please choose files for encoding"
        End If

    End Sub

    Private Function EncodeFile(file)

        Dim fileInfo As System.IO.FileInfo
        fileInfo = My.Computer.FileSystem.GetFileInfo(file)

        Dim fileName As String = fileInfo.Name
        Dim fileExtension As String = fileInfo.Extension


        If isEncodeFormat(fileExtension) Then

            Dim outPutFile As String = EncodeOutputFile(pDefaultPath, fileName.Split(".").First)

            Debug.Print("Out Put File = " & outPutFile)

            ' Method 4
            Dim task As QEncodeTask = New QEncodeTask(New EncodeTask(file, outPutFile))

            Me.outFiles.Add(outPutFile)
            queueProcessor.Add(task)


            ' Method 3
            'doEncode(file, outPutFile)

            'ListUpload.Items.Add(outPutFile)

            ' Method 1
            'doEncodeWithShell(file, outPutFile)

            ' Method 2
            'Dim encodeWork As New AsynEncode
            'encodeWork.inputFile = file
            'encodeWork.outputFile = outPutFile

            'Dim bg As BackgroundWorker = New BackgroundWorker
            'AddHandler bg.DoWork, AddressOf BackgroundWorker_DoWork
            'AddHandler bg.ProgressChanged, AddressOf BackgroundWorker_ProgressChanged
            'AddHandler bg.RunWorkerCompleted, AddressOf BackgroundWorker_RunWorkerCompleted
            'bg.WorkerSupportsCancellation = True
            'bg.WorkerReportsProgress = True
            'en_tasks.Add(bg)
            'LabelEncodeStatus.Text = "Start Encoding ..."
            'bg.RunWorkerAsync(encodeWork)

        Else
            LabelEncodeStatus.Text = "Please choose correct file for encoding"
        End If



    End Function

#Region "Method 1 use shell directly"
    Private Sub doEncodeWithShell(inputfile As String, outputfile As String)

        Dim path As String = Application.StartupPath & "\HandBrakeCLI.exe"
        Dim args As String = EncodeParams(inputfile, outputfile)

        Dim pathname As String = path & args

        Shell(pathname, 0)

    End Sub
#End Region


#Region "Method 2 use background compent"
    Private Sub BackgroundWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs)
        Dim worker As BackgroundWorker
        worker = CType(sender, BackgroundWorker)
        Dim encodeWork As AsynEncode = CType(e.Argument, AsynEncode)
        encodeWork.DoEncode(worker, e)
    End Sub

    Private Sub BackgroundWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs)
        Dim state As AsynEncode.CurrentState = CType(e.UserState, AsynEncode.CurrentState)
        Me.LabelEncodeStatus.Text = state.Message
    End Sub


    Private Sub BackgroundWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs)
        If e.Error IsNot Nothing Then
            Me.LabelEncodeStatus.Text = "Error: " & e.Error.Message
        ElseIf e.Cancelled Then
            Debug.Print("Come here")
            Me.LabelEncodeStatus.Text = "Work canceled."
        Else
            Me.LabelEncodeStatus.Text = "Finished work."
        End If
    End Sub
#End Region


#Region "Method 3 use process"
    Private Sub doEncode(inputfile As String, outputfile As String)
        Dim startInfo As New ProcessStartInfo()

        Dim args As String = EncodeParams(inputfile, outputfile)

        Debug.Print("Run command = " & args)

        With startInfo
            '.FileName = "d:\old_pc\fw-labs\win.converter\HandBrakeCLI.exe"
            .FileName = Application.StartupPath & "\HandBrakeCLI.exe"
            .Arguments = args
            '.FileName = "cmd.exe"
            '.Arguments = "/c" & Application.StartupPath & "\HandBrakeCLI.exe" & args
            '.Arguments = "/c ping www.baidu.com"
            .RedirectStandardError = True
            .RedirectStandardOutput = True
            .UseShellExecute = False
            .CreateNoWindow = True
        End With

        Dim p As Process = Process.Start(startInfo)

        en_tasks.Add(p)

        AddHandler p.OutputDataReceived, AddressOf OutPutHandler
        AddHandler p.ErrorDataReceived, AddressOf ErrorHandler



        p.BeginOutputReadLine()

        If p.Id <> -1 Then
            p.EnableRaisingEvents = True
            AddHandler p.Exited, AddressOf ExitHandler
        End If



        'p.WaitForExit()

        'p.Close()

    End Sub


    Public Sub OutPutHandler(sendingProcess As Object, outLine As DataReceivedEventArgs)

        Dim pro As Process = CType(sendingProcess, Process)
        Debug.Print("Output = " & outLine.Data)
        Dim safedelegate As New EncodeResultSafe(AddressOf EncodeResult)
        If Not String.IsNullOrEmpty(outLine.Data) Then

            If Not pro.HasExited Then
                Me.Invoke(safedelegate, (outLine.Data).Split("(").First)
            End If
        End If

    End Sub

    Public Sub ErrorHandler(sendingProcess As Object, outLine As DataReceivedEventArgs)

        Dim safedelegate As New EncodeResultSafe(AddressOf EncodeResult)
        If Not String.IsNullOrEmpty(outLine.Data) Then

            Debug.Print("Error = " & outLine.Data)
            Me.Invoke(safedelegate, outLine.Data) 'Invoke the TreadsafeDelegate
        End If

    End Sub

    Public Sub ExitHandler(sender As Object, e As System.EventArgs)

        Dim pro As Process = CType(sender, Process)

        Debug.Print("Process ExitTime = " & pro.ExitTime)
        Debug.Print("Process ExitCode = " & pro.ExitCode)

        Dim safedelegate As New EncodeResultSafe(AddressOf EncodeResult)
        If pro.ExitCode = 0 Then
            Me.Invoke(safedelegate, "Encode Finished")
        Else
            Me.Invoke(safedelegate, "Canceled by user")
        End If


    End Sub

    Private Sub EncodeResult(result As String)

        ' I had to do this, because the process donot exit by itself
        ' TODO find a good way for exit the process
        'Dim rgx As Regex = New Regex("99.\d{2}")
        'If rgx.IsMatch(result) Then
        '    LabelEncodeStatus.Text = "Encode Finish"
        'Else
        '    LabelEncodeStatus.Text = result
        'End If

        LabelEncodeStatus.Text = result
        If (result.Equals("Completed")) Then
            Dim count As Integer = Me.outFiles.Count
            'Debug.Print("Encode File Count = " & count)
            If count > 0 Then
                Dim i As Integer
                Dim file As String
                For i = 0 To count - 1
                    file = Me.outFiles.Item(i)
                    ListUpload.Items.Add(file)
                Next

            End If
        End If

    End Sub
#End Region


#Region "Method 4 use HandBrake Service"

    ''' <summary>
    ''' Handle the Queue Starting Event
    ''' </summary>
    ''' <param name="sender">
    ''' The sender.
    ''' </param>
    ''' <param name="e">
    ''' The e.
    ''' </param>
    Private Sub QueueProcessorJobProcessingStarted(ByVal sender As Object, ByVal e As QueueProgressEventArgs)
        Me.LabelEncodeStatus.Text = "Start"
    End Sub

    ''' <summary>
    ''' The Queue has completed handler
    ''' </summary>
    ''' <param name="sender">
    ''' The Sender
    ''' </param>
    ''' <param name="e">
    ''' The EventArgs
    ''' </param>
    Private Sub QueueCompleted(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim safedelegate As New EncodeResultSafe(AddressOf EncodeResult)
        Dim re As QueueCompletedEventArgs = CType(e, QueueCompletedEventArgs)
        Dim status As String
        Dim locRM As New ResourceManager("fwl.MainFormStrings", GetType(MainForm).Assembly)
        If (re.WasManuallyStopped) Then
            status = locRM.GetString("statusCancel")
        Else
            status = locRM.GetString("statusComplete")
        End If
        Me.Invoke(safedelegate, status)
    End Sub

    ''' <summary>
    ''' The queue changed.
    ''' </summary>
    ''' <param name="sender">
    ''' The sender.
    ''' </param>
    ''' <param name="e">
    ''' The EventArgs.
    ''' </param>
    Private Sub QueueChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Me.LabelEncodeStatus.Text = "Next"
    End Sub


    ''' <summary>
    ''' The Encode Status has changed Handler
    ''' </summary>
    ''' <param name="sender">
    ''' The Sender
    ''' </param>
    ''' <param name="e">
    ''' The Encode Progress Event Args
    ''' </param>
    Private Sub EncodeStatusChanged(ByVal sender As Object, ByVal e As HandBrake.ApplicationServices.EventArgs.EncodeProgressEventArgs)

        Dim status As String

        Dim safedelegate As New EncodeResultSafe(AddressOf EncodeResult)
        If Me.queueProcessor.EncodeService.IsEncoding Then
            status = String.Format("{0:00.00}%,Time Remaining:{1},Elapsed: {2}",
                                e.PercentComplete,
                                e.EstimatedTimeLeft.ToString("g"),
                                e.ElapsedTime.ToString("g"))
        Else
            status = "Finish"
        End If
        Me.Invoke(safedelegate, status)
    End Sub




#End Region

#End Region


#Region "Upload Part"

#Region "1. Create Folder on Ftp"

    Private Sub BtnCreateFolder_Click(sender As Object, e As System.EventArgs) Handles BtnCreateFolder.Click
        Dim folderName As String = TextCreateFolder.Text
        If folderName = "" Then
            LabelUploadStatus.Text = "Please input folder name before you click this button"
        ElseIf TxtPassword.Text = "" Then
            LabelUploadStatus.Text = NO_PASSWORD
        Else
            Dim ftpBasePath As String
            If ComboFolderList.SelectedItem = "" Then
                ftpBasePath = "/"
            Else
                ftpBasePath = "/" & ComboFolderList.SelectedItem & "/"
            End If

            Dim folderPath As String
            folderPath = ftpBasePath & folderName

            Debug.Print("Folder Path = " & folderPath)

            LabelUploadStatus.Text = "Start to create " & folderName & " on FTP server"

            Dim ftp As AsynFTP = New AsynFTP(pFtpIP, pFtpUserName, TxtPassword.Text)

            ftp.CreateDirectory(folderPath)

        End If

    End Sub

    Public Sub CreateDirectoryCallBack(ar As IAsyncResult)
        Dim rs As RequestState = CType(ar.AsyncState, RequestState)

        Dim req As FtpWebRequest = rs.Request

        Dim response As FtpWebResponse = CType(req.EndGetResponse(ar), FtpWebResponse)

        Dim status As String = response.StatusCode

        Dim result As String
        If status = FtpStatusCode.PathnameCreated Then
            result = "Success"
        Else
            result = "Fail"
        End If

        response.Close()

        Dim safedelegate As New CreateDirectoryResultSafe(AddressOf CreateDirectoryResult)
        Me.Invoke(safedelegate, result) 'Invoke the TreadsafeDelegate
    End Sub

    Public Sub CreateDirectoryResult(result As String)
        LabelUploadStatus.Text = "Create Folder " & result
    End Sub

#End Region

#Region "2. List Ftp Folder"
    Private Sub BtnFolderList_Click(sender As Object, e As System.EventArgs) Handles BtnFolderList.Click
        If TxtPassword.Text = "" Then
            LabelUploadStatus.Text = NO_PASSWORD
        Else
            'Clear comlist content
            ComboFolderList.Items.Clear()

            LabelUploadStatus.Text = "Start to get folder list on FTP server"

            Dim ftp As AsynFTP = New AsynFTP(pFtpIP, pFtpUserName, TxtPassword.Text)

            ftp.ListDirectory("")

        End If
    End Sub

    Public Sub ListDirectoryCallBack(ar As IAsyncResult)

        Dim rs As RequestState = CType(ar.AsyncState, RequestState)

        Dim req As FtpWebRequest = rs.Request


        Try

            Dim response As FtpWebResponse = CType(req.EndGetResponse(ar), FtpWebResponse)
            Dim size As Long = response.ContentLength
            Dim datastream As Stream = response.GetResponseStream()
            Dim sr As New StreamReader(datastream)
            Dim result As String = sr.ReadToEnd()
            sr.Close()
            datastream.Close()
            response.Close()

            'replace CRLF to CR, remove last instance
            result = result.Replace(vbCrLf, vbCr).TrimEnd(Chr(13))
            'split the string into a list
            Dim list As New List(Of String)
            list.AddRange(result.Split(Chr(13)))

            Dim safedelegate As New ChangeFolderListSafe(AddressOf ChangeFloderList)
            Me.Invoke(safedelegate, list) 'Invoke the TreadsafeDelegate

        Catch ex As Exception

            Debug.Print("Could not connect to FTP server." & ex.Message)
            Dim safedelegate As New UploadFileProgressSafe(AddressOf UploadFileProgress)
            Me.Invoke(safedelegate, ex.Message)
            Return

        End Try




    End Sub

    Public Sub ChangeFloderList(list As List(Of String))

        For Each line As String In list
            Console.WriteLine(line)
            ComboFolderList.Items.Add(line)
        Next

        LabelUploadStatus.Text = "Get List Done"

    End Sub
#End Region

#Region "3. Upload File to Ftp"

    Private Sub ListUpload_DragDrop(sender As Object, e As DragEventArgs) Handles ListUpload.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim MyFiles() As String
            Dim i As Integer

            'Assignthefilestoanarray.
            MyFiles = e.Data.GetData(DataFormats.FileDrop)
            'Loopthroughthearrayandaddthefilestothelist.
            For i = 0 To MyFiles.Length - 1
                ListUpload.Items.Add(MyFiles(i))
            Next
        End If
    End Sub

    Private Sub ListUpload_DragEnter(sender As Object, e As DragEventArgs) Handles ListUpload.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.All
        End If
    End Sub

    Private Sub BtnUploadClear_Click(sender As Object, e As System.EventArgs) Handles BtnUploadClear.Click
        ListUpload.Items.Clear()
        LabelUploadStatus.Text = "Status"
    End Sub

    Private Sub BtnUploadCancel_Click(sender As Object, e As System.EventArgs) Handles BtnUploadCancel.Click
        'Console.WriteLine("a")
        LabelUploadStatus.Text = "Start to cancel..."
        For i = 0 To tasks.Count - 1
            tasks.Item(i).abort()
        Next
        tasks.Clear()
        LabelUploadStatus.Text = "Canceled"
    End Sub

    Private Sub BtnUploadStart_Click(sender As Object, e As System.EventArgs) Handles BtnUploadStart.Click
        If TxtPassword.Text = "" Then
            LabelUploadStatus.Text = NO_PASSWORD
        Else
            Dim count As Integer = ListUpload.Items.Count
            Debug.Print("Upload File Count = " & count)
            If count > 0 Then
                Dim i As Integer
                Dim file As String
                For i = 0 To count - 1
                    file = ListUpload.Items.Item(i)
                    Debug.Print("File " & i & "  is = " & file)
                    UploadFile(file, i + 1, count)
                Next
            Else
                LabelUploadStatus.Text = "Please choose files for uploading"
            End If
        End If
    End Sub


    Private Sub UploadFile(file, currentCount, totalCount)

        'Step 1, get folder name on ftp
        'Step 2, check folder exist or create it
        'Step 3, get file path on ftp
        'Step 4, upload file to ftp

        Dim fileInfo As System.IO.FileInfo
        fileInfo = My.Computer.FileSystem.GetFileInfo(file)

        Dim fileName As String = fileInfo.Name
        Dim fileExtension As String = fileInfo.Extension

        If isUploadFormat(fileExtension) Then
            Debug.Print("File Name = " & fileName)
            Debug.Print("File Extension = " & fileExtension)
            Dim folderName = "Program_" & fileName.Split(".").GetValue(0)

            Debug.Print("Folder Name = " & folderName)

            Dim ftpBasePath As String
            If ComboFolderList.SelectedItem = "" Then
                ftpBasePath = "/"
            Else
                ftpBasePath = "/" & ComboFolderList.SelectedItem & "/"
            End If

            Debug.Print("Ftp Base Path = " & ftpBasePath)

            Dim ftpPath As String
            ftpPath = ftpBasePath & folderName
            Debug.Print("Ftp Path = " & ftpPath)

            Dim ftpFile As String
            ftpFile = ftpPath & "/" & fileName
            Debug.Print("Ftp File = " & ftpFile)

            If Not FolderExistOnFtp(ftpBasePath, folderName) Then
                LabelUploadStatus.Text = "Creating folder on ftp"
                CreateFolderOnFtp(ftpPath)
                LabelUploadStatus.Text = "Folder created, start to upload file"
            End If

            LabelUploadStatus.Text = "Start to upload file"

            Dim ftp As AsynFTP = New AsynFTP(pFtpIP, pFtpUserName, TxtPassword.Text)

            tasks.Add(ftp)

            ftp.Upload(file, ftpFile, currentCount, totalCount)
        Else
            LabelUploadStatus.Text = "You should choose mp4 file"
        End If




    End Sub

    Public Sub UploadFileRequestCallBack(ar As IAsyncResult)

        Dim state As RequestState = CType(ar.AsyncState, RequestState)
        Dim requestStream As Stream = Nothing
        Dim safedelegate As New UploadFileProgressSafe(AddressOf UploadFileProgress)
        Dim result As String

        Try

            Dim fileInfo As System.IO.FileInfo
            fileInfo = My.Computer.FileSystem.GetFileInfo(state.FileName)

            Dim total As Integer = fileInfo.Length

            requestStream = state.Request.EndGetRequestStream(ar)

            Const bufferLength As Integer = 2048

            Dim buffer(bufferLength - 1) As Byte
            Dim count As Integer = 0
            Dim readBytes As Integer = 0
            Dim stream As FileStream = File.OpenRead(state.FileName)
            Do
                readBytes = stream.Read(buffer, 0, bufferLength)
                requestStream.Write(buffer, 0, readBytes)
                count += readBytes
                result = "Task " & state.currentCount & " of " & state.totalFileCount & " uploading " & FormatNumber((count / total) * 100, 2) & "%"
                Me.Invoke(safedelegate, result)
            Loop While readBytes <> 0
            Debug.Print("Writing {0} bytes to the stream.", count)
            requestStream.Close()
            state.Request.BeginGetResponse(New AsyncCallback(AddressOf UploadFileResponseCallBack), state)

        Catch ex As Exception
            Debug.Print("Could not get the request stream, error = " & ex.Message)
            Me.Invoke(safedelegate, ex.Message)
            Return
        End Try



    End Sub

    Public Sub UploadFileProgress(ByVal result As String)
        LabelUploadStatus.Text = result
    End Sub

    Public Sub UploadFileResponseCallBack(ByVal ar As IAsyncResult)

        Dim state As RequestState = CType(ar.AsyncState, RequestState)
        Dim response As FtpWebResponse = Nothing
        Dim safedelegate As New UploadFileProgressSafe(AddressOf UploadFileProgress)
        Try
            response = CType(state.Request.EndGetResponse(ar), FtpWebResponse)
            'response.Close()
            Me.Invoke(safedelegate, "Upload finish")
        Catch ex As Exception
            Debug.Print("Error getting response.")
            Me.Invoke(safedelegate, "Upload Fail")
        End Try

    End Sub


    Private Function CreateFolderOnFtp(ftpPath As String) As Boolean
        Try

            Dim client As FTPclient = New FTPclient(RequestURL(pFtpIP), pFtpUserName, TxtPassword.Text)

            Dim result As Boolean = client.FtpCreateDirectory(ftpPath)

            Return result

        Catch ex As Exception

            Return False

        End Try
    End Function

    Private Function FolderExistOnFtp(ftpBasePath As String, folderName As String) As Boolean

        Try

            Dim client As FTPclient = New FTPclient(RequestURL(pFtpIP), pFtpUserName, TxtPassword.Text)

            Dim folderList As List(Of String) = client.ListDirectory(ftpBasePath)

            Dim temp As String

            For i = 0 To folderList.Count - 1
                temp = folderList.Item(i)
                If temp.Contains(folderName) Then
                    Return True
                End If

            Next
            Return False

        Catch ex As Exception

            Return False

        End Try


    End Function

#End Region


#End Region


#End Region


End Class
