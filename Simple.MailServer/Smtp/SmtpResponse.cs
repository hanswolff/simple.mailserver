using System.Collections.Generic;

namespace Simple.MailServer.Smtp
{
    public class SmtpResponse
    {
        public static SmtpResponse None = new SmtpResponse(0, "");
        public static SmtpResponse OK = new SmtpResponse(250, "OK");
        public static SmtpResponse DataStart = new SmtpResponse(354, "OK");
        public static SmtpResponse Disconnect = new SmtpResponse(221, "Bye");

        public List<string> Additional { get; private set; }

        public bool Success { get { return ReasonCode >= 200 && ReasonCode < 400; } }

        public int ReasonCode { get; set; }
        public string ReasonText { get; set; }

        public SmtpResponse(int reasonCode, string reasonText)
        {
            Additional = new List<string>();
            ReasonCode = reasonCode;
            ReasonText = reasonText;
        }
    }
}
