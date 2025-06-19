using BotSharp.Plugin.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BotSharp.Plugin.EntityFrameworkCore.EntityConfigurations;

public class KnowledgeDocumentEntityTypeConfiguration : IEntityTypeConfiguration<KnowledgeDocument>
{
    private readonly string _tablePrefix;

    public KnowledgeDocumentEntityTypeConfiguration(string tablePrefix)
    {
        _tablePrefix = tablePrefix;
    }

    public void Configure(EntityTypeBuilder<KnowledgeDocument> builder)
    {
        builder.ToTable($"{_tablePrefix}_KnowledgeDocument");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(e => e.Collection)
            .HasMaxLength(128)
            .IsRequired();
            
        builder.Property(e => e.FileId)
            .IsRequired();
            
        builder.Property(e => e.FileName)
            .HasMaxLength(512)
            .IsRequired();
            
        builder.Property(e => e.FileSource)
            .HasMaxLength(256);
            
        builder.Property(e => e.ContentType)
            .HasMaxLength(128);
            
        builder.Property(e => e.VectorStoreProvider)
            .HasMaxLength(64)
            .IsRequired();
            
        builder.Property(e => e.VectorDataIds)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasColumnType("TEXT");
            
        builder.Property(e => e.CreateDate)
            .IsRequired();
            
        builder.Property(e => e.CreateUserId)
            .HasMaxLength(36);

        // Indexes
        builder.HasIndex(e => e.Collection);
        builder.HasIndex(e => e.FileId);
        builder.HasIndex(e => e.VectorStoreProvider);
        builder.HasIndex(e => new { e.Collection, e.VectorStoreProvider, e.FileId }).IsUnique();
        builder.HasIndex(e => e.CreateDate);
    }
}
