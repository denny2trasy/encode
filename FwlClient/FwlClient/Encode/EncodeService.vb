Imports HandBrake.ApplicationServices
Imports System.IO
Imports System.EventArgs

Namespace FwlClient.Encode

    Public Delegate Sub EncodeProgessStatus(ByVal sender As Object, ByVal e As EncodeProgressEventArgs)
    Public Delegate Sub EncodeCompleted(ByVal sender As Object, ByVal e As EncodeCompletedEventArgs)


    Public Class EncodeService : Inherits EncodeBase


#Region "Private Variables"
        Dim processId As Integer
        Dim startTime As DateTime
        Dim initShutdown As Boolean
#End Region

#Region "Properties"
        ' Gets or sets The HB Process
        Private _hbProcess As Process
        Protected Property HbProcess As Process
            Get
                Return _hbProcess
            End Get
            Set(value As Process)
                _hbProcess = value
            End Set
        End Property

        Private _currentQTask As QEncodeTask
        Public Property CurrentQTask As QEncodeTask
            Get
                Return _currentQTask
            End Get
            Set(value As QEncodeTask)
                _currentQTask = value
            End Set
        End Property

#End Region

#Region "Constructors"

        Sub New()

        End Sub


#End Region

#Region "Public Methods"

        ' Start encode process
        Public Sub Start(task As QEncodeTask)
            Try
                If Me.IsEncoding Then
                    'Throw New GeneralApplicationException("HandBrake is already encodeing.", "Please try again in a minute", Nothing)
                End If
                Me.IsEncoding = True
                Me.CurrentQTask = task
                Dim handbrakeCLIPath As String = Path.Combine(Application.StartupPath, "HandBrakeCLI.exe")
                Dim args As String = EncodeParams(task.Task.SourceFile, task.Task.Destination)
                Dim startInfo As New ProcessStartInfo()
                With startInfo
                    .FileName = handbrakeCLIPath
                    .Arguments = args
                    .RedirectStandardError = True
                    .RedirectStandardOutput = True
                    .UseShellExecute = False
                    .CreateNoWindow = True
                End With

                HbProcess = New Process()
                HbProcess.StartInfo = startInfo
                HbProcess.Start()
                startTime = DateTime.Now
                processId = HbProcess.Id

                ' Error Message Receive
                AddHandler HbProcess.ErrorDataReceived, AddressOf HbProcErrorDataReceived
                HbProcess.BeginErrorReadLine()

                ' Output Message Receive
                AddHandler HbProcess.OutputDataReceived, AddressOf HbProcess_OutputDataReceived
                HbProcess.BeginOutputReadLine()


                If processId <> -1 Then
                    HbProcess.EnableRaisingEvents = True
                    AddHandler HbProcess.Exited, AddressOf HbProcessExited
                End If

                ' Fire the Encode Started Event
                Me.InvokeEncodeStarted(System.EventArgs.Empty)
            Catch exc As Exception
                Me.IsEncoding = False
                Me.InvokeEncodeCompleted(New EncodeCompletedEventArgs(False, exc, "An Error occured when trying to encode this source. ", Me.CurrentQTask.Task.Destination))
                Throw
            End Try
        End Sub

        ' Kill the CLI process
        Public Sub Cancel()
            Try
                If ((Not (Me.HbProcess) Is Nothing) _
                            AndAlso Not Me.HbProcess.HasExited) Then
                    Me.HbProcess.Kill()
                End If
            Catch ex As Exception
                ' No need to report anything to the user. If it fails, it's probably already stopped.
            End Try
        End Sub

        Private Sub HbProcessExited(ByVal sender As Object, ByVal e As System.EventArgs)
            HbProcess.WaitForExit()
            Try
                Me.HbProcess.CancelErrorRead()
                Me.HbProcess.CancelOutputRead()
                Me.ShutdownFileWriter()
                Me.IsEncoding = False
                Me.InvokeEncodeCompleted(New EncodeCompletedEventArgs(True, Nothing, String.Empty, Me.CurrentQTask.Task.Destination))
            Catch ex As Exception
                ' This exception doesn't warrent user interaction, but it should be logged (TODO)
            End Try
        End Sub

        ''' <summary>
        ''' Recieve the Standard Error information and process it
        ''' </summary>
        ''' <param name="sender">
        ''' The Sender Object
        ''' </param>
        ''' <param name="e">
        ''' DataReceived EventArgs
        ''' </param>
        ''' <remarks>
        ''' Worker Thread.
        ''' </remarks>
        Private Sub HbProcErrorDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
            If Not String.IsNullOrEmpty(e.Data) Then
                If (initShutdown _
                            AndAlso (Me.LogBuffer.Length < 25000000)) Then
                    initShutdown = False
                    ' Reset this flag.
                End If
                If ((Me.LogBuffer.Length > 25000000) _
                            AndAlso Not initShutdown) Then
                    Me.ProcessLogMessage("ERROR: Initiating automatic shutdown of encode process. The size of the log file indicates that there" & _
                        " is an error! ")
                    initShutdown = True
                    Me.Stop()
                End If
                Me.ProcessLogMessage(e.Data)
            End If
        End Sub

        ''' <summary>
        ''' The hb process output data received.
        ''' </summary>
        ''' <param name="sender">
        ''' The sender.
        ''' </param>
        ''' <param name="e">
        ''' The e.
        ''' </param>
        Private Sub HbProcess_OutputDataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
            If (Not String.IsNullOrEmpty(e.Data) _
                        AndAlso Me.IsEncoding) Then
                Dim eventArgs As EncodeProgressEventArgs = Me.ReadEncodeStatus(e.Data, Me.startTime)
                If (Not (eventArgs) Is Nothing) Then
                    If Not Me.IsEncoding Then
                        ' We can get events out of order since the CLI progress is monitored on a background thread.
                        ' So make sure we don't send a status update after an encode complete event.
                        Return
                    End If
                    Me.InvokeEncodeStatusChanged(eventArgs)
                End If
            End If
        End Sub

#End Region







    End Class


End Namespace

