using System;

namespace Simple.MailServer
{
    public class RequestDisconnectionEventArgs : EventArgs
    {
        public static readonly RequestDisconnectionEventArgs Expected = new RequestDisconnectionEventArgs(true);
        public static readonly RequestDisconnectionEventArgs Unexpected = new RequestDisconnectionEventArgs(false);

        public bool DisconnectionExpected { get; set; }

        public RequestDisconnectionEventArgs()
        {
        }

        public RequestDisconnectionEventArgs(bool disconnectionExpected)
        {
            DisconnectionExpected = disconnectionExpected;
        }
    }
}
