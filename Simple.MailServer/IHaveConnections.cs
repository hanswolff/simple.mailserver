using System.Collections.Generic;

namespace Simple.MailServer
{
    public interface IHaveConnections
    {
        IEnumerable<IClientConnection> GetConnections();
    }
}
