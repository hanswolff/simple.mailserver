namespace Simple.MailServer.Smtp
{
    public interface IRespondToSmtpData
    {
        SmtpResponse DataStart(SmtpSessionInfo sessionInfo);
        SmtpResponse DataLine(SmtpSessionInfo sessionInfo, byte[] lineBuf);
        SmtpResponse DataEnd(SmtpSessionInfo sessionInfo);
    }
}
