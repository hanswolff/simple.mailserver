#region Header
// IdleConnectionDisconnectorWatchdog.cs
// 
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
using System;
using System.Linq;
using System.Threading;

namespace Simple.MailServer
{
    public class IdleConnectionDisconnectWatchdog<T> where T : class, IHaveConnections
    {
        private Timer _timer;

        public T Server { get; private set; }

        public long ConnectionTimeoutMilliseconds { get; set; }
        public long IdleTimeoutMilliseconds { get; set; }

        public event EventHandler<ClientConnectionEventArgs> TerminatingConnection = (s, e) => { };

        public int Interval
        {
            get { return _interval; }
            set
            {
                lock (_startStopLock)
                {
                    if (_timer != null) _timer.Change(0, value);
                    _interval = value;
                }
            }
        }

        public IdleConnectionDisconnectWatchdog(T server)
        {
            if (server == null) throw new ArgumentNullException("server");

            Server = server;
            _interval = 1000;
        }

        private readonly object _startStopLock = new object();
        private int _interval;

        public void Start()
        {
            lock (_startStopLock)
            {
                if (_timer != null) return;
                _timer = new Timer(TimerCallback, null, 0, _interval);
            }
        }

        public void Stop()
        {
            lock (_startStopLock)
            {
                if (_timer == null) return;
                _timer.Dispose();
                _timer = null;
            }
        }

        private void TimerCallback(object state)
        {
            TimerCall();
        }

        public void TimerCall()
        {
            if (_timer == null) return;

            var connections = Server.GetConnections().ToList();

            var connectionTimeout = ConnectionTimeoutMilliseconds;
            var idleTimeout = IdleTimeoutMilliseconds;

            for (int i = connections.Count - 1; i >= 0; i--)
            {
                var connection = connections[i];

                if (connectionTimeout > 0 &&
                    (DateTime.UtcNow - connection.ConnectionInitiated).TotalMilliseconds >= connectionTimeout)
                {
                    TerminatingConnection(this, new ClientConnectionEventArgs(connection));
                    connection.Disconnect();
                    continue;
                }

                if (idleTimeout > 0 && (connection.GetIdleTimeMilliseconds() >= idleTimeout))
                {
                    TerminatingConnection(this, new ClientConnectionEventArgs(connection));
                    connection.Disconnect();
                    continue;
                }
            }
        }
    }
}
