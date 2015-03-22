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

using Simple.MailServer.Mime;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.MailServer
{
    [DebuggerDisplay("{RemoteEndPoint} -> {PortBinding}")]
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

        public abstract TimeSpan GetIdleTime();

        public async Task<byte[]> ReadLineAsync(CancellationToken cancellationToken)
        {
            var rawLine = await Reader.ReadLineAsync(cancellationToken);
            RawLineReceived(this, new RawLineEventArgs(rawLine ?? new byte[0]));
            return rawLine;
        }

        public async Task WriteLineAsyncAndFireEvents(string line)
        {
            if (Writer.BaseStream.CanWrite)
            {
                RawLineSent(this, new RawLineEventArgs(Writer.Encoding.GetBytes(line)));            
                await TextWriter.Synchronized(Writer).WriteLineAsync(line);
            }
        }

        public virtual void Disconnect()
        {
            try
            {
                NetworkStream.Close();
                TcpClient.Close();
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // ignore
            }
        }

        #region Dispose
        private bool _disposed;
        private readonly object _disposeLock = new object();

        /// <summary>
        /// Inheritable dispose method
        /// </summary>
        /// <param name="disposing">true, suppress GC finalizer call</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (_disposeLock)
            {
                if (!_disposed)
                {
                    Disconnect();

                    _disposed = true;
                    if (disposing) GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Free resources being used
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~BaseConnection()
        {
            Dispose(false);
        }
        #endregion
    }
}
