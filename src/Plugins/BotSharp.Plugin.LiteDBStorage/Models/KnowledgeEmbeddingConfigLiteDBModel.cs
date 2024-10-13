using BotSharp.Abstraction.VectorStorage.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class KnowledgeEmbeddingConfigLiteDBModel
{
    public string Provider { get; set; }
    public string Model { get; set; }
    public int Dimension { get; set; }

    public static KnowledgeEmbeddingConfigLiteDBModel ToMongoModel(KnowledgeEmbeddingConfig model)
    {
        return new KnowledgeEmbeddingConfigLiteDBModel
        {
            Provider = model.Provider,
            Model = model.Model,
            Dimension = model.Dimension
        };
    }

    public static KnowledgeEmbeddingConfig ToDomainModel(KnowledgeEmbeddingConfigLiteDBModel model)
    {
        return new KnowledgeEmbeddingConfig
        {
            Provider = model.Provider,
            Model = model.Model,
            Dimension = model.Dimension
        };
    }
}
