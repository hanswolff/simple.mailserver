using System;
using System.Threading.Tasks;
using Moq.Language.Flow;
using Simple.MailServer.Mime;
using System.IO;
using System.Text;

namespace Simple.MailServer.Tests
{
    public static class TestHelpers
    {
        public static MemoryStream TextToStream(string text)
        {
            return new MemoryStream(Encoding.Default.GetBytes(text));
        }

        public static string ReadMemoryStreamIntoString(MemoryStream mem)
        {
            return Encoding.Default.GetString(mem.ToArray());
        }

        public static StringReaderStream CreateStringReaderStreamFromText(string text, int bufferSize = 4096)
        {
            var textAsBytes = Encoding.ASCII.GetBytes(text ?? "");
            var memoryStream = new MemoryStream(textAsBytes);
            return new StringReaderStream(memoryStream, bufferSize);
        }

    }
}
