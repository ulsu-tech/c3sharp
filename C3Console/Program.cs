/*
 * Copyright (c) 2020-2022 Dmitry Lavygin <vdm.inbox@gmail.com>
 * S.P. Kapitsa Research Institute of Technology of Ulyanovsk State University.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     1. Redistributions of source code must retain the above copyright
 *        notice, this list of conditions and the following disclaimer.
 *
 *     2. Redistributions in binary form must reproduce the above copyright
 *        notice, this list of conditions and the following disclaimer in the
 *        documentation and/or other materials provided with the distribution.
 *
 *     3. Neither the name of the copyright holder nor the names of its
 *        contributors may be used to endorse or promote products derived from
 *        this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using C3SharpInterface;
using C3SharpInterface.Requests;
using C3SharpInterface.Responses;

namespace C3Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SyncClient syncClient = new SyncClient();
            syncClient.ConnectToHost(IPAddress.Parse(args.Length > 0 ? args[0] : "172.1.1.1"), 7000);

            // MESSAGE #0. READ VARIABLE (ASCII)
            VariableValueResponse valueResponse = (VariableValueResponse) syncClient.SendRequest(new ReadVariableRequest("$ACCU_STATE", true));
            Console.WriteLine("Command: {0}, Value: {1}", valueResponse.Type, valueResponse.Value);

            // MESSAGE #1. WRITE VARIABLE (ASCII)
            valueResponse = (VariableValueResponse) syncClient.SendRequest(new WriteVariableRequest("$OV_PRO", "35", true));
            Console.WriteLine("Command: {0}, Value: {1}", valueResponse.Type, valueResponse.Value);

            // MESSAGE #4. READ VARIABLE
            valueResponse = (VariableValueResponse) syncClient.SendRequest(new ReadVariableRequest("@PROXY_VERSION"));
            Console.WriteLine("Command: {0}, Value: {1}", valueResponse.Type, valueResponse.Value);

            // MESSAGE #5. WRITE VARIABLE
            valueResponse = (VariableValueResponse) syncClient.SendRequest(new WriteVariableRequest("$OV_PRO", "10"));
            Console.WriteLine("Command: {0}, Value: {1}", valueResponse.Type, valueResponse.Value);

            // MESSAGE #6. READ MULTIPLE VARIABLES
            ReadMultipleRequest request6 = new ReadMultipleRequest();
            request6.Add("PING");
            request6.Add("@PROXY_PORT");
            VariableMultipleResponse multipleResponse = (VariableMultipleResponse) syncClient.SendRequest(request6);
            Console.Write("Command: {0}, Values: ", multipleResponse.Type);
            for (int i = 0; i < multipleResponse.Count; i++)
            {
                if (i > 0)
                    Console.Write(" | ");
                Console.Write(multipleResponse[i]);
            }
            Console.WriteLine("");


            // MESSAGE #7. WRITE MULTIPLE VARIABLES
            WriteMultipleRequest request7 = new WriteMultipleRequest();
            request7.Add("$OV_PRO", "37");
            request7.Add("$OV_JOG", "100");
            multipleResponse = (VariableMultipleResponse) syncClient.SendRequest(request7);
            Console.Write("Command: {0}, Values: ", multipleResponse.Type);
            for (int i = 0; i < multipleResponse.Count; i++)
            {
                if (i > 0)
                    Console.Write(" | ");
                Console.Write(multipleResponse[i]);
            }
            Console.WriteLine("");


            // MESSAGE #10. PROGRAM CONTROL (SUBTYPE II)
            ProgramControlRequest request10 = new ProgramControlRequest(ProgramControl.Select, "/R1/CELL");
            ProgramControlResponse controlResponse = (ProgramControlResponse) syncClient.SendRequest(request10);
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, ProgramControl: {3}", controlResponse.Type, controlResponse.Success, controlResponse.ErrorCode, controlResponse.Command);

            // MESSAGE #10. PROGRAM CONTROL (SUBTYPE I)
            request10 = new ProgramControlRequest(ProgramControl.Cancel, 1);
            controlResponse = (ProgramControlResponse) syncClient.SendRequest(request10);
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, ProgramControl: {3}", controlResponse.Type, controlResponse.Success, controlResponse.ErrorCode, controlResponse.Command);

            // MESSAGE #11. MOTION CONTROL
            MotionControlRequest request11 = new MotionControlRequest(MotionType.PtpRelative, "{A6 10}");
            MotionControlResponse motionResponse = (MotionControlResponse) syncClient.SendRequest(request11);
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, MotionType: {3}", motionResponse.Type, motionResponse.Success, motionResponse.ErrorCode, motionResponse.MotionType);

            // MESSAGE #12. KCP KEY EMULATION
            KcpKeyRequest request12 = new KcpKeyRequest(KcpActionType.Start);
            KcpKeyResponse keyResponse = (KcpKeyResponse) syncClient.SendRequest(request12);
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, KcpAction: {3}", keyResponse.Type, keyResponse.Success, keyResponse.ErrorCode, keyResponse.Command);

            request12 = new KcpKeyRequest(KcpActionType.Start, KcpKeyStatus.Released);
            keyResponse = (KcpKeyResponse) syncClient.SendRequest(request12);
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, KcpAction: {3}", keyResponse.Type, keyResponse.Success, keyResponse.ErrorCode, keyResponse.Command);


            request12 = new KcpKeyRequest(KcpActionType.Stop);
            keyResponse = (KcpKeyResponse)syncClient.SendRequest(request12);
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, KcpAction: {3}", keyResponse.Type, keyResponse.Success, keyResponse.ErrorCode, keyResponse.Command);

            // MESSAGE #13. GET PROXY INFORMATION
            ProxyInformationResponse proxyInfo = (ProxyInformationResponse) syncClient.SendRequest(new ProxyInformationRequest());
            Console.WriteLine("Command: {0}, Version: {1}, DateTime: {2}, ComputerName: {3}", proxyInfo.Type, proxyInfo.Version, proxyInfo.Time, proxyInfo.ComputerName);

            // MESSAGE #14. GET PROXY FEATURES
            ProxyFeaturesResponse proxyFeatures = (ProxyFeaturesResponse) syncClient.SendRequest(new ProxyFeaturesRequest());
            List<Command> commands = proxyFeatures.AvailableCommands();
            Console.WriteLine("Command: {0}, Available Commands: ", proxyFeatures.Type);
            for (int i = 0; i < commands.Count; i++)
            {
                Console.WriteLine("    #{0}. {1} ", (int) commands[i], commands[i]);
            }
            Console.WriteLine();

            // MESSAGE #17. PERFORM PROXY BENCHMARK
            ProxyBenchmarkRequest request17 = new ProxyBenchmarkRequest(10);
            request17.Add("PING");
            request17.Add("$AXIS_ACT");
            request17.Add("$ACCU_STATE");
            ProxyBenchmarkResponse proxyBenchmark = (ProxyBenchmarkResponse) syncClient.SendRequest(request17);
            Console.WriteLine("Command: {0}, Time Difference: {1}", proxyBenchmark.Type, proxyBenchmark.Difference);

            // MESSAGE #20. SET FILE ATTRIBUTES
            SetFileAttributesRequest request20 = new SetFileAttributesRequest("KRC:/R1/PROGRAM/MASREF_USER.SRC", ItemAttribute.ReadOnly, ItemAttribute.ReadOnly);
            Response response = syncClient.SendRequest(request20);
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);

            // MESSAGE #21. LIST DIRECTORY CONTENTS
            ListDirectoryRequest request21 = new ListDirectoryRequest(@"KRC:\");
            ListDirectoryResponse response21 = (ListDirectoryResponse) syncClient.SendRequest(request21);
            Console.WriteLine("Command: {0}, Directory Tree: ", response21.Type);
            for (int i = 0; i < response21.Count; i++)
            {
                Console.WriteLine("    {0}", response21[i]);
            }
            Console.WriteLine();

            // MESSAGE #22. CREATE NEW FILE
            response = syncClient.SendRequest(new CreateFileRequest(@"KRC:\R1\PROGRAM\C3_TEST"));
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);

            // MESSAGE #23. DELETE FILE
            response = syncClient.SendRequest(new DeleteFileRequest(@"KRC:\R1\PROGRAM\C3_TEST"));
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);

            // MESSAGE #24. COPY FILE
            // MESSAGE #25. MOVE FILE
            if (syncClient.SendRequest(new CreateFileRequest(@"KRC:\R1\PROGRAM\C3_TEST")).Success)
            {
                // Copy
                response = syncClient.SendRequest(new CopyFileRequest(@"KRC:\R1\PROGRAM\C3_TEST", @"KRC:\R1\PROGRAM\C3_TEST_COPY"));
                Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);

                // Move
                response = syncClient.SendRequest(new CopyFileRequest(@"KRC:\R1\PROGRAM\C3_TEST", @"KRC:\R1\PROGRAM\C3_TEST_MOVED", true));
                Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);
            }

            // MESSAGE #26. GET FILE PROPERTIES
            FilePropertiesRequest request26 = new FilePropertiesRequest(@"KRC:\R1\PROGRAM\C3_TEST");
            FilePropertiesResponse response26 = (FilePropertiesResponse) syncClient.SendRequest(request26);
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response26.Type, response26.Success, response26.ErrorCode);
            if (response26.Success)
            {
                Console.WriteLine("    Name: {0}", response26.FileName);
                Console.WriteLine("    Size: {0}", response26.FileSize);
                Console.WriteLine("    Type: {0}", response26.FileType);
                Console.WriteLine("    Attributes: {0}", response26.Attributes);
                Console.WriteLine("    Edit Mode: {0}", response26.EditMode);
                Console.WriteLine("    Creation Time: {0}", response26.CreationTime);
                Console.WriteLine("    Last Access Time: {0}", response26.LastAccessTime);
                Console.WriteLine("    Last Write Time: {0}", response26.LastWriteTime);
            }

            // MESSAGE #27. GET FILE FULL PATH
            FilePathResponse pathResponse = (FilePathResponse) syncClient.SendRequest(new FilePathRequest("/R1/C3_TEST.SRC"));
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, Path: {3}", pathResponse.Type, pathResponse.Success, pathResponse.ErrorCode, pathResponse.Path);

            // MESSAGE #28. GET KRC PATH
            pathResponse = (FilePathResponse) syncClient.SendRequest(new FilePathRequest(@"KRC:\R1\PROGRAM\C3_TEST.SRC", PathType.KrcPath));
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, Path: {3}", pathResponse.Type, pathResponse.Success, pathResponse.ErrorCode, pathResponse.Path);

            // MESSAGE #29. WRITE FILE CONTENT
            string contentString = "";
            for (int i = 0; i < 20000; i++)
            {
                contentString += "; Line " + i.ToString() + "\r\n";
            }
            byte[] content = Encoding.ASCII.GetBytes(contentString);

            FileWriteResponse writeResponse = (FileWriteResponse) syncClient.SendRequest(new FileWriteBeginRequest(content.Length));
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, FileSize: {3}", writeResponse.Type, writeResponse.Success, writeResponse.ErrorCode, writeResponse.ResultingSize);

            writeResponse = (FileWriteResponse) syncClient.SendRequest(new FileWriteSizeRequest());
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, FileSize: {3}", writeResponse.Type, writeResponse.Success, writeResponse.ErrorCode, writeResponse.ResultingSize);

            if (writeResponse.Success && writeResponse.ResultingSize == content.Length)
            {
                int offset = 0;

                FileWriteDataRequest request29 = new FileWriteDataRequest();

                while (writeResponse.Success && offset < content.Length)
                {
                    offset += request29.SetData(content, offset);

                    writeResponse = (FileWriteResponse) syncClient.SendRequest(request29);
                    Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, DataSize: {3}, FileOffset: {4}", writeResponse.Type, writeResponse.Success, writeResponse.ErrorCode, writeResponse.ResultingSize, writeResponse.FileOffset);
                }

                if (writeResponse.Success)
                {
                    writeResponse = (FileWriteResponse) syncClient.SendRequest(new FileWriteEndRequest(@"C:\Windows\Temp\C3_FILE.TXT", CopyFlag.OverwriteExist | CopyFlag.OverwriteReadonly));
                    Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", writeResponse.Type, writeResponse.Success, writeResponse.ErrorCode);
                }
            }

            // MESSAGE #29. WRITE FILE CONTENT (ANOTHER WAY)
            SyncFileStream stream = syncClient.FileWrite(@"C:\Windows\Temp\C3_FILE2.TXT", content.Length);
            stream.Write(content, 0, content.Length);
            stream.Flush();


            // MESSAGE #30. READ FILE CONTENT
            FileReadResponse readResponse = (FileReadResponse) syncClient.SendRequest(new FileReadBeginRequest(@"C:\Windows\Temp\C3_FILE.TXT", CopyFlag.ForceBinary));
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, FileSize: {3}", readResponse.Type, readResponse.Success, readResponse.ErrorCode, readResponse.ResultingSize);

            readResponse = (FileReadResponse) syncClient.SendRequest(new FileReadSizeRequest());
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, FileSize: {3}", readResponse.Type, readResponse.Success, readResponse.ErrorCode, readResponse.ResultingSize);

            if (readResponse.Success && readResponse.ResultingSize > 0)
            {
                byte[] input = new byte[readResponse.ResultingSize];

                int offset = 0;

                FileReadDataRequest request30 = new FileReadDataRequest();

                while (readResponse.Success && offset < input.Length)
                {
                    request30.DataOffset = offset;
                    readResponse = (FileReadResponse) syncClient.SendRequest(request30);
                    Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}, DataSize: {3}, FileOffset: {4}", readResponse.Type, readResponse.Success, readResponse.ErrorCode, readResponse.ResultingSize, readResponse.FileOffset);
                    offset += readResponse.ResultingSize;

                    readResponse.GetData(input, readResponse.FileOffset);
                }

                if (readResponse.Success)
                {
                    readResponse = (FileReadResponse) syncClient.SendRequest(new FileReadEndRequest());
                    Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", readResponse.Type, readResponse.Success, readResponse.ErrorCode);
                }
            }

            // MESSAGE #30. READ FILE CONTENT (ANOTHER WAY)
            stream = syncClient.FileRead(@"C:\Windows\Temp\C3_FILE2.TXT");
            byte[] input2 = new byte[(int) stream.Length];
            stream.Read(input2, 0, input2.Length);
            stream.Flush();

            // MESSAGE #63. CONFIRM ALL
            response = syncClient.SendRequest(new ConfirmAllRequest());
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);

            syncClient.AbortConnection();
        }
    }
}
