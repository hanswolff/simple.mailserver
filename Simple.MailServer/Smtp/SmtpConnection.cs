using System;
using System.Net.Sockets;

namespace Simple.MailServer.Smtp
{
    public class SmtpConnection : BaseConnection
    {
        public SmtpSession Session { get; set; }
        public SmtpServer Server { get; set; }

        public event EventHandler<SmtpConnectionEventArgs> ClientDisconnected = (s, c) => {};

        public override long GetIdleTimeMilliseconds()
        {
            var session = Session;
            return session != null ? session.GetIdleTimeMilliseconds() : 0;
        }

        public SmtpConnection(SmtpServer server, PortListener portBinding, TcpClient tcpClient)
            : base(portBinding, tcpClient)
        {
            if (server == null) throw new ArgumentNullException("server");

            Server = server;
        }

        public override void Disconnect()
        {
            Session.Disconnect();
            base.Disconnect();
        }
    }
}
