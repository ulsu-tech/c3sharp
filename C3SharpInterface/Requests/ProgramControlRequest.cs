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
    public class ProgramControlRequest : Request
    {
        private ProgramControl _command;
        private ushort _interpreter;
        private string _name;
        private string _parameters;
        private bool _force;


        public ProgramControl Command
        {
            get
            {
                return _command;
            }
            set
            {
                Command = value;
                Build();
            }
        }

        public ushort Interpreter
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

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                Build();
            }
        }

        public string Parameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value;
                Build();
            }
        }

        public bool Force
        {
            get
            {
                return _force;
            }
            set
            {
                _force = value;
                Build();
            }
        }

        public ProgramControlRequest(ProgramControl command, ushort interpreter = 0, ushort tag = 0)
        {
            Tag = tag;
            Type = C3SharpInterface.Command.ProgramControl;

            _command = command;
            _interpreter = interpreter;
            _name = "";
            _parameters = "";
            _force = false;
            Build();
        }

        public ProgramControlRequest(ProgramControl command, string name, string parameters = "", bool force = false, ushort tag = 0)
        {
            Tag = tag;
            Type = C3SharpInterface.Command.ProgramControl;

            _command = command;
            _interpreter = 0;
            _name = name;
            _parameters = parameters;
            _force = force;
            Build();
        }

        private void Build()
        {
            Reset();
            Append((byte) _command);
            Append(_interpreter);

            switch (Command)
            {
                case ProgramControl.Select:
                case ProgramControl.Run:
                    AppendWide(_name);
                    AppendWide(_parameters);
                    Append(_force);
                    break;
                default:
                    break;
            }
        }
    }
}
