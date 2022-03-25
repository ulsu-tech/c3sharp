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

namespace C3SharpInterface.Requests
{
    public class FileWriteDataRequest : Request
    {
        public const int SafePayloadSize = 64512;

        private int _offsetOfOffset;
        private int _offsetOfSize;

        private void Prepare()
        {
            Reset();
            Append((byte) FileIoOperation.Data);
            
            _offsetOfOffset = _offset;
            Append((uint) 0);

            _offsetOfSize = _offset;
            Append((uint) 0);
        }

        public int SetData(byte[] data, int dataOffset = 0, int dataSize = -1, int fileOffset = -1)
        {
            Prepare();

            if (data.Length < 1 || dataOffset < 0 || dataOffset >= data.Length)
                return 0;

            if (dataSize < 0)
                dataSize = data.Length;

            if (dataOffset + dataSize > data.Length)
                dataSize = data.Length - dataOffset;

            if (dataSize == 0)
                return 0;

            if (dataSize > SafePayloadSize)
                dataSize = SafePayloadSize;

            if (fileOffset < 0)
                fileOffset = dataOffset;

            Append(data, dataOffset, dataSize);

            Set(_offsetOfOffset, (uint) fileOffset);
            Set(_offsetOfSize, (uint) dataSize);

            return dataSize;
        }

        public FileWriteDataRequest(ushort tag = 0)
        {
            Tag = tag;
            Type = Command.FileWriteContent;

            Prepare();
        }

        public FileWriteDataRequest(byte[] data, int dataOffset = 0, int dataSize = -1, int fileOffset = -1, ushort tag = 0)
        {
            Tag = tag;
            Type = Command.FileWriteContent;

            SetData(data, dataOffset, dataSize, fileOffset);
        }
    }
}
