using BotSharp.Abstraction.Files;
using BotSharp.Abstraction.Messaging.JsonConverters;
using BotSharp.Abstraction.Repositories;
using BotSharp.Abstraction.Repositories.Enums;
using BotSharp.Abstraction.Repositories.Settings;
using BotSharp.Core;
using BotSharp.OpenAPI;
using BotSharp.Plugin.EntityFrameworkCore;
using BotSharp.Plugin.EntityFrameworkCore.Repository;
using BotSharp.Plugin.Pgvector.DbContexts;
using BotSharp.Plugin.PostgreSqlFileStorage.DbContexts;
using BotSharp.Plugin.PostgreSqlFileStorage.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


string[] allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[]
    {
        "http://0.0.0.0:5015",
        "https://botsharp.scisharpstack.org",
        "https://chat.scisharpstack.org"
    };
// Add BotSharp
builder.Services.AddBotSharpCore(builder.Configuration, options =>
{
    options.JsonSerializerOptions.Converters.Add(new RichContentJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new TemplateMessageJsonConverter());
}).AddBotSharpOpenAPI(builder.Configuration, allowedOrigins, builder.Environment);

var dbSettings = new BotSharpDatabaseSettings();
builder.Configuration.Bind("Database", dbSettings);

builder.Services.AddSingleton(dbSettings);

if (dbSettings.Default == RepositoryEnum.PostgreSqlRepository)
{
    builder.Services.AddDbContext<BotSharpEfCoreDbContext>(options =>
    {
        options.UseNpgsql(dbSettings.BotSharpPostgreSql, x => x.MigrationsAssembly("BotSharp.Plugin.EntityFrameworkCore.PostgreSql"));
    });

}

builder.Services.AddScoped<IBotSharpRepository, EfCoreRepository>();

var fileCoreSettings = new FileCoreSettings();
builder.Configuration.Bind("FileCore", fileCoreSettings);

if (fileCoreSettings.Storage == FileStorageEnum.PostgreSqlFileStorage)
{
    // Register the file storage specific DbContext

    var dataSource = new NpgsqlDataSourceBuilder(dbSettings.BotSharpPostgreSql).EnableDynamicJson().Build();

    builder.Services.AddDbContext<PostgreSqlFileStorageDbContext>(options => options.UseNpgsql(dataSource, x => x.MigrationsAssembly("BotSharp.Plugin.PostgreSqlFileStorage")));

    builder.Services.AddDbContext<PgvectorDbContext>(options =>
    options.UseNpgsql(dataSource, x =>
    {
        x.MigrationsAssembly("BotSharp.Plugin.Pgvector");
        x.UseVector();
    }));
    builder.Services.AddScoped<IFileStorageService, PostgreSqlFileStorageService>();
}

var app = builder.Build();

app.Run();