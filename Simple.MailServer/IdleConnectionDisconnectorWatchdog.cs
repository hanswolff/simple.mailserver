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
