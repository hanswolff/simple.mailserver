#region Header
// Copyright (c) 2013-2015 Hans Wolff
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System;
using Simple.MailServer.Mime;
using Simple.MailServer.Smtp.Config;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpResponderFactory<T> : ISmtpResponderFactory where T : IConfiguredSmtpRestrictions
    {
        private readonly T _configuration;
        private readonly IEmailValidator _emailValidator;

        public DefaultSmtpResponderFactory(T configuration)
            : this(configuration, new XamarinEmailValidator())
        {
        }

        public DefaultSmtpResponderFactory(T configuration, IEmailValidator emailValidator)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (emailValidator == null) throw new ArgumentNullException("emailValidator");

            _configuration = configuration;
            _emailValidator = emailValidator;

            _dataResponder = new DefaultSmtpDataResponder<T>(_configuration);
            _identificationResponder = new DefaultSmtpIdentificationResponder<T>(_configuration);
            _mailFromResponder = new DefaultSmtpMailFromResponder<T>(_configuration, emailValidator);
            _recipientToResponder = new DefaultSmtpRecipientToResponder<T>(_configuration, emailValidator);
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
            set { _mailFromResponder = value ?? new DefaultSmtpMailFromResponder<T>(_configuration, _emailValidator); }
        }

        private IRespondToSmtpRecipientTo _recipientToResponder;
        public IRespondToSmtpRecipientTo RecipientToResponder
        {
            get { return _recipientToResponder; }
            set { _recipientToResponder = value ?? new DefaultSmtpRecipientToResponder<T>(_configuration, _emailValidator); }
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
