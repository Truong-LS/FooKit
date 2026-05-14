using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProject.Domain.Entities;

namespace MyProject.Infrastructure.Data.Configurations
{
    public class IngredientDictionaryConfiguration : IEntityTypeConfiguration<IngredientDictionary>
    {
        public void Configure(EntityTypeBuilder<IngredientDictionary> builder)
        {
            builder.ToTable("IngredientDictionaries");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RawKeywordFromApi)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.HasOne(x => x.StandardIngredient)
                   .WithMany(s => s.IngredientDictionaries)
                   .HasForeignKey(x => x.StandardIngredientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
