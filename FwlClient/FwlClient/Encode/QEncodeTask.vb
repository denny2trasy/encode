Namespace FwlClient.Encode
    Public Class QEncodeTask

#Region "Constants"

        Sub New()
            Me.Status = QueueItemStatus.Waiting
        End Sub

        Sub New(task As EncodeTask)
            Me.Task = task
            Me.Status = QueueItemStatus.Waiting
        End Sub

#End Region

#Region "Properties"
        Private _status As QueueItemStatus
        Private _task As EncodeTask

        Public Property Status As QueueItemStatus
            Get
                Return _status
            End Get
            Set(value As QueueItemStatus)
                _status = value
            End Set
        End Property

        Public Property Task As EncodeTask
            Get
                Return _task
            End Get
            Set(value As EncodeTask)
                _task = value
            End Set
        End Property


#End Region

    End Class
End Namespace

