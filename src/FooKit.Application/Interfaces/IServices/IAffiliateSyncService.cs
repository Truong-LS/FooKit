using System.Threading.Tasks;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IAffiliateSyncService
    {
        Task ManualSyncAsync(bool forceSyncAll, string targetIngredientId);
    }
}
