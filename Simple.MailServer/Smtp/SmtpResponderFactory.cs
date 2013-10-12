namespace Simple.MailServer.Smtp
{
    public class SmtpResponderFactory
    {
        public static readonly SmtpResponderFactory Default = new SmtpResponderFactory();

        private IRespondToSmtpData _dataResponder = DefaultSmtpDataResponder.Instance;
        public IRespondToSmtpData DataResponder
        {
            get { return _dataResponder; }
            set { _dataResponder = value ?? DefaultSmtpDataResponder.Instance; }
        }

        private IRespondToSmtpIdentification _identificationResponder = DefaultSmtpIdentificationResponder.Instance;
        public IRespondToSmtpIdentification IdentificationResponder
        {
            get { return _identificationResponder; }
            set { _identificationResponder = value ?? DefaultSmtpIdentificationResponder.Instance; }
        }

        private IRespondToSmtpMailFrom _mailFromResponder = DefaultSmtpMailFromResponder.Instance;
        public IRespondToSmtpMailFrom MailFromResponder
        {
            get { return _mailFromResponder; }
            set { _mailFromResponder = value ?? DefaultSmtpMailFromResponder.Instance; }
        }

        private IRespondToSmtpRecipientTo _recipientToResponder = DefaultSmtpRecipientToResponder.Instance;
        public IRespondToSmtpRecipientTo RecipientToResponder
        {
            get { return _recipientToResponder; }
            set { _recipientToResponder = value ?? DefaultSmtpRecipientToResponder.Instance; }
        }

        private IRespondToSmtpRawLine _rawLineResponder = DefaultSmtpRawLineResponder.Instance;
        public IRespondToSmtpRawLine RawLineResponder
        {
            get { return _rawLineResponder; }
            set { _rawLineResponder = value ?? DefaultSmtpRawLineResponder.Instance; }
        }

        private IRespondToSmtpReset _resetResponder = DefaultSmtpResetResponder.Instance;
        public IRespondToSmtpReset ResetResponder
        {
            get { return _resetResponder; }
            set { _resetResponder = value ?? DefaultSmtpResetResponder.Instance; }
        }

        private IRespondToSmtpVerify _verifyResponder = DefaultSmtpVerifyResponder.Instance;
        public IRespondToSmtpVerify VerifyResponder
        {
            get { return _verifyResponder; }
            set { _verifyResponder = value ?? DefaultSmtpVerifyResponder.Instance; }
        }
    }
}
