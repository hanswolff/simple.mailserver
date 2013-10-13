using Simple.MailServer.Smtp.Config;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpMailFromResponder<T> : IRespondToSmtpMailFrom where T : IConfiguredSmtpRestrictions
    {
        protected readonly T Configuration;

        public DefaultSmtpMailFromResponder(T configuration)
        {
            Configuration = configuration;
        }

        public SmtpResponse VerifyMailFrom(SmtpSessionInfo sessionInfo, MailAddressWithParameters mailAddressWithParameters)
        {
            return SmtpResponse.OK;
        }
    }
}
