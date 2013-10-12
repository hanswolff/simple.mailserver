using Simple.MailServer.Mime;
using Xunit;
using Xunit.Extensions;

namespace Simple.MailServer.Tests.Mime
{
    public class Rfc5335DecoderFacts
    {
        [Fact]
        public void decode_null()
        {
            Assert.Null(Rfc5335Decoder.Decode(null));
        }

        [Fact]
        public void decode_empty_string()
        {
            Assert.Empty(Rfc5335Decoder.Decode(""));
        }

        [Fact]
        public void decode_empty_quotedprintable_string()
        {
            Assert.Empty(Rfc5335Decoder.Decode("=?ISO-8859-1?Q??="));
        }

        [Fact]
        public void decode_empty_base64_string()
        {
            Assert.Empty(Rfc5335Decoder.Decode("=?ISO-8859-1?B??="));
        }

        [Fact]
        public void ignore_plain_text()
        {
            Assert.Equal("some plain text", Rfc5335Decoder.Decode("some plain text"));
        }

        [Fact]
        public void decode_single_utf8_quotedprintable()
        {
            Assert.Equal("RE: Frühling", Rfc5335Decoder.Decode("=?UTF-8?Q?RE=3A_Fr=C3=BChling?="));
        }

        [Fact]
        public void decode_single_utf8_base64()
        {
            Assert.Equal("test", Rfc5335Decoder.Decode("=?UTF-8?B?dGVzdA==?="));
        }

        [Theory]
        [InlineData("=?ISO-8859-1?Q?a?= =?ISO-8859-1?Q?b?=", "ab")]
        [InlineData("=?ISO-8859-1?Q?a?=  =?ISO-8859-1?Q?b?=", "ab")]
        [InlineData("=?ISO-8859-1?Q?a?=\r\n  =?ISO-8859-1?Q?b?=", "ab")]
        public void remove_whitespace_between_encoded_words(string encoded, string decoded)
        {
            Assert.Equal(decoded, Rfc5335Decoder.Decode(encoded));
        }

        [Theory]
        [InlineData("=?ISO-8859-1?Q?a?= b", "a b")]
        [InlineData("=?ISO-8859-1?Q?a?=  b", "a b")]
        public void keep_single_space_between_encoded_word_and_plain_word(string encoded, string decoded)
        {
            Assert.Equal(decoded, Rfc5335Decoder.Decode(encoded));
        }
    }
}
