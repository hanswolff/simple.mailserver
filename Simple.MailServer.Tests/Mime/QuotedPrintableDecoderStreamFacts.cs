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

using Simple.MailServer.Mime;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit;

namespace Simple.MailServer.Tests.Mime
{
    public class QuotedPrintableDecoderStreamFacts
    {
        [Fact]
        public void decode_text_with_equals_sign()
        {
            var decoded = DecodeUsingQuotedPrintableDecoderStream("truth=3Dbeauty");
            Assert.Equal("truth=beauty", decoded);
        }

        [Fact]
        public void read_up_to_MaxReadPosition_only()
        {
            var decoded = DecodeUsingQuotedPrintableDecoderStream("truthordare", maxReadPosition: 5);
            Assert.Equal("truth", decoded);
        }

        [Fact]
        public void exception_when_invalid_quoted_printable_and_IgnoreExceptions_false()
        {
            Assert.Throws<InvalidOperationException>(
                () =>
                {
                    DecodeUsingQuotedPrintableDecoderStream("truth=3G", ignoreErrors: false);
                });
        }

        [Fact]
        public void replace_invalid_quoted_printable_with_zero_byte_when_IgnoreExceptions_true()
        {
            var decoded = DecodeUsingQuotedPrintableDecoderStream("truth=3G", ignoreErrors: true);
            Assert.Equal("truth\0", decoded);
        }

        [Fact]
        public void text_with_linebreak()
        {
            var decoded = DecodeUsingQuotedPrintableDecoderStream("truth\r\ndare");
            Assert.Equal("truth\r\ndare", decoded);
        }

        [Fact]
        public void text_with_dollar_sign()
        {
            var decoded = DecodeUsingQuotedPrintableDecoderStream("dollar sign $");
            Assert.Equal("dollar sign $", decoded);
        }

        [Fact]
        public void text_with_cent_sign_different_encoding()
        {
            var decoded = DecodeUsingQuotedPrintableDecoderStream("cent sign =A2", Encoding.GetEncoding("iso-8859-1"));
            Assert.Equal("cent sign ¢", decoded);
        }

        [Fact]
        public void text_over_75_chars_with_end_of_line_concatenation()
        {
            var decoded = DecodeUsingQuotedPrintableDecoderStream("If you believe that truth=3Dbeauty, then surely mathematics is the most bea=\r\nutiful branch of philosophy.");
            Assert.Equal("If you believe that truth=beauty, then surely mathematics is the most beautiful branch of philosophy.", decoded);
        }

        [Fact(Skip = "performance test")]
        public void PerformanceTest()
        {
            var buf = new byte[1 * 1024 * 1024];
            for (var i = 0; i < buf.Length; i++)
                buf[i] = (byte)i;

            using (var mem = new MemoryStream(3 * buf.Length))
            using (var encoder = new QuotedPrintableEncoderStream(mem))
            {
                var stopwatch = Stopwatch.StartNew();
                encoder.Write(buf, 0, buf.Length);
                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                var message = String.Format(
                    "Encoded {0} bytes in {1} ms ({2} MB/s)",
                    buf.Length,
                    elapsedMilliseconds,
                    ((double) buf.Length*1000/1024/1024/stopwatch.ElapsedMilliseconds).ToString("F2")
                    );

                Assert.True(false, message);
            }
        }

        #region Helpers

        public static string DecodeUsingQuotedPrintableDecoderStream(string text, Encoding encoding = null, bool? ignoreErrors = null, long? maxReadPosition = null)
        {
            var textStream = TestHelpers.TextToStream(text);

            var quotedPrintableDecoderStream = new QuotedPrintableDecoderStream(textStream);
            quotedPrintableDecoderStream.IgnoreErrors = ignoreErrors ?? quotedPrintableDecoderStream.IgnoreErrors;
            quotedPrintableDecoderStream.MaxReadPosition = maxReadPosition ?? quotedPrintableDecoderStream.MaxReadPosition;

            return new StreamReader(quotedPrintableDecoderStream, encoding ?? Encoding.UTF8).ReadToEnd();
        }

        #endregion
    }
}
