using System;

namespace Simple.MailServer.Smtp.Config
{
    public interface ISmtpServerConfiguration : IConfiguredSmtpRestrictions
    {
        string DefaultGreeting { get; set; }
        long GlobalConnectionTimeout { get; set; }
        long ConnectionIdleTimeout { get; set; }
        long MaxNumberOfRecipients { get; set; }

        event Action<ISmtpServerConfiguration> ConfigurationChanged;
    }
}
