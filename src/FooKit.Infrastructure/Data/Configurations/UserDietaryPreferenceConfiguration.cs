using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FooKit.Domain.Entities;

namespace FooKit.Infrastructure.Data.Configurations
{
    public class UserDietaryPreferenceConfiguration : IEntityTypeConfiguration<UserDietaryPreference>
    {
        public void Configure(EntityTypeBuilder<UserDietaryPreference> builder)
        {
            builder.ToTable("UserDietaryPreferences");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.User)
                   .WithMany(u => u.DietaryPreferences)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.DietaryType)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);
        }
    }
}
