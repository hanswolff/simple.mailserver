using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mail;

namespace Simple.MailServer
{
    public class MailAddressWithParameters
    {
        public string MailAddress { get; private set; }
        public string User { get; private set; }
        public string Host { get; private set; }
        
        public string InternalUser { get; set; }
        public string InternalAddress { get { return String.IsNullOrEmpty(InternalUser) ? MailAddress : InternalUser + "@" + Host; } }

        public string RawLine { get; private set; }

        public NameValueCollection Parameters { get; private set; }

        private MailAddressWithParameters()
        {
            Parameters = new NameValueCollection();
        }

        public static MailAddressWithParameters Parse(string line)
        {
            if (line == null) throw new ArgumentNullException("line");
            line = line.Trim();

            var res = new MailAddressWithParameters { RawLine = line };
            res.ParseLine(line);
            return res;
        }

        private void ParseLine(string line)
        {
            if (line == null) throw new ArgumentNullException("line");

            var pos = line.IndexOf(' ');
            if (pos < 0)
            {
                AssignMailAddress(new MailAddress(line));
                return;
            }

            AssignMailAddress(new MailAddress(line.Substring(0, pos)));

            var parts = line.Substring(pos + 1).Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            SplitNameValuesInParts(parts, Parameters);
        }

        private void AssignMailAddress(MailAddress mailAddress)
        {
            MailAddress = mailAddress.ToString();
            User = mailAddress.User;
            Host = mailAddress.Host;
        }

        private void SplitNameValuesInParts(IEnumerable<string> parts, NameValueCollection parameters)
        {
            foreach (var part in parts)
            {
                var nameValue = part.Split('=');
                if (nameValue.Length == 1)
                {
                    parameters[nameValue[0]] = "";
                    continue;
                }
                parameters[nameValue[0]] = String.Join("=", nameValue.Skip(1));
            }
        }

        public override string ToString()
        {
            return RawLine;
        }
    }
}
