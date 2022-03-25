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
    public class CopyFileRequest : Request
    {
        private CopyFlag _flags;
        private string _source;
        private string _destination;

        public CopyFlag Flags
        {
            get
            {
                return _flags;
            }
            set
            {
                _flags = value;
                Build();
            }
        }

        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
                Build();
            }
        }

        public string Destination
        {
            get
            {
                return _destination;
            }
            set
            {
                _destination = value;
                Build();
            }
        }

        public bool RemoveOriginal
        {
            get
            {
                return (Type == Command.FileMove);
            }
            set
            {
                Type = value ? Command.FileMove : Command.FileCopy;
                Build();
            }
        }

        public CopyFileRequest(string source, string destination, bool removeOriginal = false, CopyFlag flags = CopyFlag.None, ushort tag = 0)
        {
            Tag = tag;
            Type = removeOriginal ? Command.FileMove : Command.FileCopy;

            _flags = flags;
            _source = source;
            _destination = destination;
            
            Build();
        }

        private void Build()
        {
            Reset();
            Append((int) _flags);
            AppendWide(_source);
            AppendWide(_destination);
        }
    }
}
