using System;

namespace Simple.MailServer.Smtp
{
    public class SmtpSessionEventArgs : EventArgs
    {
        public SmtpSession Session { get; protected set; }

        public SmtpSessionEventArgs(SmtpSession session)
        {
            Session = session;
        }
    }
}
