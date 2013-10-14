using System.Threading;
using Xunit;

namespace Simple.MailServer.Tests
{
    public class BaseSessionFacts
    {
        [Fact]
        public void GetIdleTimeMilliseconds_starts_at_zero()
        {
            var session = new BaseSession();
            Assert.InRange(session.GetIdleTimeMilliseconds(), 0, 10);
        }

        [Fact]
        public void GetIdleTimeMilliseconds_increases_over_time()
        {
            var session = new BaseSession();
            Thread.Sleep(20);
            Assert.InRange(session.GetIdleTimeMilliseconds(), 10, 100);
        }
    }
}
