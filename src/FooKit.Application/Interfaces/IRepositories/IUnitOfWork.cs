using FooKit.Domain.Entities;

namespace FooKit.Application.Interfaces.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IGenericRepository<Role> Roles { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IUserLoginRepository UserLogins { get; }
        IPaymentRepository Payments { get; }
        IGenericRepository<SubscriptionPlan> SubscriptionPlans { get; }
        IUserSubscriptionRepository UserSubscriptions { get; }
        IStandardIngredientRepository StandardIngredients { get; }
        IIngredientDictionaryRepository IngredientDictionaries { get; }
        IAffiliateProductRepository AffiliateProducts { get; }
        IGenericRepository<DishCache> DishCaches { get; }
        IGenericRepository<SuggestionRequest> SuggestionRequests { get; }
        IGenericRepository<SuggestionResult> SuggestionResults { get; }
        IGenericRepository<UserHomepageCache> UserHomepageCaches { get; }
        IGenericRepository<UserHistory> UserHistories { get; }
        IGenericRepository<UserAllergy> UserAllergies { get; }
        IGenericRepository<ThirdPartyApiLog> ThirdPartyApiLogs { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
