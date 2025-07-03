using BotSharp.Plugin.Pgvector.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BotSharp.Plugin.Pgvector.EntityConfigurations;

public class VectorDataConfiguration : IEntityTypeConfiguration<VectorData>
{
    public void Configure(EntityTypeBuilder<VectorData> builder)
    {
        // Primary key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(x => x.CollectionName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Text)
            .IsRequired();

        builder.Property(x => x.PayloadJson)
            .HasColumnType("jsonb")
            .HasDefaultValue("{}");

        builder.Property(x => x.DataSource)
            .HasMaxLength(50)
            .HasDefaultValue(VectorDataSource.Api);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Vector property configuration
        builder.Property(x => x.Embedding)
            .HasColumnType("vector(384)");

        // Indexes
        builder.HasIndex(x => x.CollectionName);
        builder.HasIndex(x => x.DataSource);
        builder.HasIndex(x => x.CreatedAt);

        // Vector indexes will be created by the service based on collection settings
        // This is because pgvector indexes require specific parameters per collection

        // GIN index for JSONB payload
        builder.HasIndex(x => x.PayloadJson)
            .HasMethod("gin");

        // Relationships
        builder.HasOne(x => x.Collection)
            .WithMany(x => x.VectorData)
            .HasForeignKey(x => x.CollectionName)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
