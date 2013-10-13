using Simple.MailServer.Smtp.Config;
using System;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpRawLineResponder : IRespondToSmtpRawLine
    {
        protected readonly IConfiguredSmtpRestrictions Configuration;

        public DefaultSmtpRawLineResponder(IConfiguredSmtpRestrictions configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            Configuration = configuration;
        }

        public SmtpResponse RawLine(SmtpSessionInfo sessionInfo, string line)
        {
            return SmtpResponse.None;
        }
    }
}
