using BotSharp.Plugin.Pgvector.Entities;
using BotSharp.Plugin.Pgvector.EntityConfigurations;
using Pgvector.EntityFrameworkCore;

namespace BotSharp.Plugin.Pgvector.DbContexts;

public class PgvectorDbContext : DbContext
{
    public DbSet<VectorCollection> VectorCollections { get; set; }
    public DbSet<VectorData> VectorData { get; set; }

    public PgvectorDbContext(DbContextOptions<PgvectorDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // This will only be used if the context is not configured in DI
            optionsBuilder.UseNpgsql(o => o.UseVector());
        }
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Enable vector extension
        modelBuilder.HasPostgresExtension("vector");
        

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new VectorCollectionConfiguration());
        modelBuilder.ApplyConfiguration(new VectorDataConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}
