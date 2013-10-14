using Moq;
using Simple.MailServer.Smtp;
using Simple.MailServer.Smtp.Config;
using System.Linq;
using System.Text;
using Xunit;

namespace Simple.MailServer.Tests.Smtp
{
    public class SmtpSessionInfoResponderFacts
    {
        [Fact]
        public void invalid_command_returns_not_implemented_response()
        {
            var parseResponder = DefaultResponder();
            var response = SendCommand(parseResponder, "INVALID");

            Assert.Equal(SmtpResponse.NotImplemented, response);
        }

        [Fact]
        public void command_too_long_returns_error_response()
        {
            var parseResponder = DefaultResponder();
            var response = SendCommand(parseResponder, "HELO localhost" + new string(' ', 2100));

            Assert.Equal(SmtpResponse.LineTooLong, response);
        }

        [Fact]
        public void DATA_not_identified_returns_error_response()
        {
            var parseResponder = DefaultResponder();
            var response = SendCommand(parseResponder, "DATA");

            Assert.False(response.Success);
            Assert.False(parseResponder.InDataMode);
        }

        [Fact]
        public void DATA_identified_but_missing_mail_from_returns_error_response()
        {
            var parseResponder = IdentifiedParseResponder();
            var response = SendCommand(parseResponder, "DATA");

            Assert.False(response.Success);
            Assert.False(parseResponder.InDataMode);
        }

        [Fact]
        public void DATA_identified_and_mail_from_but_no_recipients_returns_error_response()
        {
            var parseResponder = MailFromIdentifiedParseResponder();
            var response = SendCommand(parseResponder, "DATA");

            Assert.False(response.Success);
            Assert.False(parseResponder.InDataMode);
        }

        [Fact]
        public void DATA_identified_mail_from_and_recipients_returns_success_response_and_is_in_DataMode()
        {
            var parseResponder = RecipientMailFromIdentifiedParseResponder();
            var response = SendCommand(parseResponder, "DATA");

            Assert.Equal(354, response.ResponseCode);
            Assert.True(parseResponder.InDataMode);
        }

        [Fact]
        public void DATA_identified_mail_from_and_recipients_should_use_IRespondToDataInterface()
        {
            var parseResponder = CreateParseResponderWithMockedIRespondToSmtpData(TestResponseSuccess);

            var response = SendCommand(parseResponder, "DATA");

            Assert.Equal(TestResponseSuccess, response);
            Assert.True(parseResponder.InDataMode);
        }

        [Fact]
        public void DATA_in_DataMode_after_dot_returns_out_of_DataMode()
        {
            var parseResponder = RecipientMailFromIdentifiedParseResponder();
            SendCommand(parseResponder, "DATA");

            var response = SendCommand(parseResponder, "some data");
            Assert.Equal(SmtpResponse.None, response);

            response = SendCommand(parseResponder, ".");
            Assert.Equal(SmtpResponse.OK, response);

            Assert.False(parseResponder.InDataMode);
        }

        [Fact]
        public void DATA_in_DataMode_after_dot_returns_out_of_DataMode_and_should_use_IRespondToDataInterface()
        {
            var parseResponder = CreateParseResponderWithMockedIRespondToSmtpData(TestResponseSuccess);

            SendCommand(parseResponder, "DATA");

            var response = SendCommand(parseResponder, "some data");
            Assert.Equal(TestResponseSuccess, response);

            response = SendCommand(parseResponder, ".");
            Assert.Equal(TestResponseSuccess, response);

            Assert.False(parseResponder.InDataMode);
        }

        [Fact]
        public void EHLO_without_argument_returns_error_response()
        {
            var parseResponder = DefaultResponder();
            var response = SendCommand(parseResponder, "EHLO");

            Assert.False(response.Success);
        }

