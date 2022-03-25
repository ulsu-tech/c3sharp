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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;

namespace C3SharpInterface
{
    public class AsyncClient
    {
        private const uint DefaultConnectTimeout = 2000;
        private const uint DefaultRequestTimeout = 3000;

        private IPAddress _address;
        private int _port = 0;

        private Mutex _mutex = new Mutex();
        private TcpClient _socket = null;
        private System.Timers.Timer _connectTimer = new System.Timers.Timer();
        private System.Timers.Timer _requestTimer = new System.Timers.Timer();
        private ManualResetEvent _signal = new ManualResetEvent(false);
        private Event _event = null;

        private ClientStatus _status = ClientStatus.Disconnected;

        private byte[] _buffer = new byte[65540];
        private int _offset = 0;

        public enum ClientStatus
        {
            Disconnected,
            Connecting,
            Connected,
            Busy
        }

        public ClientStatus Status
        {
            get
            {
                _mutex.WaitOne();
                ClientStatus result = _status;
                _mutex.ReleaseMutex();
                return result;
            }
        }

        public uint ConnectTimeout
        {
            get
            {
                _mutex.WaitOne();
                uint result = Convert.ToUInt32(_connectTimer.Interval);
                _mutex.ReleaseMutex();
                return result;
            }
            set
            {
                _mutex.WaitOne();
                if (value < 1)
                    _connectTimer.Stop();

                if (value < int.MaxValue)
                {
                    _connectTimer.Interval = (double) value;
                }
                else
                {
                    _connectTimer.Interval = (double) int.MaxValue;
                }
                _mutex.ReleaseMutex();
            }
        }

        public uint RequestTimeout
        {
            get
            {
                _mutex.WaitOne();
                uint result = Convert.ToUInt32(_requestTimer.Interval);
                _mutex.ReleaseMutex();
                return result;
            }
            set
            {
                _mutex.WaitOne();
                if (value < 1)
                    _requestTimer.Stop();

                if (value < int.MaxValue)
                {
                    _requestTimer.Interval = (double) value;
                }
                else
                {
                    _requestTimer.Interval = (double) int.MaxValue;
                }
                _mutex.ReleaseMutex();
            }
        }

        public ManualResetEvent Signal
        {
            get { return _signal; }
        }

        public AsyncClient()
        {
            _connectTimer.AutoReset = false;
            _connectTimer.Interval = (double) DefaultConnectTimeout;
            _connectTimer.Elapsed += ConnectTimerCallback;

            _requestTimer.AutoReset = false;
            _requestTimer.Interval = (double) DefaultRequestTimeout;
            _requestTimer.Elapsed += RequestTimerCallback;
        }

        public void AbortConnection()
        {
            _mutex.WaitOne();
            bool emitEvent = (_status != ClientStatus.Disconnected);
            _connectTimer.Stop();
            _requestTimer.Stop();
            _socket.Close();
            _status = ClientStatus.Disconnected;
            if (emitEvent)
                SetDisconnectedEvent();

            _mutex.ReleaseMutex();
        }

        public void ConnectToHost(IPAddress address, int port)
        {
            _mutex.WaitOne();

            _address = address;
            _port = port;

            try
            {
                if (_status == ClientStatus.Disconnected)
                    _socket = new TcpClient();

                _socket.BeginConnect(address, port, new AsyncCallback(ConnectCallback), this);
            }
            catch
            {
                _mutex.ReleaseMutex();
                throw;
            }

            _status = ClientStatus.Connecting;
            _event = null;
            _signal.Reset();

            try
            {
                _connectTimer.Start();
            }
            catch { }

            _mutex.ReleaseMutex();
        }

        public void Reconnect()
        {
            ConnectToHost(_address, _port);
        }

        public Event FetchEvent()
        {
            _mutex.WaitOne();
            Event result = _event;
            _event = null;
            _signal.Reset();
            _mutex.ReleaseMutex();
            return result;
        }

