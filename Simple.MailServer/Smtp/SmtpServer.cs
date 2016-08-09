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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Simple.MailServer.Logging;
using Simple.MailServer.Mime;
using Simple.MailServer.Smtp.Config;

namespace Simple.MailServer.Smtp
{
    public class SmtpServer : IHaveConnections, IDisposable
    {
        public string Greeting { get; set; }

        public ConcurrentBag<PortListener> Bindings { get; protected set; }
        public ConcurrentDictionary<EndPoint, SmtpConnection> Connections { get; protected set; }
        public ISmtpServerConfiguration Configuration { get; set; }

        public IEmailValidator EmailValidator
        {
            get { return _emailValidator ?? (_emailValidator = new XamarinEmailValidator()); }
            set { _emailValidator = value; }
        }

        private ISmtpResponderFactory _defaultResponderFactory;
        public ISmtpResponderFactory DefaultResponderFactory
        {
            get { return _defaultResponderFactory ?? (_defaultResponderFactory = new SmtpResponderFactory(Configuration, EmailValidator)); }
            set { _defaultResponderFactory = value; }
        }

        public event EventHandler<SmtpConnectionEventArgs> ClientConnected = (sender, args) => MailServerLogger.Instance.Info("Client connected from " + args.Connection.RemoteEndPoint);
        public event EventHandler<SmtpConnectionEventArgs> ClientDisconnected = (sender, args) => MailServerLogger.Instance.Info("Client disconnected from " + args.Connection.RemoteEndPoint);
        public event EventHandler<SmtpSessionEventArgs> GreetingSent = (sender, args) => MailServerLogger.Instance.Debug("Greeting sent to " + args.Session.Connection.RemoteEndPoint);

        public IdleConnectionDisconnectWatchdog<SmtpServer> Watchdog { get; private set; }

        public SmtpServer()
        {
            Bindings      = new ConcurrentBag<PortListener>();
            Connections   = new ConcurrentDictionary<EndPoint, SmtpConnection>();
            Configuration = new SmtpServerConfiguration();
            Watchdog      = new IdleConnectionDisconnectWatchdog<SmtpServer>(this);

            WatchForConfigurationChange();
        }

        public static SmtpServer CreateAndBind(IPAddress serverListenAddress, int port)
        {
            var smtpServer = new SmtpServer();
            smtpServer.BindAndListenTo(serverListenAddress, port);

            return smtpServer;
        }

