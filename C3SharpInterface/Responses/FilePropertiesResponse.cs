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

namespace C3SharpInterface.Responses
{
    public class FilePropertiesResponse : Response
    {
        private ItemType _fileType;
        private ItemAttribute _attributes;
        private EditMode _editMode;
        private DateTime _created;
        private DateTime _accessed;
        private DateTime _modified;
        private long _fileSize;
        private string _fileName;

        public ItemType FileType
        {
            get { return _fileType; }
        }

        public ItemAttribute Attributes
        {
            get { return _attributes; }
        }

        public EditMode EditMode
        {
            get { return _editMode; }
        }

        public DateTime CreationTime
        {
            get { return _created; }
        }

        public DateTime LastAccessTime
        {
            get { return _accessed; }
        }

        public DateTime LastWriteTime
        {
            get { return _modified; }
        }

        public long FileSize
        {
            get { return _fileSize; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        private long MakeLong(int low, int high)
        {
            return (long) (((ulong) high << 32) | ((ulong) low & (ulong) uint.MaxValue));
        }

        private DateTime MakeDateTime(int low, int high, int bias)
        {
            DateTime result = DateTime.FromFileTime(MakeLong(low, high));
            return result + (new TimeSpan(0, bias, 0));
        }

        protected override void ParseInternal()
        {
            int low;
            int high;
            int value;

            Extract(out value);
            _fileType = (ItemType) value;

            Extract(out low);
            Extract(out high);
            _fileSize = MakeLong(low, high);

            Extract(out value);
            _attributes = (ItemAttribute) value;

            Extract(out low);
            Extract(out high);
            Extract(out value);
            _created = MakeDateTime(low, high, value);

            Extract(out low);
            Extract(out high);
            Extract(out value);
            _accessed = MakeDateTime(low, high, value);

            Extract(out low);
            Extract(out high);
            Extract(out value);
            _modified = MakeDateTime(low, high, value);

            Extract(out value);
            _editMode = (EditMode) value;

            ExtractWide(out _fileName);
        }
    }
}
