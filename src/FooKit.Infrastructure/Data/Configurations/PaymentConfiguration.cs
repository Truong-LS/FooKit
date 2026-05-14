using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProject.Domain.Entities;
using MyProject.Domain.Enums;

namespace MyProject.Infrastructure.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TransactionRef)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(x => x.TransactionRef)
                   .IsUnique();

            builder.Property(x => x.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,0)");

            builder.Property(x => x.OrderInfo)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasDefaultValue(PaymentStatus.Pending);

            builder.Property(x => x.VnPayTransactionNo)
                   .HasMaxLength(50);

            builder.Property(x => x.VnPayResponseCode)
                   .HasMaxLength(10);

            builder.Property(x => x.BankCode)
                   .HasMaxLength(20);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.SubscriptionPlan)
                   .WithMany()
                   .HasForeignKey(x => x.SubscriptionPlanId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
