namespace Simple.MailServer.Smtp
{
    public interface IRespondToSmtpRawLine
    {
        SmtpResponse RawLine(SmtpSessionInfo sessionInfo, string line);
    }
}
