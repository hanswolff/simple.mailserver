#region Header
// SmtpResponseFacts.cs
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
using Simple.MailServer.Smtp;
using Xunit;

namespace Simple.MailServer.Tests.Smtp
{
    public class SmtpResponseFacts
    {
        [Fact]
        public void AdditionalLines_should_never_be_null()
        {
            var response = new SmtpResponse(250, "");
            Assert.NotNull(response.AdditionalLines);
        }

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
            var response = new SmtpResponse(250, "", new[] { "additional" });

            var cloned = response.Clone();
            Assert.NotSame(response.AdditionalLines, cloned.AdditionalLines);
            Assert.Equal(response.AdditionalLines, cloned.AdditionalLines);
        }

        [Fact]
        public void CloneAndChange_with_ResponseText_should_change_response_text()
        {
            var response = new SmtpResponse(250, "old text", new[] { "additional" });
            var cloned = response.CloneAndChange("new text");

            Assert.Equal("new text", cloned.ResponseText);
            
            var clonedBack = cloned.CloneAndChange("old text");
            Assert.Equal(response, clonedBack);
        }

        [Fact]
        public void Equals_is_false_ResponseCode_doesnt_match()
        {
            var response1 = new SmtpResponse(250, "");
            var response2 = new SmtpResponse(251, "");

            Assert.False(response1.Equals(response2));
            Assert.False(response1.Equals((object)response2));
            Assert.NotEqual(response1.GetHashCode(), response2.GetHashCode());
        }

        [Fact]
        public void Equals_is_false_ResponseText_doesnt_match()
        {
            var response1 = new SmtpResponse(250, "text");
            var response2 = new SmtpResponse(250, "different text");

            Assert.False(response1.Equals(response2));
            Assert.False(response1.Equals((object)response2));
            Assert.NotEqual(response1.GetHashCode(), response2.GetHashCode());
        }

        [Fact]
        public void Equals_is_false_Additional_missing_in_other_response()
        {
            var response1 = new SmtpResponse(250, "", new[] { "line" });
            var response2 = new SmtpResponse(250, "");

            Assert.False(response1.Equals(response2));
            Assert.False(response1.Equals((object)response2));
        }

        [Fact]
        public void Equals_is_false_Additional_different_in_other_response()
        {
            var response1 = new SmtpResponse(250, "", new[] { "line" });
            var response2 = new SmtpResponse(250, "", new[] { "different line" });

            Assert.False(response1.Equals(response2));
            Assert.False(response1.Equals((object)response2));
        }

        [Fact]
        public void Equals_is_true_when_everything_matches()
        {
            var response1 = new SmtpResponse(250, "same", new[] { "line" });
            var response2 = new SmtpResponse(250, "same", new[] { "line" });
            
            Assert.True(response1.Equals(response2));
            Assert.True(response1.Equals((object)response2));
            Assert.Equal(response1.GetHashCode(), response2.GetHashCode());
        }
    }
}
