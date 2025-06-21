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
    }
}
