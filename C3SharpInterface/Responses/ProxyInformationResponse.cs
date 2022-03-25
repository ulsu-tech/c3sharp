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
    public class ProxyInformationResponse : Response
    {
        private Version _version;
        private DateTime _time;
        private string _computerName;

        public Version Version
        {
            get { return _version; }
        }

        public DateTime Time
        {
            get { return _time; }
        }

        public string ComputerName
        {
            get { return _computerName; }
        }

        protected override void ParseInternal()
        {
            if (Valid)
            {
                byte major;
                byte minor;
                byte type;
                Extract(out major);
                Extract(out minor);
                Extract(out type);
                _version = new Version(major, minor, type);

                ushort year;
                ushort month;
                ushort dayOfWeek;
                ushort day;
                ushort hour;
                ushort minute;
                ushort second;
                ushort millisecond;
                Extract(out year);
                Extract(out month);
                Extract(out dayOfWeek);
                Extract(out day);
                Extract(out hour);
                Extract(out minute);
                Extract(out second);
                Extract(out millisecond);
                _time = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);

                ExtractWide(out _computerName);
            }
        }
    }
}
