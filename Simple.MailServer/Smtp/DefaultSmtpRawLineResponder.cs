namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpRawLineResponder : IRespondToSmtpRawLine
    {
        public static readonly DefaultSmtpRawLineResponder Instance = new DefaultSmtpRawLineResponder();

        public SmtpResponse RawLine(SmtpSessionInfo sessionInfo, string line)
        {
            return SmtpResponse.None;
        }
    }
}
