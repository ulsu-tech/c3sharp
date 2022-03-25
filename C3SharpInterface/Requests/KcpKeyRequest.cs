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
    public class KcpKeyRequest : Request
    {
        private KcpActionType _command;
        private uint _interpreter;
        private int _key;
        private KcpKeyDirection _direction;
        private KcpKeyStatus _status;

        public KcpActionType Command
        {
            get
            {
                return _command;
            }
            set
            {
                _command = value;
                Build();
            }
        }

        public uint Interpreter
        {
            get
            {
                return _interpreter;
            }
            set
            {
                _interpreter = value;
                Build();
            }
        }

        public uint Axis
        {
            get
            {
                return _interpreter;
            }
            set
            {
                _interpreter = value;
                Build();
            }
        }

        public int Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
                Build();
            }
        }

        public KcpKeyDirection Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
                Build();
            }
        }

        public KcpKeyStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                Build();
            }
        }

        public KcpKeyRequest(KcpActionType command, KcpKeyStatus status = KcpKeyStatus.Pressed, uint interpreter = 0, KcpKeyDirection direction = KcpKeyDirection.Forward, ushort tag = 0)
        {
            Tag = tag;
            Type = C3SharpInterface.Command.KcpAction;

            _command = command;
            _interpreter = interpreter;
            _key = 0;
            _direction = direction;
            _status = status;
            Build();
        }

        public KcpKeyRequest(KcpActionType command, int key, uint axis = 0, KcpKeyStatus status = KcpKeyStatus.Pressed, KcpKeyDirection direction = KcpKeyDirection.Forward, ushort tag = 0)
        {
            Tag = tag;
            Type = C3SharpInterface.Command.KcpAction;

            _command = command;
            _interpreter = axis;
            _key = key;
            _direction = direction;
            _status = status;
            Build();
        }


        private void Build()
        {
            Reset();
            Append((byte) _command);
            Append(_interpreter);
            Append(_key);
            Append(_direction == KcpKeyDirection.Backward);
            Append(_status == KcpKeyStatus.Released);
        }
    }
}
