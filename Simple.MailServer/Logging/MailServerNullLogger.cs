using System;

namespace Simple.MailServer.Logging
{
    public class MailServerNullLogger : IMailServerLogger
    {
        public void Debug(string message)
        {
        }

        public void Info(string message)
        {
        }

        public void Warn(string message)
        {
        }

        public void Error(string message)
        {
        }

        public void Error(Exception ex)
        {
        }

        public void Error(Exception ex, string message)
        {
        }
    }
}
