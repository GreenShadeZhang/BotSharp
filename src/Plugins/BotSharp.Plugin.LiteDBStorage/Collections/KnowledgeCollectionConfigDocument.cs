namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class KnowledgeCollectionConfigDocument : LiteDBBase
{
    public string Name { get; set; }
    public string Type { get; set; }
    public KnowledgeVectorStoreConfigLiteDBModel VectorStore { get; set; }
    public KnowledgeEmbeddingConfigLiteDBModel TextEmbedding { get; set; }
}
