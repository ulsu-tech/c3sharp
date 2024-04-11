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
    public class Response
    {
        protected int _offset = 0;
        protected int _size = 0;
        protected bool _valid = true;

        protected virtual void ParseInternal()
        {
        }

        public ushort Tag = 0;
        public Command Type = Command.Invalid;
        public Error ErrorCode = Error.General;
        public byte[] Payload = new byte[65534];
        public bool Success = false;
        public bool Valid
        {
            get { return _valid && (Type != Command.Invalid); }
        }
        public int Size
        {
            get { return (_valid && _size >= 0 && _size <= Payload.Length) ? _size : 0; }
        }

        public static Response CreateByType(byte type)
        {
            Response result = null;
            switch ((Command) type)
            {
                case Command.ReadVariableAscii:
                case Command.WriteVariableAscii:
                case Command.ReadVariable:
                case Command.WriteVariable:
                    result = new Responses.VariableValueResponse();
                    break;
                case Command.ReadMultiple:
                case Command.WriteMultiple:
                    result = new Responses.VariableMultipleResponse();
                    break;
                case Command.ProgramControl:
                    result = new Responses.ProgramControlResponse();
                    break;
                case Command.Motion:
                    result = new Responses.MotionControlResponse();
                    break;
                case Command.KcpAction:
                    result = new Responses.KcpKeyResponse();
                    break;
                case Command.ProxyInfo:
                    result = new Responses.ProxyInformationResponse();
                    break;
                case Command.ProxyFeatures:
                    result = new Responses.ProxyFeaturesResponse();
                    break;
                case Command.ProxyBenchmark:
                    result = new Responses.ProxyBenchmarkResponse();
                    break;
                case Command.FileNameList:
                    result = new Responses.ListDirectoryResponse();
                    break;
                case Command.FileGetProperties:
                    result = new Responses.FilePropertiesResponse();
                    break;
                case Command.FileGetFullName:
                case Command.FileGetKrcName:
                    result = new Responses.FilePathResponse();
                    break;
                case Command.FileWriteContent:
                    result = new Responses.FileWriteResponse();
                    break;
                case Command.FileReadContent:
                    result = new Responses.FileReadResponse();
                    break;
                case Command.VolumeGetProperties:
                    result = new Responses.VolumePropertiesResponse();
                    break;
                case Command.VolumeList:
                    result = new Responses.VolumeListResponse();
                    break;
                default:
                    result = new Response();
                    break;
            }
            result.Type = (Command) type;
            return result;
        }

        public void Reset()
        {
            _offset = 0;
            _valid = true;
        }

        public void Parse(byte[] source, int sourceOffset, int size)
        {
            if (size > Payload.Length)
            {
                _size = 0;
                _valid = false;
                return;
            }

            Array.Copy(source, sourceOffset, Payload, 0, size);
            _size = size;
            Reset();
            ParseInternal();
        }

        public void Extract(out bool value)
        {
            value = false;
            byte boolean;
            Extract(out boolean);
            value = (boolean != 0);
        }

        public void Extract(byte[] data)
        {
            if (!_valid || _offset + data.Length > _size)
            {
                _valid = false;
                return;
            }

            Array.Copy(Payload, _offset, data, 0, data.Length);
            _offset += data.Length;
        }

        public void Extract(out sbyte data)
        {
            data = 0;

            if (!_valid || _offset + sizeof(sbyte) > _size)
            {
                _valid = false;
                return;
            }

            data = (sbyte) Payload[_offset++];
        }

        public void Extract(out byte data)
        {
            data = 0;

            if (!_valid || _offset + sizeof(byte) > _size)
            {
                _valid = false;
                return;
            }

            data = Payload[_offset++];
        }

        public void Extract(out short data)
        {
            data = 0;

            if (!_valid || _offset + sizeof(short) > _size)
            {
                _valid = false;
                return;
            }

            data = (short) Payload[_offset++];
            data <<= 8;
            data |= (short) Payload[_offset++];
        }

        public void Extract(out ushort data)
        {
            data = 0;

            if (!_valid || _offset + sizeof(ushort) > _size)
            {
                _valid = false;
                return;
            }

            data = (ushort) Payload[_offset++];
            data <<= 8;
            data |= (ushort) Payload[_offset++];
        }

        public void Extract(out int data)
        {
            data = 0;

            if (!_valid || _offset + sizeof(int) > _size)
            {
                _valid = false;
                return;
            }

            data = (int) Payload[_offset++];
            data <<= 8;
            data |= (int) Payload[_offset++];
            data <<= 8;
            data |= (int) Payload[_offset++];
            data <<= 8;
            data |= (int) Payload[_offset++];
        }

        public void Extract(out uint data)
        {
            data = 0;

            if (!_valid || _offset + sizeof(uint) > _size)
            {
                _valid = false;
                return;
            }

            data = (uint) Payload[_offset++];
            data <<= 8;
            data |= (uint) Payload[_offset++];
            data <<= 8;
            data |= (uint) Payload[_offset++];
            data <<= 8;
            data |= (uint) Payload[_offset++];
        }

        public void Extract(out long data)
        {
            data = 0;

            if (!_valid || _offset + sizeof(long) > _size)
            {
                _valid = false;
                return;
            }

            data = (long) Payload[_offset++];
            data <<= 8;
            data |= (long) Payload[_offset++];
            data <<= 8;
            data |= (long) Payload[_offset++];
            data <<= 8;
            data |= (long) Payload[_offset++];
            data <<= 8;
            data |= (long)Payload[_offset++];
            data <<= 8;
            data |= (long)Payload[_offset++];
            data <<= 8;
            data |= (long)Payload[_offset++];
            data <<= 8;
            data |= (long)Payload[_offset++];
        }

        public void Extract(out ulong data)
        {
            data = 0;

            if (!_valid || _offset + sizeof(ulong) > _size)
            {
                _valid = false;
                return;
            }

            data = (ulong) Payload[_offset++];
            data <<= 8;
            data |= (ulong) Payload[_offset++];
            data <<= 8;
            data |= (ulong) Payload[_offset++];
            data <<= 8;
            data |= (ulong) Payload[_offset++];
            data <<= 8;
            data |= (ulong) Payload[_offset++];
            data <<= 8;
            data |= (ulong) Payload[_offset++];
            data <<= 8;
            data |= (ulong) Payload[_offset++];
            data <<= 8;
            data |= (ulong) Payload[_offset++];
        }

        public void ExtractAscii(out string data)
        {
            data = "";
            ushort length;

            Extract(out length);

            if (!_valid || _offset + length > _size)
            {
                _valid = false;
                return;
            }

            data = Encoding.ASCII.GetString(Payload, _offset, (int) length);
            _offset += (int) length;
        }

        public void ExtractWide(out string data)
        {
            data = "";
            ushort length;

            Extract(out length);

            length *= 2;

            if (!_valid || _offset + length > _size)
            {
                _valid = false;
                return;
            }

            data = Encoding.Unicode.GetString(Payload, _offset, (int) length);
            _offset += (int) length;
        }

        public void Get(byte[] data, int offset, int dataOffset, int dataSize)
        {
            if (!_valid || offset + dataSize > _size)
            {
                _valid = false;
                return;
            }

            Array.Copy(Payload, offset, data, dataOffset, dataSize);
            _offset += dataSize;
        }
    }
}