        [Fact]
        public void EHLO_sets_SessionInfo_IdentificationMode_and_Identification()
        {
            var parseResponder = DefaultResponder();
            var response = SendCommand(parseResponder, "EHLO test");

            Assert.True(response.Success);
            Assert.Equal(SmtpIdentificationMode.EHLO, parseResponder.SessionInfo.Identification.Mode);
            Assert.Equal("test", parseResponder.SessionInfo.Identification.Arguments);
        }

        [Fact]
        public void EHLO_uses_IRespondToSmtpIdentification()
        {
            var parseResponder = CreateParseResponderWithMockedIRespondToSmtpIdentification(TestResponseSuccess);
            var response = SendCommand(parseResponder, "EHLO test");
            Assert.Equal(TestResponseSuccess, response);
            Assert.Equal(SmtpIdentificationMode.EHLO, parseResponder.SessionInfo.Identification.Mode);
            Assert.Equal("test", parseResponder.SessionInfo.Identification.Arguments);
        }

        [Fact]
        public void EHLO_drops_identification_on_error_response_using_IRespondToSmtpIdentification()
        {
            var parseResponder = CreateParseResponderWithMockedIRespondToSmtpIdentification(TestResponseError);
            var response = SendCommand(parseResponder, "EHLO test");
            Assert.Equal(TestResponseError, response);
            Assert.Equal(SmtpIdentificationMode.NotIdentified, parseResponder.SessionInfo.Identification.Mode);
            Assert.Null(parseResponder.SessionInfo.Identification.Arguments);
        }

        [Fact]
        public void EHLO_called_twice_overrides_old_SessionInfo_IdentificationMode_and_Identification()
        {
            var parseResponder = DefaultResponder();
            SendCommand(parseResponder, "EHLO test");
            var response = SendCommand(parseResponder, "EHLO new");

            Assert.True(response.Success);
            Assert.Equal(SmtpIdentificationMode.EHLO, parseResponder.SessionInfo.Identification.Mode);
            Assert.Equal("new", parseResponder.SessionInfo.Identification.Arguments);
        }

        [Fact]
        public void HELO_without_argument_returns_error_response()
        {
            var parseResponder = DefaultResponder();
            var response = SendCommand(parseResponder, "HELO");

            Assert.False(response.Success);
        }

        [Fact]
        public void HELO_sets_SessionInfo_IdentificationMode_and_Identification()
        {
            var parseResponder = DefaultResponder();
            var response = SendCommand(parseResponder, "HELO test");

            Assert.True(response.Success);
            Assert.Equal(SmtpIdentificationMode.HELO, parseResponder.SessionInfo.Identification.Mode);
            Assert.Equal("test", parseResponder.SessionInfo.Identification.Arguments);
        }

        [Fact]
        public void HELO_uses_IRespondToSmtpIdentification()
        {
            var parseResponder = CreateParseResponderWithMockedIRespondToSmtpIdentification(TestResponseSuccess);
            var response = SendCommand(parseResponder, "HELO test");
            Assert.Equal(TestResponseSuccess, response);
            Assert.Equal(SmtpIdentificationMode.HELO, parseResponder.SessionInfo.Identification.Mode);
            Assert.Equal("test", parseResponder.SessionInfo.Identification.Arguments);
        }

        [Fact]
        public void HELO_drops_identification_on_error_response_using_IRespondToSmtpIdentification()
        {
            var parseResponder = CreateParseResponderWithMockedIRespondToSmtpIdentification(TestResponseError);
            var response = SendCommand(parseResponder, "HELO test");
            Assert.Equal(TestResponseError, response);
            Assert.Equal(SmtpIdentificationMode.NotIdentified, parseResponder.SessionInfo.Identification.Mode);
            Assert.Null(parseResponder.SessionInfo.Identification.Arguments);
        }

        [Fact]
        public void HELO_called_twice_overrides_SessionInfo_IdentificationMode_and_Identification()
        {
            var parseResponder = DefaultResponder();
            SendCommand(parseResponder, "HELO test");
            var response = SendCommand(parseResponder, "HELO new");

            Assert.True(response.Success);
            Assert.Equal(SmtpIdentificationMode.HELO, parseResponder.SessionInfo.Identification.Mode);
            Assert.Equal("new", parseResponder.SessionInfo.Identification.Arguments);
        }

