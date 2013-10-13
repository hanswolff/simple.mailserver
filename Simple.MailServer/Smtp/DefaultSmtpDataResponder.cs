using Simple.MailServer.Smtp.Config;
using System;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpDataResponder : IRespondToSmtpData
    {
        protected readonly IConfiguredSmtpRestrictions Configuration;

        public DefaultSmtpDataResponder(IConfiguredSmtpRestrictions configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
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
