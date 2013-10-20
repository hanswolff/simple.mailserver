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
using System.Globalization;
using System.IO;

namespace Simple.MailServer.Mime
{
    public class QuotedPrintableDecoderStream : Stream
    {
        public Stream BaseStream { get; private set; }

        public bool IgnoreErrors { get; set; }

        public long MaxReadPosition { get; set; }

        public QuotedPrintableDecoderStream(Stream baseStream, bool ignoreErrors = false, long maxReadPosition = -1)
        {
            CheckBaseStreamRequirements(baseStream);

            BaseStream = baseStream;
            IgnoreErrors = ignoreErrors;
            MaxReadPosition = maxReadPosition;
        }

        private static void CheckBaseStreamRequirements(Stream baseStream)
        {
            if (baseStream == null) throw new ArgumentNullException("baseStream");

            if (!baseStream.CanRead)
                throw new ArgumentException("baseStream must be readable", "baseStream");
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return BaseStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return BaseStream.CanWrite; }
        }

        public override long Length
        {
            get { return BaseStream.Length; }
        }

        public override long Position
        {
            get { return BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        private int _bytesAfterEqualsSign;
        private readonly byte[] _triBuf = { (byte)'=', 0, 0 };
        private readonly byte[] _singleBuf = new byte[1];
        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = 0;
            for (int i = offset; read < count; i++)
            {
                if (MaxReadPosition >= 0 && BaseStream.Position >= MaxReadPosition)
                    break;

                if (BaseStream.Read(_singleBuf, 0, 1) <= 0)
                    break;

                var b = _singleBuf[0];
                if (_bytesAfterEqualsSign == 0)
                {
                    if (b == '=')
                    {
                        _bytesAfterEqualsSign = 1;
                        continue;
                    }
                    buffer[read++] = b;
                    continue;
                }

                _triBuf[_bytesAfterEqualsSign] = b;
                _bytesAfterEqualsSign++;
                if (_bytesAfterEqualsSign < 3) continue;
                _bytesAfterEqualsSign = 0;

                if (_triBuf[1] == '\r' || _triBuf[1] == '\n')
                {
                    if (!IgnoreErrors && _triBuf[2] != '\r' && _triBuf[2] != '\n')
                        throw new InvalidOperationException("Cannot decode sequence =" + Escape(_triBuf[1]) + Escape(_triBuf[2]));
                    continue;
                }

                int val1 = _triBuf[1];
                val1 -= (val1 < 58 ? 48 : (val1 < 97 ? 55 : 87));
                int val2 = _triBuf[2];
                val2 -= (val2 < 58 ? 48 : (val2 < 97 ? 55 : 87));
                if (val1 < 0 || val1 > 15)
                {
                    if (!IgnoreErrors)
                        throw new InvalidOperationException("Cannot decode sequence =" + Escape(_triBuf[1]) + Escape(_triBuf[2]));
                    val1 = 0;
                    val2 = 0;
                }
                if (val2 < 0 || val2 > 15)
                {
                    if (!IgnoreErrors)
                        throw new InvalidOperationException("Cannot decode sequence =" + Escape(_triBuf[1]) + Escape(_triBuf[2]));
                    val1 = 0;
                    val2 = 0;
                }
                buffer[read++] = ((byte)((val1 << 4) | val2));
            }
            return read;
        }

        private static string Escape(byte c)
        {
            switch (c)
            {
                case (byte)'\0': return @"\0";
                case (byte)'\b': return @"\b";
                case (byte)'\n': return @"\n";
                case (byte)'\r': return @"\r";
                case (byte)'\t': return @"\t";
            }
            return ((char)c).ToString(CultureInfo.InvariantCulture);
        }
    }
}