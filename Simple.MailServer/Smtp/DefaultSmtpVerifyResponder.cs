using Simple.MailServer.Smtp.Config;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpVerifyResponder<T> : IRespondToSmtpVerify where T : IConfiguredSmtpRestrictions
    {
        protected readonly T Configuration;

        public DefaultSmtpVerifyResponder(T configuration)
        {
            Configuration = configuration;
        }

        public SmtpResponse Verify(SmtpSessionInfo sessionInfo, string arguments)
        {
            return new SmtpResponse(252, "2.5.2 Send some mail, i'll try my best");
        }
    }
}
