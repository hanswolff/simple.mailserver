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

using Simple.MailServer.Logging;
using System;
using System.Net.Sockets;

namespace Simple.MailServer.Smtp
{
    public class SmtpConnection : BaseConnection
    {
        public SmtpSession Session { get; set; }
        public SmtpServer Server { get; set; }

        public event EventHandler<SmtpConnectionEventArgs> ClientDisconnected = (s, c) => {};
        public event EventHandler<SmtpSessionEventArgs> SessionCreated = (sender, args) => MailServerLogger.Instance.Debug("Session created for " + args.Session.Connection.RemoteEndPoint);

        public override TimeSpan GetIdleTime()
        {
            var session = Session;
            return session != null ? session.GetIdleTime() : TimeSpan.Zero;
        }

        public SmtpConnection(SmtpServer server, PortListener portBinding, TcpClient tcpClient)
            : base(portBinding, tcpClient)
        {
            if (server == null) throw new ArgumentNullException("server");

            Server = server;
        }

        public SmtpSession CreateSession(ISmtpResponderFactory responderFactory)
        {
            var session = new SmtpSession(this, responderFactory);
            session.OnSessionDisconnected +=
                (sender, args) => ClientDisconnected(this, new SmtpConnectionEventArgs(this));
            Session = session;

            SessionCreated(this, new SmtpSessionEventArgs(Session));
            return session;
        }


        public override void Disconnect()
        {
            Session.Disconnect();
            base.Disconnect();
        }
    }
}
