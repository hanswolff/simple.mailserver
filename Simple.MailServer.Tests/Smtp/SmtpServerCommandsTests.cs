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
using Simple.MailServer.Smtp;
using Simple.MailServer.Tests.Helpers;
using Xunit;

namespace Simple.MailServer.Tests.Smtp
{
    public class SmtpServerCommandsTests
    {
        [Theory]
        [InlineData("DATA")]
        [InlineData("MAIL FROM:<test@test.com>")]
        [InlineData("RCPT TO:<test@test.com>")]
        [InlineData("VRFY test@test.com")]
        public void SmtpServer_commands_before_HELO_or_EHLO_not_allowed(string command)
        {
            var testPort = TestHelpers.GetTestPort();
            using (SmtpServer.CreateAndBind(IPAddress.Loopback, testPort))
            using (var client = new TestConnection(IPAddress.Loopback, testPort))
            {
                client.RunCommand(command, SmtpResponses.NotIdentified);
            }
        }

        [Theory]
        [InlineData("NOOP")]
        [InlineData("RSET")]
        public void SmtpServer_commands_before_HELO_or_EHLO_allowed(string command)
        {
            var testPort = TestHelpers.GetTestPort();
            using (SmtpServer.CreateAndBind(IPAddress.Loopback, testPort))
            using (var client = new TestConnection(IPAddress.Loopback, testPort))
            {
                client.RunCommand(command, 250);
            }
        }
    }
}
