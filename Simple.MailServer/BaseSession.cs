using System;
using System.Diagnostics;
using System.Threading;

namespace Simple.MailServer
{
    public class BaseSession : IHaveIdleTimeSpan, IDisposable
    {
        public bool Active { get; protected set; }
        public bool Disconnected { get; protected set; }

        public object Tag { get; set; }

        protected readonly Stopwatch Stopwatch = Stopwatch.StartNew();
        protected long LastActivity;

        public long GetIdleTimeMilliseconds()
        {
            var lastActivity = Interlocked.Read(ref LastActivity);
            return Stopwatch.ElapsedMilliseconds - lastActivity;
        }

        public BaseSession()
        {
            Active = true;
            LastActivity = Stopwatch.ElapsedMilliseconds;            
        }

        public void UpdateActivity()
        {
            Interlocked.Exchange(ref LastActivity, Stopwatch.ElapsedMilliseconds);
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
        ~BaseSession()
        {
            Dispose(false);
        }
        #endregion
    }
}
