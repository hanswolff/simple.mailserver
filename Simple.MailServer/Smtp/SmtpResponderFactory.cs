using Simple.MailServer.Smtp.Config;
using System;

namespace Simple.MailServer.Smtp
{
    public class SmtpResponderFactory
    {
        private readonly IConfiguredSmtpRestrictions _configuration;

        public SmtpResponderFactory(IConfiguredSmtpRestrictions configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            _configuration = configuration;

            _dataResponder = new DefaultSmtpDataResponder(_configuration);
            _identificationResponder = new DefaultSmtpIdentificationResponder(_configuration);
            _mailFromResponder = new DefaultSmtpMailFromResponder(_configuration);
            _recipientToResponder = new DefaultSmtpRecipientToResponder(_configuration);
            _rawLineResponder = new DefaultSmtpRawLineResponder(_configuration);
            _resetResponder = new DefaultSmtpResetResponder(_configuration);
            _verifyResponder = new DefaultSmtpVerifyResponder(_configuration);
        }

        private IRespondToSmtpData _dataResponder;
        public IRespondToSmtpData DataResponder
        {
            get { return _dataResponder; }
            set { _dataResponder = value ?? new DefaultSmtpDataResponder(_configuration); }
        }

        private IRespondToSmtpIdentification _identificationResponder;
        public IRespondToSmtpIdentification IdentificationResponder
        {
            get { return _identificationResponder; }
            set { _identificationResponder = value ?? new DefaultSmtpIdentificationResponder(_configuration); }
        }

        private IRespondToSmtpMailFrom _mailFromResponder;
        public IRespondToSmtpMailFrom MailFromResponder
        {
            get { return _mailFromResponder; }
            set { _mailFromResponder = value ?? new DefaultSmtpMailFromResponder(_configuration); }
        }

        private IRespondToSmtpRecipientTo _recipientToResponder;
        public IRespondToSmtpRecipientTo RecipientToResponder
        {
            get { return _recipientToResponder; }
            set { _recipientToResponder = value ?? new DefaultSmtpRecipientToResponder(_configuration); }
        }

        private IRespondToSmtpRawLine _rawLineResponder;
        public IRespondToSmtpRawLine RawLineResponder
        {
            get { return _rawLineResponder; }
            set { _rawLineResponder = value ?? new DefaultSmtpRawLineResponder(_configuration); }
        }

        private IRespondToSmtpReset _resetResponder;
        public IRespondToSmtpReset ResetResponder
        {
            get { return _resetResponder; }
            set { _resetResponder = value ?? new DefaultSmtpResetResponder(_configuration); }
        }

        private IRespondToSmtpVerify _verifyResponder;
        public IRespondToSmtpVerify VerifyResponder
        {
            get { return _verifyResponder; }
            set { _verifyResponder = value ?? new DefaultSmtpVerifyResponder(_configuration); }
        }
    }
}
