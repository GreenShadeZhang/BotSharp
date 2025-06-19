using BotSharp.Plugin.EntityFrameworkCore.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class KnowledgeCollectionConfig
{
    [Key]
    public string Id { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public string Type { get; set; } = string.Empty;

    [Column(TypeName = "json")]

    public KnowledgeVectorStoreConfigElement VectorStore { get; set; } = new();

    [Column(TypeName = "json")]

    public KnowledgeEmbeddingConfigElement TextEmbedding { get; set; } = new();
}
