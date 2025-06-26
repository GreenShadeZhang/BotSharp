using BotSharp.Plugin.PostgreSqlFileStorage.Entities;
using BotSharp.Plugin.PostgreSqlFileStorage.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace BotSharp.Plugin.PostgreSqlFileStorage.DbContexts;

public class PostgreSqlFileStorageDbContext : DbContext
{
    public DbSet<FileStorage> FileStorages { get; set; }

    public PostgreSqlFileStorageDbContext(DbContextOptions<PostgreSqlFileStorageDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FileStorageEntityTypeConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}
