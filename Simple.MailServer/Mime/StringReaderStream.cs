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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.MailServer.Mime
{
    public class StringReaderStream : IDisposable
    {
        public Stream BaseStream { get; private set; }

        private readonly byte[] _buffer;

        public StringReaderStream(Stream baseStream, int bufferSize = 4096)
        {
            if (baseStream == null) throw new ArgumentNullException("baseStream");

            if (!baseStream.CanRead)
                throw new ArgumentException("baseStream must be readable", "baseStream");

            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize", "buffer size must be larger than 0");

            _buffer = new byte[bufferSize];

            BaseStream = baseStream;
        }

        private byte _lastByte;
        private bool _bufferFull;
        private bool _eof;

        private static readonly byte[] BlankLine = new byte[0];

        public virtual byte[] ReadLine()
        {
            if (_eof) return null;

            for (var i = 0; i < _buffer.Length; i++)
            {
                var b = BaseStream.ReadByte();
                if (b < 0)
                {
                    _eof = true;
                    if (i == 0) return null;

                    var res = new byte[i];
                    Buffer.BlockCopy(_buffer, 0, res, 0, i);
                    return res;
                }

                if (b == 10)
                {
                    var lastByte = i > 0 ? _buffer[i - 1] : _lastByte;
                    if (lastByte == 13 || _bufferFull)
                    {
                        _bufferFull = false;
                        _lastByte = (byte)b;
                        i--;
                        continue;
                    }

                    var res = new byte[i];
                    Buffer.BlockCopy(_buffer, 0, res, 0, i);
                    _lastByte = (byte)b;
                    _bufferFull = false;
                    return res;
                }

                if (b == 13)
                {
                    if (_bufferFull)
                    {
                        _bufferFull = false;
                        _lastByte = (byte)b;
                        i--;
                        continue;
                    }

                    if (i == 0)
                    {
                        _lastByte = (byte)b;
                        return BlankLine;
                    }

                    var res = new byte[i];
                    Buffer.BlockCopy(_buffer, 0, res, 0, i);
                    _lastByte = (byte) b;
                    _bufferFull = false;
                    return res;
                }

                _buffer[i] = (byte)b;
            }

            _lastByte = _buffer[_buffer.Length - 1];
            _bufferFull = true;
            return _buffer;
        }

        private readonly byte[] _singleBuf = new byte[1];

        public virtual async Task<byte[]> ReadLineAsync()
        {
            return await ReadLineAsync(CancellationToken.None);
        }

        public virtual async Task<byte[]> ReadLineAsync(CancellationToken cancellationToken)
        {
            if (_eof) return null;

            for (var i = 0; i < _buffer.Length; i++)
            {
                var read = await BaseStream.ReadAsync(_singleBuf, 0, 1, cancellationToken);
                if (read <= 0)
                {
                    _eof = true;
                    if (i == 0) return null;

                    var res = new byte[i];
                    Buffer.BlockCopy(_buffer, 0, res, 0, i);
                    return res;
                }

                int b = _singleBuf[0];
                if (b == 10)
                {
                    var lastByte = i > 0 ? _buffer[i - 1] : _lastByte;
                    if (lastByte == 13 || _bufferFull)
                    {
                        _bufferFull = false;
                        _lastByte = (byte)b;
                        i--;
                        continue;
                    }

                    var res = new byte[i];
                    Buffer.BlockCopy(_buffer, 0, res, 0, i);
                    _lastByte = (byte)b;
                    _bufferFull = false;
                    return res;
                }

                if (b == 13)
                {
                    if (_bufferFull)
                    {
                        _bufferFull = false;
                        _lastByte = (byte)b;
                        i--;
                        continue;
                    }

                    if (i == 0)
                    {
                        _lastByte = (byte)b;
                        return BlankLine;
                    }

                    var res = new byte[i];
                    Buffer.BlockCopy(_buffer, 0, res, 0, i);
                    _lastByte = (byte)b;
                    _bufferFull = false;
                    return res;
                }

                _buffer[i] = (byte)b;
            }

            _lastByte = _buffer[_buffer.Length - 1];
            _bufferFull = true;
            return _buffer;
        }

        public static byte[] ProcessBackslashes(byte[] buf)
        {
            if (buf == null) return null;
            int offset = 0;

            for (var i = 0; i < buf.Length; i++)
            {
                var b = buf[i];
                if (b == '\b')
                {
                    if (offset < i)
                        offset++;
                    offset++;
                    continue;
                }
                if (offset > 0)
                    buf[i - offset] = b;
            }

            var length = buf.Length - offset;
            if (length != buf.Length)
            {
                var res = new byte[length];
                Buffer.BlockCopy(buf, 0, res, 0, length);
                return res;
            }
            return buf;
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
        ~StringReaderStream()
        {
            Dispose(false);
        }
        #endregion

    }
}