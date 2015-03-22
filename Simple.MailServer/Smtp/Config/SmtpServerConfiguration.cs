#region Header
// Copyright (c) 2013-2015 Hans Wolff
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

namespace Simple.MailServer.Smtp.Config
{
    public class SmtpServerConfiguration : ISmtpServerConfiguration
    {
        private string _defaultGreeting = "localhost";
        /// <summary>
        /// Greeting to be sent to clients (can be overridden per connection)
        /// </summary>
        public string DefaultGreeting
        {
            get { return _defaultGreeting; }
            set { _defaultGreeting = value; FireConfigurationChanged(); }
        }

        private TimeSpan _globalConnectionTimeout = TimeSpan.FromMinutes(10);
        /// <summary>
        /// Maximum time for a connection session before being disconnected
        /// </summary>
        public TimeSpan GlobalConnectionTimeout
        {
            get { return _globalConnectionTimeout; }
            set { _globalConnectionTimeout = value; FireConfigurationChanged(); }
        }

        private TimeSpan _connectionIdleTimeout = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Maximum time for an idle connection session before being disconnected
        /// </summary>
        public TimeSpan ConnectionIdleTimeout
        {
            get { return _connectionIdleTimeout; }
            set { _connectionIdleTimeout = value; FireConfigurationChanged(); }
        }

        private long _maxMailMessageSize = 20 * 1024 * 1024;

        /// <summary>
        /// Maximum size in bytes for a mail message
        /// </summary>
        public long MaxMailMessageSize
        {
            get { return _maxMailMessageSize; }
            set { _maxMailMessageSize = value; FireConfigurationChanged(); }
        }

        private long _maxNumberOfRecipients = 100;
        /// <summary>
        /// Maximum number of recipients
        /// </summary>
        public long MaxNumberOfRecipients
        {
            get { return _maxNumberOfRecipients; }
            set { _maxNumberOfRecipients = value; FireConfigurationChanged(); }
        }

        public event Action<ISmtpServerConfiguration> ConfigurationChanged = c => { };

        private void FireConfigurationChanged()
        {
            ConfigurationChanged(this);
        }
    }
}
