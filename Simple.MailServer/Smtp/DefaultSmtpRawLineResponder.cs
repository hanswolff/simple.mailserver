using Simple.MailServer.Smtp.Config;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpRawLineResponder<T> : IRespondToSmtpRawLine where T : IConfiguredSmtpRestrictions
    {
        protected readonly T Configuration;

        public DefaultSmtpRawLineResponder(T configuration)
        {
            Configuration = configuration;
        }

        public SmtpResponse RawLine(SmtpSessionInfo sessionInfo, string line)
        {
            return SmtpResponse.None;
        }
    }
}
