using BotSharp.Abstraction.Plugins.Models;

namespace BotSharp.Plugin.LiteDBStorage.Repository;

public partial class MongoRepository
{
    #region Plugin
    public PluginConfig GetPluginConfig()
    {
        var config = new PluginConfig();
        var found = _dc.Plugins.Query().FirstOrDefault();
        if (found != null)
        {
            config = new PluginConfig()
            {
                EnabledPlugins = found.EnabledPlugins
            };
        }
        return config;
    }

    public void SavePluginConfig(PluginConfig config)
    {
        if (config == null || config.EnabledPlugins == null) return;

        var plugin = _dc.Plugins.Query().FirstOrDefault();
        if (plugin != null)
        {
            plugin.EnabledPlugins = config.EnabledPlugins;
            _dc.Plugins.Update(plugin);
            return;
        }
        else
        {
            _dc.Plugins.Insert(new PluginDocument
            {
                Id = Guid.NewGuid().ToString(),
                EnabledPlugins = config.EnabledPlugins
            });
        }
    }
    #endregion
}