        [Fact]
        public void MAIL_FROM_not_identified_returns_error_response()
        {
            var parseResponder = DefaultResponder();
            var response = SendCommand(parseResponder, "MAIL FROM:<test@localhost>");

            Assert.False(response.Success);
        }

        [Fact]
        public void MAIL_FROM_identified_returns_success_response()
        {
            var parseResponder = IdentifiedParseResponder();
            var response = SendCommand(parseResponder, "MAIL FROM:<test@localhost>");

            Assert.True(response.Success);
            Assert.Equal("test@localhost", parseResponder.SessionInfo.MailFrom.MailAddress);
        }

        [Fact]
        public void MAIL_FROM_identified_called_twice_overrides_old_mail_from()
        {
            var parseResponder = IdentifiedParseResponder();
            SendCommand(parseResponder, "MAIL FROM:<test@localhost>");
            var response = SendCommand(parseResponder, "MAIL FROM:<new@localhost>");

            Assert.True(response.Success);
            Assert.Equal("new@localhost", parseResponder.SessionInfo.MailFrom.MailAddress);
        }

        [Fact]
        public void MAIL_FROM_identified_but_invalid_argument_returns_syntax_error_response()
        {
            var parseResponder = IdentifiedParseResponder();
            var response = SendCommand(parseResponder, "MAIL FROM:-");

            Assert.Equal(SmtpResponse.SyntaxError, response);
        }

        [Fact]
        public void MAIL_FROM_uses_IRespondToMailFrom()
        {
            var parseResponder = CreateParseResponderWithMockedIRespondToMailFrom(TestResponseSuccess);
            var response = SendCommand(parseResponder, "MAIL FROM:<new@localhost>");
            Assert.Equal(TestResponseSuccess, response);
            Assert.Equal("new@localhost", parseResponder.SessionInfo.MailFrom.MailAddress);
        }

        [Fact]
        public void MAIL_FROM_drops_data_on_error_response_using_IRespondToMailFrom()
        {
            var parseResponder = CreateParseResponderWithMockedIRespondToMailFrom(TestResponseError);
            var response = SendCommand(parseResponder, "MAIL FROM:<new@localhost>");
            Assert.Equal(TestResponseError, response);
            Assert.Null(parseResponder.SessionInfo.MailFrom);
        }

        [Fact]
        public void NOOP_success()
        {
            var parseResponder = DefaultResponder();
            var response = SendCommand(parseResponder, "NOOP");

            Assert.True(response.Success);
        }

        [Fact]
        public void QUIT_returns_special_disconnect_response()
        {
            var parseResponder = DefaultResponder();
            var response = SendCommand(parseResponder, "QUIT");

            Assert.Equal(SmtpResponse.Disconnect, response);
        }

        [Fact]
        public void RCPT_TO_not_identified_returns_error_response()
        {
            var parseResponder = DefaultResponder();
            var response = SendCommand(parseResponder, "RCPT TO:<test@localhost>");

            Assert.False(response.Success);
        }

        [Fact]
        public void RCPT_TO_identified_but_missing_MAIL_FROM_returns_error_response()
        {
            var parseResponder = IdentifiedParseResponder();
            var response = SendCommand(parseResponder, "RCPT TO:<test@localhost>");

            Assert.False(response.Success);
        }

        [Fact]
        public void RCPT_TO_identified_and_has_MAIL_FROM_returns_success_response()
        {
            var parseResponder = MailFromIdentifiedParseResponder();
            var response = SendCommand(parseResponder, "RCPT TO:<test@localhost>");

            Assert.True(response.Success);
            Assert.Equal("test@localhost", parseResponder.SessionInfo.Recipients.First().MailAddress);
        }

