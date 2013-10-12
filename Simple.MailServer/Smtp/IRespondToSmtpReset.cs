namespace Simple.MailServer.Smtp
{
    public interface IRespondToSmtpReset
    {
        SmtpResponse Reset(SmtpSessionInfo sessionInfo);
    }
}
