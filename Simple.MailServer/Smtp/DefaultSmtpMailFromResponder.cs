using Simple.MailServer.Smtp.Config;
using System;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpMailFromResponder : IRespondToSmtpMailFrom
    {
        protected readonly IConfiguredSmtpRestrictions Configuration;

        public DefaultSmtpMailFromResponder(IConfiguredSmtpRestrictions configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            Configuration = configuration;
        }

        public SmtpResponse VerifyMailFrom(SmtpSessionInfo sessionInfo, MailAddressWithParameters mailAddressWithParameters)
        {
            return SmtpResponse.OK;
        }
    }
}
