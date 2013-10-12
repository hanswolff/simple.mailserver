using System;

namespace Simple.MailServer
{
    public class ClientConnectionEventArgs : EventArgs
    {
        public IClientConnection Connection { get; protected set; }

        public ClientConnectionEventArgs(IClientConnection connection)
        {
            Connection = connection;
        }
    }
}
