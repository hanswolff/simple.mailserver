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
using System.IO;
using Simple.MailServer.Tests.Helpers;
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
