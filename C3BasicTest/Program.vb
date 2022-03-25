Imports System.Net
Imports System.Text
Imports C3SharpInterface
Imports C3SharpInterface.Requests
Imports C3SharpInterface.Responses

' Copyright (c) 2020-2022 Dmitry Lavygin <vdm.inbox@gmail.com>
' S.P. Kapitsa Research Institute of Technology of Ulyanovsk State University.
' All rights reserved.
'
' Redistribution and use in source and binary forms, with or without
' modification, are permitted provided that the following conditions are met:
'     1. Redistributions of source code must retain the above copyright
'        notice, this list of conditions and the following disclaimer.
'
'     2. Redistributions in binary form must reproduce the above copyright
'        notice, this list of conditions and the following disclaimer in the
'        documentation and/or other materials provided with the distribution.
'
'     3. Neither the name of the copyright holder nor the names of its
'        contributors may be used to endorse or promote products derived from
'        this software without specific prior written permission.
'
' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
' "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
' LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
' A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
' HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
' SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
' LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
' DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
' THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
' (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
' OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

Module Program

    Sub Main(args As String())
        Dim syncClient As SyncClient = New SyncClient()

        If args.Length > 0 Then
            syncClient.ConnectToHost(IPAddress.Parse(args(0)), 7000)
        Else
            syncClient.ConnectToHost(IPAddress.Parse("172.1.1.1"), 7000)
        End If


        Dim valueResponse As VariableValueResponse

        ' MESSAGE #0. READ VARIABLE (ASCII)
        valueResponse = syncClient.SendRequest(New ReadVariableRequest("$ACCU_STATE", True))
        Console.WriteLine("Command: {0}, Value: {1}", valueResponse.Type, valueResponse.Value)

        ' MESSAGE #1. WRITE VARIABLE (ASCII)
        valueResponse = syncClient.SendRequest(New WriteVariableRequest("$OV_PRO", "35", True))
        Console.WriteLine("Command: {0}, Value: {1}", valueResponse.Type, valueResponse.Value)

        ' MESSAGE #4. READ VARIABLE
        valueResponse = syncClient.SendRequest(New ReadVariableRequest("@PROXY_VERSION"))
        Console.WriteLine("Command: {0}, Value: {1}", valueResponse.Type, valueResponse.Value)

        ' MESSAGE #5. WRITE VARIABLE
        valueResponse = syncClient.SendRequest(New WriteVariableRequest("$OV_PRO", "10"))
        Console.WriteLine("Command: {0}, Value: {1}", valueResponse.Type, valueResponse.Value)


        Dim multipleResponse As VariableMultipleResponse

        ' MESSAGE #6. READ MULTIPLE VARIABLES
        Dim request6 As ReadMultipleRequest = New ReadMultipleRequest()
        request6.Add("PING")
        request6.Add("@PROXY_PORT")
        multipleResponse = syncClient.SendRequest(request6)
        Console.Write("Command: {0}, Values: ", multipleResponse.Type)
        For i As Integer = 0 To multipleResponse.Count - 1
            If i > 0 Then
                Console.Write(" | ")
            End If
            Console.Write(multipleResponse(i))
        Next
        Console.WriteLine("")


        ' MESSAGE #7. WRITE MULTIPLE VARIABLES
        Dim request7 As WriteMultipleRequest = New WriteMultipleRequest()
        request7.Add("$OV_PRO", "37")
        request7.Add("$OV_JOG", "100")
        multipleResponse = syncClient.SendRequest(request7)
        Console.Write("Command: {0}, Values: ", multipleResponse.Type)
        For j As Integer = 0 To multipleResponse.Count - 1
            If j > 0 Then
                Console.Write(" | ")
            End If
            Console.Write(multipleResponse(j))
        Next
        Console.WriteLine("")


        ' MESSAGE #10. PROGRAM CONTROL (SUBTYPE II)
        Dim request10 As ProgramControlRequest
        request10 = New ProgramControlRequest(ProgramControl.[Select], "/R1/CELL", "")

        Dim controlResponse As ProgramControlResponse
        controlResponse = syncClient.SendRequest(request10)
        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, ProgramControl: {3}", New Object() {controlResponse.Type, controlResponse.Success, controlResponse.ErrorCode, controlResponse.Command})

        ' MESSAGE #10. PROGRAM CONTROL (SUBTYPE I)
        request10 = New ProgramControlRequest(ProgramControl.Cancel, 1US)
        controlResponse = syncClient.SendRequest(request10)
        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, ProgramControl: {3}", New Object() {controlResponse.Type, controlResponse.Success, controlResponse.ErrorCode, controlResponse.Command})

        ' MESSAGE #11. MOTION CONTROL
        Dim request11 As MotionControlRequest = New MotionControlRequest(MotionType.PtpRelative, "{A6 10}")
        Dim motionResponse As MotionControlResponse = syncClient.SendRequest(request11)
        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, MotionType: {3}", New Object() {motionResponse.Type, motionResponse.Success, motionResponse.ErrorCode, motionResponse.MotionType})

        ' MESSAGE #12. KCP KEY EMULATION
        Dim request12 As KcpKeyRequest = New KcpKeyRequest(KcpActionType.Start, KcpKeyStatus.Pressed, 0UI, KcpKeyDirection.Forward, 0US)
        Dim keyResponse As KcpKeyResponse = syncClient.SendRequest(request12)
        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, KcpAction: {3}", New Object() {keyResponse.Type, keyResponse.Success, keyResponse.ErrorCode, keyResponse.Command})

        request12 = New KcpKeyRequest(KcpActionType.Start, KcpKeyStatus.Released)
        keyResponse = syncClient.SendRequest(request12)
        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, KcpAction: {3}", New Object() {keyResponse.Type, keyResponse.Success, keyResponse.ErrorCode, keyResponse.Command})

        request12 = New KcpKeyRequest(KcpActionType.[Stop])
        keyResponse = syncClient.SendRequest(request12)
        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, KcpAction: {3}", New Object() {keyResponse.Type, keyResponse.Success, keyResponse.ErrorCode, keyResponse.Command})

        ' MESSAGE #13. GET PROXY INFORMATION
        Dim proxyInfo As ProxyInformationResponse = syncClient.SendRequest(New ProxyInformationRequest())
        Console.WriteLine("Command: {0}, Version: {1}, DateTime: {2}, ComputerName: {3}", New Object() {proxyInfo.Type, proxyInfo.Version, proxyInfo.Time, proxyInfo.ComputerName})

        ' MESSAGE #14. GET PROXY FEATURES
        Dim proxyFeatures As ProxyFeaturesResponse = syncClient.SendRequest(New ProxyFeaturesRequest())
        Dim commands As List(Of Command) = proxyFeatures.AvailableCommands()
        Console.WriteLine("Command: {0}, Available Commands: ", proxyFeatures.Type)
        For k As Integer = 0 To commands.Count - 1
            Console.WriteLine("    #{0}. {1} ", CInt(commands(k)), commands(k))
        Next
        Console.WriteLine()

        ' MESSAGE #17. PERFORM PROXY BENCHMARK
        Dim request17 As ProxyBenchmarkRequest = New ProxyBenchmarkRequest(10)
        request17.Add("PING", "")
        request17.Add("$AXIS_ACT", "")
        request17.Add("$ACCU_STATE", "")
        Dim proxyBenchmark As ProxyBenchmarkResponse = syncClient.SendRequest(request17)
        Console.WriteLine("Command: {0}, Time Difference: {1}", proxyBenchmark.Type, proxyBenchmark.Difference)

        ' MESSAGE #20. SET FILE ATTRIBUTES
        Dim request20 As SetFileAttributesRequest = New SetFileAttributesRequest("KRC:/R1/PROGRAM/MASREF_USER.SRC", ItemAttribute.[ReadOnly], ItemAttribute.[ReadOnly])
        Dim response As Response = syncClient.SendRequest(request20)
        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode)

        ' MESSAGE #21. LIST DIRECTORY CONTENTS
        Dim request21 As ListDirectoryRequest = New ListDirectoryRequest("KRC:\")
        Dim response21 As ListDirectoryResponse = syncClient.SendRequest(request21)
        Console.WriteLine("Command: {0}, Directory Tree: ", response21.Type)
        For l As Integer = 0 To response21.Count - 1
            Console.WriteLine("    {0}", response21(l))
        Next
        Console.WriteLine()

        ' MESSAGE #22. CREATE NEW FILE
        response = syncClient.SendRequest(New CreateFileRequest("KRC:\R1\PROGRAM\C3_TEST"))
        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode)

        ' MESSAGE #23. DELETE FILE
        response = syncClient.SendRequest(New DeleteFileRequest("KRC:\R1\PROGRAM\C3_TEST"))
        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode)

        ' MESSAGE #24. COPY FILE
        ' MESSAGE #25. MOVE FILE
        If syncClient.SendRequest(New CreateFileRequest("KRC:\R1\PROGRAM\C3_TEST")).Success Then
            ' Copy
            response = syncClient.SendRequest(New CopyFileRequest("KRC:\R1\PROGRAM\C3_TEST", "KRC:\R1\PROGRAM\C3_TEST_COPY"))
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode)

            ' Move
            response = syncClient.SendRequest(New CopyFileRequest("KRC:\R1\PROGRAM\C3_TEST", "KRC:\R1\PROGRAM\C3_TEST_MOVED", True))
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode)
        End If

        ' MESSAGE #26. GET FILE PROPERTIES
        Dim request26 As FilePropertiesRequest = New FilePropertiesRequest("KRC:\R1\PROGRAM\C3_TEST")
        Dim response26 As FilePropertiesResponse = syncClient.SendRequest(request26)
        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response26.Type, response26.Success, response26.ErrorCode)
        If response26.Success Then
            Console.WriteLine("    Name: {0}", response26.FileName)
            Console.WriteLine("    Size: {0}", response26.FileSize)
            Console.WriteLine("    Type: {0}", response26.FileType)
            Console.WriteLine("    Attributes: {0}", response26.Attributes)
            Console.WriteLine("    Edit Mode: {0}", response26.EditMode)
            Console.WriteLine("    Creation Time: {0}", response26.CreationTime)
            Console.WriteLine("    Last Access Time: {0}", response26.LastAccessTime)
            Console.WriteLine("    Last Write Time: {0}", response26.LastWriteTime)
        End If

        ' MESSAGE #27. GET FILE FULL PATH
        Dim pathResponse As FilePathResponse = syncClient.SendRequest(New FilePathRequest("/R1/C3_TEST.SRC"))
        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, Path: {3}", New Object() {pathResponse.Type, pathResponse.Success, pathResponse.ErrorCode, pathResponse.Path})

        ' MESSAGE #28. GET KRC PATH
        pathResponse = syncClient.SendRequest(New FilePathRequest("KRC:\R1\PROGRAM\C3_TEST.SRC", PathType.KrcPath))
        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, Path: {3}", New Object() {pathResponse.Type, pathResponse.Success, pathResponse.ErrorCode, pathResponse.Path})

        ' MESSAGE #29. WRITE FILE CONTENT
        Dim contentString As String = ""
        For m As Integer = 0 To 20000 - 1
            contentString = contentString + "; Line " + m.ToString() + vbCrLf
        Next
        Dim content As Byte() = Encoding.ASCII.GetBytes(contentString)

        Dim stream As SyncFileStream = syncClient.FileWrite("C:\Windows\Temp\C3_FILE2.TXT", content.Length)
        stream.Write(content, 0, content.Length)
        stream.Flush()

        ' MESSAGE #30. READ FILE CONTENT
        stream = syncClient.FileRead("C:\Windows\Temp\C3_FILE2.TXT")
        Dim input2 As Byte() = New Byte(CInt(stream.Length) - 1) {}
        stream.Read(input2, 0, input2.Length)
        stream.Flush()

        ' MESSAGE #63. CONFIRM ALL
        response = syncClient.SendRequest(New ConfirmAllRequest())
        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode)

        syncClient.AbortConnection()
    End Sub

End Module
