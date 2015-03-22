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
using System.Linq;
using Simple.MailServer.Smtp.Capabilities;
using Simple.MailServer.Smtp.Config;

namespace Simple.MailServer.Smtp
{
    public class SmtpIdentificationResponder : IRespondToSmtpIdentification
    {
        protected readonly IConfiguredSmtpRestrictions Configuration;
        private readonly IGetSmtpCapabilities _getSmtpCapabilities;

        public SmtpIdentificationResponder(IConfiguredSmtpRestrictions configuration, IGetSmtpCapabilities getSmtpCapabilities)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (getSmtpCapabilities == null) throw new ArgumentNullException("getSmtpCapabilities");

            Configuration = configuration;
            _getSmtpCapabilities = getSmtpCapabilities;
        }

        public virtual SmtpResponse VerifyIdentification(ISmtpSessionInfo sessionInfo, SmtpIdentification smtpIdentification)
        {
            if (smtpIdentification == null) throw new ArgumentNullException("smtpIdentification");

            switch (smtpIdentification.Mode)
            {
                case SmtpIdentificationMode.HELO:
                    return VerifyHelo();

                case SmtpIdentificationMode.EHLO:
                    return VerifyEhlo();
            }

            return new SmtpResponse(500, "Invalid Identification (" + smtpIdentification.Mode + ")");
        }

        private SmtpResponse VerifyHelo()
        {
            return SmtpResponses.OK;
        }

        private SmtpResponse VerifyEhlo()
        {
            var smtpCapabilities = _getSmtpCapabilities.GetCapabilities().ToList();

            if (!smtpCapabilities.Any())
                return SmtpResponses.OK;

            var additionalLines = smtpCapabilities.Skip(1).Select(capability => "250-" + capability).ToList();

            var response = new SmtpResponse(250, smtpCapabilities.First().ToString(), additionalLines);
            return response;
        }
    }
}
