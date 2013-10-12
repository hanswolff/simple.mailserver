using System;

namespace Simple.MailServer.Logging
{
    public interface IMailServerLogger
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Error(Exception ex);
        void Error(Exception ex, string message);
    }
}
