using MyProject.Domain.Entities;

namespace MyProject.Application.Interfaces.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IUserLoginRepository UserLogins { get; }
        IPaymentRepository Payments { get; }
        IGenericRepository<SubscriptionPlan> SubscriptionPlans { get; }
        IUserSubscriptionRepository UserSubscriptions { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
