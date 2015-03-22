#region Header
// Copyright (c) 2013-2015 Hans Wolff
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

using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Simple.MailServer.Tests
{
    public class IdleConnectionDisconnectorWatchdogFacts
    {
        [Fact]
        public void watchdog_should_not_disconnect_if_idle_timeout_not_reached()
        {
            var server = new ServerConnectionsStub();
            var watchdog = StartWatchdogWithIdleTimeout(server, TimeSpan.FromSeconds(1));

            var connection = CreateMockedConnection(DateTime.UtcNow, TimeSpan.Zero);
            server.Connections.Add(connection.Object);

            watchdog.TimerCall();
            connection.Verify(x => x.Disconnect(), Times.Never);
        }

        [Fact]
        public void watchdog_should_be_able_to_Stop()
        {
            var server = new ServerConnectionsStub();
            var watchdog = StartWatchdogWithIdleTimeout(server, TimeSpan.FromSeconds(1));
            watchdog.Stop();

            var connection = CreateMockedConnection(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            server.Connections.Add(connection.Object);

            watchdog.TimerCall();
            connection.Verify(x => x.Disconnect(), Times.Never);
        }

        [Fact]
        public void watchdog_should_disconnect_if_idle_timeout_is_reached()
        {
            var server = new ServerConnectionsStub();
            var watchdog = StartWatchdogWithIdleTimeout(server, TimeSpan.FromSeconds(1));

            var connection = CreateMockedConnection(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            server.Connections.Add(connection.Object);

            watchdog.TimerCall();
            connection.Verify(x => x.Disconnect());
        }

        [Fact]
        public void watchdog_should_fire_TerminateConnection_if_idle_timeout_is_reached()
        {
            var server = new ServerConnectionsStub();
            var watchdog = StartWatchdogWithIdleTimeout(server, TimeSpan.FromMilliseconds(1));
            
            var terminatingConnection = false;
            watchdog.TerminatingConnection += (s, e) => terminatingConnection = true;

            var connection = CreateMockedConnection(DateTime.UtcNow, TimeSpan.FromMilliseconds(2));
            server.Connections.Add(connection.Object);

            watchdog.TimerCall();
            Assert.True(terminatingConnection);
        }

        [Fact]
        public void watchdog_should_disconnect_if_connection_timeout_is_reached()
        {
            var server = new ServerConnectionsStub();
            var watchdog = StartWatchdogWithConnectionTimeout(server, TimeSpan.FromSeconds(1));
            watchdog.FuncTimeSource = () => new DateTime(2000, 1, 1).AddSeconds(2);

            var connection = CreateMockedConnection(new DateTime(2000, 1, 1), TimeSpan.Zero);
            server.Connections.Add(connection.Object);

            watchdog.TimerCall();
            connection.Verify(x => x.Disconnect());
        }

        [Fact]
        public void watchdog_should_fire_TerminateConnection_if_connection_timeout_is_reached()
        {
            var server = new ServerConnectionsStub();
            var watchdog = StartWatchdogWithConnectionTimeout(server, TimeSpan.FromSeconds(1));
            watchdog.FuncTimeSource = () => new DateTime(2000, 1, 1).AddSeconds(2);

            var terminatingConnection = false;
            watchdog.TerminatingConnection += (s, e) => terminatingConnection = true;

            var connection = CreateMockedConnection(new DateTime(2000, 1, 1), TimeSpan.Zero);
            server.Connections.Add(connection.Object);

            watchdog.TimerCall();
            Assert.True(terminatingConnection);
        }

        [Fact]
        public void watchdog_should_not_disconnect_if_connection_timeout_is_not_reached()
        {
            var server = new ServerConnectionsStub();
            var watchdog = StartWatchdogWithConnectionTimeout(server, TimeSpan.FromSeconds(2));
            watchdog.FuncTimeSource = () => new DateTime(2000, 1, 1).AddSeconds(1);

            var connection = CreateMockedConnection(new DateTime(2000, 1, 1), TimeSpan.Zero);
            connection.Setup(x => x.Disconnect()).Throws(new Exception("RequestDisconnection shouldn't be called"));
            server.Connections.Add(connection.Object);
            watchdog.TimerCall();
        }

        #region Helpers

        private static IdleConnectionDisconnectWatchdog<ServerConnectionsStub> StartWatchdogWithIdleTimeout(ServerConnectionsStub server, TimeSpan idleTimeout)
        {
            var watchdog = new IdleConnectionDisconnectWatchdog<ServerConnectionsStub>(server)
            {
                ConnectionTimeout = new TimeSpan(-1),
                Interval = 1,
                IdleTimeout = idleTimeout
            };
            watchdog.Start();
            return watchdog;
        }

        private static IdleConnectionDisconnectWatchdog<ServerConnectionsStub> StartWatchdogWithConnectionTimeout(ServerConnectionsStub server, TimeSpan connectionTimeout)
        {
            var watchdog = new IdleConnectionDisconnectWatchdog<ServerConnectionsStub>(server)
            {
                ConnectionTimeout = connectionTimeout,
                Interval = 1,
                IdleTimeout = new TimeSpan(-1)
            };
            watchdog.Start();
            return watchdog;
        }

        private static Mock<IClientConnection> CreateMockedConnection(DateTime connectedDateTime, TimeSpan idleTime)
        {
            var connection = new Mock<IClientConnection>();
            connection.Setup(x => x.ConnectionInitiated).Returns(connectedDateTime);
            connection.Setup(x => x.GetIdleTime()).Returns(idleTime);

            return connection;
        }

        private class ServerConnectionsStub : IHaveConnections
        {
            public IList<IClientConnection> Connections { get; private set; }

            public ServerConnectionsStub()
            {
                Connections = new List<IClientConnection>();
            }

            public IEnumerable<IClientConnection> GetConnections()
            {
                return Connections;
            }
        }

        #endregion
    }
}
