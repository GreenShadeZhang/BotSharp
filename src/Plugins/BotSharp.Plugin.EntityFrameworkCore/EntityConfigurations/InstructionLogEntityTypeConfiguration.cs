using BotSharp.Plugin.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BotSharp.Plugin.EntityFrameworkCore.EntityConfigurations;

public class InstructionLogEntityTypeConfiguration : IEntityTypeConfiguration<InstructionLog>
{
    private readonly string _tablePrefix;

    public InstructionLogEntityTypeConfiguration(string tablePrefix)
    {
        _tablePrefix = tablePrefix;
    }

    public void Configure(EntityTypeBuilder<InstructionLog> builder)
    {
        builder.ToTable($"{_tablePrefix}_InstructionLog");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(e => e.AgentId)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(e => e.CreatedTime)
            .IsRequired();

        // Indexes
        builder.HasIndex(e => e.AgentId);
        builder.HasIndex(e => e.CreatedTime);
    }
}
