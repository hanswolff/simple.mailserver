using System;

namespace Simple.MailServer
{
    public interface IObservableServerConfiguration
    {
        string DefaultGreeting { get; set; }
        long GlobalConnectionTimeout { get; set; }
        long ConnectionIdleTimeout { get; set; }
        long MaxNumberOfRecipients { get; set; }

        event Action<IObservableServerConfiguration> ConfigurationChanged;
    }
}
