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

using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Simple.MailServer.Mime;
using Simple.MailServer.Tests.Helpers;
using Simple.MailServer.Smtp;
using Simple.MailServer.Smtp.Config;
using Xunit;

namespace Simple.MailServer.Tests.Smtp
{
    public class SmtpServerFacts
    {
        [Fact]
        public void SmtpServer_should_be_able_to_receive_mail_from_SmtpClient()
        {
            var mockEmailValidator = new Mock<IEmailValidator>();
            mockEmailValidator.Setup(x => x.Validate(It.IsAny<string>())).Returns(true);

            var testPort = TestHelpers.GetTestPort();
            var mailDataCollector = new TestMailDataCollector();

            using (var smtpServer = new SmtpServer())
            {
                var responderFactory = new SmtpResponderFactory<ISmtpServerConfiguration>(smtpServer.Configuration, mockEmailValidator.Object) { DataResponder = mailDataCollector };
                smtpServer.DefaultResponderFactory = responderFactory;
                smtpServer.BindAndListenTo(IPAddress.Loopback, testPort);

                var sendMailTask =
                    Task.Factory.StartNew(() => SendMail(testPort, "mail@from.me", "mail@to.you", "subject", "body"),
                        TaskCreationOptions.LongRunning);

                Assert.True(sendMailTask.Wait(3000));
                mockEmailValidator.Verify(x => x.Validate("mail@from.me"));
                mockEmailValidator.Verify(x => x.Validate("mail@to.you"));

                var mailData = mailDataCollector.MailData;
                Assert.Contains("mail@from.me", mailData);
                Assert.Contains("mail@to.you", mailData);
                Assert.Contains("subject", mailData);
                Assert.Contains("body", mailData);
            }
        }

        #region Helpers

        private class TestMailDataCollector : IRespondToSmtpData
        {
            private readonly StringBuilder _mailData = new StringBuilder();
            public string MailData
            {
                get { return _mailData.ToString(); }
            }

            public SmtpResponse DataStart(ISmtpSessionInfo sessionInfo)
            {
                return SmtpResponses.DataStart;
            }

            public SmtpResponse DataLine(ISmtpSessionInfo sessionInfo, byte[] lineBuf)
            {
                _mailData.AppendLine(Encoding.UTF8.GetString(lineBuf));
                return SmtpResponses.None;
            }

            public SmtpResponse DataEnd(ISmtpSessionInfo sessionInfo)
            {
                return SmtpResponses.OK;
            }
        }

        private void SendMail(int port, string mailFrom, string recipients, string subject, string body)
        {
            using (var smtpClient = new SmtpClient(IPAddress.Loopback.ToString(), port))
            {
                smtpClient.Send(mailFrom, recipients, subject, body);
            }
        }

        #endregion
    }
}
