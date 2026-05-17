using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProject.Domain.Entities;

namespace MyProject.Infrastructure.Data.Configurations
{
    public class AffiliateProductConfiguration : IEntityTypeConfiguration<AffiliateProduct>
    {
        public void Configure(EntityTypeBuilder<AffiliateProduct> builder)
        {
            builder.ToTable("AffiliateProducts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductName)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.ProductUrl)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(x => x.Platform)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.OwnsOne(x => x.CurrentPrice, p =>
            {
                p.Property(m => m.Amount).HasColumnType("decimal(18,2)").HasColumnName("CurrentPriceAmount");
                p.Property(m => m.Currency).HasMaxLength(10).HasColumnName("CurrentPriceCurrency");
            });

            builder.Property(x => x.LastUpdatedPriceAt)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.HasIndex(x => new { x.StandardIngredientId, x.IsActive });

            builder.HasOne(x => x.StandardIngredient)
                   .WithMany(s => s.AffiliateProducts)
                   .HasForeignKey(x => x.StandardIngredientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
