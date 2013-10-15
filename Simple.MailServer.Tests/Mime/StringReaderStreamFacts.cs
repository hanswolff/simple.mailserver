#region Header
// StringReaderStreamFacts.cs
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
using Moq;
using Simple.MailServer.Mime;
using System;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace Simple.MailServer.Tests.Mime
{
    public class StringReaderStreamFacts
    {
        [Fact]
        public void buffer_size_cannot_be_zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => TestHelpers.CreateStringReaderStreamFromText(null, 0));
        }

        [Fact]
        public void stream_must_be_readable()
        {
            Assert.Throws<ArgumentException>(() => new StringReaderStream(CreateNonReadableStream(), 0));
        }

        [Theory]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void text_stream_should_return_whole_text_if_fits_in_buffer_size(int bufferSizeGreaterTextLength)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText("text", bufferSizeGreaterTextLength);
            var expected = Encoding.Default.GetBytes("text");
            Assert.Equal(expected, stringReaderStream.ReadLine());
        }

        [Fact]
        public void text_stream_should_split_text_if_length_multiple_of_buffer_size()
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText("text1234", 4);
            var expected = Encoding.Default.GetBytes("text");
            Assert.Equal(expected, stringReaderStream.ReadLine());
            expected = Encoding.Default.GetBytes("1234");
            Assert.Equal(expected, stringReaderStream.ReadLine());
        }

        [Theory]
        [InlineData("text", 2, "te", "xt")]
        [InlineData("text", 3, "tex", "t")]
        [InlineData("text12", 4, "text", "12")]
        public void text_stream_should_split_text_if_length_greater_than_buffer_size_sync(string text, int bufSize, string expectedPart1, string expectedPart2)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText(text, bufSize);
            var expected = Encoding.Default.GetBytes(expectedPart1);
            Assert.Equal(expected, stringReaderStream.ReadLine());
            expected = Encoding.Default.GetBytes(expectedPart2);
            Assert.Equal(expected, stringReaderStream.ReadLine());
        }

        [Theory]
        [InlineData("text", 2, "te", "xt")]
        [InlineData("text", 3, "tex", "t")]
        [InlineData("text12", 4, "text", "12")]
        public void text_stream_should_split_text_if_length_greater_than_buffer_size_async(string text, int bufSize, string expectedPart1, string expectedPart2)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText(text, bufSize);
            var expected = Encoding.Default.GetBytes(expectedPart1);
            Assert.Equal(expected, stringReaderStream.ReadLineAsync().Result);
            expected = Encoding.Default.GetBytes(expectedPart2);
            Assert.Equal(expected, stringReaderStream.ReadLineAsync().Result);
        }

        [Theory]
        [InlineData("text\n", 5)]
        [InlineData("text\r", 5)]
        [InlineData("text\r\n", 4)]
        [InlineData("text\r\n", 5)]
        public void text_stream_with_text_length_equals_buffer_size_ending_with_linefeed_should_return_text_only_sync(string text, int bufSize)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText(text, 5);
            var expected = Encoding.Default.GetBytes(text.TrimEnd());
            Assert.Equal(expected, stringReaderStream.ReadLine());
            Assert.Null(stringReaderStream.ReadLine());
        }

        [Theory]
        [InlineData("text\n", 5)]
        [InlineData("text\r", 5)]
        [InlineData("text\r\n", 4)]
        [InlineData("text\r\n", 5)]
        public void text_stream_with_text_length_equals_buffer_size_ending_with_linefeed_should_return_text_only_async(string text, int bufSize)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText(text, 5);
            var expected = Encoding.Default.GetBytes(text.TrimEnd());
            Assert.Equal(expected, stringReaderStream.ReadLineAsync().Result);
            Assert.Null(stringReaderStream.ReadLine());
        }

        [Theory]
        [InlineData("text\n\n", 4)]
        [InlineData("text\n\n", 5)]
        [InlineData("text\n\n", 6)]
        [InlineData("text\r\r", 4)]
        [InlineData("text\r\r", 5)]
        [InlineData("text\r\r", 6)]
        [InlineData("text\r\n\r\n", 4)]
        [InlineData("text\r\n\r\n", 5)]
        [InlineData("text\r\n\r\n", 6)]
        [InlineData("text\r\n\r\n", 7)]
        [InlineData("text\r\n\r\n", 8)]
        public void text_stream_that_has_two_linebreaks_sync(string text, int bufSize)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText(text, 5);
            var expected = Encoding.Default.GetBytes(text.TrimEnd());
            Assert.Equal(expected, stringReaderStream.ReadLine());
            expected = Encoding.Default.GetBytes("");
            Assert.Equal(expected, stringReaderStream.ReadLine());
            Assert.Null(stringReaderStream.ReadLine());
        }

        [Theory]
        [InlineData("text\n\n", 4)]
        [InlineData("text\n\n", 5)]
        [InlineData("text\n\n", 6)]
        [InlineData("text\r\r", 4)]
        [InlineData("text\r\r", 5)]
        [InlineData("text\r\r", 6)]
        [InlineData("text\r\n\r\n", 4)]
        [InlineData("text\r\n\r\n", 5)]
        [InlineData("text\r\n\r\n", 6)]
        [InlineData("text\r\n\r\n", 7)]
        [InlineData("text\r\n\r\n", 8)]
        public void text_stream_that_has_two_linebreaks_async(string text, int bufSize)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText(text, 5);
            var expected = Encoding.Default.GetBytes(text.TrimEnd());
            Assert.Equal(expected, stringReaderStream.ReadLineAsync().Result);
            expected = Encoding.Default.GetBytes("");
            Assert.Equal(expected, stringReaderStream.ReadLineAsync().Result);
            Assert.Null(stringReaderStream.ReadLine());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void blank_stream_should_return_null_on_ReadLine(int bufferSize)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText(null, bufferSize);
            Assert.Null(stringReaderStream.ReadLine());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void describe_one_char_stream(int bufferSize)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText("a", bufferSize);
            var expected = Encoding.ASCII.GetBytes("a");
            Assert.Equal(expected, stringReaderStream.ReadLine());
            Assert.Null(stringReaderStream.ReadLine());
        }

        [Theory]
        [InlineData("\r", 1)]
        [InlineData("\r", 2)]
        [InlineData("\r", 3)]
        [InlineData("\r\n", 1)]
        [InlineData("\r\n", 2)]
        [InlineData("\r\n", 3)]
        [InlineData("\n", 1)]
        [InlineData("\n", 2)]
        [InlineData("\n", 3)]
        public void single_line_break_stream_sync(string lineBreak, int bufferSize)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText("a" + lineBreak, bufferSize);
            var expected = Encoding.ASCII.GetBytes("a");
            Assert.Equal(expected, stringReaderStream.ReadLine());
            Assert.Null(stringReaderStream.ReadLine());
        }

        [Theory]
        [InlineData("\r", 1)]
        [InlineData("\r", 2)]
        [InlineData("\r", 3)]
        [InlineData("\r\n", 1)]
        [InlineData("\r\n", 2)]
        [InlineData("\r\n", 3)]
        [InlineData("\n", 1)]
        [InlineData("\n", 2)]
        [InlineData("\n", 3)]
        public void single_line_break_stream_async(string lineBreak, int bufferSize)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText("a" + lineBreak, bufferSize);
            var expected = Encoding.ASCII.GetBytes("a");
            Assert.Equal(expected, stringReaderStream.ReadLineAsync().Result);
            Assert.Null(stringReaderStream.ReadLineAsync().Result);
        }

        [Theory]
        [InlineData("\n\r", 1)]
        [InlineData("\n\r", 2)]
        [InlineData("\n\r", 3)]
        [InlineData("\n\n", 1)]
        [InlineData("\n\n", 2)]
        [InlineData("\n\n", 3)]
        [InlineData("\r\r", 1)]
        [InlineData("\r\r", 2)]
        [InlineData("\r\r", 3)]
        public void describe_two_line_break_stream(string lineBreak, int bufferSize)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText("a" + lineBreak, bufferSize);

            var firstExpected = Encoding.ASCII.GetBytes("a");
            Assert.Equal(firstExpected, stringReaderStream.ReadLine());

            var thenExpected = Encoding.ASCII.GetBytes("");
            Assert.Equal(thenExpected, stringReaderStream.ReadLine());

            Assert.Null(stringReaderStream.ReadLine());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void multiple_lines_stream_sync(int bufferSize)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText("a\r\nb\rc", bufferSize);
            
            var firstExpected = Encoding.ASCII.GetBytes("a");
            Assert.Equal(firstExpected, stringReaderStream.ReadLine());

            var secondExpected = Encoding.ASCII.GetBytes("b");
            Assert.Equal(secondExpected, stringReaderStream.ReadLine());

            var thirdExpected = Encoding.ASCII.GetBytes("c");
            Assert.Equal(thirdExpected, stringReaderStream.ReadLine());

            Assert.Null(stringReaderStream.ReadLine());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void multiple_lines_stream_async(int bufferSize)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText("a\r\nb\rc", bufferSize);

            var firstExpected = Encoding.ASCII.GetBytes("a");
            Assert.Equal(firstExpected, stringReaderStream.ReadLineAsync().Result);

            var secondExpected = Encoding.ASCII.GetBytes("b");
            Assert.Equal(secondExpected, stringReaderStream.ReadLineAsync().Result);

            var thirdExpected = Encoding.ASCII.GetBytes("c");
            Assert.Equal(thirdExpected, stringReaderStream.ReadLineAsync().Result);

            Assert.Null(stringReaderStream.ReadLine());
        }

        [Theory]
        [InlineData("A\r\n\r\nB\r\rC", 1)]
        [InlineData("A\r\n\r\nB\r\rC", 2)]
        [InlineData("A\r\n\r\nB\r\rC", 3)]
        [InlineData("A\r\n\r\nB\r\n\r\nC", 1)]
        [InlineData("A\r\n\r\nB\r\n\r\nC", 2)]
        [InlineData("A\r\n\r\nB\r\n\r\nC", 3)]
        public void multiple_lines_double_linebreaks_stream_sync(string text, int bufferSize)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText(text, bufferSize);

            var firstExpected = Encoding.ASCII.GetBytes("A");
            Assert.Equal(firstExpected, stringReaderStream.ReadLine());

            stringReaderStream.ReadLine();

            var secondExpected = Encoding.ASCII.GetBytes("B");
            Assert.Equal(secondExpected, stringReaderStream.ReadLine());

            stringReaderStream.ReadLine();

            var thirdExpected = Encoding.ASCII.GetBytes("C");
            Assert.Equal(thirdExpected, stringReaderStream.ReadLine());

            Assert.Null(stringReaderStream.ReadLine());
        }

        [Theory]
        [InlineData("A\r\n\r\nB\r\rC", 1)]
        [InlineData("A\r\n\r\nB\r\rC", 2)]
        [InlineData("A\r\n\r\nB\r\rC", 3)]
        [InlineData("A\r\n\r\nB\r\n\r\nC", 1)]
        [InlineData("A\r\n\r\nB\r\n\r\nC", 2)]
        [InlineData("A\r\n\r\nB\r\n\r\nC", 3)]
        public void multiple_lines_double_linebreaks_stream_async(string text, int bufferSize)
        {
            var stringReaderStream = TestHelpers.CreateStringReaderStreamFromText(text, bufferSize);

            var firstExpected = Encoding.ASCII.GetBytes("A");
            Assert.Equal(firstExpected, stringReaderStream.ReadLineAsync().Result);

            stringReaderStream.ReadLine();

            var secondExpected = Encoding.ASCII.GetBytes("B");
            Assert.Equal(secondExpected, stringReaderStream.ReadLineAsync().Result);

            stringReaderStream.ReadLine();

            var thirdExpected = Encoding.ASCII.GetBytes("C");
            Assert.Equal(thirdExpected, stringReaderStream.ReadLineAsync().Result);

            Assert.Null(stringReaderStream.ReadLine());
        }


        [Theory]
        [InlineData("", "")]
        [InlineData("abc", "abc")]
        [InlineData("a\bb", "b")]
        [InlineData("\ba", "a")]
        [InlineData("a\b\bb", "b")]
        public void backslash_removes_previous_char(string textWithBackslashes, string textWithoutBackslashes)
        {
            var textBytesWithoutBacklashes = Encoding.ASCII.GetBytes(textWithoutBackslashes);
            var textBytesWithBackslashes = Encoding.ASCII.GetBytes(textWithBackslashes);

            Assert.Equal(textBytesWithoutBacklashes, StringReaderStream.ProcessBackslashes(textBytesWithBackslashes));
        }

        #region Helpers

        private static Stream CreateNonReadableStream()
        {
            var mockedStreamThatThrowsIOException = new Mock<Stream>();
            mockedStreamThatThrowsIOException.Setup(x => x.CanRead).Returns(false);
            return mockedStreamThatThrowsIOException.Object;
        }

        #endregion
    }
}
