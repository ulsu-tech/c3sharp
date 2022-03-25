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
using System.Net;
using System.Text;

namespace C3SharpInterface
{
    public class SyncClient
    {
        private AsyncClient _client = new AsyncClient();

        public uint ConnectTimeout
        {
            get
            {
                return _client.ConnectTimeout;
            }
            set
            {
                _client.ConnectTimeout = value;
            }
        }

        public uint RequestTimeout
        {
            get
            {
                return _client.RequestTimeout;
            }
            set
            {
                _client.RequestTimeout = value;
            }
        }

        public void AbortConnection()
        {
            _client.AbortConnection();
            _client.FetchEvent();
        }

        public void ConnectToHost(IPAddress address, int port)
        {
            _client.ConnectToHost(address, port);

            _client.Signal.WaitOne();
            Event ev = _client.FetchEvent();

            if (ev.Type != Event.TypeEnum.Connected)
            {
                if (ev.Exception != null)
                {
                    throw ev.Exception;
                }
                else if (ev.TimedOut)
                {
                    throw new TimeoutException();
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public Response SendRequest(Request request)
        {
            _client.SendRequest(request);

            _client.Signal.WaitOne();

            Event ev = _client.FetchEvent();
            if (ev.Type != Event.TypeEnum.DataReceived)
            {
                if (ev.Exception != null)
                {
                    throw ev.Exception;
                }
                else if (ev.TimedOut)
                {
                    throw new TimeoutException();
                }
                else
                {
                    throw new Exception();
                }
            }

            return ev.Response;
        }

        public SyncFileStream FileRead(string fileName, CopyFlag flags = CopyFlag.None, ushort tag = 0)
        {
            return new SyncFileStream(this, fileName, false, flags, tag);
        }

        public SyncFileStream FileWrite(string fileName, int fileSize = 0, CopyFlag flags = CopyFlag.None, ushort tag = 0)
        {
            SyncFileStream result = new SyncFileStream(this, fileName, true, flags, tag);
            result.SetLength(fileSize);
            return result;
        }
    }
}
