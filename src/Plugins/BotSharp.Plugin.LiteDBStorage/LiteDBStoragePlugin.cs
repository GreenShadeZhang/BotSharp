using BotSharp.Abstraction.Plugins.Models;
using BotSharp.Abstraction.Repositories.Enums;
using BotSharp.Abstraction.Users.Enums;
using BotSharp.Plugin.LiteDBStorage.Repository;

namespace BotSharp.Plugin.LiteDBStorage;

/// <summary>
/// MongoDB as the repository
/// </summary>
public class LiteDBStoragePlugin : IBotSharpPlugin
{
    public string Id => "9c34e593-cc6d-49f7-b3c3-399f4c7f8421";
    public string Name => "LiteDB Storage";
    public string Description => "LiteDB";
    public string IconUrl => "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRrFrT-_0VYV4PraApwSUmsf4pBGWgvLTaLZGUd7942FxjErsA5iaL4n5Q7CplOmVtwEQ&usqp=CAU";

    public void RegisterDI(IServiceCollection services, IConfiguration config)
    {
        var dbSettings = new BotSharpDatabaseSettings();
        config.Bind("Database", dbSettings);

        if (dbSettings.Default == RepositoryEnum.LiteDBRepository)
        {
            services.AddScoped((IServiceProvider x) =>
            {
                var dbSettings = x.GetRequiredService<BotSharpDatabaseSettings>();
                return new LiteDBContext(dbSettings);
            });

            services.AddScoped<IBotSharpRepository, MongoRepository>();
        }
    }

    public bool AttachMenu(List<PluginMenuDef> menu)
    {
        var section = menu.First(x => x.Label == "Apps");
        menu.Add(new PluginMenuDef("MongoDB", icon: "bx bx-data", link: "page/mongodb", weight: section.Weight + 10)
        {
            Roles = new List<string> { UserRole.Admin }
        });
        return true;
    }
}
