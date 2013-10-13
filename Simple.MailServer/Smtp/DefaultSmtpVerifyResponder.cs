using Simple.MailServer.Smtp.Config;
using System;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpVerifyResponder : IRespondToSmtpVerify
    {
        protected readonly IConfiguredSmtpRestrictions Configuration;

        public DefaultSmtpVerifyResponder(IConfiguredSmtpRestrictions configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            Configuration = configuration;
        }

        public SmtpResponse Verify(SmtpSessionInfo sessionInfo, string arguments)
        {
            return new SmtpResponse(252, "2.5.2 Send some mail, i'll try my best");
        }
    }
}
