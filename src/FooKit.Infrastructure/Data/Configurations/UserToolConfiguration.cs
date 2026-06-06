using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FooKit.Domain.Entities;

namespace FooKit.Infrastructure.Data.Configurations
{
    public class UserToolConfiguration : IEntityTypeConfiguration<UserTool>
    {
        public void Configure(EntityTypeBuilder<UserTool> builder)
        {
            builder.ToTable("UserTools");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.User)
                   .WithMany(u => u.Tools)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ToolName)
                   .IsRequired()
                   .HasMaxLength(100);
        }
    }
}
