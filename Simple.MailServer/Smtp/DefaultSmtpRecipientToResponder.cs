using Simple.MailServer.Smtp.Config;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpRecipientToResponder<T> : IRespondToSmtpRecipientTo where T : IConfiguredSmtpRestrictions
    {
        protected readonly T Configuration;

        public DefaultSmtpRecipientToResponder(T configuration)
        {
            Configuration = configuration;
        }

        public SmtpResponse VerifyRecipientTo(SmtpSessionInfo sessionInfo, MailAddressWithParameters mailAddressWithParameters)
        {
            return SmtpResponse.OK;
        }
    }
}
