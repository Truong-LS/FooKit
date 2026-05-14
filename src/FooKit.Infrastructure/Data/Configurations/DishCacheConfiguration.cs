using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProject.Domain.Entities;

namespace MyProject.Infrastructure.Data.Configurations
{
    public class DishCacheConfiguration : IEntityTypeConfiguration<DishCache>
    {
        public void Configure(EntityTypeBuilder<DishCache> builder)
        {
            builder.ToTable("DishCaches");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ExternalApiId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.ImageUrl)
                   .HasMaxLength(1000);

            builder.Property(x => x.DietaryTagsJson)
                   .HasColumnType("nvarchar(max)");

            builder.Property(x => x.RequiredToolsJson)
                   .HasColumnType("nvarchar(max)");

            builder.Property(x => x.RawIngredientsJson)
                   .HasColumnType("nvarchar(max)");

            builder.Property(x => x.LastFetchedAt)
                   .IsRequired();
        }
    }
}
