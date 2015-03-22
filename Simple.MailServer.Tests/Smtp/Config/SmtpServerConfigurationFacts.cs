#region Header
// Copyright (c) 2013 Hans Wolff
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

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

            config.ConnectionIdleTimeout = TimeSpan.Zero;
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

            config.GlobalConnectionTimeout = TimeSpan.Zero;
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
