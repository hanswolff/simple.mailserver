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

namespace Simple.MailServer.Smtp
{
    public static class SmtpResponses
    {
        public static readonly int DisconnectResponseCode = 221;

        public static readonly SmtpResponse None = SmtpResponse.None;
        
        public static SmtpResponse OK = new SmtpResponse(250, "OK");
        public static SmtpResponse DataStart = new SmtpResponse(354, "OK");
        public static SmtpResponse Disconnect = new SmtpResponse(DisconnectResponseCode, "Bye");

        public static SmtpResponse InternalServerError = new SmtpResponse(500, "Internal Server Error");
        public static SmtpResponse LineTooLong = new SmtpResponse(500, "Line Too Long");
        public static SmtpResponse NotImplemented = new SmtpResponse(502, "5.5.2 Command not implemented");
        public static SmtpResponse NotIdentified = new SmtpResponse(502, "5.5.1 Use HELO/EHLO first.");
        public static SmtpResponse SyntaxError = new SmtpResponse(501, "Syntax error in parameters or arguments");
        
        public static SmtpResponse EhloMissingDomainAddress = new SmtpResponse(501, "EHLO Missing domain address.");
        public static SmtpResponse HeloMissingDomainAddress = new SmtpResponse(501, "HELO Missing domain address.");
        public static SmtpResponse VrfyMissingArguments = new SmtpResponse(501, "VRFY Missing parameter.");
        public static SmtpResponse UseMailFromFirst = new SmtpResponse(502, "5.5.1 Use MAIL FROM first.");
        public static SmtpResponse MustHaveRecipientFirst = new SmtpResponse(503, "5.5.1 Must have recipient first");
        public static SmtpResponse VerifyDummyResponse = new SmtpResponse(252, "2.5.2 Send some mail, I'll try my best");
    }
}
