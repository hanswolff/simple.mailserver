namespace Simple.MailServer.Logging
{
    public static class MailServerLogger
    {
        private static IMailServerLogger _instance = new MailServerDebugLogger(MailServerLogLevel.Debug);
        public static IMailServerLogger Instance
        {
            get { return _instance; }
        }

        public static void Set(IMailServerLogger loggerInstance)
        {
            _instance = loggerInstance ?? new MailServerNullLogger();
        }
    }
}
