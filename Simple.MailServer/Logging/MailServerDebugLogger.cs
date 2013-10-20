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

using System;

namespace Simple.MailServer.Logging
{
    public class MailServerDebugLogger : IMailServerLogger
    {
        public MailServerLogLevel LogLevel { get; set; }

        public MailServerDebugLogger(MailServerLogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        public void Debug(string message)
        {
            if (LogLevel < MailServerLogLevel.Debug) return;
            
            System.Diagnostics.Debug.WriteLine("{0}: DEBUG - {1}", GetTimestamp(), message);
        }

        public void Info(string message)
        {
            if (LogLevel < MailServerLogLevel.Info) return;

            System.Diagnostics.Debug.WriteLine("{0}: INFO - {1}", GetTimestamp(), message);
        }

        public void Warn(string message)
        {
            if (LogLevel < MailServerLogLevel.Warn) return;

            System.Diagnostics.Debug.WriteLine("{0}: WARN - {1}", GetTimestamp(), message);
        }

        public void Error(string message)
        {
            if (LogLevel < MailServerLogLevel.Error) return;

            System.Diagnostics.Debug.WriteLine("{0}: ERROR - {1}", GetTimestamp(), message);
        }

        public void Error(Exception ex)
        {
            if (LogLevel < MailServerLogLevel.Error) return;

            System.Diagnostics.Debug.WriteLine("{0}: ERROR - Exception: {1}", GetTimestamp(), ex);
        }

        public void Error(Exception ex, string message)
        {
            if (LogLevel < MailServerLogLevel.Error) return;

            System.Diagnostics.Debug.WriteLine("{0}: ERROR - {1}, Exception: {2}", GetTimestamp(), message, ex);
        }

        private string GetTimestamp()
        {
            return DateTime.UtcNow.ToString("u");
        }
    }
}
