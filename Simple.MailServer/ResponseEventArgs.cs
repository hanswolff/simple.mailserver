using System;

namespace Simple.MailServer
{
    public class ResponseEventArgs : EventArgs
    {
        public bool Handled { get; set; }
        public bool Rejected { get; set; }
        public bool SuppressResponse { get; set; }
    }
}
