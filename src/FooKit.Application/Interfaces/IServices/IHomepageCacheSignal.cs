using System.Threading;
using Microsoft.Extensions.Primitives;

namespace MyProject.Application.Interfaces.IServices
{
    public interface IHomepageCacheSignal
    {
        IChangeToken GetToken();
        void ResetToken();
    }
}
