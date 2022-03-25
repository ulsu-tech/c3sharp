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

namespace C3SharpInterface
{
    public class Request
    {
        protected int _offset = 0;
        protected bool _valid = true;

        public ushort Tag = 0;
        public Command Type = Command.Invalid;
        public byte[] Payload = new byte[65534];
        public bool Valid
        {
            get { return _valid && (Type != Command.Invalid); }
        }
        public int Size
        {
            get { return (_valid && _offset >= 0 && _offset <= Payload.Length) ? _offset : 0; }
        }
    
        public void Reset()
        {
            _offset = 0;
            _valid = true;
        }

        public void Append(bool value)
        {
            Append((byte) (value ? 1 : 0));
        }

        public void Append(byte[] data)
        {
            if (!_valid || _offset + data.Length > Payload.Length)
            {
                _valid = false;
                return;
            }

            Array.Copy(data, 0, Payload, _offset, data.Length);
            _offset += data.Length;
        }

        public void Append(byte[] data, int dataOffset, int dataSize)
        {
            if (!_valid || dataOffset < 0 || dataSize < 0 || _offset + dataSize > Payload.Length)
            {
                _valid = false;
                return;
            }

            Array.Copy(data, dataOffset, Payload, _offset, dataSize);
            _offset += dataSize;
        }

        public void Append(sbyte value)
        {
            if (!_valid || _offset + sizeof(sbyte) > Payload.Length)
            {
                _valid = false;
                return;
            }
            Payload[_offset++] = (byte) value;
        }

        public void Append(byte value)
        {
            if (!_valid || _offset + sizeof(byte) > Payload.Length)
            {
                _valid = false;
                return;
            }
            Payload[_offset++] = value;
        }

        public void Append(short value)
        {
            if (!_valid || _offset + sizeof(short) > Payload.Length)
            {
                _valid = false;
                return;
            }
            Payload[_offset++] = (byte) (value >> 8);
            Payload[_offset++] = (byte) (value & 0x00FF);
        }

        public void Append(ushort value)
        {
            if (!_valid || _offset + sizeof(ushort) > Payload.Length)
            {
                _valid = false;
                return;
            }
            Payload[_offset++] = (byte) (value >> 8);
            Payload[_offset++] = (byte) (value & 0x00FF);
        }

        public void Append(int value)
        {
            if (!_valid || _offset + sizeof(int) > Payload.Length)
            {
                _valid = false;
                return;
            }
            Payload[_offset++] = (byte) ((value >> 24) & 0x000000FF);
            Payload[_offset++] = (byte) ((value >> 16) & 0x000000FF);
            Payload[_offset++] = (byte) ((value >> 8) & 0x000000FF);
            Payload[_offset++] = (byte) (value & 0x000000FF);
        }

        public void Append(uint value)
        {
            if (!_valid || _offset + sizeof(uint) > Payload.Length)
            {
                _valid = false;
                return;
            }
            Payload[_offset++] = (byte) ((value >> 24) & 0x000000FF);
            Payload[_offset++] = (byte) ((value >> 16) & 0x000000FF);
            Payload[_offset++] = (byte) ((value >> 8) & 0x000000FF);
            Payload[_offset++] = (byte) (value & 0x000000FF);
        }

        public void AppendAscii(string data)
        {
            byte[] array = Encoding.ASCII.GetBytes(data);
            Append((ushort) array.Length);
            Append(array);
        }

        public void AppendWide(string data)
        {
            byte[] array = Encoding.Unicode.GetBytes(data);
            Append((ushort) data.Length);
            Append(array);
        }

        public void Set(int offset, uint value)
        {
            if (!_valid || offset < 0 || offset + sizeof(uint) > Payload.Length)
            {
                _valid = false;
                return;
            }
            Payload[offset++] = (byte) ((value >> 24) & 0x000000FF);
            Payload[offset++] = (byte) ((value >> 16) & 0x000000FF);
            Payload[offset++] = (byte) ((value >> 8) & 0x000000FF);
            Payload[offset++] = (byte) (value & 0x000000FF);
        }
    }
}
