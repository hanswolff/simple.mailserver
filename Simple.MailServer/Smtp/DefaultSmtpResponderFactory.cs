using Simple.MailServer.Smtp.Config;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpResponderFactory<T> : ISmtpResponderFactory where T : IConfiguredSmtpRestrictions
    {
        private readonly T _configuration;

        public DefaultSmtpResponderFactory(T configuration)
        {
            _configuration = configuration;

            _dataResponder = new DefaultSmtpDataResponder<T>(_configuration);
            _identificationResponder = new DefaultSmtpIdentificationResponder<T>(_configuration);
            _mailFromResponder = new DefaultSmtpMailFromResponder<T>(_configuration);
            _recipientToResponder = new DefaultSmtpRecipientToResponder<T>(_configuration);
            _rawLineResponder = new DefaultSmtpRawLineResponder<T>(_configuration);
            _resetResponder = new DefaultSmtpResetResponder<T>(_configuration);
            _verifyResponder = new DefaultSmtpVerifyResponder<T>(_configuration);
        }

        private IRespondToSmtpData _dataResponder;
        public IRespondToSmtpData DataResponder
        {
            get { return _dataResponder; }
            set { _dataResponder = value ?? new DefaultSmtpDataResponder<T>(_configuration); }
        }

        private IRespondToSmtpIdentification _identificationResponder;
        public IRespondToSmtpIdentification IdentificationResponder
        {
            get { return _identificationResponder; }
            set { _identificationResponder = value ?? new DefaultSmtpIdentificationResponder<T>(_configuration); }
        }

        private IRespondToSmtpMailFrom _mailFromResponder;
        public IRespondToSmtpMailFrom MailFromResponder
        {
            get { return _mailFromResponder; }
            set { _mailFromResponder = value ?? new DefaultSmtpMailFromResponder<T>(_configuration); }
        }

        private IRespondToSmtpRecipientTo _recipientToResponder;
        public IRespondToSmtpRecipientTo RecipientToResponder
        {
            get { return _recipientToResponder; }
            set { _recipientToResponder = value ?? new DefaultSmtpRecipientToResponder<T>(_configuration); }
        }

        private IRespondToSmtpRawLine _rawLineResponder;
        public IRespondToSmtpRawLine RawLineResponder
        {
            get { return _rawLineResponder; }
            set { _rawLineResponder = value ?? new DefaultSmtpRawLineResponder<T>(_configuration); }
        }

        private IRespondToSmtpReset _resetResponder;
        public IRespondToSmtpReset ResetResponder
        {
            get { return _resetResponder; }
            set { _resetResponder = value ?? new DefaultSmtpResetResponder<T>(_configuration); }
        }

        private IRespondToSmtpVerify _verifyResponder;
        public IRespondToSmtpVerify VerifyResponder
        {
            get { return _verifyResponder; }
            set { _verifyResponder = value ?? new DefaultSmtpVerifyResponder<T>(_configuration); }
        }
    }
}
