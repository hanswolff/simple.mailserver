using Simple.MailServer.Smtp.Config;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpDataResponder<T> : IRespondToSmtpData where T : IConfiguredSmtpRestrictions
    {
        protected readonly T Configuration;

        public DefaultSmtpDataResponder(T configuration)
        {
            Configuration = configuration;
        }

        public virtual SmtpResponse DataStart(SmtpSessionInfo sessionInfo)
        {
            return SmtpResponse.DataStart;
        }

        public virtual SmtpResponse DataLine(SmtpSessionInfo sessionInfo, byte[] lineBuf)
        {
            return SmtpResponse.None;
        }

        public virtual SmtpResponse DataEnd(SmtpSessionInfo sessionInfo)
        {
            return SmtpResponse.OK;
        }
    }
}
