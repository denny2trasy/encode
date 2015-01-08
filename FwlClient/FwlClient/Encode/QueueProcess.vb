Imports System.ComponentModel

Namespace FwlClient.Encode
    Public Class QueueProcess

#Region "Constants and Fields"

        Dim QueueLock As Object = New Object
        Dim queue As BindingList(Of QEncodeTask) = New BindingList(Of QEncodeTask)
        Dim queueFile As String
        Dim isclearCompleted As Boolean

#End Region

#Region "Constructors and Destructors"

        Sub New(enService As EncodeService)

            Me.EncodeService = enService

            Me.queueFile = String.Format("hb_queue_recovery{0}.xml", GeneralUtilities.ProcessId)

        End Sub

#End Region

#Region "Delegates"

        Public Delegate Sub QueueProgressStatus(ByVal sender As Object, ByVal e As QueueProgressEventArgs)
        Public Delegate Sub QueueCompletedEventDelegate(ByVal sender As Object, ByVal e As QueueCompletedEventArgs)

#End Region

#Region "Events"


        Public Event JobProcessingStarted As QueueProgressStatus

        ''' <summary>
        ''' Fires when a job is Added, Removed or Re-Ordered.
        ''' Should be used for triggering an update of the Queue Window.
        ''' </summary>
        Public Event QueueChanged As EventHandler

        ''' <summary>
        ''' Fires when the entire encode queue has completed.
        ''' </summary>
        Public Event QueueCompleted As QueueCompletedEventDelegate

        ''' <summary>
        ''' Fires when a pause to the encode queue has been requested.
        ''' </summary>
        Public Event QueuePaused As EventHandler

#End Region

#Region "Properties"


        ''' <summary>
        ''' Gets the number of jobs in the queue;
        ''' </summary>
        Public ReadOnly Property Count As Integer
            Get
                Return Me.queue.Count
            End Get
        End Property

        ''' <summary>
        ''' Gets the IEncodeService instance.
        ''' </summary>
        '''
        Private _encodeService As EncodeService
        Public Property EncodeService As EncodeService
            Get
                Return _encodeService
            End Get
            Set(value As EncodeService)
                _encodeService = value
            End Set
        End Property

        ''' <summary>
        ''' Gets a value indicating whether IsProcessing.
        ''' </summary>
        ''' 
        Private _IsProcessing As Boolean
        Public Property IsProcessing As Boolean
            Get
                Return _IsProcessing
            End Get
            Set(value As Boolean)
                _IsProcessing = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets Last Processed Job.
        ''' This is set when the job is poped of the queue by GetNextJobForProcessing();
        ''' </summary>
        ''' 
        Private _lastProcessedJob As QEncodeTask
        Public Property LastProcessedJob As QEncodeTask
            Get
                Return _lastProcessedJob
            End Get
            Set(value As QEncodeTask)
                _lastProcessedJob = value
            End Set
        End Property


#End Region


