using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FooKit.Domain.Entities;
using FooKit.Domain.Enums;

namespace FooKit.Infrastructure.Data.Configurations
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

            var converter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<IngredientCategory, string>(
                v => ConvertCategoryToString(v),
                v => ParseIngredientCategory(v)
            );

            builder.Property(x => x.Category)
                   .IsRequired()
                   .HasConversion(converter)
                   .HasMaxLength(50);

            builder.Property(x => x.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }

        private static string ConvertCategoryToString(IngredientCategory category)
        {
            return category switch
            {
                IngredientCategory.DairyAndOther => "Bơ sữa & Khác",
                IngredientCategory.Starch => "Tinh bột",
                IngredientCategory.Spice => "Gia vị",
                IngredientCategory.VegetablesAndFruits => "Rau củ quả",
                IngredientCategory.MeatAndSeafood => "Thịt & Hải sản",
                _ => "Bơ sữa & Khác"
            };  
        }

        private static IngredientCategory ParseIngredientCategory(string value)
        {
            if (string.IsNullOrEmpty(value)) return IngredientCategory.DairyAndOther;
            
            var lower = value.ToLowerInvariant().Trim();
            if (lower == "thịt & hải sản") return IngredientCategory.MeatAndSeafood;
            if (lower == "tinh bột") return IngredientCategory.Starch;
            if (lower == "gia vị") return IngredientCategory.Spice;
            if (lower == "rau củ quả") return IngredientCategory.VegetablesAndFruits;
            
            return IngredientCategory.DairyAndOther;
        }
    }
}
