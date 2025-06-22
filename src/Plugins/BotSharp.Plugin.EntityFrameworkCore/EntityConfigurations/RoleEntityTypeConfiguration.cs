using BotSharp.Plugin.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BotSharp.Plugin.EntityFrameworkCore.EntityConfigurations;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    private readonly string _tablePrefix;

    public RoleEntityTypeConfiguration(string tablePrefix)
    {
        _tablePrefix = tablePrefix;
    }

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable($"{_tablePrefix}_Role");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(e => e.Name)
            .HasMaxLength(128)
            .IsRequired();
            
        builder.Property(e => e.Permissions)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2048);
            
        builder.Property(e => e.CreatedTime)
            .IsRequired();
            
        builder.Property(e => e.UpdatedTime)
            .IsRequired();

        // Indexes
        builder.HasIndex(e => e.Name);
    }
}
