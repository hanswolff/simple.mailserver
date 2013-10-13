using Simple.MailServer.Smtp.Config;
using System;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpIdentificationResponder : IRespondToSmtpIdentification
    {
        protected readonly IConfiguredSmtpRestrictions Configuration;

        public DefaultSmtpIdentificationResponder(IConfiguredSmtpRestrictions configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            Configuration = configuration;
        }

        public virtual SmtpResponse VerifyIdentification(SmtpSessionInfo sessionInfo, SmtpIdentification smtpIdentification)
        {
            if (smtpIdentification.Mode == SmtpIdentificationMode.HELO)
            {
                return VerifyHelo();
            }

            if (smtpIdentification.Mode == SmtpIdentificationMode.EHLO)
            {
                return VerifyEhlo();
            }

            return new SmtpResponse(500, "Invalid Identification (" + smtpIdentification.Mode + ")");
        }

        private SmtpResponse VerifyHelo()
        {
            return SmtpResponse.OK;
        }

        private SmtpResponse VerifyEhlo()
        {
            var response = new SmtpResponse(250, "SIZE " + Configuration.MaxMailMessageSize);
            response.AdditionalLines.Add("250-PIPELINING");
            return response;
        }
    }
}
