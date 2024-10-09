using LiteDB;

namespace BotSharp.Plugin.LiteDBStorage;

public abstract class LiteDBBase
{
    [BsonId]
    public string Id { get; set; }
}


