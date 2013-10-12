namespace Simple.MailServer.Smtp
{
    public interface IRespondToSmtpVerify
    {
        SmtpResponse Verify(SmtpSessionInfo sessionInfo, string arguments);
    }
}
