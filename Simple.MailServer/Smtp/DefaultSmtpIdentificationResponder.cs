#region Header
// DefaultSmtpIdentificationResponder.cs
// 
// Copyright (c) 2013 Hans Wolff
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
using Simple.MailServer.Smtp.Config;
using System;

namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpIdentificationResponder<T> : IRespondToSmtpIdentification where T : IConfiguredSmtpRestrictions
    {
        protected readonly T Configuration;

        public DefaultSmtpIdentificationResponder(T configuration)
        {
            Configuration = configuration;
        }

        public virtual SmtpResponse VerifyIdentification(SmtpSessionInfo sessionInfo, SmtpIdentification smtpIdentification)
        {
            if (smtpIdentification.Mode == SmtpIdentificationMode.HELO)
            {
                return VerifyHelo();
            }

            if (smtpIdentification.Mode == SmtpIdentificationMode.EHLO)
            {
                return VerifyEhlo();
            }

            return new SmtpResponse(500, "Invalid Identification (" + smtpIdentification.Mode + ")");
        }

        private SmtpResponse VerifyHelo()
        {
            return SmtpResponse.OK;
        }

        private SmtpResponse VerifyEhlo()
        {
            var response = new SmtpResponse(250, "SIZE " + Configuration.MaxMailMessageSize, new[] { "250-PIPELINING" });
            return response;
        }
    }
}
