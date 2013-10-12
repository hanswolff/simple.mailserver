namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpIdentificationResponder : IRespondToSmtpIdentification
    {
        public static readonly DefaultSmtpIdentificationResponder Instance = new DefaultSmtpIdentificationResponder();

        public SmtpResponse VerifyIdentification(SmtpSessionInfo sessionInfo, SmtpIdentification smtpIdentification)
        {
            return SmtpResponse.OK;
        }
    }
}
