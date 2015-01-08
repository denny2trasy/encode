Namespace FwlClient.Encode
    Public Class EncodeTask

#Region "Property"
        Private _Destination As String
        Public Property Destination As String
            Get
                Return _Destination
            End Get
            Set(value As String)
                _Destination = value
            End Set
        End Property

        Private _SourceFile As String
        Public Property SourceFile As String
            Get
                Return _SourceFile
            End Get
            Set(value As String)
                _SourceFile = value
            End Set
        End Property
#End Region

#Region "Constructors"

        Sub New()

        End Sub

        Sub New(inputFile As String, outpuFile As String)
            Me.SourceFile = inputFile
            Me.Destination = outpuFile
        End Sub

#End Region
    End Class
End Namespace

