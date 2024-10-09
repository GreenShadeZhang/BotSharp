namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class KnowledgeCollectionConfigDocument : LiteDBBase
{
    public string Name { get; set; }
    public string Type { get; set; }
    public KnowledgeVectorStoreConfigMongoModel VectorStore { get; set; }
    public KnowledgeEmbeddingConfigMongoModel TextEmbedding { get; set; }
}
