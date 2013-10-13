using System;
using System.IO;

namespace Simple.MailServer.Mime
{
    public class DelimitingReadableStream : Stream
    {
        public Stream BaseStream { get; private set; }

        public long MaxReadPosition { get; set; }

        public DelimitingReadableStream(Stream baseStream, long maxReadPosition = -1)
        {
            if (baseStream == null) throw new ArgumentNullException("baseStream");

            if (!baseStream.CanRead)
                throw new ArgumentException("baseStream must be readable", "baseStream");

            BaseStream = baseStream;
            MaxReadPosition = maxReadPosition;
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
            get { return false; }
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

        public override int Read(byte[] buffer, int offset, int count)
        {
            var position = BaseStream.Position;
            if (MaxReadPosition >= 0)
            {
                if (position >= MaxReadPosition) return 0;
                if (position + count > MaxReadPosition)
                    count = (int)(MaxReadPosition - position);
                if (count <= 0) return 0;
            }

            return BaseStream.Read(buffer, offset, count);
        }
    }
}