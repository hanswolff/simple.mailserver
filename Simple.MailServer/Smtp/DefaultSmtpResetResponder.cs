namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpResetResponder : IRespondToSmtpReset
    {
        public static readonly DefaultSmtpResetResponder Instance = new DefaultSmtpResetResponder();

        public SmtpResponse Reset(SmtpSessionInfo sessionInfo)
        {
            return SmtpResponse.OK;
        }
    }
}
