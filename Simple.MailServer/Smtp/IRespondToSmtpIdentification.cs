namespace Simple.MailServer.Smtp
{
    public interface IRespondToSmtpIdentification
    {
        SmtpResponse VerifyIdentification(SmtpSessionInfo sessionInfo, SmtpIdentification smtpIdentification);
    }
}
