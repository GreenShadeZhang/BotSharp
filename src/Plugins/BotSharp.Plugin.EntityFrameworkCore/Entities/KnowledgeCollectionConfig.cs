using System.ComponentModel.DataAnnotations;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class KnowledgeCollectionConfig
{
    [Key]
    public string Id { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public string Type { get; set; } = string.Empty;
    
    public KnowledgeVectorStoreConfigElement VectorStore { get; set; } = new();
    
    public KnowledgeEmbeddingConfigElement TextEmbedding { get; set; } = new();
}
