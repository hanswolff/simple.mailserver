using Simple.MailServer.Logging;
using Simple.MailServer.Smtp;
using System;
using System.Net;

namespace Simple.MailServer.Example
{
    class Program
    {
        private const string RootMailDir = @"C:\Temp\Mail\";

        static void Main()
        {
            MailServerLogger.Set(new MailServerConsoleLogger(MailServerLogLevel.Debug));

            using (StartSmtpServer())
            {
                Console.WriteLine("Hit ENTER to return.");
                Console.ReadLine();
            }
        }

        private static SmtpServer StartSmtpServer()
        {
            var smtpServer = new SmtpServer();
            smtpServer.Configuration.DefaultGreeting = "Simple.MailServer Example";
            smtpServer.DefaultResponderFactory = new SmtpResponderFactory(smtpServer.Configuration)
            {
                DataResponder = new ExampleDataResponder(smtpServer.Configuration, RootMailDir)
            };

            smtpServer.BindAndListenTo(IPAddress.Loopback, 25);
            return smtpServer;
        }
    }
}
