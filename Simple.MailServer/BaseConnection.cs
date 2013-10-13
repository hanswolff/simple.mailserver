using Simple.MailServer.Mime;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.MailServer
{
    public abstract class BaseConnection : IClientConnection, ICanReadLineAsync
    {
        public NetworkStream NetworkStream { get; protected set; }
        public IPEndPoint RemoteEndPoint { get; protected set; }
        public PortListener PortBinding { get; protected set; }
        public TcpClient TcpClient { get; protected set; }
        public DateTime ConnectionInitiated { get; protected set; }

        protected StringReaderStream Reader;
        protected StreamWriter Writer;

        public event EventHandler<RawLineEventArgs> RawLineReceived = (s, e) => { };
        public event EventHandler<RawLineEventArgs> RawLineSent = (s, e) => { };

        protected BaseConnection(PortListener portBinding, TcpClient tcpClient)
        {
            if (portBinding == null) throw new ArgumentNullException("portBinding");
            if (tcpClient == null) throw new ArgumentNullException("tcpClient");

            PortBinding = portBinding;
            TcpClient = tcpClient;

            ConnectionInitiated = DateTime.UtcNow;
            NetworkStream = tcpClient.GetStream();
            Reader = new StringReaderStream(NetworkStream);
            Writer = new StreamWriter(NetworkStream) { AutoFlush = true };
            RemoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
        }

        public abstract long GetIdleTimeMilliseconds();

        public async Task<byte[]> ReadLineAsync(CancellationToken cancellationToken)
        {
            var rawLine = await Reader.ReadLineAsync(cancellationToken);
            RawLineReceived(this, new RawLineEventArgs(rawLine));
            return rawLine;
        }

        public async Task WriteLineAsyncAndFireEvents(string line)
        {
            RawLineSent(this, new RawLineEventArgs(Writer.Encoding.GetBytes(line)));
            await Writer.WriteLineAsync(line);
        }

        public virtual void Disconnect()
        {
            TcpClient.Close();
        }
    }
}
