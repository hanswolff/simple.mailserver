using System;

namespace Simple.MailServer.Smtp
{
    internal class SmtpSessionInfoResponder : SmtpCommandParser
    {
        public SmtpSessionInfo SessionInfo { get; private set; }
        
        private readonly SmtpResponderFactory _responderFactory;

        public SmtpSessionInfoResponder(SmtpResponderFactory responderFactory, SmtpSessionInfo sessionInfo)
        {
            if (sessionInfo == null) throw new ArgumentNullException("sessionInfo");

            SessionInfo = sessionInfo;
            _responderFactory = responderFactory ?? SmtpResponderFactory.Default;
        }

        protected override SmtpResponse ProcessCommandDataStart(string name, string arguments)
        {
            var notIdentified = CreateResponseIfNotIdentified();
            if (notIdentified != SmtpResponse.None) return notIdentified;

            var hasNotMailFrom = CreateResponseIfHasNotMailFrom();
            if (hasNotMailFrom != SmtpResponse.None) return hasNotMailFrom;

            var hasNoRecipients = CreateResponseIfHasNoRecipients();
            if (hasNoRecipients != SmtpResponse.None) return hasNotMailFrom;

            var response = _responderFactory.DataResponder.DataStart(SessionInfo);

            if (response.Success)
            {
                InDataMode = true;
                SessionInfo.HasData = true;
            }

            return response;
        }

        protected override SmtpResponse ProcessCommandEhlo(string name, string arguments)
        {
            if (String.IsNullOrWhiteSpace(arguments))
            {
                return new SmtpResponse(501, "EHLO Missing domain address.");
            }

            var identification = new SmtpIdentification(SmtpIdentificationMode.EHLO, arguments);
            var response = _responderFactory.IdentificationResponder.VerifyIdentification(SessionInfo, identification);

            if (response.Success)
            {
                SessionInfo.Identification = identification;
            }

            return response;
        }

        protected override SmtpResponse ProcessCommandHelo(string name, string arguments)
        {
            if (String.IsNullOrWhiteSpace(arguments))
            {
                return new SmtpResponse(501, "HELO Missing domain address.");
            }

            var identification = new SmtpIdentification(SmtpIdentificationMode.HELO, arguments);
            var response = _responderFactory.IdentificationResponder.VerifyIdentification(SessionInfo, identification);

            if (response.Success)
            {
                SessionInfo.Identification = identification;
            }

            return response;
        }

        protected override SmtpResponse ProcessCommandMailFrom(string name, string arguments)
        {
            var notIdentified = CreateResponseIfNotIdentified();
            if (notIdentified != SmtpResponse.None) return notIdentified;

            var mailFrom = arguments.Trim();
            MailAddressWithParameters mailAddressWithParameters;
            try { mailAddressWithParameters = MailAddressWithParameters.Parse(mailFrom); }
            catch (FormatException)
            {
                return new SmtpResponse(501, "Syntax error in parameters or arguments");
            }

            var response = _responderFactory.MailFromResponder.VerifyMailFrom(SessionInfo, mailAddressWithParameters);
            if (response.Success)
            {
                SessionInfo.MailFrom = mailAddressWithParameters;
            }

            return response;
        }

        protected override SmtpResponse ProcessCommandNoop(string name, string arguments)
        {
            return SmtpResponse.OK;
        }

        protected override SmtpResponse ProcessCommandQuit(string name, string arguments)
        {
            return SmtpResponse.Disconnect;
        }

        protected override SmtpResponse ProcessCommandRcptTo(string name, string arguments)
        {
            var notIdentified = CreateResponseIfNotIdentified();
            if (notIdentified != SmtpResponse.None) return notIdentified;

            var hasNotMailFrom = CreateResponseIfHasNotMailFrom();
            if (hasNotMailFrom != SmtpResponse.None) return hasNotMailFrom;

            var recipient = arguments.Trim();
            MailAddressWithParameters mailAddressWithParameters;
            try { mailAddressWithParameters = MailAddressWithParameters.Parse(recipient); }
            catch (FormatException)
            {
                return new SmtpResponse(501, "Syntax error in parameters or arguments");
            }

            var response = _responderFactory.RecipientToResponder.VerifyRecipientTo(SessionInfo, mailAddressWithParameters);
            if (response.Success)
            {
                SessionInfo.Recipients.Add(mailAddressWithParameters);
            }

            return response;
        }

        protected override SmtpResponse ProcessCommandRset(string name, string arguments)
        {
            var response = _responderFactory.ResetResponder.Reset(SessionInfo);
            if (response.Success)
            {
                SessionInfo.Reset();
                InDataMode = false;
            }
            return response;
        }

        protected override SmtpResponse ProcessCommandVrfy(string name, string arguments)
        {
            var notIdentified = CreateResponseIfNotIdentified();
            if (notIdentified != SmtpResponse.None) return notIdentified;

            if (String.IsNullOrWhiteSpace(arguments))
            {
                return new SmtpResponse(501, "VRFY Missing parameter.");
            }

            var response = _responderFactory.VerifyResponder.Verify(SessionInfo, arguments);
            return response;
        }

        private SmtpResponse CreateResponseIfNotIdentified()
        {
            if (SessionInfo.Identification.Mode == SmtpIdentificationMode.NotIdentified)
            {
                return new SmtpResponse(502, "5.5.1 Use HELO/EHLO first.");
            }
            return SmtpResponse.None;
        }

        private SmtpResponse CreateResponseIfHasNotMailFrom()
        {
            if (SessionInfo.MailFrom == null)
            {
                return new SmtpResponse(502, "5.5.1 Use MAIL FROM first.");
            }
            return SmtpResponse.None;
        }

        private SmtpResponse CreateResponseIfHasNoRecipients()
        {
            if (SessionInfo.Recipients.Count <= 0)
            {
                return new SmtpResponse(503, "5.5.1 Must have recipient first");
            }
            return SmtpResponse.None;
        }

        protected override SmtpResponse ProcessRawLine(string line)
        {
            var response = _responderFactory.RawLineResponder.RawLine(SessionInfo, line);
            return response;
        }

        protected override SmtpResponse ProcessCommandDataEnd()
        {
            var response = _responderFactory.DataResponder.DataEnd(SessionInfo);
            if (response.Success)
            {
                InDataMode = false;
            }
            return response;
        }

        protected override SmtpResponse ProcessDataLine(byte[] line)
        {
            var response = _responderFactory.DataResponder.DataLine(SessionInfo, line);
            return response;
        }
    }
}
