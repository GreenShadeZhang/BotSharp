using BotSharp.Plugin.PostgreSqlFileStorage.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BotSharp.Plugin.PostgreSqlFileStorage.EntityConfigurations;

public class FileStorageEntityTypeConfiguration : IEntityTypeConfiguration<FileStorage>
{
    public void Configure(EntityTypeBuilder<FileStorage> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasIndex(e => e.FilePath)
            .IsUnique();
            
        builder.HasIndex(e => e.Category);
        
        builder.HasIndex(e => e.EntityId);
        
        builder.HasIndex(e => e.Directory);
        
        builder.HasIndex(e => e.CreatedAt);
        
        builder.Property(e => e.CreatedAt)
            .IsRequired();
            
        builder.Property(e => e.UpdatedAt)
            .IsRequired();
            
        builder.Property(e => e.FilePath)
            .IsRequired();
            
        builder.Property(e => e.FileName)
            .IsRequired();
            
        builder.Property(e => e.ContentType)
            .IsRequired();
            
        builder.Property(e => e.Category)
            .IsRequired();
            
        builder.Property(e => e.Directory)
            .IsRequired();
    }
}
