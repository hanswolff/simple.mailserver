using Simple.MailServer.Smtp.Config;
using System;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpRecipientToResponder : IRespondToSmtpRecipientTo
    {
        protected readonly IConfiguredSmtpRestrictions Configuration;

        public DefaultSmtpRecipientToResponder(IConfiguredSmtpRestrictions configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            Configuration = configuration;
        }

        public SmtpResponse VerifyRecipientTo(SmtpSessionInfo sessionInfo, MailAddressWithParameters mailAddressWithParameters)
        {
            return SmtpResponse.OK;
        }
    }
}
