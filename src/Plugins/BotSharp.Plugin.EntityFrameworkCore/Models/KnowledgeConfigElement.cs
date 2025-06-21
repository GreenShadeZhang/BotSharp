using Microsoft.EntityFrameworkCore;

namespace BotSharp.Plugin.EntityFrameworkCore.Models;

[Owned]
public class KnowledgeVectorStoreConfigElement
{
    public string Provider { get; set; } = string.Empty;
}

[Owned]

public class KnowledgeEmbeddingConfigElement
{
    public string Provider { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Dimension { get; set; }
}
