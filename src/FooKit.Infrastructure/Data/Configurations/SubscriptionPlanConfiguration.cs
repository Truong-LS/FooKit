using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FooKit.Domain.Entities;

namespace FooKit.Infrastructure.Data.Configurations
{
    public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
        {
            builder.ToTable("SubscriptionPlans");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PlanName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.OwnsOne(x => x.Price, p =>
            {
                p.Property(m => m.Amount).HasColumnType("decimal(18,2)").HasColumnName("PriceAmount");
                p.Property(m => m.Currency).HasMaxLength(10).HasColumnName("PriceCurrency");
            });

            builder.Property(x => x.DurationInDays)
                   .IsRequired();

            builder.Property(x => x.FeaturesJson)
                   .HasColumnType("nvarchar(max)");
        }
    }
}
