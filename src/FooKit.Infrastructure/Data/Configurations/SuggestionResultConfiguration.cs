using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FooKit.Domain.Entities;

namespace FooKit.Infrastructure.Data.Configurations
{
    public class SuggestionResultConfiguration : IEntityTypeConfiguration<SuggestionResult>
    {
        public void Configure(EntityTypeBuilder<SuggestionResult> builder)
        {
            builder.ToTable("SuggestionResults");

            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.TotalEstimatedPrice, p =>
            {
                p.Property(m => m.Amount).HasColumnType("decimal(18,2)").HasColumnName("TotalEstimatedPriceAmount");
                p.Property(m => m.Currency).HasMaxLength(10).HasColumnName("TotalEstimatedPriceCurrency");
            });

            builder.Property(x => x.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.SuggestionRequest)
                   .WithMany(r => r.SuggestionResults)
                   .HasForeignKey(x => x.SuggestionRequestId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.DishCache)
                   .WithMany(d => d.SuggestionResults)
                   .HasForeignKey(x => x.DishCacheId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