        [Fact]
        public void RCPT_TO_identified_and_has_MAIL_FROM_should_be_tolerant_about_spaces()
        {
            var parseResponder = MailFromIdentifiedParseResponder();
            var response = SendCommand(parseResponder, "RCPT TO:   <test@localhost>");

            Assert.True(response.Success);
            Assert.Equal("test@localhost", parseResponder.SessionInfo.Recipients.First().MailAddress);
        }

        [Fact]
        public void RCPT_TO_identified_and_has_MAIL_FROM_should_be_tolerant_about_mail_address_format()
        {
            var parseResponder = MailFromIdentifiedParseResponder();
            var response = SendCommand(parseResponder, "RCPT TO:test@localhost");

            Assert.True(response.Success);
            Assert.Equal("test@localhost", parseResponder.SessionInfo.Recipients.First().MailAddress);
        }

        [Fact]
        public void RCPT_TO_called_twice_identified_and_has_MAIL_FROM_collects_mail_addresses()
        {
            var parseResponder = MailFromIdentifiedParseResponder();
            SendCommand(parseResponder, "RCPT TO:<first@localhost>");
            var response = SendCommand(parseResponder, "RCPT TO:<second@localhost>");

            Assert.True(response.Success);
            Assert.Equal("first@localhost", parseResponder.SessionInfo.Recipients.First().MailAddress);
            Assert.Equal("second@localhost", parseResponder.SessionInfo.Recipients.Skip(1).First().MailAddress);
        }

        [Fact]
        public void RCPT_TO_identified_and_has_MAIL_FROM_but_invalid_recipient_returns_syntax_error_response()
        {
            var parseResponder = MailFromIdentifiedParseResponder();
            var response = SendCommand(parseResponder, "RCPT TO:-");

            Assert.Equal(SmtpResponse.SyntaxError, response);
        }

        [Fact]
        public void RCPT_TO_uses_IRespondToRecipientTo()
        {
            var parseResponder = CreateParseResponderWithMockedIRespondToRecipientTo(TestResponseSuccess);
            var response = SendCommand(parseResponder, "RCPT TO:<test@localhost>");
            Assert.Equal(TestResponseSuccess, response);
            Assert.Equal("test@localhost", parseResponder.SessionInfo.Recipients[0].MailAddress);
        }

        [Fact]
        public void RCPT_TO_drops_data_on_error_response_using_IRespondToRecipientTo()
        {
            var parseResponder = CreateParseResponderWithMockedIRespondToRecipientTo(TestResponseError);
            var response = SendCommand(parseResponder, "RCPT TO:<test@localhost>");
            Assert.Equal(TestResponseError, response);
            Assert.Empty(parseResponder.SessionInfo.Recipients);
        }

        [Fact]
        public void RSET_success()
        {
            var parseResponder = DefaultResponder();
            var response = SendCommand(parseResponder, "RSET");

            Assert.True(response.Success);
        }

        [Fact]
        public void RSET_must_not_reset_identification()
        {
            var parseResponder = IdentifiedParseResponder();
            SendCommand(parseResponder, "RSET");

            Assert.Equal(SmtpIdentificationMode.HELO, parseResponder.SessionInfo.Identification.Mode);
            Assert.Equal("localhost", parseResponder.SessionInfo.Identification.Arguments);
        }

        [Fact]
        public void RSET_resets_mail_from()
        {
            var parseResponder = MailFromIdentifiedParseResponder();
            SendCommand(parseResponder, "RSET");

            Assert.Null(parseResponder.SessionInfo.MailFrom);
        }

        [Fact]
        public void RSET_resets_recipients()
        {
            var parseResponder = RecipientMailFromIdentifiedParseResponder();
            SendCommand(parseResponder, "RSET");

            Assert.Empty(parseResponder.SessionInfo.Recipients);
        }

        [Fact]
        public void VRFY_not_identified_returns_error_response()
        {
            var parseResponder = DefaultResponder();
            var response = SendCommand(parseResponder, "VRFY");

            Assert.False(response.Success);
        }

        [Fact]
        public void VRFY_without_argument_returns_error_response()
        {
            var parseResponder = IdentifiedParseResponder();
            var response = SendCommand(parseResponder, "VRFY");

            Assert.False(response.Success);
        }

