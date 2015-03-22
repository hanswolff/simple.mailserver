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
using System.IO;
using Simple.MailServer.Tests.Helpers;
using Xunit;
using Xunit.Extensions;

namespace Simple.MailServer.Tests.Mime
{
    public class DelimitingReadableStreamFacts
    {
        [Fact]
        public void DelimitingReadableStream_is_never_writable()
        {
            var stream = new MemoryStream();
            Assert.False(new DelimitingReadableStream(stream).CanWrite);
        }

        [Fact]
        public void writing_to_DelimitingReadableStream_throws_NotSupportedException()
        {
            var buffer = new byte[0];

            var stream = new MemoryStream();
            Assert.Throws<NotSupportedException>(() => new DelimitingReadableStream(stream).Write(buffer, 0, 0));
        }

        [Fact]
        public void not_passing_MaxReadPosition_does_not_restrict_reading_stream()
        {
            const string inputText = "text1234";
            const string expected = "text1234";

            var stream = new DelimitingReadableStream(TestHelpers.TextToStream(inputText));
            Assert.Equal(expected, new StreamReader(stream).ReadToEnd());
        }

        [Theory]
        [InlineData("", 0, "")]
        [InlineData("test", 0, "")]
        [InlineData("test", 1, "t")]
        [InlineData("test", 4, "test")]
        [InlineData("test", 5, "test")]
        [InlineData("test1234", 4, "test")]
        public void passing_MaxReadPosition_should_restrict_reading_stream(string text, int delimitedTo, string expected)
        {
            var stream = new DelimitingReadableStream(TestHelpers.TextToStream(text), delimitedTo);
            Assert.Equal(expected, new StreamReader(stream).ReadToEnd());
        }
    }
}
