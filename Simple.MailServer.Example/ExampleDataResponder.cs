using Simple.MailServer.Smtp;
using Simple.MailServer.Smtp.Config;
using System;
using System.IO;
using System.Text;

namespace Simple.MailServer.Example
{
    class ExampleDataResponder : DefaultSmtpDataResponder<ISmtpServerConfiguration>
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

        public override SmtpResponse DataStart(SmtpSessionInfo sessionInfo)
        {
            Console.WriteLine("Start receiving mail: {0}", GetFileNameFromSessionInfo(sessionInfo));
            return SmtpResponse.DataStart;
        }

        private string GetFileNameFromSessionInfo(SmtpSessionInfo sessionInfo)
        {
            var fileName = Path.Combine(_mailDir, sessionInfo.CreatedTimestamp.ToString("yyyy-MM-dd_HHmmss_fff") + ".eml");
            return fileName;
        }

        public override SmtpResponse DataLine(SmtpSessionInfo sessionInfo, byte[] lineBuf)
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

            return SmtpResponse.None;
        }

        public override SmtpResponse DataEnd(SmtpSessionInfo sessionInfo)
        {
            var fileName = GetFileNameFromSessionInfo(sessionInfo);
            var size = GetFileSize(fileName);

            Console.WriteLine("Mail received ({0} bytes): {1}", size, fileName);

            var response = SmtpResponse.OK.CloneAndChange(String.Format("{0} bytes received", size));

            return response;
        }

        private long GetFileSize(string fileName)
        {
            return new FileInfo(fileName).Length;
        }
    }
}
