using BotSharp.Plugin.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BotSharp.Plugin.EntityFrameworkCore.EntityConfigurations;

public class AgentEntityTypeConfiguration: IEntityTypeConfiguration<Entities.Agent>
{
    private readonly string _tablePrefix;
    public AgentEntityTypeConfiguration(string tablePrefix)
    {
        _tablePrefix = tablePrefix;
    }
    public void Configure(EntityTypeBuilder<Entities.Agent> configuration)
    {
        configuration
            .ToTable($"{_tablePrefix}_Agents");

        configuration
            .HasIndex(a => a.Id);

        configuration
            .Property(a => a.Id)
            .HasMaxLength(64);
    }
}
