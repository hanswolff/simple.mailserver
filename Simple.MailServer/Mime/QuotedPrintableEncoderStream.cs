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
using System.Text;

namespace Simple.MailServer.Mime
{
    public class QuotedPrintableEncoderStream : Stream
    {
        public Stream BaseStream { get; private set; }

        private Encoding _defaultEncoding = Encoding.GetEncoding("iso-8859-1");
        public Encoding DefaultEncoding
        {
            get { return _defaultEncoding; }
            set { _defaultEncoding = value; }
        }

        public QuotedPrintableEncoderStream(Stream baseStream)
        {
            if (baseStream == null) throw new ArgumentNullException("baseStream");
            BaseStream = baseStream;
        }

        public override bool CanRead
        {
            get { return false; }
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

        public override long Position { get; set; }

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

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        private int _charsInLine;

        public void Write(string text)
        {
            var buf = DefaultEncoding.GetBytes(text);
            Write(buf, 0, buf.Length);
        }

        public void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
        }

        private static readonly byte[] LineBreak = { (byte)'=', 13, 10 };
        private readonly byte[] _triBuf = { (byte)'=', 0, 0 };
        private static readonly byte[] Hex = Encoding.ASCII.GetBytes("0123456789ABCDEF");

        public override void Write(byte[] buffer, int offset, int count)
        {
            for (int i = offset; i < count; i++)
            {
                int currentLen;

                byte b = buffer[i];
                if (b > 127 || b == '=')
                {
                    _triBuf[1] = Hex[b >> 4];
                    _triBuf[2] = Hex[b % 16];
                    currentLen = 3;
                }
                else
                {
                    if (b == 13 || b == 10)
                    {
                        BaseStream.WriteByte(b);
                        _charsInLine = 0;
                        continue;
                    }
                    currentLen = 1;
                }

                _charsInLine += currentLen;
                if (_charsInLine > 75)
                {
                    BaseStream.Write(LineBreak, 0, LineBreak.Length);
                    _charsInLine = currentLen;
                }
                if (currentLen == 1)
                    BaseStream.WriteByte(b);
                else
                    BaseStream.Write(_triBuf, 0, _triBuf.Length);
            }
        }
    }
}