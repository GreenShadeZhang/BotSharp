using BotSharp.Plugin.PostgreSqlFileStorage.DbContexts;
using BotSharp.Plugin.PostgreSqlFileStorage.Services;
using Npgsql;

namespace BotSharp.Plugin.PostgreSqlFileStorage;

public class PostgreSqlFileStoragePlugin : IBotSharpPlugin
{
    public string Id => "f8e9d7c6-b5a4-3210-9876-543210fedcba";

    public string Name => "PostgreSQL File Storage";

    public string Description => "PostgreSQL-based file storage service for BotSharp. Suitable for testing environments to simplify deployment.";

    public string IconUrl => "https://cdn-icons-png.flaticon.com/128/10464/10464288.png";

    public void RegisterDI(IServiceCollection services, IConfiguration config)
    {
        var fileCoreSettings = new FileCoreSettings();
        config.Bind("FileCore", fileCoreSettings);

        if (fileCoreSettings.Storage == FileStorageEnum.PostgreSqlFileStorage)
        {
            var dbSettings = new BotSharpDatabaseSettings();
            config.Bind("Database", dbSettings);

            // Register the file storage specific DbContext

            var dataSource = new NpgsqlDataSourceBuilder(dbSettings.BotSharpPostgreSql).EnableDynamicJson().Build();

            services.AddDbContext<PostgreSqlFileStorageDbContext>(options => options.UseNpgsql(dataSource, x => x.MigrationsAssembly("BotSharp.Plugin.PostgreSqlFileStorage")));

            services.AddScoped<IFileStorageService, PostgreSqlFileStorageService>();
        }
    }
}
