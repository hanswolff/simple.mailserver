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
using Simple.MailServer.Mime;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.MailServer
{
    public class RawLineDecoder
    {
        private readonly ICanReadLineAsync _readLineSource;

        public event EventHandler DetectedActivity = (s, e) => { };
        public event EventHandler<BufferEventArgs> ProcessLineCommand = (s, e) => { };
        public event EventHandler<RequestDisconnectionEventArgs> RequestDisconnection = (s, e) => { };

        public RawLineDecoder(ICanReadLineAsync readLineSource)
        {
            _readLineSource = readLineSource;
        }

        private CancellationTokenSource _cts = new CancellationTokenSource();
        public void Cancel()
        {
            _cts.Cancel();
        }

        public async Task ProcessCommandsAsync()
        {
            var cancellationToken = _cts.Token;
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    byte[] line;
                    try
                    {
                        line = await _readLineSource.ReadLineAsync(cancellationToken);
                        if (line == null)
                        {
                            RequestDisconnection(this, RequestDisconnectionEventArgs.Expected);
                            return;
                        }
                    }
                    catch (IOException)
                    {
                        RequestDisconnection(this, RequestDisconnectionEventArgs.Unexpected);
                        return;
                    }
                    catch (TaskCanceledException)
                    {
                        return;
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }

                    DetectedActivity(this, EventArgs.Empty);

                    line = StringReaderStream.ProcessBackslashes(line);

                    ProcessLineCommand(this, new BufferEventArgs(line));
                }
            }
            catch (Exception ex)
            {
                MailServerLogger.Instance.Error(ex);
                RequestDisconnection(this, RequestDisconnectionEventArgs.Unexpected);
            }
            finally
            {
                _cts = new CancellationTokenSource();
            }
        }
    }
}
