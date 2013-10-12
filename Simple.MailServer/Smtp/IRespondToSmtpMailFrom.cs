namespace Simple.MailServer.Smtp
{
    public interface IRespondToSmtpMailFrom
    {
        SmtpResponse VerifyMailFrom(SmtpSessionInfo sessionInfo, MailAddressWithParameters mailAddressWithParameters);
    }
}
