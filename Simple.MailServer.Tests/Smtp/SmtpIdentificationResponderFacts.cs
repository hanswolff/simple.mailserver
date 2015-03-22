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

using Moq;
using Simple.MailServer.Smtp;
using Simple.MailServer.Smtp.Capabilities;
using Simple.MailServer.Smtp.Config;
using Xunit;

namespace Simple.MailServer.Tests.Smtp
{
    public class SmtpIdentificationResponderFacts
    {
        [Fact]
        public void EHLO_respond_with_capabilities()
        {
            var mockGetCapabilities = new Mock<IGetSmtpCapabilities>();
            mockGetCapabilities.Setup(x => x.GetCapabilities())
                .Returns(new[] { new SmtpCapability("CAP1", "param"), new SmtpCapability("CAP2"), new SmtpCapability("CAP3", "with parameter") });

            var responder = new SmtpIdentificationResponder(new Mock<IConfiguredSmtpRestrictions>().Object,
                mockGetCapabilities.Object);

            var response = responder.VerifyIdentification(new Mock<ISmtpSessionInfo>().Object,
                new SmtpIdentification(SmtpIdentificationMode.EHLO, "localhost"));

            Assert.Equal(250, response.ResponseCode);
            Assert.Equal("CAP1 param", response.ResponseText);

            Assert.Equal("250-CAP2", response.AdditionalLines[0]);
            Assert.Equal("250-CAP3 with parameter", response.AdditionalLines[1]);
        }
    }
}
