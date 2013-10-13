using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.MailServer.Smtp
{
    public class SmtpResponse : ICloneable
    {
        public static SmtpResponse None = new SmtpResponse(0, "");
        public static SmtpResponse OK = new SmtpResponse(250, "OK");
        public static SmtpResponse DataStart = new SmtpResponse(354, "OK");
        public static SmtpResponse Disconnect = new SmtpResponse(221, "Bye");

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
    }
}
