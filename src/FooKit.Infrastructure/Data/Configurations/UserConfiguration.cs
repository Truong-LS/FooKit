using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProject.Domain.Entities;

namespace MyProject.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Username)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.PasswordHash)
                   .IsRequired(false)
                   .HasMaxLength(255);

            builder.Property(x => x.Email)
                   .IsRequired(false)
                   .HasMaxLength(256);

            builder.HasIndex(x => x.Username)
                   .IsUnique();

            builder.HasIndex(x => x.Email)
                   .IsUnique()
                   .HasFilter("[Email] IS NOT NULL");
        }
    }
}
