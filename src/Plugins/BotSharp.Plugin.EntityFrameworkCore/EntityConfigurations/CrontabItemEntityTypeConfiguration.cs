using BotSharp.Plugin.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace BotSharp.Plugin.EntityFrameworkCore.EntityConfigurations;

public class CrontabItemEntityTypeConfiguration : IEntityTypeConfiguration<CrontabItem>
{
    private readonly string _tablePrefix;

    public CrontabItemEntityTypeConfiguration(string tablePrefix)
    {
        _tablePrefix = tablePrefix;
    }

    public void Configure(EntityTypeBuilder<CrontabItem> builder)
    {
        builder.ToTable($"{_tablePrefix}_CrontabItem");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(e => e.ConversationId)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(e => e.Title)
            .HasMaxLength(512);
            
        builder.Property(e => e.Description)
            .HasMaxLength(2048);
            
            
        builder.Property(e => e.AgentId)
            .HasMaxLength(36);
            
        builder.Property(e => e.UserId)
            .HasMaxLength(36);
            
            
        builder.Property(e => e.CreatedTime)
            .IsRequired();
            
        // Indexes
        builder.HasIndex(e => e.ConversationId).IsUnique();
        builder.HasIndex(e => e.AgentId);
        builder.HasIndex(e => e.UserId);
    }
}
