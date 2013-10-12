namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpRecipientToResponder : IRespondToSmtpRecipientTo
    {
        public static readonly DefaultSmtpRecipientToResponder Instance = new DefaultSmtpRecipientToResponder();

        public SmtpResponse VerifyRecipientTo(SmtpSessionInfo sessionInfo, MailAddressWithParameters mailAddressWithParameters)
        {
            return SmtpResponse.OK;
        }
    }
}
