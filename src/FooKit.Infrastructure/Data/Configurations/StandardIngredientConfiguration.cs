using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProject.Domain.Entities;

namespace MyProject.Infrastructure.Data.Configurations
{
    public class StandardIngredientConfiguration : IEntityTypeConfiguration<StandardIngredient>
    {
        public void Configure(EntityTypeBuilder<StandardIngredient> builder)
        {
            builder.ToTable("StandardIngredients");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.Category)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);
        }
    }
}
