using System;
using System.Collections.Generic;

namespace Simple.MailServer.Smtp
{
    public class SmtpSessionInfo
    {
        public bool HasData { get; set; }
        public SmtpIdentification Identification { get; set; }
        public MailAddressWithParameters MailFrom { get; set; }
        public List<MailAddressWithParameters> Recipients { get; private set; }
        public object Tag { get; set; }

        public DateTime CreatedTimestamp { get; private set; }

        public SmtpSessionInfo()
        {
            CreatedTimestamp = DateTime.UtcNow;

            Identification = new SmtpIdentification();
            Recipients = new List<MailAddressWithParameters>();
        }

        public void Reset()
        {
            HasData = false;
            MailFrom = null;
            Recipients.Clear();
        }
    }
}
