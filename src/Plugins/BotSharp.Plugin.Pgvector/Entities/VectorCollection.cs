using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace BotSharp.Plugin.Pgvector.Entities;

[Table("vector_collections")]
public class VectorCollection
{
    [Key]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public int Dimension { get; set; }

    [MaxLength(100)]
    public string Type { get; set; } = string.Empty;

    [MaxLength(50)]
    public string IndexType { get; set; } = "hnsw";

    [MaxLength(50)]
    public string DistanceFunction { get; set; } = "cosine";

    public bool IsIndexed { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<VectorData> VectorData { get; set; } = new List<VectorData>();
}
