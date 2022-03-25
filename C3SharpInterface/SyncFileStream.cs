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
using System.Text;
using System.IO;
using C3SharpInterface.Requests;
using C3SharpInterface.Responses;

namespace C3SharpInterface
{
    public class SyncFileStream : Stream
    {
        private bool _writable = false;
        private int _offset = 0;
        private int _size = 0;
        private SyncClient _client = null;
        private string _fileName = "";
        private CopyFlag _flags = CopyFlag.None;
        private ushort _tag = 0;

        public SyncFileStream(SyncClient client, string fileName, bool write = false, CopyFlag flags = CopyFlag.None, ushort tag = 0)
        {
            _client = client;
            _fileName = fileName;
            _writable = write;
            _flags = flags;
            _tag = tag;

            if (_client != null && !_writable)
            {
                FileReadResponse response = (FileReadResponse) _client.SendRequest(new FileReadBeginRequest(_fileName, _flags, _tag));
                _size = response.Success ? response.ResultingSize : 0;
            }
        }

        public override bool CanRead { get { return (_size > 0) && !_writable; } }

        public override bool CanSeek { get { return (_size > 0); } }

        public override bool CanWrite { get { return (_size > 0) && _writable; } }

        public override long Length { get { return (long) _size; } }

        public override long Position
        {
            get
            {
                return (long) _offset;
            }
            set
            {
                if (value < 0)
                {
                    _offset = 0;
                }
                else if (value > _size)
                {
                    _offset = _size;
                }
                else
                {
                    _offset = (int) value;
                }
            }
        }

        public override void Flush()
        {
            if (_size <= 0)
                return;

            if (_client == null)
            {
                _size = 0;
                return;
            }

            if (_writable)
            {
                _client.SendRequest(new FileWriteEndRequest(_fileName, _flags, _tag));
            }
            else
            {
                _client.SendRequest(new FileReadEndRequest(_tag));
            }

            _size = 0;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_client == null || _writable || _size < 1)
                return 0;

            int result = _offset;

            FileReadDataRequest request = new FileReadDataRequest(_offset, count, _tag);

            while (count > 0)
            {
                request.DataOffset = _offset;
                request.DataSize = count;

                FileReadResponse response = (FileReadResponse) _client.SendRequest(request);
                
                if (!response.Success || response.ResultingSize < 1)
                    break;

                response.GetData(buffer, offset);
                offset += response.ResultingSize;

                _offset += response.ResultingSize;
                count -= response.ResultingSize;
            }

            return _offset - result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    break;
                case SeekOrigin.Current:
                    offset += _offset;
                    break;
                case SeekOrigin.End:
                    offset += _size;
                    break;
            }

            if (offset < 0)
            {
                _offset = 0;
            }
            else if (offset > _size)
            {
                _offset = _size;
            }
            else
            {
                _offset = (int) offset;
            }

            return (long) _offset;
        }

        public override void SetLength(long value)
        {
            if (_client != null && _writable && value > 0)
            {
                FileWriteResponse response = (FileWriteResponse) _client.SendRequest(new FileWriteBeginRequest((int) value, _tag));
                _size = response.Success ? response.ResultingSize : 0;
            }
            else if (_writable)
            {
                _size = 0;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_client == null || !_writable || _size < 1)
                return;

            FileWriteDataRequest request = new FileWriteDataRequest(_tag);

            while (count > 0)
            {
                request.SetData(buffer, offset, count, _offset);

                FileWriteResponse response = (FileWriteResponse) _client.SendRequest(request);

                if (!response.Success || response.ResultingSize < 1)
                    break;

                offset += response.ResultingSize;

                _offset += response.ResultingSize;
                count -= response.ResultingSize;
            }
        }
    }
}
