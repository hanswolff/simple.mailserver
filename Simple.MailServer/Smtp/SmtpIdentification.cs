namespace Simple.MailServer.Smtp
{
    public class SmtpIdentification
    {
        public SmtpIdentificationMode Mode { get; set; }
        public string Arguments { get; set; }

        public SmtpIdentification()
        {
        }

        public SmtpIdentification(SmtpIdentificationMode mode, string argument)
        {
            Mode = mode;
            Arguments = argument;
        }
    }
}
