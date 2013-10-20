#region Header
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
