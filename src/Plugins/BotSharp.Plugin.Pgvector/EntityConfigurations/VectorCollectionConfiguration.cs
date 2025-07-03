using BotSharp.Plugin.Pgvector.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BotSharp.Plugin.Pgvector.EntityConfigurations;

public class VectorCollectionConfiguration : IEntityTypeConfiguration<VectorCollection>
{
    public void Configure(EntityTypeBuilder<VectorCollection> builder)
    {
        // Primary key
        builder.HasKey(x => x.Name);

        // Properties
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IndexType)
            .HasMaxLength(50)
            .HasDefaultValue("hnsw");

        builder.Property(x => x.DistanceFunction)
            .HasMaxLength(50)
            .HasDefaultValue("cosine");

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(x => x.Type);
        builder.HasIndex(x => x.CreatedAt);

        // Relationships
        builder.HasMany(x => x.VectorData)
            .WithOne(x => x.Collection)
            .HasForeignKey(x => x.CollectionName)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
