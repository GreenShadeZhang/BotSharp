using BotSharp.Abstraction.VectorStorage.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class KnowledgeVectorStoreConfigLiteDBModel
{
    public string Provider { get; set; }

    public static KnowledgeVectorStoreConfigLiteDBModel ToLiteDBModel(VectorStoreConfig model)
    {
        return new KnowledgeVectorStoreConfigLiteDBModel
        {
            Provider = model.Provider
        };
    }

    public static VectorStoreConfig ToDomainModel(KnowledgeVectorStoreConfigLiteDBModel model)
    {
        return new VectorStoreConfig
        {
            Provider = model.Provider
        };
    }
}
