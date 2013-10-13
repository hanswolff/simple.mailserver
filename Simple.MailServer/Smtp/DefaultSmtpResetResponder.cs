using Simple.MailServer.Smtp.Config;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpResetResponder<T> : IRespondToSmtpReset where T : IConfiguredSmtpRestrictions
    {
        protected readonly T Configuration;

        public DefaultSmtpResetResponder(T configuration)
        {
            Configuration = configuration;
        }

        public SmtpResponse Reset(SmtpSessionInfo sessionInfo)
        {
            return SmtpResponse.OK;
        }
    }
}
