using System;

namespace Simple.MailServer.Smtp
{
    public class SmtpSession : BaseSession
    {
        public SmtpConnection Connection { get; private set; }
        public SmtpResponderFactory ResponderFactory { get; set; }
        public SmtpSessionInfo SessionInfo { get; set; }

        public event EventHandler<SmtpSessionEventArgs> OnSessionDisconnected = (s, e) => { };

        public SmtpSession(SmtpConnection connection, SmtpResponderFactory responderFactory)
        {
            if (responderFactory == null) throw new ArgumentNullException("responderFactory");
            Connection = connection;
            ResponderFactory = responderFactory;
            SessionInfo = new SmtpSessionInfo();
        }

        public void Disconnect()
        {
            if (!Disconnected)
            {
                Disconnected = true;
                Active = false;
                if (Connection != null) 
                    Connection.Disconnect();
                OnSessionDisconnected(this, new SmtpSessionEventArgs(this));
            }
        }

        #region Dispose
        private bool _disposed;
        private readonly object _disposeLock = new object();

        /// <summary>
        /// Inheritable dispose method
        /// </summary>
        /// <param name="disposing">true, suppress GC finalizer call</param>
        protected override void Dispose(bool disposing)
        {
            lock (_disposeLock)
            {
                if (!_disposed)
                {
                    Disconnect();

                    _disposed = true;
                    if (disposing) GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SmtpSession()
        {
            Dispose(false);
        }
        #endregion
    }
}
