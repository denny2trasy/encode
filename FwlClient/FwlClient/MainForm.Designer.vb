<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意:  以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.BtnHelp = New System.Windows.Forms.Button()
        Me.BtnSClear = New System.Windows.Forms.Button()
        Me.BtnSSave = New System.Windows.Forms.Button()
        Me.BtnSLoad = New System.Windows.Forms.Button()
        Me.TxtFtpIP = New System.Windows.Forms.TextBox()
        Me.TxtFtpUserName = New System.Windows.Forms.TextBox()
        Me.TxtDefaultPath = New System.Windows.Forms.TextBox()
        Me.LabFtpIP = New System.Windows.Forms.Label()
        Me.LabFtpUserName = New System.Windows.Forms.Label()
        Me.LabDefaultPath = New System.Windows.Forms.Label()
        Me.PanelLeft = New System.Windows.Forms.Panel()
        Me.LabelEncodeStatus = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.BtnEncodeStart = New System.Windows.Forms.Button()
        Me.BtnEncodeClear = New System.Windows.Forms.Button()
        Me.BtnEncodeCancel = New System.Windows.Forms.Button()
        Me.ListEncode = New System.Windows.Forms.ListBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.BtnFolderList = New System.Windows.Forms.Button()
        Me.BtnCreateFolder = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.BtnUploadStart = New System.Windows.Forms.Button()
        Me.BtnUploadClear = New System.Windows.Forms.Button()
        Me.BtnUploadCancel = New System.Windows.Forms.Button()
        Me.LabelUploadStatus = New System.Windows.Forms.Label()
        Me.ComboFolderList = New System.Windows.Forms.ComboBox()
        Me.TextCreateFolder = New System.Windows.Forms.TextBox()
        Me.TxtPassword = New System.Windows.Forms.TextBox()
        Me.ListUpload = New System.Windows.Forms.ListBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.PanelRight = New System.Windows.Forms.Panel()
        Me.PanelLeft.SuspendLayout()
        Me.PanelRight.SuspendLayout()
        Me.SuspendLayout()
        '
        'BtnHelp
        '
        resources.ApplyResources(Me.BtnHelp, "BtnHelp")
        Me.BtnHelp.Name = "BtnHelp"
        Me.BtnHelp.UseVisualStyleBackColor = True
        '
        'BtnSClear
        '
        resources.ApplyResources(Me.BtnSClear, "BtnSClear")
        Me.BtnSClear.Name = "BtnSClear"
        Me.BtnSClear.UseVisualStyleBackColor = True
        '
        'BtnSSave
        '
        resources.ApplyResources(Me.BtnSSave, "BtnSSave")
        Me.BtnSSave.Name = "BtnSSave"
        Me.BtnSSave.UseVisualStyleBackColor = True
        '
        'BtnSLoad
        '
        resources.ApplyResources(Me.BtnSLoad, "BtnSLoad")
        Me.BtnSLoad.Name = "BtnSLoad"
        Me.BtnSLoad.UseVisualStyleBackColor = True
        '
        'TxtFtpIP
        '
        resources.ApplyResources(Me.TxtFtpIP, "TxtFtpIP")
        Me.TxtFtpIP.Name = "TxtFtpIP"
        '
        'TxtFtpUserName
        '
        resources.ApplyResources(Me.TxtFtpUserName, "TxtFtpUserName")
        Me.TxtFtpUserName.Name = "TxtFtpUserName"
        '
        'TxtDefaultPath
        '
        resources.ApplyResources(Me.TxtDefaultPath, "TxtDefaultPath")
        Me.TxtDefaultPath.Name = "TxtDefaultPath"
        '
        'LabFtpIP
        '
        resources.ApplyResources(Me.LabFtpIP, "LabFtpIP")
        Me.LabFtpIP.Name = "LabFtpIP"
        '
        'LabFtpUserName
        '
        resources.ApplyResources(Me.LabFtpUserName, "LabFtpUserName")
        Me.LabFtpUserName.Name = "LabFtpUserName"
        '
        'LabDefaultPath
        '
        resources.ApplyResources(Me.LabDefaultPath, "LabDefaultPath")
        Me.LabDefaultPath.Name = "LabDefaultPath"
        '
        'PanelLeft
        '
        resources.ApplyResources(Me.PanelLeft, "PanelLeft")
        Me.PanelLeft.BackColor = System.Drawing.SystemColors.ControlLight
        Me.PanelLeft.Controls.Add(Me.LabelEncodeStatus)
        Me.PanelLeft.Controls.Add(Me.LabDefaultPath)
        Me.PanelLeft.Controls.Add(Me.Label6)
        Me.PanelLeft.Controls.Add(Me.Label5)
        Me.PanelLeft.Controls.Add(Me.BtnEncodeStart)
        Me.PanelLeft.Controls.Add(Me.BtnEncodeClear)
        Me.PanelLeft.Controls.Add(Me.TxtDefaultPath)
        Me.PanelLeft.Controls.Add(Me.BtnEncodeCancel)
        Me.PanelLeft.Controls.Add(Me.ListEncode)
        Me.PanelLeft.Controls.Add(Me.Label2)
        Me.PanelLeft.Name = "PanelLeft"
        '
        'LabelEncodeStatus
        '
        resources.ApplyResources(Me.LabelEncodeStatus, "LabelEncodeStatus")
        Me.LabelEncodeStatus.Name = "LabelEncodeStatus"
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Name = "Label5"
        '
        'BtnEncodeStart
        '
        resources.ApplyResources(Me.BtnEncodeStart, "BtnEncodeStart")
        Me.BtnEncodeStart.Name = "BtnEncodeStart"
        Me.BtnEncodeStart.UseVisualStyleBackColor = True
        '
        'BtnEncodeClear
        '
        resources.ApplyResources(Me.BtnEncodeClear, "BtnEncodeClear")
        Me.BtnEncodeClear.Name = "BtnEncodeClear"
        Me.BtnEncodeClear.UseVisualStyleBackColor = True
        '
        'BtnEncodeCancel
        '
        resources.ApplyResources(Me.BtnEncodeCancel, "BtnEncodeCancel")
        Me.BtnEncodeCancel.Name = "BtnEncodeCancel"
        Me.BtnEncodeCancel.UseVisualStyleBackColor = True
        '
        'ListEncode
        '
        resources.ApplyResources(Me.ListEncode, "ListEncode")
        Me.ListEncode.AllowDrop = True
        Me.ListEncode.FormattingEnabled = True
        Me.ListEncode.Name = "ListEncode"
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'BtnFolderList
        '
        resources.ApplyResources(Me.BtnFolderList, "BtnFolderList")
        Me.BtnFolderList.Name = "BtnFolderList"
        Me.BtnFolderList.UseVisualStyleBackColor = True
        '
        'BtnCreateFolder
        '
        resources.ApplyResources(Me.BtnCreateFolder, "BtnCreateFolder")
        Me.BtnCreateFolder.Name = "BtnCreateFolder"
        Me.BtnCreateFolder.UseVisualStyleBackColor = True
        '
        'Label8
        '
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Name = "Label8"
        '
        'BtnUploadStart
        '
        resources.ApplyResources(Me.BtnUploadStart, "BtnUploadStart")
        Me.BtnUploadStart.Name = "BtnUploadStart"
        Me.BtnUploadStart.UseVisualStyleBackColor = True
        '
        'BtnUploadClear
        '
        resources.ApplyResources(Me.BtnUploadClear, "BtnUploadClear")
        Me.BtnUploadClear.Name = "BtnUploadClear"
        Me.BtnUploadClear.UseVisualStyleBackColor = True
        '
        'BtnUploadCancel
        '
        resources.ApplyResources(Me.BtnUploadCancel, "BtnUploadCancel")
        Me.BtnUploadCancel.Name = "BtnUploadCancel"
        Me.BtnUploadCancel.UseVisualStyleBackColor = True
        '
        'LabelUploadStatus
        '
        resources.ApplyResources(Me.LabelUploadStatus, "LabelUploadStatus")
        Me.LabelUploadStatus.Name = "LabelUploadStatus"
        '
        'ComboFolderList
        '
        resources.ApplyResources(Me.ComboFolderList, "ComboFolderList")
        Me.ComboFolderList.FormattingEnabled = True
        Me.ComboFolderList.Name = "ComboFolderList"
        '
        'TextCreateFolder
        '
        resources.ApplyResources(Me.TextCreateFolder, "TextCreateFolder")
        Me.TextCreateFolder.Name = "TextCreateFolder"
        '
        'TxtPassword
        '
        resources.ApplyResources(Me.TxtPassword, "TxtPassword")
        Me.TxtPassword.Name = "TxtPassword"
        '
        'ListUpload
        '
        resources.ApplyResources(Me.ListUpload, "ListUpload")
        Me.ListUpload.AllowDrop = True
        Me.ListUpload.FormattingEnabled = True
        Me.ListUpload.Name = "ListUpload"
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'PanelRight
        '
        resources.ApplyResources(Me.PanelRight, "PanelRight")
        Me.PanelRight.BackColor = System.Drawing.SystemColors.ControlLight
        Me.PanelRight.Controls.Add(Me.Label8)
        Me.PanelRight.Controls.Add(Me.BtnFolderList)
        Me.PanelRight.Controls.Add(Me.BtnCreateFolder)
        Me.PanelRight.Controls.Add(Me.TxtFtpIP)
        Me.PanelRight.Controls.Add(Me.BtnUploadStart)
        Me.PanelRight.Controls.Add(Me.ListUpload)
        Me.PanelRight.Controls.Add(Me.TxtPassword)
        Me.PanelRight.Controls.Add(Me.BtnUploadClear)
        Me.PanelRight.Controls.Add(Me.TxtFtpUserName)
        Me.PanelRight.Controls.Add(Me.BtnUploadCancel)
        Me.PanelRight.Controls.Add(Me.LabFtpUserName)
        Me.PanelRight.Controls.Add(Me.LabFtpIP)
        Me.PanelRight.Controls.Add(Me.LabelUploadStatus)
        Me.PanelRight.Controls.Add(Me.Label3)
        Me.PanelRight.Controls.Add(Me.TextCreateFolder)
        Me.PanelRight.Controls.Add(Me.ComboFolderList)
        Me.PanelRight.Name = "PanelRight"
        '
        'MainForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.PanelRight)
        Me.Controls.Add(Me.BtnSClear)
        Me.Controls.Add(Me.PanelLeft)
        Me.Controls.Add(Me.BtnSSave)
        Me.Controls.Add(Me.BtnSLoad)
        Me.Controls.Add(Me.BtnHelp)
        Me.MaximizeBox = False
        Me.Name = "MainForm"
        Me.PanelLeft.ResumeLayout(False)
        Me.PanelLeft.PerformLayout()
        Me.PanelRight.ResumeLayout(False)
        Me.PanelRight.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BtnHelp As System.Windows.Forms.Button
    Friend WithEvents BtnSClear As System.Windows.Forms.Button
    Friend WithEvents BtnSSave As System.Windows.Forms.Button
    Friend WithEvents BtnSLoad As System.Windows.Forms.Button
    Friend WithEvents TxtFtpIP As System.Windows.Forms.TextBox
    Friend WithEvents TxtFtpUserName As System.Windows.Forms.TextBox
    Friend WithEvents TxtDefaultPath As System.Windows.Forms.TextBox
    Friend WithEvents LabFtpIP As System.Windows.Forms.Label
    Friend WithEvents LabFtpUserName As System.Windows.Forms.Label
    Friend WithEvents LabDefaultPath As System.Windows.Forms.Label
    Friend WithEvents PanelLeft As System.Windows.Forms.Panel
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents LabelEncodeStatus As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents BtnUploadStart As System.Windows.Forms.Button
    Friend WithEvents BtnUploadClear As System.Windows.Forms.Button
    Friend WithEvents BtnUploadCancel As System.Windows.Forms.Button
    Friend WithEvents BtnEncodeStart As System.Windows.Forms.Button
    Friend WithEvents LabelUploadStatus As System.Windows.Forms.Label
    Friend WithEvents ComboFolderList As System.Windows.Forms.ComboBox
    Friend WithEvents TextCreateFolder As System.Windows.Forms.TextBox
    Friend WithEvents TxtPassword As System.Windows.Forms.TextBox
    Friend WithEvents BtnEncodeClear As System.Windows.Forms.Button
    Friend WithEvents BtnEncodeCancel As System.Windows.Forms.Button
    Friend WithEvents ListUpload As System.Windows.Forms.ListBox
    Friend WithEvents ListEncode As System.Windows.Forms.ListBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents BtnFolderList As System.Windows.Forms.Button
    Friend WithEvents BtnCreateFolder As System.Windows.Forms.Button
    Friend WithEvents PanelRight As System.Windows.Forms.Panel

End Class
