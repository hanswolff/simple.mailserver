using System;

namespace Simple.MailServer
{
    public class BufferEventArgs : EventArgs
    {
        public byte[] Buffer { get; protected set; }

        public BufferEventArgs(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
}
