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

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Simple.MailServer.Tests
{
    public class PortListenerFacts
    {
        [Fact]
        public void port_listener_is_listening_to_port()
        {
            var testPort = GetTestPort();
            using (var portListener = new PortListener(IPAddress.Loopback, testPort))
            {
                var clientConnectedRaised = new ManualResetEventSlim();
                portListener.ClientConnected += (sender, client) => clientConnectedRaised.Set();
                portListener.StartListen();

                using (var client = new TcpClient())
                {
                    client.Connect(IPAddress.Loopback.ToString(), testPort);
                    Assert.True(clientConnectedRaised.Wait(200));
                }
            }
        }

        [Fact]
        public void port_listener_cannot_start_twice()
        {
            var testPort = GetTestPort();
            using (var portListener = new PortListener(IPAddress.Loopback, testPort))
            {
                portListener.StartListen();
                Assert.Throws<InvalidOperationException>(() => portListener.StartListen());
            }
        }

        [Fact]
        public void StartListen_then_StopListen_could_not_be_able_to_connect()
        {
            var testPort = GetTestPort();
            using (var portListener = new PortListener(IPAddress.Loopback, testPort))
            {
                portListener.StartListen();
                portListener.StopListen();

                var task = Task<bool>.Factory.StartNew(() =>
                {
                    using (var client = new TcpClient())
                    {
                        try
                        {
                            client.Connect(IPAddress.Loopback.ToString(), testPort);
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                        return true;
                    }
                }, TaskCreationOptions.LongRunning);

                if (task.Wait(200))
                {
                    Assert.False(task.Result);
                }
            }
        }

        #region

        int TestPort { get; set; }

        private int GetTestPort()
        {
            return TestPort++;
        }

        public PortListenerFacts()
        {
            TestPort = new Random(Environment.TickCount).Next(10000, 65000);
        }

        #endregion
    }
}
