using System.Threading;
using Microsoft.Extensions.Primitives;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IHomepageCacheSignal
    {
        IChangeToken GetToken();
        void ResetToken();
    }
}
