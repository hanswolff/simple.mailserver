#region Header
// Copyright (c) 2013-2015 Hans Wolff
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
using Xunit;

namespace Simple.MailServer.Tests
{
    public class XamarinEmailValidatorFacts
    {
        [Theory]
        [InlineData("\"Abc\\@def\"@example.com")]
        [InlineData("\"Fred Bloggs\"@example.com")]
        [InlineData("\"Joe\\\\Blow\"@example.com")]
        [InlineData("\"Abc@def\"@example.com")]
        [InlineData("customer/department=shipping@example.com")]
        [InlineData("$A12345@example.com")]
        [InlineData("!def!xyz%abc@example.com")]
        [InlineData("_somename@example.com")]
        [InlineData("valid.ipv4.addr@[123.1.72.10]")]
        [InlineData("valid.ipv6.addr@[IPv6:0::1]")]
        [InlineData("valid.ipv6.addr@[IPv6:2607:f0d0:1002:51::4]")]
        [InlineData("valid.ipv6.addr@[IPv6:fe80::230:48ff:fe33:bc33]")]
        [InlineData("valid.ipv6v4.addr@[IPv6:aaaa:aaaa:aaaa:aaaa:aaaa:aaaa:127.0.0.1]")]
        // examples from wikipedia
        [InlineData("niceandsimple@example.com")]
        [InlineData("very.common@example.com")]
        [InlineData("a.little.lengthy.but.fine@dept.example.com")]
        [InlineData("disposable.style.email.with+symbol@example.com")]
        [InlineData("user@[IPv6:2001:db8:1ff::a0b:dbd0]")]
        [InlineData("\"much.more unusual\"@example.com")]
        [InlineData("\"very.unusual.@.unusual.com\"@example.com")]
        [InlineData("\"very.(),:;<>[]\\\".VERY.\\\"very@\\\\ \\\"very\\\".unusual\"@strange.example.com")]
        [InlineData("postbox@com")]
        [InlineData("admin@mailserver1")]
        [InlineData("!#$%&'*+-/=?^_`{}|~@example.org")]
        [InlineData("\"()<>[]:,;@\\\\\\\"!#$%&'*+-/=?^_`{}| ~.a\"@example.org")]
        [InlineData("\" \"@example.org")]
        public void TestValidAddresses(string validEmail)
        {
            var validator = new XamarinEmailValidator();
            Assert.True(validator.Validate(validEmail));
        }

        [Theory]
        [InlineData("")]
        [InlineData("invalid")]
        [InlineData("invalid@")]
        [InlineData("invalid @")]
        [InlineData("invalid@[555.666.777.888]")]
        [InlineData("invalid@[IPv6:123456]")]
        [InlineData("invalid@[127.0.0.1.]")]
        [InlineData("invalid@[127.0.0.1].")]
        [InlineData("invalid@[127.0.0.1]x")]
        // examples from wikipedia
        [InlineData("Abc.example.com")]
        [InlineData("A@b@c@example.com")]
        [InlineData("a\"b(c)d,e:f;g<h>i[j\\k]l@example.com")]
        [InlineData("just\"not\"right@example.com")]
        [InlineData("this is\"not\\allowed@example.com")]
        [InlineData("this\\ still\\\"not\\\\allowed@example.com")]
        public void TestInvalidAddresses(string invalidEmail)
        {
            var validator = new XamarinEmailValidator();
            Assert.False(validator.Validate(invalidEmail));
        }
    }
}