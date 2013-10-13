using Simple.MailServer.Mime;
using System;
using System.IO;
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