        [Fact]
        public void VRFY_with_argument_returns_success_response()
        {
            var parseResponder = IdentifiedParseResponder();
            var response = SendCommand(parseResponder, "VRFY test");

            Assert.True(response.Success);
        }

        [Fact]
        public void VRFY_uses_IRespondToVerify()
        {
            var parseResponder = CreateParseResponderWithMockedIRespondToVerify(TestResponseSuccess);
            var response = SendCommand(parseResponder, "VRFY test");
            Assert.Equal(TestResponseSuccess, response);
        }

        #region Helpers

        internal static SmtpResponse TestResponseSuccess = new SmtpResponse(200, "");
        internal static SmtpResponse TestResponseError = new SmtpResponse(500, "");

        private static SmtpSessionInfoResponder DefaultResponder(ISmtpResponderFactory factory = null)
        {
            return new SmtpSessionInfoResponder(factory ?? new DefaultSmtpResponderFactory<ISmtpServerConfiguration>(new SmtpServerConfiguration()), new SmtpSessionInfo());
        }

        private static SmtpSessionInfoResponder IdentifiedParseResponder(ISmtpResponderFactory factory = null)
        {
            var parseResponder = DefaultResponder(factory);
            parseResponder.SessionInfo.Identification.Mode = SmtpIdentificationMode.HELO;
            parseResponder.SessionInfo.Identification.Arguments = "localhost";
            return parseResponder;
        }

        private static SmtpSessionInfoResponder MailFromIdentifiedParseResponder(ISmtpResponderFactory factory = null)
        {
            var parseResponder = IdentifiedParseResponder(factory);
            parseResponder.SessionInfo.MailFrom = MailAddressWithParameters.Parse("test@localhost");
            return parseResponder;
        }

        private static SmtpSessionInfoResponder RecipientMailFromIdentifiedParseResponder(ISmtpResponderFactory factory = null)
        {
            var parseResponder = MailFromIdentifiedParseResponder(factory);
            parseResponder.SessionInfo.Recipients.Add(MailAddressWithParameters.Parse("test@localhost"));
            return parseResponder;
        }

        private static SmtpResponse SendCommand(SmtpSessionInfoResponder parseResponder, string rawCommand)
        {
            var command = Encoding.Default.GetBytes(rawCommand);
            var response = parseResponder.ProcessLineCommand(command);
            return response;
        }

        private static SmtpSessionInfoResponder CreateParseResponderWithMockedIRespondToSmtpData(SmtpResponse testResponse)
        {
            var respondToSmtpData = MockIRespondToSmtpDataToReturnResponse(testResponse);

            var factory = new DefaultSmtpResponderFactory<ISmtpServerConfiguration>(new SmtpServerConfiguration()) { DataResponder = respondToSmtpData };
            var parseResponder = RecipientMailFromIdentifiedParseResponder(factory);
            return parseResponder;
        }

        private static IRespondToSmtpData MockIRespondToSmtpDataToReturnResponse(SmtpResponse testResponse)
        {
            var mockedInterface = new Mock<IRespondToSmtpData>();
            mockedInterface
                .Setup(x => x.DataLine(It.IsAny<SmtpSessionInfo>(), It.IsAny<byte[]>()))
                .Returns<SmtpSessionInfo, byte[]>((sessionInfo, line) => testResponse);
            mockedInterface
                .Setup(x => x.DataStart(It.IsAny<SmtpSessionInfo>()))
                .Returns<SmtpSessionInfo>(sessionInfo => testResponse);
            mockedInterface
                .Setup(x => x.DataEnd(It.IsAny<SmtpSessionInfo>()))
                .Returns<SmtpSessionInfo>(sessionInfo => testResponse);
            return mockedInterface.Object;
        }

