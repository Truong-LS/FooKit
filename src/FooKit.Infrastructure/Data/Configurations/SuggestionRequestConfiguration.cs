using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProject.Domain.Entities;

namespace MyProject.Infrastructure.Data.Configurations
{
    public class SuggestionRequestConfiguration : IEntityTypeConfiguration<SuggestionRequest>
    {
        public void Configure(EntityTypeBuilder<SuggestionRequest> builder)
        {
            builder.ToTable("SuggestionRequests");

            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.TargetBudget, p =>
            {
                p.Property(m => m.Amount).HasColumnType("decimal(18,2)").HasColumnName("TargetBudgetAmount");
                p.Property(m => m.Currency).HasMaxLength(10).HasColumnName("TargetBudgetCurrency");
            });

            builder.Property(x => x.DietaryRequirement)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(x => x.AvailableToolsJson)
                   .HasColumnType("nvarchar(max)");

            builder.Property(x => x.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.User)
                   .WithMany(u => u.SuggestionRequests)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
