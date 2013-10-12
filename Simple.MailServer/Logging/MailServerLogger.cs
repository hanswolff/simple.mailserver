namespace Simple.MailServer.Logging
{
    public static class MailServerLogger
    {
        private static IMailServerLogger _instance = new MailServerDebugLogger();
        public static IMailServerLogger Instance
        {
            get { return _instance; }
            set { _instance = value ?? new MailServerDebugLogger(); }
        }
    }
}