        public void SendRequest(Request request)
        {
            _mutex.WaitOne();

            if (_status != ClientStatus.Connected)
            {
                _mutex.ReleaseMutex();
                throw new InvalidOperationException();
            }

            if (!request.Valid)
            {
                _mutex.ReleaseMutex();
                throw new ArgumentException();
            }

            ushort size = (ushort) request.Size;

            byte[] data = new byte[5 + size];
            data[0] = (byte) (request.Tag >> 8);
            data[1] = (byte) (request.Tag & 0x00FF);
            data[2] = (byte) ((size + 1) >> 8);
            data[3] = (byte) ((size + 1) & 0x00FF);
            data[4] = (byte) request.Type;

            if (size > 0)
                Array.Copy(request.Payload, 0, data, 5, (int) size);

            try
            {
                NetworkStream stream = _socket.GetStream();
                stream.BeginWrite(data, 0, data.Length, new AsyncCallback(WriteCallback), this);
                _status = ClientStatus.Busy;
            }
            catch
            {
                _mutex.ReleaseMutex();
                throw;
            }

            _mutex.ReleaseMutex();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            _mutex.WaitOne();
            _connectTimer.Stop();
            _requestTimer.Stop();

            try
            {
                _socket.EndConnect(ar);
                if (_status != ClientStatus.Disconnected)
                    _status = ClientStatus.Connected;
            }
            catch (SocketException e)
            {
                SetDisconnectedEvent(e.SocketErrorCode == SocketError.TimedOut, e);
            }
            catch (Exception e)
            {
                SetDisconnectedEvent(false, e);
            }

            if (_status == ClientStatus.Connected)
            {
                _event = new Event();
                _event.Type = Event.TypeEnum.Connected;
                _signal.Set();
            }
            else if (_status != ClientStatus.Disconnected)
            {
                _socket.Close();
                _status = ClientStatus.Disconnected;
                SetDisconnectedEvent();
            }
            _mutex.ReleaseMutex();
        }

        private void WriteCallback(IAsyncResult ar)
        {
            _mutex.WaitOne();

            bool ok = false;

            try
            {
                NetworkStream stream = _socket.GetStream();
                stream.EndWrite(ar);

                _offset = 0;
                stream.BeginRead(_buffer, _offset, _buffer.Length - _offset, new AsyncCallback(ReadCallback), this);

                ok = true;
            }
            catch (Exception e)
            {
                _socket.Close();
                _status = ClientStatus.Disconnected;
                SetDisconnectedEvent(false, e);
            }

            if (ok)
            {
                try
                {
                    _requestTimer.Start();
                }
                catch { }
            }

            _mutex.ReleaseMutex();
        }

        private void ReadCallback(IAsyncResult ar)
        {
            _mutex.WaitOne();
            try
            {
                NetworkStream stream = _socket.GetStream();
                int received = stream.EndRead(ar);
                _offset += received;

                if (_offset >= 4)
                {
                    ushort size = (ushort) ((_buffer[2] << 8) | _buffer[3]);

                    // Wrong message
                    if (size < 4)
                        throw new Exception("Invalid message");

                    if (_offset < size + 4)
                    {
                        stream.BeginRead(_buffer, _offset, _buffer.Length - _offset, new AsyncCallback(ReadCallback), this);
                    }
                    else
                    {
                        ushort tag = (ushort) ((_buffer[0] << 8) | _buffer[1]);
                        ushort code = (ushort) ((_buffer[size + 1] << 8) | _buffer[size + 2]);

                        _event = new Event();
                        _event.Type = Event.TypeEnum.DataReceived;
                        _event.Response = Response.CreateByType(_buffer[4]);

                        _event.Response.Tag = tag;
                        _event.Response.ErrorCode = (Error) code;
                        _event.Response.Success = (_buffer[size + 3] != 0);
                        _event.Response.Parse(_buffer, 5, (int) size - 4);

                        _signal.Set();

                        _status = ClientStatus.Connected;
                        _requestTimer.Stop();
                    }

                }
                else
                {
                    stream.BeginRead(_buffer, _offset, _buffer.Length - _offset, new AsyncCallback(ReadCallback), this);
                }
            }
            catch (Exception e)
            {
                _socket.Close();
                _status = ClientStatus.Disconnected;
                SetDisconnectedEvent(false, e);
            }
            _mutex.ReleaseMutex();
        }

        private void ConnectTimerCallback(object source, ElapsedEventArgs arguments)
        {
            _mutex.WaitOne();
            if (_status == ClientStatus.Connecting)
            {
                _socket.Close();
                _status = ClientStatus.Disconnected;
                SetDisconnectedEvent(true);
            }
            _mutex.ReleaseMutex();
        }

        private void RequestTimerCallback(object source, ElapsedEventArgs arguments)
        {
            _mutex.WaitOne();
            if (_status == ClientStatus.Busy)
            {
                _socket.Close();
                _status = ClientStatus.Disconnected;
                SetDisconnectedEvent(true);
            }
            _mutex.ReleaseMutex();
        }

        private void SetDisconnectedEvent(bool timedOut = false, Exception e = null)
        {
            _event = new Event();
            _event.Exception = e;
            _event.Type = Event.TypeEnum.Disconnected;
            _event.TimedOut = timedOut;
            _signal.Set();
        }
    }
}
