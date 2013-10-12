using System;
using Xunit;
using Xunit.Extensions;

namespace Simple.MailServer.Tests
{
    public class MailAddressWithParametersFacts
    {
        [Fact]
        public void parse_null_throws_exception()
        {
            Assert.Throws<ArgumentNullException>(
                () => MailAddressWithParameters.Parse(null));
        }

        [Fact]
        public void parse_blank_throws_exception()
        {
            Assert.Throws<ArgumentException>(
                () => MailAddressWithParameters.Parse(""));
        }

        [Theory]
        [InlineData("<test@test.com>")]
        [InlineData("test@test.com")]
        [InlineData(" test@test.com")]
        [InlineData("test@test.com ")]
        public void parse_valid_mail_addresses_with_same_host(string rawMailAddress)
        {
            Assert.Equal("test.com", MailAddressWithParameters.Parse(rawMailAddress).Host);
        }

        [Theory]
        [InlineData("<test@test.com> NAME=VALUE")]
        [InlineData("test@test.com NAME=VALUE")]
        [InlineData(" test@test.com NAME=VALUE")]
        [InlineData("test@test.com  NAME=VALUE")]
        public void mail_address_parsed_with_whitespaces_and_parameter_values(string rawMailAddress)
        {
            Assert.Equal("test@test.com", MailAddressWithParameters.Parse(rawMailAddress).MailAddress);
        }

        [Theory]
        [InlineData("<test@test.com> NAME=VALUE")]
        [InlineData("test@test.com NAME=VALUE")]
        [InlineData(" test@test.com NAME=VALUE")]
        [InlineData("test@test.com  NAME=VALUE")]
        public void parameter_values_parsed_with_mail_address_and_whitespaces(string rawMailAddress)
        {
            var mailAddressWithParameters = MailAddressWithParameters.Parse(rawMailAddress);
            Assert.Equal("VALUE", mailAddressWithParameters.Parameters["name"]);
        }

        [Theory]
        [InlineData("<test@test.com> PARAM1 PARAM2")]
        [InlineData("test@test.com PARAM1 PARAM2")]
        [InlineData(" test@test.com PARAM1 PARAM2")]
        [InlineData("test@test.com  PARAM1 PARAM2")]
        public void multiple_nonvalue_parameters_with_mail_address_and_whitespaces(string rawMailAddress)
        {
            var mailAddressWithParameters = MailAddressWithParameters.Parse(rawMailAddress);
            Assert.Equal("", mailAddressWithParameters.Parameters["PARAM1"]);
            Assert.Equal("", mailAddressWithParameters.Parameters["PARAM2"]);
        }

        [Theory]
        [InlineData("<test@test.com> NAME1=VALUE1 NAME2=VALUE2")]
        [InlineData("test@test.com NAME1=VALUE1 NAME2=VALUE2")]
        [InlineData(" test@test.com NAME1=VALUE1 NAME2=VALUE2")]
        [InlineData("test@test.com  NAME1=VALUE1 NAME2=VALUE2")]
        public void multiple_value_parameters_parsed_with_mail_address_and_whitespaces(string rawMailAddress)
        {
            var mailAddressWithParameters = MailAddressWithParameters.Parse(rawMailAddress);
            Assert.Equal("VALUE1", mailAddressWithParameters.Parameters["NAME1"]);
            Assert.Equal("VALUE2", mailAddressWithParameters.Parameters["NAME2"]);
        }
    }
}
