using BotSharp.Abstraction.Settings;
using BotSharp.Plugin.Pgvector.DbContexts;
using BotSharp.Plugin.Pgvector.Services;
using BotSharp.Plugin.Pgvector.Settings;
using Npgsql;
using Pgvector.EntityFrameworkCore;

namespace BotSharp.Plugin.Pgvector;

public class PgvectorPlugin : IBotSharpPlugin
{
    public string Id => "a1b2c3d4-e5f6-7890-abcd-ef1234567890";

    public string Name => "Pgvector";

    public string Description => "PostgreSQL with pgvector extension for vector storage and similarity search in BotSharp.";

    public string IconUrl => "https://cdn-icons-png.flaticon.com/128/5968/5968342.png";

    public void RegisterDI(IServiceCollection services, IConfiguration config)
    {
        // Register settings
        services.AddScoped(provider =>
        {
            var settingService = provider.GetRequiredService<ISettingService>();
            return settingService.Bind<PgvectorSettings>("Pgvector");
        });

        // Get database settings
        var dbSettings = new BotSharpDatabaseSettings();
        config.Bind("Database", dbSettings);

        // Register the vector storage specific DbContext with proper pgvector configuration
        services.AddDbContext<PgvectorDbContext>(options =>
        {
            options.UseNpgsql(dbSettings.BotSharpPostgreSql, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly("BotSharp.Plugin.Pgvector");
                npgsqlOptions.UseVector();
            });
        });

        // Register the vector database service
        services.AddScoped<IVectorDb, PgvectorDb>();
    }
}