        private void WatchForConfigurationChange()
        {
            Configuration.ConfigurationChanged +=
                c =>
                {
                    Watchdog.ConnectionTimeout = c.GlobalConnectionTimeout;
                    Watchdog.IdleTimeout = c.ConnectionIdleTimeout;
                };
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
                    var oldBindings = Bindings;
                    Bindings = new ConcurrentBag<PortListener>();

                    StopListenTo(oldBindings);

                    _disposed = true;
                    if (disposing) GC.SuppressFinalize(this);
                }
            }
        }

        private static void StopListenTo(IEnumerable<PortListener> bindings)
        {
            if (bindings == null) return;

            foreach (var binding in bindings)
            {
                binding.StopListen();
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
        ~SmtpServer()
        {
            Dispose(false);
        }

        #endregion

        public IEnumerable<IClientConnection> GetConnections()
        {
            return Connections.Values;
        }

        private readonly object _bindAndListenLock = new object();
        private IEmailValidator _emailValidator;

        public PortListener BindAndListenTo(IPAddress serverListenAddress, int serverPort)
        {
            lock (_bindAndListenLock)
            {
                StartWatchdogOnFirstCall();

                var portBinding = CreateNewPortBindingAndStartListen(serverListenAddress, serverPort);
                Bindings.Add(portBinding);

                return portBinding;
            }
        }

        private void StartWatchdogOnFirstCall()
        {
            if (Bindings.Count == 0)
                Watchdog.Start();
        }

        private PortListener CreateNewPortBindingAndStartListen(IPAddress serverListenAddress, int serverPort)
        {
            var portBinding = new PortListener(serverListenAddress ?? IPAddress.Any, serverPort);
            portBinding.ClientConnected += PortBindingClientConnected;
            portBinding.StartListen();
            MailServerLogger.Instance.Info(String.Format("Started listening to {0}:{1}", serverListenAddress, serverPort));

            return portBinding;
        }

        private async void PortBindingClientConnected(PortListener serverPortBinding, TcpClient newConnectedTcpClient)
        {
            var connection = new SmtpConnection(this, serverPortBinding, newConnectedTcpClient);
            connection.ClientDisconnected += (sender, args) => ClientDisconnected(this, new SmtpConnectionEventArgs(args.Connection));

            SmtpConnection cnn;
            connection.ClientDisconnected += (sender, args) => Connections.TryRemove(connection.RemoteEndPoint, out cnn);

            Connections[connection.RemoteEndPoint] = connection;
            ClientConnected(this, new SmtpConnectionEventArgs(connection));

            try
            {
                await CreateSessionAndProcessCommands(connection);
            } catch(Exception ex)
            {
                MailServerLogger.Instance.Error(ex);
            }
        }

        private async Task CreateSessionAndProcessCommands(SmtpConnection connection)
        {
            var session = connection.CreateSession(DefaultResponderFactory);
            await SetupSessionThenProcessCommands(connection, session);
        }

        private async Task SetupSessionThenProcessCommands(SmtpConnection connection, SmtpSession session)
        {
            await SendGreetingAsync(connection, Greeting ?? Configuration.DefaultGreeting);

            var sessionInfoParseResponder = new SmtpSessionInfoResponder(session.ResponderFactory, session.SessionInfo);

            var rawLineDecoder = new RawLineDecoder(connection);
            rawLineDecoder.RequestDisconnection += (s, e) =>
            {
                if (!e.DisconnectionExpected)
                {
                    MailServerLogger.Instance.Warn(String.Format("Connection unexpectedly lost {0}", connection.RemoteEndPoint));
                }

                rawLineDecoder.Cancel();
                session.Disconnect();
            };
            rawLineDecoder.DetectedActivity += (s, e) => session.UpdateActivity();
            rawLineDecoder.ProcessLineCommand += async (s, e) =>
            {
                var response = sessionInfoParseResponder.ProcessLineCommand(e.Buffer);
                if (response == null || !response.HasValue) return;

                if (response.ResponseCode == SmtpResponses.DisconnectResponseCode)
                {
                    await SendResponseAsync(connection, response);

                    MailServerLogger.Instance.Debug(String.Format("Remote connection disconnected {0}", connection.RemoteEndPoint));
                    rawLineDecoder.Cancel();
                    await Task.Delay(100).ContinueWith(t => session.Disconnect());
                    return;
                }

                await SendResponseAsync(connection, response);
            };

#pragma warning disable 4014
            rawLineDecoder.ProcessCommandsAsync();
#pragma warning restore 4014
        }

        private static async Task SendResponseAsync(SmtpConnection connection, SmtpResponse response)
        {
            LogResponse(response);

            try
            {
                foreach (var additional in response.AdditionalLines)
                    await connection.WriteLineAsyncAndFireEvents(additional);

                await connection.WriteLineAsyncAndFireEvents(response.ResponseCode + " " + response.ResponseText);

            } catch(Exception ex)
            {
                MailServerLogger.Instance.Error(ex);
            }
        }

        private static void LogResponse(SmtpResponse response)
        {
            var logger = MailServerLogger.Instance;
            if (logger.LogLevel < MailServerLogLevel.Debug) return;

            var logMessage = new StringBuilder();
            foreach (var additionalLine in response.AdditionalLines)
            {
                logMessage.AppendLine(">>> " + additionalLine);
            }

            logMessage.AppendLine(">>> " + response.ResponseCode + " " + response.ResponseText);
            logger.Debug(logMessage.ToString());
        }


        protected async Task SendGreetingAsync(SmtpConnection connectionToSendGreetingTo, string greeting)
        {
            if (connectionToSendGreetingTo == null) throw new ArgumentNullException("connectionToSendGreetingTo");

            var greetingLine = "220 " + greeting;

            MailServerLogger.Instance.Debug(">>> " + greetingLine);
            await connectionToSendGreetingTo.WriteLineAsyncAndFireEvents(greetingLine);
            GreetingSent(this, new SmtpSessionEventArgs(connectionToSendGreetingTo.Session));
        }
    }
}
