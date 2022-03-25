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
    public class WriteMultipleRequest : Request
    {
        private List<string> _variables = new List<string>();
        private List<string> _values = new List<string>();

        public int Count { get { return _variables.Count; } }

        public void Add(string variable, string value)
        {
            _variables.Add(variable);
            _values.Add(value);
            Build();
        }

        public void Remove(string variable)
        {
            int index = _variables.IndexOf(variable);
            if (index != -1)
            {
                _variables.RemoveAt(index);
                _values.RemoveAt(index);
            }
            Build();
        }

        public void Clear()
        {
            _variables.Clear();
            _values.Clear();
            Build();
        }

        public KeyValuePair<string, string> this[int index]
        { 
            get
            {
                return new KeyValuePair<string, string>(_variables[index], _values[index]);
            }
        }

        public string Variable(int index)
        {
            return _variables[index];
        }

        public string Value(int index)
        {
            return _values[index];
        }

        public WriteMultipleRequest(ushort tag = 0)
        {
            Tag = tag;
            Type = Command.WriteMultiple;

            Build();
        }

        private void Build()
        {
            Reset();

            if (_variables.Count > 255)
            {
                _valid = false;
                return;
            }

            Append((byte) _variables.Count);

            for (int i = 0; i < _variables.Count; i++)
            {
                AppendWide(_variables[i]);
                AppendWide(_values[i]);
            }
        }
    }
}
