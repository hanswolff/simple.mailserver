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
