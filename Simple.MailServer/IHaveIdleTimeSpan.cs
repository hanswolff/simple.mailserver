using System;

namespace Simple.MailServer
{
    public interface IHaveIdleTimeSpan
    {
        long GetIdleTimeMilliseconds();
    }
}
