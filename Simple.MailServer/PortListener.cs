#region Header
// Copyright (c) 2013 Hans Wolff
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using Simple.MailServer.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Simple.MailServer
{
    [DebuggerDisplay("{ListenAddress}:{Port}")]
    public class PortListener : IDisposable
    {
        public int Port { get; private set; }
        public IPAddress ListenAddress { get; private set; }

        private readonly TcpListener _tcpListener;
        private Task _task;
        private bool _stopRequested;

        public event Action<PortListener, TcpClient> ClientConnected = (portListener, tcpClient) => { };

        public PortListener(IPAddress listenAddress, int port)
        {
            ListenAddress = listenAddress;
            Port = port;

            _tcpListener = new TcpListener(new IPEndPoint(ListenAddress, Port));
        }

        private readonly object _startStopLock = new object();
        public void StartListen()
        {
            lock (_startStopLock)
            {
                if (_task != null)
                    throw new InvalidOperationException("Already started listening");

                _stopRequested = false;
                _tcpListener.Start();
                _task = ListenForClients();
            }
        }

        public void StopListen()
        {
            lock (_startStopLock)
            {
                if (_task == null) return;

                _stopRequested = true;
                _tcpListener.Stop();
                _task.Wait();
                _task = null;
            }
        }

        private async Task ListenForClients()
        {
            try
            {
                while (!_stopRequested)
                {
                    var client = await _tcpListener.AcceptTcpClientAsync();

                    try { ClientConnected(this, client); }
                    catch (Exception ex) { MailServerLogger.Instance.Error(ex); }
                }
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public override string ToString()
        {
            return ListenAddress + ":" + Port;
        }

        #region Dispose
        private bool _disposed;
        private readonly object _disposeLock = new object();

        /// <summary>
        /// Inheritable dispose method
        /// </summary>
        /// <param name="disposing">true, suppress GC finalizer call</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (_disposeLock)
            {
                if (!_disposed)
                {
                    StopListen();

                    _disposed = true;
                    if (disposing) GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Free resources being used
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~PortListener()
        {
            Dispose(false);
        }
        #endregion
    }
}
