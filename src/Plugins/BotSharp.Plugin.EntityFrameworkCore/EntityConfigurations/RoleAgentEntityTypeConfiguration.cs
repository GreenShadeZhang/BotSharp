using BotSharp.Plugin.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BotSharp.Plugin.EntityFrameworkCore.EntityConfigurations;

public class RoleAgentEntityTypeConfiguration : IEntityTypeConfiguration<RoleAgent>
{
    private readonly string _tablePrefix;

    public RoleAgentEntityTypeConfiguration(string tablePrefix)
    {
        _tablePrefix = tablePrefix;
    }

    public void Configure(EntityTypeBuilder<RoleAgent> builder)
    {
        builder.ToTable($"{_tablePrefix}_RoleAgent");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(e => e.RoleId)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(e => e.AgentId)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(e => e.CreatedTime)
            .IsRequired();
            
        builder.Property(e => e.UpdatedTime)
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.Role)
            .WithMany(r => r.RoleAgents)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Agent)
            .WithMany()
            .HasForeignKey(e => e.AgentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.RoleId);
        builder.HasIndex(e => e.AgentId);
        builder.HasIndex(e => new { e.RoleId, e.AgentId }).IsUnique();
    }
}
