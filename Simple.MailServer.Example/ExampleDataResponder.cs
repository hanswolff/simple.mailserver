#region Header
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

using Simple.MailServer.Smtp;
using Simple.MailServer.Smtp.Config;
using System;
using System.IO;
using System.Text;

namespace Simple.MailServer.Example
{
    class ExampleDataResponder : SmtpDataResponder
    {
        private readonly string _mailDir;

        public ExampleDataResponder(ISmtpServerConfiguration configuration, string mailDir)
            : base(configuration)
        {
            _mailDir = mailDir;
            EnsureDirExists(mailDir);
        }

        private static void EnsureDirExists(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        public override SmtpResponse DataStart(ISmtpSessionInfo sessionInfo)
        {
            Console.WriteLine("Start receiving mail: {0}", GetFileNameFromSessionInfo(sessionInfo));
            return SmtpResponses.DataStart;
        }

        private string GetFileNameFromSessionInfo(ISmtpSessionInfo sessionInfo)
        {
            var fileName = sessionInfo.CreatedTimestamp.ToString("yyyy-MM-dd_HHmmss_fff") + ".eml";
            var fullName = Path.Combine(_mailDir, fileName);
            return fullName;
        }

        public override SmtpResponse DataLine(ISmtpSessionInfo sessionInfo, byte[] lineBuf)
        {
            var fileName = GetFileNameFromSessionInfo(sessionInfo);

            Console.WriteLine("{0} <<< {1}", fileName, Encoding.UTF8.GetString(lineBuf));

            using (var stream = File.OpenWrite(fileName))
            {
                stream.Seek(0, SeekOrigin.End);
                stream.Write(lineBuf, 0, lineBuf.Length);

                stream.WriteByte(13);
                stream.WriteByte(10);
            }

            return SmtpResponses.None;
        }

        public override SmtpResponse DataEnd(ISmtpSessionInfo sessionInfo)
        {
            var fileName = GetFileNameFromSessionInfo(sessionInfo);
            var size = GetFileSize(fileName);

            Console.WriteLine("Mail received ({0} bytes): {1}", size, fileName);

            var successMessage = String.Format("{0} bytes received", size);
            var response = SmtpResponses.OK.CloneAndChange(successMessage);

            return response;
        }

        private long GetFileSize(string fileName)
        {
            return new FileInfo(fileName).Length;
        }
    }
}
