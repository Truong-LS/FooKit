using System.Threading;
using Microsoft.Extensions.Primitives;
using MyProject.Application.Interfaces.IServices;

namespace MyProject.Application.Services
{
    public class HomepageCacheSignal : IHomepageCacheSignal
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public IChangeToken GetToken()
        {
            return new CancellationChangeToken(_cts.Token);
        }

        public void ResetToken()
        {
            // Cancel the old token to trigger cache eviction
            var oldCts = Interlocked.Exchange(ref _cts, new CancellationTokenSource());
            oldCts.Cancel();
            oldCts.Dispose();
        }
    }
}
