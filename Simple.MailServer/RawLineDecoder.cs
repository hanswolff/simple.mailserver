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
        public event EventHandler RequestDisconnection = (s, e) => { };

        public RawLineDecoder(ICanReadLineAsync readLineSource)
        {
            _readLineSource = readLineSource;
        }

        public async Task ProcessCommandsAsync()
        {
            await ProcessCommandsAsync(CancellationToken.None);
        }

        public async Task ProcessCommandsAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    byte[] line;
                    try
                    {
                        line = await _readLineSource.ReadLineAsync(cancellationToken);
                        if (line == null)
                        {
                            RequestDisconnection(this, EventArgs.Empty);
                            return;
                        }
                    }
                    catch (IOException)
                    {
                        RequestDisconnection(this, EventArgs.Empty);
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
                RequestDisconnection(this, EventArgs.Empty);
            }
        }
    }
}
