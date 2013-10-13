using Simple.MailServer.Logging;
using Xunit;

namespace Simple.MailServer.Tests.Logging
{
    public class MailServerLoggerFacts
    {
        [Fact]
        public void use_MailServerNullLogger_when_setting_instance_to_null()
        {
            MailServerLogger.Set(null);
            Assert.IsType<MailServerNullLogger>(MailServerLogger.Instance);
        }

        [Fact]
        public void calling_Set_is_setting_instance()
        {
            MailServerLogger.Set(null); // reset, just in case

            MailServerLogger.Set(new MailServerDebugLogger(MailServerLogLevel.Debug));
            Assert.IsType<MailServerDebugLogger>(MailServerLogger.Instance);
        }
    }
}
