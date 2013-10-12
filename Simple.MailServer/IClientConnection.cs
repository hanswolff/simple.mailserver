using System;
using System.Net;
using System.Net.Sockets;

namespace Simple.MailServer
{
    public interface IClientConnection : ICanDisconnect, IHaveIdleTimeSpan
    {
        NetworkStream NetworkStream { get; }
        IPEndPoint RemoteEndPoint { get; }
        PortListener PortBinding { get; }
        TcpClient TcpClient { get; }
        DateTime ConnectionInitiated { get; }
    }
}
