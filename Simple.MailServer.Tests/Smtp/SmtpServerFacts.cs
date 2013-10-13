using Simple.MailServer.Smtp;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Simple.MailServer.Tests.Smtp
{
    public class SmtpServerFacts
    {
        [Fact(Timeout = 1000)]
        public void SmtpServer_should_be_able_to_receive_mail_from_SmtpClient()
        {
            var testPort = GetTestPort();
            var mailDataCollector = new TestMailDataCollector();

            using (var smtpServer = new SmtpServer())
            {
                var responderFactory = new DefaultSmtpResponderFactory(smtpServer.Configuration) { DataResponder = mailDataCollector };
                smtpServer.DefaultResponderFactory = responderFactory;
                smtpServer.BindAndListenTo(IPAddress.Loopback, testPort);

                var sendMailTask =
                    Task.Factory.StartNew(() => SendMail(testPort, "mail@from.me", "mail@to.you", "subject", "body"),
                        TaskCreationOptions.LongRunning);

                Assert.True(sendMailTask.Wait(1000));

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

            public SmtpResponse DataStart(SmtpSessionInfo sessionInfo)
            {
                return SmtpResponse.DataStart;
            }

            public SmtpResponse DataLine(SmtpSessionInfo sessionInfo, byte[] lineBuf)
            {
                _mailData.AppendLine(Encoding.UTF8.GetString(lineBuf));
                return SmtpResponse.None;
            }

            public SmtpResponse DataEnd(SmtpSessionInfo sessionInfo)
            {
                return SmtpResponse.OK;
            }
        }

        int TestPort { get; set; }

        private int GetTestPort()
        {
            return TestPort++;
        }

        public SmtpServerFacts()
        {
            TestPort = new Random(Environment.TickCount).Next(10000, 65000);
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
