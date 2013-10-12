namespace Simple.MailServer.Smtp
{
    public interface IRespondToSmtpRecipientTo
    {
        SmtpResponse VerifyRecipientTo(SmtpSessionInfo sessionInfo, MailAddressWithParameters mailAddressWithParameters);
    }
}
