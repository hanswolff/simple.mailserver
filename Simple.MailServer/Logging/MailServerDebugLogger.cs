using System;

namespace Simple.MailServer.Logging
{
    public class MailServerDebugLogger : IMailServerLogger
    {
        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine("{0}: DEBUG - {1}", GetTimestamp(), message);
        }

        public void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine("{0}: INFO - {1}", GetTimestamp(), message);
        }

        public void Warn(string message)
        {
            System.Diagnostics.Debug.WriteLine("{0}: WARN - {1}", GetTimestamp(), message);
        }

        public void Error(string message)
        {
            System.Diagnostics.Debug.WriteLine("{0}: ERROR - {1}", GetTimestamp(), message);
        }

        public void Error(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("{0}: ERROR - Exception: {1}", GetTimestamp(), ex);
        }

        public void Error(Exception ex, string message)
        {
            System.Diagnostics.Debug.WriteLine("{0}: ERROR - {1}, Exception: {2}", GetTimestamp(), message, ex);
        }

        private string GetTimestamp()
        {
            return DateTime.UtcNow.ToString("u");
        }
    }
}
