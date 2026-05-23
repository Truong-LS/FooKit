using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyProject.Application.Interfaces.IRepositories;
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
        public IRefreshTokenRepository RefreshTokens { get; private set; }
        public IUserLoginRepository UserLogins { get; private set; }
        public IPaymentRepository Payments { get; private set; }
        public IGenericRepository<Domain.Entities.SubscriptionPlan> SubscriptionPlans { get; private set; }
        public IUserSubscriptionRepository UserSubscriptions { get; private set; }
        public IGenericRepository<Domain.Entities.StandardIngredient> StandardIngredients { get; private set; }
        public IGenericRepository<Domain.Entities.IngredientDictionary> IngredientDictionaries { get; private set; }
        public IGenericRepository<Domain.Entities.AffiliateProduct> AffiliateProducts { get; private set; }
        public IGenericRepository<Domain.Entities.DishCache> DishCaches { get; private set; }
        public IGenericRepository<Domain.Entities.SuggestionRequest> SuggestionRequests { get; private set; }
        public IGenericRepository<Domain.Entities.SuggestionResult> SuggestionResults { get; private set; }
        public IGenericRepository<Domain.Entities.UserHomepageCache> UserHomepageCaches { get; private set; }
        public IGenericRepository<Domain.Entities.UserHistory> UserHistories { get; private set; }
        public IGenericRepository<Domain.Entities.UserAllergy> UserAllergies { get; private set; }

        public UnitOfWork(FooKitDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            RefreshTokens = new RefreshTokenRepository(_context);
            UserLogins = new UserLoginRepository(_context);
            Payments = new PaymentRepository(_context);
            SubscriptionPlans = new GenericRepository<Domain.Entities.SubscriptionPlan>(_context);
            UserSubscriptions = new UserSubscriptionRepository(_context);
            StandardIngredients = new GenericRepository<Domain.Entities.StandardIngredient>(_context);
            IngredientDictionaries = new GenericRepository<Domain.Entities.IngredientDictionary>(_context);
            AffiliateProducts = new GenericRepository<Domain.Entities.AffiliateProduct>(_context);
            DishCaches = new GenericRepository<Domain.Entities.DishCache>(_context);
            SuggestionRequests = new GenericRepository<Domain.Entities.SuggestionRequest>(_context);
            SuggestionResults = new GenericRepository<Domain.Entities.SuggestionResult>(_context);
            UserHomepageCaches = new GenericRepository<Domain.Entities.UserHomepageCache>(_context);
            UserHistories = new GenericRepository<Domain.Entities.UserHistory>(_context);
            UserAllergies = new GenericRepository<Domain.Entities.UserAllergy>(_context);
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
