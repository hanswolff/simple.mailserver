using Simple.MailServer.Mime;
using System.IO;
using Xunit;

namespace Simple.MailServer.Tests.Mime
{
    public class QuotedPrintableEncoderStreamFacts
    {
        [Fact]
        public void encode_test_with_equals_sign()
        {
            var encoded = EncodeUsingQuotedPrintableEncoderStream("truth=beauty");
            Assert.Equal("truth=3Dbeauty", encoded);
        }

        [Fact]
        public void text_with_linebreak()
        {
            var encoded = EncodeUsingQuotedPrintableEncoderStream("truth\r\nbeauty");
            Assert.Equal("truth\r\nbeauty", encoded);
        }

        [Fact]
        public void text_with_dollar_sign()
        {
            var encoded = EncodeUsingQuotedPrintableEncoderStream("dollar sign $");
            Assert.Equal("dollar sign $", encoded);
        }

        [Fact]
        public void text_with_cent_sign()
        {
            var encoded = EncodeUsingQuotedPrintableEncoderStream("cent sign ¢");
            Assert.Equal("cent sign =A2", encoded);
        }

        [Fact]
        public void text_over_75_chars_with_end_of_line_concatenation()
        {
            const string text = "If you believe that truth=beauty, then surely mathematics is the most beautiful branch of philosophy.";
            var encoded = EncodeUsingQuotedPrintableEncoderStream(text);
            Assert.Equal("If you believe that truth=3Dbeauty, then surely mathematics is the most bea=\r\nutiful branch of philosophy.", encoded);
        }

        #region Helpers

        public static string EncodeUsingQuotedPrintableEncoderStream(string text)
        {
            var mem = new MemoryStream();

            var quotedPrintableEncoderStream = new QuotedPrintableEncoderStream(mem);
            quotedPrintableEncoderStream.Write(text);

            return TestHelpers.ReadMemoryStreamIntoString(mem);
        }


        #endregion
    }
}
