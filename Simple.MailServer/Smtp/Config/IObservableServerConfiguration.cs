using System;

namespace Simple.MailServer.Smtp.Config
{
    public interface IObservableServerConfiguration : IConfiguredSmtpRestrictions
    {
        string DefaultGreeting { get; set; }
        long GlobalConnectionTimeout { get; set; }
        long ConnectionIdleTimeout { get; set; }
        long MaxNumberOfRecipients { get; set; }

        event Action<IObservableServerConfiguration> ConfigurationChanged;
    }
}
