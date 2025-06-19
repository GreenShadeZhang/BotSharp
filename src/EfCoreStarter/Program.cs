using BotSharp.Abstraction.Repositories;
using BotSharp.Abstraction.Repositories.Enums;
using BotSharp.Abstraction.Repositories.Settings;
using BotSharp.Plugin.EntityFrameworkCore;
using BotSharp.Plugin.EntityFrameworkCore.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

var app = builder.Build();

app.Run();