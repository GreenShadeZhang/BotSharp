using BotSharp.Plugin.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace BotSharp.Plugin.EntityFrameworkCore.EntityConfigurations;

public class GlobalStatEntityTypeConfiguration : IEntityTypeConfiguration<GlobalStat>
{
    private readonly string _tablePrefix;

    private JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        AllowTrailingCommas = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

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
                v => JsonSerializer.Serialize(v, _options),
                v => JsonSerializer.Deserialize<GlobalStatsCountElement>(v, _options) ?? new());

        builder.Property(e => e.LlmCost)
            .HasConversion(
                v => JsonSerializer.Serialize(v, _options),
                v => JsonSerializer.Deserialize<GlobalStatsLlmCostElement>(v, _options) ?? new());
    }
}