#Region "Public Methods"

        ''' <summary>
        ''' Add a job to the Queue. 
        ''' This method is Thread Safe.
        ''' </summary>
        ''' <param name="job">
        ''' The encode Job object.
        ''' </param>
        Public Sub Add(ByVal job As QEncodeTask)
            Me.queue.Add(job)
            'Debug.Print("Add Task, Now queue count = " & Me.queue.Count)
            'Me.InvokeQueueChanged(EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Clear down all Queue Items
        ''' </summary>
        Public Sub Clear()
            Me.queue.Clear()
            Me.IsProcessing = False
            'Dim deleteList As List(Of QEncodeTask) = Me.queue.ToList
            'For Each item As QEncodeTask In deleteList
            '    Me.queue.Remove(item)
            'Next
            'Me.InvokeQueueChanged(EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Starts encoding the first job in the queue and continues encoding until all jobs
        ''' have been encoded.
        ''' </summary>
        ''' <param name="isClearCompleted">
        ''' The is Clear Completed.
        ''' </param>
        Public Sub Start(ByVal isClearCompleted As Boolean)
            'Debug.Print("Current queue count = " & Me.queue.Count)
            Me.isclearCompleted = isClearCompleted
            AddHandler Me.EncodeService.EncodeCompleted, AddressOf Me.EncodeServiceEncodeCompleted
            If Not Me.EncodeService.IsEncoding Then
                Me.ProcessNextJob()
            End If
            Me.IsProcessing = True


            'If Me.IsProcessing Then
            '    'Throw New Exception("Already Processing the Queue")
            'End If
            'Me.isclearCompleted = isClearCompleted
            'AddHandler Me.EncodeService.EncodeCompleted, AddressOf Me.EncodeServiceEncodeCompleted

            'If Not Me.EncodeService.IsEncoding Then
            '    Me.ProcessNextJob()
            'End If
            'Me.IsProcessing = True
        End Sub


        ''' <summary>
        ''' Requests a pause of the encode queue.
        ''' </summary>
        Public Sub Pause()
            Me.InvokeQueuePaused(EventArgs.Empty)
            Me.IsProcessing = False
        End Sub

        ''' <summary>
        ''' Clear down the Queue�s completed items
        ''' </summary>
        Public Sub ClearCompleted()
            If (Me.queue.Count > 0) Then
                For i = 0 To Me.queue.Count - 1
                    Dim job As QEncodeTask = CType(queue.Item(i), QEncodeTask)
                    If (job.Status.Equals(QueueItemStatus.Completed)) Then
                        Me.queue.Remove(job)
                    End If
                Next
            End If
            'Me.InvokeQueueChanged(EventArgs.Empty)
        End Sub


        ''' <summary>
        ''' Run through all the jobs on the queue.
        ''' </summary>
        Private Sub ProcessNextJob()
            Dim job As QEncodeTask = Me.GetNextJobForProcessing
            If (Not job Is Nothing) Then
                'Me.InvokeJobProcessingStarted(New QueueProgressEventArgs(job))
                Me.EncodeService.Start(job)
            Else
                'Debug.Print("ProcessNextJob")
                ' No more jobs to process, so unsubscribe the event
                RemoveHandler Me.EncodeService.EncodeCompleted, AddressOf Me.EncodeServiceEncodeCompleted
                ' Fire the event to tell connected services.
                Me.OnQueueCompleted(New QueueCompletedEventArgs(False))
            End If
        End Sub


        ''' <summary>
        ''' Get the first job on the queue for processing.
        ''' This also removes the job from the Queue and sets the LastProcessedJob
        ''' </summary>
        ''' <returns>
        ''' An encode Job object.
        ''' </returns>
        Public Function GetNextJobForProcessing() As QEncodeTask
            If (Me.queue.Count > 0) Then
                For i = 0 To Me.queue.Count - 1
                    Dim job As QEncodeTask = CType(queue.Item(i), QEncodeTask)
                    If (job.Status.Equals(QueueItemStatus.Waiting)) Then
                        job.Status = QueueItemStatus.InProgress
                        Me.LastProcessedJob = job
                        'Me.InvokeQueueChanged(EventArgs.Empty)
                        Return job
                    End If
                Next
                Return Nothing
            End If
            Return Nothing
        End Function



#End Region

#Region "Methods"


        ''' <summary>
        ''' After an encode is complete, move onto the next job.
        ''' </summary>
        ''' <param name="sender">
        ''' The sender.
        ''' </param>
        ''' <param name="e">
        ''' The EncodeCompletedEventArgs.
        ''' </param>
        Private Sub EncodeServiceEncodeCompleted(ByVal sender As Object, ByVal e As EncodeCompletedEventArgs)

            Me.LastProcessedJob.Status = QueueItemStatus.Completed

            ' Clear the completed item of the queue if the setting is set.
            'If isclearCompleted Then
            '    Me.ClearCompleted()
            'End If
            If Not e.Successful Then
                Me.LastProcessedJob.Status = QueueItemStatus.Error
                'Me.Pause()
            End If
            ' Handling Log Data 
            'Me.EncodeService.ProcessLogs(Me.LastProcessedJob.Task.Destination, "")
            ' Move onto the next job.

            If Me.IsProcessing Then
                Me.ProcessNextJob()
            Else
                'Debug.Print("EncodeServiceEncodeCompleted!!!")
                RemoveHandler Me.EncodeService.EncodeCompleted, AddressOf Me.EncodeServiceEncodeCompleted
                Me.OnQueueCompleted(New QueueCompletedEventArgs(True))
            End If
        End Sub


        ''' <summary>
        ''' Invoke the JobProcessingStarted event
        ''' </summary>
        ''' <param name="e">
        ''' The QueueProgressEventArgs.
        ''' </param>
        Private Sub InvokeJobProcessingStarted(ByVal e As QueueProgressEventArgs)
            RaiseEvent JobProcessingStarted(Me, e)
        End Sub

        ''' <summary>
        ''' Invoke the Queue Changed Event
        ''' </summary>
        ''' <param name="e">
        ''' The e.
        ''' </param>
        Private Sub InvokeQueueChanged(ByVal e As EventArgs)
            RaiseEvent QueueChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Invoke the QueuePaused event
        ''' </summary>
        ''' <param name="e">
        ''' The EventArgs.
        ''' </param>
        Private Sub InvokeQueuePaused(ByVal e As EventArgs)
            Me.IsProcessing = False
            RaiseEvent QueuePaused(Me, e)
        End Sub

        ''' <summary>
        ''' The on queue completed.
        ''' </summary>
        ''' <param name="e">
        ''' The e.
        ''' </param>
        Protected Overridable Sub OnQueueCompleted(ByVal e As QueueCompletedEventArgs)
            RaiseEvent QueueCompleted(Me, e)
            Me.IsProcessing = False
        End Sub




#End Region



    End Class
End Namespace

