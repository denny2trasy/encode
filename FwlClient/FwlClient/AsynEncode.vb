Imports System.IO
Imports System.Text
Imports System.Threading

Public Class AsynEncode

    Public Class CurrentState
        Public Message As String
    End Class

    Public inputFile As String
    Public outputFile As String



    Public Sub DoEncode(ByVal worker As System.ComponentModel.BackgroundWorker, ByVal e As System.ComponentModel.DoWorkEventArgs)

        Dim state As CurrentState = New CurrentState

        Dim startInfo As New ProcessStartInfo("cmd.exe")

        Dim cmd As String = " d:\old_pc\fw-labs\win.converter\HandBrakeCLI.exe " + EncodeParams(inputFile, outputFile)

        Debug.Print("Run command = " & cmd)

        With startInfo
            .Arguments = "/c" + cmd
            .RedirectStandardError = True
            .RedirectStandardOutput = True
            .UseShellExecute = False
            .CreateNoWindow = True
        End With

        Dim p As Process = Process.Start(startInfo)

        Dim stroutput As String = p.StandardOutput.ReadToEnd

        Debug.Print(stroutput)

        'state.Message = stroutput
        'worker.ReportProgress(0, state)

        'AddHandler p.OutputDataReceived, AddressOf MainForm.OutPutHandler

        'p.BeginOutputReadLine()



        p.WaitForExit()

        p.Close()

        'Dim myStreamReader As StreamReader = p.StandardOutput

        'Do
        '    state.Message = myStreamReader.ReadLine
        '    Debug.Print("Encode Message = " & state.Message)
        '    worker.ReportProgress(0, state)
        'Loop While myStreamReader.ReadToEnd

        'state.Message = "Worker Finish"
        'worker.ReportProgress(0, state)

        'Dim stroutput As String = p.StandardOutput.ReadToEnd
        'Debug.Print("AsynEncode : ")
        'Debug.Print(stroutput)

        'p.WaitForExit()

        'If stroutput.Length <> 0 Then
        '    state.Message = stroutput
        '    worker.ReportProgress(0, state)
        '    Debug.Print(stroutput)
        'End If

    End Sub



End Class
