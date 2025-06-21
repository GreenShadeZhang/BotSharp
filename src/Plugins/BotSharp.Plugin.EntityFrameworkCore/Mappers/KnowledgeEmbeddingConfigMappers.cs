using BotSharp.Abstraction.VectorStorage.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class KnowledgeEmbeddingConfigMappers
{
    public static KnowledgeEmbeddingConfigElement ToEntity(this KnowledgeEmbeddingConfig model)
    {
        return new KnowledgeEmbeddingConfigElement
        {
            Provider = model.Provider,
            Model = model.Model,
            Dimension = model.Dimension
        };
    }

    public static KnowledgeEmbeddingConfig ToModel(this KnowledgeEmbeddingConfigElement model)
    {
        return new KnowledgeEmbeddingConfig
        {
            Provider = model.Provider,
            Model = model.Model,
            Dimension = model.Dimension
        };
    }
}
