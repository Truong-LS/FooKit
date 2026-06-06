using Microsoft.EntityFrameworkCore;
using FooKit.Domain.Entities;
using System;
using System.Reflection;

namespace FooKit.Infrastructure.Data.DBContext
{
    public class FooKitDbContext : DbContext
    {
        public FooKitDbContext(DbContextOptions<FooKitDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<UserDietaryPreference> UserDietaryPreferences { get; set; }
        public DbSet<UserTool> UserTools { get; set; }
        public DbSet<DishCache> DishCaches { get; set; }
        public DbSet<StandardIngredient> StandardIngredients { get; set; }
        public DbSet<IngredientDictionary> IngredientDictionaries { get; set; }
        public DbSet<AffiliateProduct> AffiliateProducts { get; set; }
        public DbSet<SuggestionRequest> SuggestionRequests { get; set; }
        public DbSet<SuggestionResult> SuggestionResults { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UserHomepageCache> UserHomepageCaches { get; set; }
        public DbSet<UserHistory> UserHistories { get; set; }
        public DbSet<UserAllergy> UserAllergies { get; set; }
        public DbSet<ThirdPartyApiLog> ThirdPartyApiLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
