using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace Simple.MailServer.Tests
{
    public class IdleConnectionDisconnectorWatchdogFacts
    {
        [Fact]
        public void watchdog_should_not_disconnect_if_idle_timeout_not_reached()
        {
            var server = new ServerConnectionsStub();
            var watchdog = new IdleConnectionDisconnectWatchdog<ServerConnectionsStub>(server)
            {
                Interval = 1,
                IdleTimeoutMilliseconds = 1
            };

            var connection = CreateMockedConnectionWithIdleTime(0);
            connection.Setup(x => x.Disconnect()).Throws(new Exception("RequestDisconnection shouldn't be called"));
            server.Connections.Add(connection.Object);

            watchdog.TimerCall();
        }

        [Fact(Timeout = 1000)]
        public void watchdog_should_disconnect_if_idle_timeout_is_reached()
        {
            var server = new ServerConnectionsStub();
            var watchdog = new IdleConnectionDisconnectWatchdog<ServerConnectionsStub>(server)
            {
                Interval = 1,
                IdleTimeoutMilliseconds = 1
            };

            var connection = CreateMockedConnectionWithIdleTime(1);
            server.Connections.Add(connection.Object);

            watchdog.TimerCall();
            connection.Verify(x => x.Disconnect());
        }

        [Fact]
        public void watchdog_should_fire_TerminateConnection_if_idle_timeout_is_reached()
        {
            var server = new ServerConnectionsStub();
            var watchdog = new IdleConnectionDisconnectWatchdog<ServerConnectionsStub>(server)
            {
                Interval = 1,
                IdleTimeoutMilliseconds = 1
            };
            bool terminatingConnection = false;
            watchdog.TerminatingConnection += (s, e) => terminatingConnection = true;

            var connection = CreateMockedConnectionWithIdleTime(1);
            server.Connections.Add(connection.Object);

            watchdog.TimerCall();
            Assert.True(terminatingConnection);
        }

        [Fact(Timeout = 1000)]
        public void watchdog_should_disconnect_if_connection_timeout_is_reached()
        {
            var server = new ServerConnectionsStub();
            var watchdog = new IdleConnectionDisconnectWatchdog<ServerConnectionsStub>(server)
            {
                Interval = 1,
                ConnectionTimeoutMilliseconds = 1
            };

            var connection = CreateMockedConnectionWithIdleTime(0);
            server.Connections.Add(connection.Object);

            Thread.Sleep(1);
            watchdog.TimerCall();
            connection.Verify(x => x.Disconnect());
        }

        [Fact]
        public void watchdog_should_fire_TerminateConnection_if_connection_timeout_is_reached()
        {
            var server = new ServerConnectionsStub();
            var watchdog = new IdleConnectionDisconnectWatchdog<ServerConnectionsStub>(server)
            {
                Interval = 1,
                ConnectionTimeoutMilliseconds = 1
            };
            bool terminatingConnection = false;
            watchdog.TerminatingConnection += (s, e) => terminatingConnection = true;

            var connection = CreateMockedConnectionWithIdleTime(0);
            server.Connections.Add(connection.Object);

            Thread.Sleep(1);
            watchdog.TimerCall();
            Assert.True(terminatingConnection);
        }

        [Fact]
        public void watchdog_should_not_disconnect_if_connection_timeout_is_reached()
        {
            var server = new ServerConnectionsStub();
            var watchdog = new IdleConnectionDisconnectWatchdog<ServerConnectionsStub>(server)
            {
                Interval = 1,
                ConnectionTimeoutMilliseconds = 1000
            };

            var connection = CreateMockedConnectionWithIdleTime(0);
            connection.Setup(x => x.Disconnect()).Throws(new Exception("RequestDisconnection shouldn't be called"));
            server.Connections.Add(connection.Object);

            watchdog.TimerCall();
        }

        #region Helpers

        private static Mock<IClientConnection> CreateMockedConnectionWithIdleTime(int idleTimeMillis)
        {
            var connection = new Mock<IClientConnection>();
            connection.Setup(x => x.ConnectionInitiated).Returns(DateTime.UtcNow);
            connection.Setup(x => x.GetIdleTimeMilliseconds()).Returns(idleTimeMillis);

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
