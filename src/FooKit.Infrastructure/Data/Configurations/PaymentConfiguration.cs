using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FooKit.Domain.Entities;
using FooKit.Domain.Enums;

namespace FooKit.Infrastructure.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderCode)
                   .IsRequired();

            builder.HasIndex(x => x.OrderCode)
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

            builder.Property(x => x.PaymentLinkId)
                   .HasMaxLength(100);

            builder.Property(x => x.PayOsTransactionRef)
                   .HasMaxLength(100);

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
