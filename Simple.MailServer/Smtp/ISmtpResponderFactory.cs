namespace Simple.MailServer.Smtp
{
    public interface ISmtpResponderFactory
    {
        IRespondToSmtpData DataResponder { get; }
        IRespondToSmtpIdentification IdentificationResponder { get; }
        IRespondToSmtpMailFrom MailFromResponder { get; }
        IRespondToSmtpRecipientTo RecipientToResponder { get; }
        IRespondToSmtpRawLine RawLineResponder { get; }
        IRespondToSmtpReset ResetResponder { get; }
        IRespondToSmtpVerify VerifyResponder { get; }
    }
}
