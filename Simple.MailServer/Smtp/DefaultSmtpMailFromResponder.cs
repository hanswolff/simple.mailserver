namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpMailFromResponder : IRespondToSmtpMailFrom
    {
        public static readonly DefaultSmtpMailFromResponder Instance = new DefaultSmtpMailFromResponder();

        public SmtpResponse VerifyMailFrom(SmtpSessionInfo sessionInfo, MailAddressWithParameters mailAddressWithParameters)
        {
            return SmtpResponse.OK;
        }
    }
}
