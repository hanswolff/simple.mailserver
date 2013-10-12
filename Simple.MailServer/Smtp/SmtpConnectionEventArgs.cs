using System;

namespace Simple.MailServer.Smtp
{
    public class SmtpConnectionEventArgs : EventArgs
    {
        public SmtpConnection Connection { get; protected set; }

        public SmtpConnectionEventArgs(SmtpConnection connection)
        {
            Connection = connection;
        }
    }
}
