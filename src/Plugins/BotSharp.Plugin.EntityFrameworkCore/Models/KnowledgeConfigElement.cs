namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class KnowledgeVectorStoreConfigElement
{
    public string Provider { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Dimension { get; set; }
}

public class KnowledgeEmbeddingConfigElement
{
    public string Provider { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Dimension { get; set; }
}
