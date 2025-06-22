using BotSharp.Plugin.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BotSharp.Plugin.EntityFrameworkCore.EntityConfigurations;

class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    private readonly string _tablePrefix;
    public UserEntityTypeConfiguration(string tablePrefix)
    {
        _tablePrefix = tablePrefix;
    }
    
    public void Configure(EntityTypeBuilder<User> configuration)
    {
        configuration.ToTable($"{_tablePrefix}_Users");

        configuration.HasKey(u => u.Id);

        configuration.Property(u => u.Id)
            .HasMaxLength(36)
            .IsRequired();

        configuration.Property(u => u.UserName)
            .HasMaxLength(100)
            .IsRequired();

        configuration.Property(u => u.FirstName)
            .HasMaxLength(64)
            .IsRequired();

        configuration.Property(u => u.LastName)
            .HasMaxLength(64);

        configuration.Property(u => u.Email)
            .HasMaxLength(100);

        configuration.Property(u => u.Phone)
            .HasMaxLength(20);

        configuration.Property(u => u.Source)
            .HasMaxLength(64)
            .IsRequired();

        configuration.Property(u => u.ExternalId)
            .HasMaxLength(100);

        configuration.Property(u => u.Type)
            .HasMaxLength(64)
            .IsRequired();

        configuration.Property(u => u.Role)
            .HasMaxLength(64)
            .IsRequired();

        configuration.Property(u => u.VerificationCode)
            .HasMaxLength(64);

        configuration.Property(u => u.RegionCode)
            .HasMaxLength(64);

        configuration.Property(u => u.AffiliateId)
            .HasMaxLength(512);

        configuration.Property(u => u.EmployeeId)
            .HasMaxLength(512);

        configuration.Property(u => u.Permissions)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(2048);

        // Indexes
        configuration.HasIndex(u => u.UserName);
        configuration.HasIndex(u => u.Email);
        configuration.HasIndex(u => u.Phone);
        configuration.HasIndex(u => u.ExternalId);
        configuration.HasIndex(u => u.CreatedTime);
    }
}
