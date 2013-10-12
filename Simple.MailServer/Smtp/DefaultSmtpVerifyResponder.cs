namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpVerifyResponder : IRespondToSmtpVerify
    {
        public static readonly DefaultSmtpVerifyResponder Instance = new DefaultSmtpVerifyResponder();

        public SmtpResponse Verify(SmtpSessionInfo sessionInfo, string arguments)
        {
            return new SmtpResponse(252, "2.5.2 Send some mail, i'll try my best");
        }
    }
}
