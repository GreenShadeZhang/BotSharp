using BotSharp.Abstraction.VectorStorage.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class KnowledgeVectorStoreConfigMappers
{
    public static KnowledgeVectorStoreConfigElement ToEntity(this VectorStoreConfig model)
    {
        return new KnowledgeVectorStoreConfigElement
        {
            Provider = model.Provider
        };
    }

    public static VectorStoreConfig ToModel(this KnowledgeVectorStoreConfigElement model)
    {
        return new VectorStoreConfig
        {
            Provider = model.Provider
        };
    }
}
