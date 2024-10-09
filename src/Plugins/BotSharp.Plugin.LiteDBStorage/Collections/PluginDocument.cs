namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class PluginDocument : LiteDBBase
{
    public List<string> EnabledPlugins { get; set; }
}
