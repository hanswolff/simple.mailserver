using Simple.MailServer.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.MailServer.Smtp
{
    public abstract class SmtpCommandParser
    {
        private readonly Dictionary<string, Func<string, string, SmtpResponse>> _commandMapping;

        public bool InDataMode { get; set; }

        protected SmtpCommandParser()
        {
            _commandMapping = CreateCommandMapping();
        }

        private Dictionary<string, Func<string, string, SmtpResponse>> CreateCommandMapping()
        {
            var mapping = new Dictionary<string, Func<string, string, SmtpResponse>>
            {
                { "DATA", ProcessCommandDataStart },
                { "EHLO", ProcessCommandEhlo },
                { "HELO", ProcessCommandHelo },
                { "MAIL FROM:", ProcessCommandMailFrom },
                { "NOOP", ProcessCommandNoop },
                { "QUIT", ProcessCommandQuit },
                { "RCPT TO:", ProcessCommandRcptTo },
                { "RSET", ProcessCommandRset },
                { "VRFY", ProcessCommandVrfy },
            };

            return mapping;
        }

        protected abstract SmtpResponse ProcessCommandDataStart(string name, string value);
        protected abstract SmtpResponse ProcessCommandDataEnd();
        protected abstract SmtpResponse ProcessCommandEhlo(string name, string value);
        protected abstract SmtpResponse ProcessCommandHelo(string name, string value);
        protected abstract SmtpResponse ProcessCommandMailFrom(string name, string value);
        protected abstract SmtpResponse ProcessCommandNoop(string name, string value);
        protected abstract SmtpResponse ProcessCommandQuit(string name, string value);
        protected abstract SmtpResponse ProcessCommandRcptTo(string name, string value);
        protected abstract SmtpResponse ProcessCommandRset(string name, string value);
        protected abstract SmtpResponse ProcessCommandVrfy(string name, string value);

        protected abstract SmtpResponse ProcessDataLine(byte[] lineBuf);
        protected abstract SmtpResponse ProcessRawLine(string line);

        public SmtpResponse ProcessLineCommand(byte[] lineBuf)
        {
            try
            {
                return ProcessLineCommandDontCareAboutException(lineBuf);
            }
            catch (Exception ex)
            {
                MailServerLogger.Instance.Error(ex);
                return SmtpResponse.InternalServerError;
            }
        }

        private SmtpResponse ProcessLineCommandDontCareAboutException(byte[] lineBuf)
        {
            SmtpResponse smtpResponse;

            if (IsLineTooLong(lineBuf, out smtpResponse))
                return smtpResponse;

            if (InDataMode)
                return ProcessLineInDataMode(lineBuf);

            var line = Encoding.UTF8.GetString(lineBuf);

            if (ProcessRawLineHasResponse(line, out smtpResponse))
                return smtpResponse;

            Func<string, string, SmtpResponse> commandFunc;

            var commandWithArguments = GetCommandWithArgumentsAndCommandFunction(line, out commandFunc);

            if (commandFunc != null)
            {
                var response = commandFunc(commandWithArguments.Command, commandWithArguments.Arguments);
                return response;
            }

            return SmtpResponse.NotImplemented;
        }

        private static bool IsLineTooLong(byte[] lineBuf, out SmtpResponse smtpResponse)
        {
            if (lineBuf.Length > 2040)
            {
                smtpResponse = SmtpResponse.LineTooLong;
                return true;
            }

            smtpResponse = SmtpResponse.None;
            return false;
        }

        private bool ProcessRawLineHasResponse(string line, out SmtpResponse smtpResponse)
        {
            smtpResponse = ProcessRawLine(line);
            return (smtpResponse != SmtpResponse.None);
        }

        private SmtpResponse ProcessLineInDataMode(byte[] lineBuf)
        {
            if (lineBuf.Length == 1 && lineBuf[0] == '.')
            {
                return ProcessCommandDataEnd();
            }
            return ProcessDataLine(lineBuf);
        }

        private CommandWithArguments GetCommandWithArgumentsAndCommandFunction(string line, out Func<string, string, SmtpResponse> commandFunc)
        {
            var commandWithArguments = SplitCommandWithArgumentsAtCharacter(line, ':');

            if (commandWithArguments == CommandWithArguments.Empty ||
                !_commandMapping.TryGetValue(commandWithArguments.Command, out commandFunc))
            {
                commandWithArguments = SplitCommandWithArgumentsAtCharacter(line, ' ');
            }

            if (commandWithArguments == CommandWithArguments.Empty ||
                !_commandMapping.TryGetValue(commandWithArguments.Command, out commandFunc))
            {
                commandWithArguments = new CommandWithArguments { Command = line.ToUpperInvariant().Trim() };
                _commandMapping.TryGetValue(commandWithArguments.Command, out commandFunc);
            }
            return commandWithArguments;
        }

        private CommandWithArguments SplitCommandWithArgumentsAtCharacter(string line, char splitChar)
        {
            var pos = line.IndexOf(splitChar);
            if (pos >= 0)
            {
                var command = line.Substring(0, pos + 1).ToUpperInvariant().Trim();
                var arguments = line.Substring(pos + 1);

                return new CommandWithArguments { Command = command, Arguments = arguments };
            }

            return CommandWithArguments.Empty;
        }

        private class CommandWithArguments
        {
            public static readonly CommandWithArguments Empty = new CommandWithArguments();

            public string Command { get; set; }
            public string Arguments { get; set; }
        }
    }
}
