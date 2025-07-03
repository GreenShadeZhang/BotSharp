#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Pgvector;

namespace BotSharp.Plugin.Pgvector.Entities;

[Table("vector_data")]
public class VectorData
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    public string CollectionName { get; set; } = string.Empty;

    [JsonIgnore]
    public Vector Embedding { get; set; }

    [Required]
    public string Text { get; set; } = string.Empty;

    [Column(TypeName = "jsonb")]
    public string PayloadJson { get; set; } = "{}";

    [MaxLength(50)]
    public string DataSource { get; set; } = VectorDataSource.Api;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties  
    [ForeignKey("CollectionName")]
    public virtual VectorCollection? Collection { get; set; }

    // Helper property for payload access  
    [NotMapped]
    public Dictionary<string, object> Payload
    {
        get => JsonSerializer.Deserialize<Dictionary<string, object>>(PayloadJson) ?? new();
        set => PayloadJson = JsonSerializer.Serialize(value);
    }
}
