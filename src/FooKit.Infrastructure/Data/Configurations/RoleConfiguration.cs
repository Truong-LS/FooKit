using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FooKit.Domain.Entities;

namespace FooKit.Infrastructure.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(x => x.Name)
                   .IsUnique();

            builder.Property(x => x.Description)
                   .HasMaxLength(250);

            // Seed default roles using deterministic GUIDs
            builder.HasData(
                new Role 
                { 
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), 
                    Name = "Admin", 
                    Description = "System Administrator role" 
                },
                new Role 
                { 
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), 
                    Name = "User", 
                    Description = "Standard user role" 
                }
            );
        }
    }
}
