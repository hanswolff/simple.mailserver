namespace Simple.MailServer.Smtp
{
    public class DefaultSmtpDataResponder : IRespondToSmtpData
    {
        public static readonly DefaultSmtpDataResponder Instance = new DefaultSmtpDataResponder();

        public SmtpResponse DataStart(SmtpSessionInfo sessionInfo)
        {
            return SmtpResponse.DataStart;
        }

        public SmtpResponse DataLine(SmtpSessionInfo sessionInfo, byte[] lineBuf)
        {
            return SmtpResponse.None;
        }

        public SmtpResponse DataEnd(SmtpSessionInfo sessionInfo)
        {
            return SmtpResponse.OK;
        }
    }
}
