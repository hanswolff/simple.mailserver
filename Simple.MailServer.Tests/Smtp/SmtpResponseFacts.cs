using Simple.MailServer.Smtp;
using Xunit;

namespace Simple.MailServer.Tests.Smtp
{
    public class SmtpResponseFacts
    {
        [Fact]
        public void Clone_should_copy_ReasonCode()
        {
            var response = new SmtpResponse(250, "");
            var cloned = response.Clone();
            Assert.Equal(response.ResponseCode, cloned.ResponseCode);
        }

        [Fact]
        public void Clone_should_copy_ReasonText()
        {
            var response = new SmtpResponse(250, "Text");
            var cloned = response.Clone();
            Assert.Equal(response.ResponseText, cloned.ResponseText);
        }

        [Fact]
        public void Clone_should_deep_copy_Additional()
        {
            var response = new SmtpResponse(250, "");
            response.AdditionalLines.Add("additional");

            var cloned = response.Clone();
            Assert.NotSame(response.AdditionalLines, cloned.AdditionalLines);
            Assert.Equal(response.AdditionalLines, cloned.AdditionalLines);
        }

        [Fact]
        public void Equals_is_false_ResponseCode_doesnt_match()
        {
            var response1 = new SmtpResponse(250, "");
            var response2 = new SmtpResponse(251, "");
            Assert.False(response1.Equals(response2));
        }

        [Fact]
        public void Equals_is_false_ResponseText_doesnt_match()
        {
            var response1 = new SmtpResponse(250, "text");
            var response2 = new SmtpResponse(250, "different text");
            Assert.False(response1.Equals(response2));
        }

        [Fact]
        public void Equals_is_false_Additional_missing_in_other_response()
        {
            var response1 = new SmtpResponse(250, "");
            response1.AdditionalLines.Add("line");

            var response2 = new SmtpResponse(250, "");
            Assert.False(response1.Equals(response2));
        }

        [Fact]
        public void Equals_is_false_Additional_different_in_other_response()
        {
            var response1 = new SmtpResponse(250, "");
            response1.AdditionalLines.Add("line");

            var response2 = new SmtpResponse(250, "");
            response2.AdditionalLines.Add("different line");

            Assert.False(response1.Equals(response2));
        }

        [Fact]
        public void Equals_is_true_when_everything_matches()
        {
            var response1 = new SmtpResponse(250, "same");
            response1.AdditionalLines.Add("line");

            var response2 = new SmtpResponse(250, "same");
            response2.AdditionalLines.Add("line");
            
            Assert.True(response1.Equals(response2));
        }
    }
}
