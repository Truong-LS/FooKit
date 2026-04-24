using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProject.Domain.Entities;

namespace MyProject.Infrastructure.Data.Configurations
{
    public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
    {
        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTable("UserLogins");

            builder.HasKey(x => new { x.LoginProvider, x.ProviderKey });

            builder.Property(x => x.LoginProvider)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.Property(x => x.ProviderKey)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.Property(x => x.ProviderDisplayName)
                   .IsRequired(false)
                   .HasMaxLength(128);

            builder.HasOne(x => x.User)
                   .WithMany(u => u.UserLogins)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.UserId);
        }
    }
}
