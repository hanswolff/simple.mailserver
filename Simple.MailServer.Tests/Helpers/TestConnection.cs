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
using System.IO;
using System.Net;
using System.Net.Sockets;
using Simple.MailServer.Smtp;

namespace Simple.MailServer.Tests.Helpers
{
    sealed class TestConnection : IDisposable
    {
        private readonly TcpClient _tcpClient;
        private readonly StreamWriter _writer;
        private readonly StreamReader _reader;

        public TestConnection(IPAddress ipAddress, int port)
            : this(ipAddress.ToString(), port)
        {
        }

        public TestConnection(string hostName, int port)
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(hostName, port);

            var networkStream = _tcpClient.GetStream();
            _writer = new StreamWriter(networkStream) { AutoFlush = true };
            _reader = new StreamReader(networkStream);

            _reader.ReadLine(); // ignore greeting line
        }

        public void RunCommand(string command, int responseCode)
        {
            _writer.WriteLine(command);

            var line = _reader.ReadLine();

            if (line == null)
                throw new InvalidOperationException("Stream has unexpectedly closed");

            if (!line.StartsWith(responseCode + " "))
                throw new InvalidOperationException(String.Format("After command '{0}' received '{1}' but expected response with code '{2}'", command, line, responseCode));
        }

        public void RunCommand(string command, SmtpResponse expectedSmtpResponse)
        {
            _writer.WriteLine(command);

            var line = _reader.ReadLine();

            if (line == null)
                throw new InvalidOperationException("Stream has unexpectedly closed");

            if (line != expectedSmtpResponse.ToString())
                throw new InvalidOperationException(String.Format("After command '{0}' received '{1}' but expected '{2}'", command, line, expectedSmtpResponse));
        }

        public void Dispose()
        {
            _writer.Dispose();
            _reader.Dispose();
            _tcpClient.Close();
        }
    }
}
