using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.MailServer.Smtp
{
    public class SmtpResponse : ICloneable
    {
        public static readonly SmtpResponse None = new SmtpResponse(0, "");
        public static readonly SmtpResponse OK = new SmtpResponse(250, "OK");
        public static readonly SmtpResponse DataStart = new SmtpResponse(354, "OK");
        public static readonly SmtpResponse Disconnect = new SmtpResponse(221, "Bye");

        public static readonly SmtpResponse InternalServerError = new SmtpResponse(500, "Internal Server Error");
        public static readonly SmtpResponse LineTooLong = new SmtpResponse(500, "Line Too Long");
        public static readonly SmtpResponse NotImplemented = new SmtpResponse(502, "5.5.2 Command not implemented");
        public static readonly SmtpResponse NotIdentified = new SmtpResponse(502, "5.5.1 Use HELO/EHLO first.");
        public static readonly SmtpResponse SyntaxError = new SmtpResponse(501, "Syntax error in parameters or arguments");

        public List<string> AdditionalLines { get; private set; }

        public bool Success { get { return ResponseCode >= 200 && ResponseCode < 400; } }

        public int ResponseCode { get; set; }
        public string ResponseText { get; set; }

        public SmtpResponse(int responseCode, string responseText)
        {
            AdditionalLines = new List<string>();
            ResponseCode = responseCode;
            ResponseText = responseText;
        }

        public SmtpResponse Clone()
        {
            return new SmtpResponse(ResponseCode, ResponseText)
            {
                AdditionalLines = AdditionalLines.ToList()
            };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public override string ToString()
        {
            return ResponseCode + " " + ResponseText;
        }
    }
}
