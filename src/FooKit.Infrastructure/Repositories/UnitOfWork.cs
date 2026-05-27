using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyProject.Application.Interfaces.IRepositories;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Data.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FooKitDbContext _context;
        private IDbContextTransaction? _currentTransaction;

        public IUserRepository Users { get; private set; }
        public IGenericRepository<Role> Roles { get; private set; }
        public IRefreshTokenRepository RefreshTokens { get; private set; }
        public IUserLoginRepository UserLogins { get; private set; }
        public IPaymentRepository Payments { get; private set; }
        public IGenericRepository<SubscriptionPlan> SubscriptionPlans { get; private set; }
        public IUserSubscriptionRepository UserSubscriptions { get; private set; }
        public IStandardIngredientRepository StandardIngredients { get; private set; }
        public IIngredientDictionaryRepository IngredientDictionaries { get; private set; }
        public IAffiliateProductRepository AffiliateProducts { get; private set; }
        public IGenericRepository<DishCache> DishCaches { get; private set; }
        public IGenericRepository<SuggestionRequest> SuggestionRequests { get; private set; }
        public IGenericRepository<SuggestionResult> SuggestionResults { get; private set; }
        public IGenericRepository<UserHomepageCache> UserHomepageCaches { get; private set; }
        public IGenericRepository<UserHistory> UserHistories { get; private set; }
        public IGenericRepository<UserAllergy> UserAllergies { get; private set; }
        public IGenericRepository<ThirdPartyApiLog> ThirdPartyApiLogs { get; private set; }

        public UnitOfWork(FooKitDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Roles = new GenericRepository<Role>(_context);
            RefreshTokens = new RefreshTokenRepository(_context);
            UserLogins = new UserLoginRepository(_context);
            Payments = new PaymentRepository(_context);
            SubscriptionPlans = new GenericRepository<SubscriptionPlan>(_context);
            UserSubscriptions = new UserSubscriptionRepository(_context);
            StandardIngredients = new StandardIngredientRepository(_context);
            IngredientDictionaries = new IngredientDictionaryRepository(_context);
            AffiliateProducts = new AffiliateProductRepository(_context);
            DishCaches = new GenericRepository<DishCache>(_context);
            SuggestionRequests = new GenericRepository<SuggestionRequest>(_context);
            SuggestionResults = new GenericRepository<SuggestionResult>(_context);
            UserHomepageCaches = new GenericRepository<UserHomepageCache>(_context);
            UserHistories = new GenericRepository<UserHistory>(_context);
            UserAllergies = new GenericRepository<UserAllergy>(_context);
            ThirdPartyApiLogs = new GenericRepository<ThirdPartyApiLog>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null) return;
            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync(); 
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw; 
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync();
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public Task ExecuteInTransactionAsync(Func<Task> action)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await action();
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public void Dispose()
        {
            _context.Dispose();
            _currentTransaction?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
