using System;
using System.IO;
using System.Text;

namespace Simple.MailServer.Mime
{
    public class QuotedPrintableEncoderStream : Stream
    {
        public Stream BaseStream { get; private set; }

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

        private static readonly Encoding _encoding = Encoding.GetEncoding("iso-8859-1");
        public void Write(string text)
        {
            var buf = _encoding.GetBytes(text);
            Write(buf, 0, buf.Length);
        }

        public void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
        }

        private static readonly byte[] _lineBreak = new[] { (byte)'=', (byte)13, (byte)10 };
        private readonly byte[] _triBuf = new[] { (byte)'=', (byte)0, (byte)0 };
        private readonly byte[] Hex = Encoding.ASCII.GetBytes("0123456789ABCDEF");
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
                    BaseStream.Write(_lineBreak, 0, _lineBreak.Length);
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