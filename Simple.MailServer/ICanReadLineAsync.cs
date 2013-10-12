using System.Threading;
using System.Threading.Tasks;

namespace Simple.MailServer
{
    public interface ICanReadLineAsync
    {
        Task<byte[]> ReadLineAsync(CancellationToken cancellationToken);
    }
}
