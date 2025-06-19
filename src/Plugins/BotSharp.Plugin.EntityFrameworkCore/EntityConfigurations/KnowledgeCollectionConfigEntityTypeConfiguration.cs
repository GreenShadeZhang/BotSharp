using BotSharp.Plugin.EntityFrameworkCore.Entities;
using BotSharp.Plugin.EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace BotSharp.Plugin.EntityFrameworkCore.EntityConfigurations;

public class KnowledgeCollectionConfigEntityTypeConfiguration : IEntityTypeConfiguration<KnowledgeCollectionConfig>
{
    private readonly string _tablePrefix;

    public KnowledgeCollectionConfigEntityTypeConfiguration(string tablePrefix)
    {
        _tablePrefix = tablePrefix;
    }

    public void Configure(EntityTypeBuilder<KnowledgeCollectionConfig> builder)
    {
        builder.ToTable($"{_tablePrefix}_KnowledgeCollectionConfig");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(e => e.Name)
            .HasMaxLength(128)
            .IsRequired();
            
        builder.Property(e => e.Type)
            .HasMaxLength(64)
            .IsRequired();
            
        builder.Property(e => e.VectorStore)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<KnowledgeVectorStoreConfigElement>(v, (JsonSerializerOptions)null!) ?? new())
            .HasColumnType("TEXT");
            
        builder.Property(e => e.TextEmbedding)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<KnowledgeEmbeddingConfigElement>(v, (JsonSerializerOptions)null!) ?? new())
            .HasColumnType("TEXT");

        // Indexes
        builder.HasIndex(e => e.Name).IsUnique();
        builder.HasIndex(e => e.Type);
    }
}
