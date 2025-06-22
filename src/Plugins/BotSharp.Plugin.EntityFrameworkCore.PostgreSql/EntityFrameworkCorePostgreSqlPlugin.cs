using BotSharp.Abstraction.Plugins;
using BotSharp.Abstraction.Plugins.Models;
using BotSharp.Abstraction.Repositories;
using BotSharp.Abstraction.Repositories.Enums;
using BotSharp.Abstraction.Repositories.Settings;
using BotSharp.Abstraction.Users.Enums;
using BotSharp.Plugin.EntityFrameworkCore.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

namespace BotSharp.Plugin.EntityFrameworkCore.PostgreSql;

public class EntityFrameworkCorePostgreSqlPlugin : IBotSharpPlugin
{
    public string Id => "a1df76ef-20dc-403e-b375-7f8f5d42250d";
    public string Name => "PostgreSql Storage";
    public string Description => "PostgreSql as the repository, PostgreSQL: The World's Most Advanced Open Source Relational Database.";
    public string IconUrl => "https://cdn-icons-png.flaticon.com/128/10464/10464288.png";

    public void RegisterDI(IServiceCollection services, IConfiguration config)
    {
        var dbSettings = new BotSharpDatabaseSettings();
        config.Bind("Database", dbSettings);

        if (dbSettings.Default == RepositoryEnum.PostgreSqlRepository)
        {
            services.AddSingleton(dbSettings);

            var dataSource = new NpgsqlDataSourceBuilder(dbSettings.BotSharpPostgreSql).EnableDynamicJson().Build();

            services.AddDbContext<BotSharpEfCoreDbContext>(options => options.UseNpgsql(dataSource, x => x.MigrationsAssembly("BotSharp.Plugin.EntityFrameworkCore.PostgreSql")));

            services.AddScoped<IBotSharpRepository, EfCoreRepository>();
        }
    }
}
