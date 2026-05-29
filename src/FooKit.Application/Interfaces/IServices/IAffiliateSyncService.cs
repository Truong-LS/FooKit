using System.Threading.Tasks;

namespace MyProject.Application.Interfaces.IServices
{
    public interface IAffiliateSyncService
    {
        Task ManualSyncAsync(bool forceSyncAll, string targetIngredientId);
    }
}
