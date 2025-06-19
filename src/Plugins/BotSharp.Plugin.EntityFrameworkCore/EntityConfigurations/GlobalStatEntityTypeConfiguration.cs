using BotSharp.Plugin.EntityFrameworkCore.Entities;
using BotSharp.Plugin.EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace BotSharp.Plugin.EntityFrameworkCore.EntityConfigurations;

public class GlobalStatEntityTypeConfiguration : IEntityTypeConfiguration<GlobalStat>
{
    private readonly string _tablePrefix;

    public GlobalStatEntityTypeConfiguration(string tablePrefix)
    {
        _tablePrefix = tablePrefix;
    }

    public void Configure(EntityTypeBuilder<GlobalStat> builder)
    {
        builder.ToTable($"{_tablePrefix}_GlobalStat");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(e => e.AgentId)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(e => e.Count)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<GlobalStatsCountElement>(v, (JsonSerializerOptions)null!) ?? new())
            .HasColumnType("TEXT");
            
        builder.Property(e => e.LlmCost)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<GlobalStatsLlmCostElement>(v, (JsonSerializerOptions)null!) ?? new())
            .HasColumnType("TEXT");
            
        builder.Property(e => e.Interval)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(e => e.RecordTime)
            .IsRequired();
            
        builder.Property(e => e.StartTime)
            .IsRequired();
            
        builder.Property(e => e.EndTime)
            .IsRequired();

        // Indexes
        builder.HasIndex(e => e.AgentId);
        builder.HasIndex(e => new { e.AgentId, e.StartTime, e.EndTime }).IsUnique();
        builder.HasIndex(e => e.RecordTime);
    }
}
