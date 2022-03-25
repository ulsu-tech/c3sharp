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
    public class CreateFileRequest : Request
    {
        private ItemType _fileType;
        private ModulePart _part;
        private bool _always;
        private string _fileName;
        private string _template;

        public ItemType FileType
        {
            get
            {
                return _fileType;
            }
            set
            {
                _fileType = value;
                Build();
            }
        }
        public ModulePart Part
        {
            get
            {
                return _part;
            }
            set
            {
                _part = value;
                Build();
            }
        }
        public bool Always
        {
            get
            {
                return _always;
            }
            set
            {
                _always = value;
                Build();
            }
        }
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                Build();
            }
        }
        public string Template
        {
            get
            {
                return _template;
            }
            set
            {
                _template = value;
                Build();
            }
        }

        public CreateFileRequest(string fileName, ItemType fileType = ItemType.Module, ModulePart part = ModulePart.SRC, bool always = false, string template = "", ushort tag = 0)
        {
            Tag = tag;
            Type = Command.FileCreate;

            _fileName = fileName;
            _fileType = fileType;
            _part = part;
            _always = always;
            _template = template;

            Build();
        }

        private void Build()
        {
            Reset();
            Append((ushort) _fileType);
            Append((byte) _part);
            Append(_always);
            AppendWide(_fileName);
            AppendWide(_template);
        }
    }
}
