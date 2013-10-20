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

using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Simple.MailServer.Tests
{
    public class RawLineProcessorFacts
    {
        [Fact]
        public void request_disconnect_if_null_received()
        {
            var readLineAsyncSource = new ReadLineAsyncAlwaysNull();
            var rawLineDecoder = new RawLineDecoder(readLineAsyncSource);

            var disconnectRequested = false;
            rawLineDecoder.RequestDisconnection += (s, e) => { disconnectRequested = true; rawLineDecoder.Cancel(); };
            var processLineCommand = false;
            rawLineDecoder.ProcessLineCommand += (s, l) => processLineCommand = true;

            rawLineDecoder.ProcessCommandsAsync().Wait(100);
            rawLineDecoder.Cancel();

            Assert.True(disconnectRequested);
            Assert.False(processLineCommand);
        }

        [Fact]
        public void request_disconnect_if_stream_causes_IOException()
        {
            var readLineAsyncSource = new ReadLineAsyncThrowIOException();
            var rawLineDecoder = new RawLineDecoder(readLineAsyncSource);

            var disconnectRequested = false;
            rawLineDecoder.RequestDisconnection += (s, e) => { disconnectRequested = true; rawLineDecoder.Cancel(); };

            rawLineDecoder.ProcessCommandsAsync().Wait(100);
            rawLineDecoder.Cancel();

            Assert.True(disconnectRequested);
        }

        [Fact]
        public void detect_activity()
        {
            var readLineAsyncSource = new ReadLineAsyncReturnsText("a");
            var rawLineDecoder = new RawLineDecoder(readLineAsyncSource);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            var detectedActivity = false;
            rawLineDecoder.DetectedActivity += (s, e) => { detectedActivity = true; rawLineDecoder.Cancel(); };

            rawLineDecoder.ProcessCommandsAsync().Wait(100);
            rawLineDecoder.Cancel();

            Assert.True(detectedActivity);
        }

        [Fact]
        public void read_single_line_command()
        {
            var readLineAsyncSource = new ReadLineAsyncReturnsText("command");
            var rawLineDecoder = new RawLineDecoder(readLineAsyncSource);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            byte[] processLineCommand = null;
            rawLineDecoder.ProcessLineCommand += (s, e) => { processLineCommand = e.Buffer; rawLineDecoder.Cancel(); };

            rawLineDecoder.ProcessCommandsAsync().Wait(100);
            rawLineDecoder.Cancel();

            Assert.Equal("command", Encoding.Default.GetString(processLineCommand));
        }

        [Fact]
        public void cancel_reading_single_line_command()
        {
            var readLineAsyncSource = new ReadLineAsyncReturnsText("command");
            var rawLineDecoder = new RawLineDecoder(readLineAsyncSource);

            byte[] processLineCommand = null;
            rawLineDecoder.ProcessLineCommand += (s, e) => processLineCommand = e.Buffer;

            rawLineDecoder.Cancel();
            rawLineDecoder.ProcessCommandsAsync().Wait(100);

            Assert.Null(processLineCommand);
        }

        #region Helpers

        private class ReadLineAsyncAlwaysNull : ICanReadLineAsync
        {
            public Task<byte[]> ReadLineAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult((byte[])null);
            }
        }

        private class ReadLineAsyncReturnsText : ICanReadLineAsync
        {
            private readonly string _text;

            public ReadLineAsyncReturnsText(string text)
            {
                _text = text;
            }

            public Task<byte[]> ReadLineAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(Encoding.UTF8.GetBytes(_text));
            }
        }

        private class ReadLineAsyncThrowIOException : ICanReadLineAsync
        {
            public Task<byte[]> ReadLineAsync(CancellationToken cancellationToken)
            {
                throw new IOException();
            }
        }

        #endregion
    }
}
