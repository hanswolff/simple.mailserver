using Simple.MailServer.Smtp.Config;
using System;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpResetResponder : IRespondToSmtpReset
    {
        protected readonly IConfiguredSmtpRestrictions Configuration;

        public DefaultSmtpResetResponder(IConfiguredSmtpRestrictions configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            Configuration = configuration;
        }

        public SmtpResponse Reset(SmtpSessionInfo sessionInfo)
        {
            return SmtpResponse.OK;
        }
    }
}
