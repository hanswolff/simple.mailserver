using System;

namespace Simple.MailServer
{
    public class RawLineEventArgs : EventArgs
    {
        public byte[] RawLine { get; set; }

        public RawLineEventArgs(byte[] rawLine)
        {
            RawLine = rawLine;
        }
    }
}