        private static SmtpSessionInfoResponder CreateParseResponderWithMockedIRespondToSmtpIdentification(SmtpResponse testResponse)
        {
            var respondToSmtpIdentification = MockIRespondToSmtpIdentificationToReturnResponse(testResponse);

            var factory = new DefaultSmtpResponderFactory<ISmtpServerConfiguration>(new SmtpServerConfiguration()) { IdentificationResponder = respondToSmtpIdentification };
            var parseResponder = DefaultResponder(factory);
            return parseResponder;
        }

        private static IRespondToSmtpIdentification MockIRespondToSmtpIdentificationToReturnResponse(SmtpResponse testResponse)
        {
            var mockedInterface = new Mock<IRespondToSmtpIdentification>();
            mockedInterface
                .Setup(x => x.VerifyIdentification(It.IsAny<SmtpSessionInfo>(), It.IsAny<SmtpIdentification>()))
                .Returns<SmtpSessionInfo, SmtpIdentification>((sessionInfo, identification) => testResponse);
            return mockedInterface.Object;
        }

        private static SmtpSessionInfoResponder CreateParseResponderWithMockedIRespondToMailFrom(SmtpResponse testResponse)
        {
            var respondToMailFrom = MockIRespondToMailFromToReturnResponse(testResponse);

            var factory = new DefaultSmtpResponderFactory<ISmtpServerConfiguration>(new SmtpServerConfiguration()) { MailFromResponder = respondToMailFrom };
            var parseResponder = IdentifiedParseResponder(factory);
            return parseResponder;
        }

        private static IRespondToSmtpMailFrom MockIRespondToMailFromToReturnResponse(SmtpResponse testResponse)
        {
            var mockedInterface = new Mock<IRespondToSmtpMailFrom>();
            mockedInterface
                .Setup(x => x.VerifyMailFrom(It.IsAny<SmtpSessionInfo>(), It.IsAny<MailAddressWithParameters>()))
                .Returns<SmtpSessionInfo, MailAddressWithParameters>((sessionInfo, mailAddressWithParameters) => testResponse);
            return mockedInterface.Object;
        }

        private static SmtpSessionInfoResponder CreateParseResponderWithMockedIRespondToRecipientTo(SmtpResponse testResponse)
        {
            var respondToRecipientTo = MockIRespondToRecipientToToReturnResponse(testResponse);

            var factory = new DefaultSmtpResponderFactory<ISmtpServerConfiguration>(new SmtpServerConfiguration()) { RecipientToResponder = respondToRecipientTo };
            var parseResponder = MailFromIdentifiedParseResponder(factory);
            return parseResponder;
        }

        private static IRespondToSmtpRecipientTo MockIRespondToRecipientToToReturnResponse(SmtpResponse testResponse)
        {
            var mockedInterface = new Mock<IRespondToSmtpRecipientTo>();
            mockedInterface
                .Setup(x => x.VerifyRecipientTo(It.IsAny<SmtpSessionInfo>(), It.IsAny<MailAddressWithParameters>()))
                .Returns<SmtpSessionInfo, MailAddressWithParameters>((sessionInfo, mailAddressWithParameters) => testResponse);
            return mockedInterface.Object;
        }

        private static SmtpSessionInfoResponder CreateParseResponderWithMockedIRespondToVerify(SmtpResponse testResponse)
        {
            var respondToVerify = MockIRespondToVerifyToReturnResponse(testResponse);

            var factory = new DefaultSmtpResponderFactory<ISmtpServerConfiguration>(new SmtpServerConfiguration()) { VerifyResponder = respondToVerify };
            var parseResponder = MailFromIdentifiedParseResponder(factory);
            return parseResponder;
        }

        private static IRespondToSmtpVerify MockIRespondToVerifyToReturnResponse(SmtpResponse testResponse)
        {
            var mockedInterface = new Mock<IRespondToSmtpVerify>();
            mockedInterface
                .Setup(x => x.Verify(It.IsAny<SmtpSessionInfo>(), It.IsAny<string>()))
                .Returns<SmtpSessionInfo, string>((sessionInfo, args) => testResponse);
            return mockedInterface.Object;
        }

        #endregion
    }
}
