using Simple.MailServer.Smtp.Config;
using System;
using Xunit;

namespace Simple.MailServer.Tests.Smtp.Config
{
    public class SmtpServerConfigurationFacts
    {
        [Fact]
        public void setting_ConnectionIdleTimeout_should_fire_ConfigurationChanged()
        {
            var changedEventFired = false;
            var config = CreateWithConfigurationChangedEvent(() => changedEventFired = true);

            config.ConnectionIdleTimeout = 0;
            Assert.True(changedEventFired);
        }

        [Fact]
        public void setting_DefaultGreeting_should_fire_ConfigurationChanged()
        {
            var changedEventFired = false;
            var config = CreateWithConfigurationChangedEvent(() => changedEventFired = true);

            config.DefaultGreeting = "";
            Assert.True(changedEventFired);
        }

        [Fact]
        public void setting_GlobalConnectionTimeout_should_fire_ConfigurationChanged()
        {
            var changedEventFired = false;
            var config = CreateWithConfigurationChangedEvent(() => changedEventFired = true);

            config.GlobalConnectionTimeout = 0;
            Assert.True(changedEventFired);
        }

        [Fact]
        public void setting_MaxMailMessageSize_should_fire_ConfigurationChanged()
        {
            var changedEventFired = false;
            var config = CreateWithConfigurationChangedEvent(() => changedEventFired = true);

            config.MaxMailMessageSize = 0;
            Assert.True(changedEventFired);
        }

        [Fact]
        public void setting_MaxNumberOfRecipients_should_fire_ConfigurationChanged()
        {
            var changedEventFired = false;
            var config = CreateWithConfigurationChangedEvent(() => changedEventFired = true);

            config.MaxNumberOfRecipients = 0;
            Assert.True(changedEventFired);
        }
        
        #region Helpers

        private static SmtpServerConfiguration CreateWithConfigurationChangedEvent(Action onChangedDelegate)
        {
            var config = new SmtpServerConfiguration();
            config.ConfigurationChanged += c => onChangedDelegate();
            return config;
        }

        #endregion
        }
}
