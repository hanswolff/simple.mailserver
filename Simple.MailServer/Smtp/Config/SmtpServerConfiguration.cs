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

        private long _globalConnectionTimeout = 600000;
        /// <summary>
        /// Maximum total number of milliseconds for a connection session before being disconnected
        /// </summary>
        public long GlobalConnectionTimeout
        {
            get { return _globalConnectionTimeout; }
            set { _globalConnectionTimeout = value; FireConfigurationChanged(); }
        }

        private long _connectionIdleTimeout = 60000;

        /// <summary>
        /// Maximum total number of milliseconds for an idle connection session before being disconnected
        /// </summary>
        public long ConnectionIdleTimeout
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
