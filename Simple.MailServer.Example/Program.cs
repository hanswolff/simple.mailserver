#region Header
// Program.cs
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
using Simple.MailServer.Logging;
using Simple.MailServer.Smtp;
using System;
using System.Net;
using Simple.MailServer.Smtp.Config;

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
            smtpServer.DefaultResponderFactory = new DefaultSmtpResponderFactory<ISmtpServerConfiguration>(smtpServer.Configuration)
            {
                DataResponder = new ExampleDataResponder(smtpServer.Configuration, RootMailDir)
            };

            smtpServer.BindAndListenTo(IPAddress.Loopback, 25);
            return smtpServer;
        }
    }
}
