using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotSharp.Plugin.PostgreSqlFileStorage.Entities;

/// <summary>
/// File storage entity for PostgreSQL
/// </summary>
[Table("file_storages")]
public class FileStorage
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// File path (e.g., conversations/123/files/456/user/1/test.pdf)
    /// </summary>
    [Column("file_path")]
    [StringLength(1000)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Original file name
    /// </summary>
    [Column("file_name")]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File content type (MIME type)
    /// </summary>
    [Column("content_type")]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    [Column("file_size")]
    public long FileSize { get; set; }

    /// <summary>
    /// File binary data
    /// </summary>
    [Column("file_data")]
    public byte[] FileData { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// File category (conversation, user, knowledge, etc.)
    /// </summary>
    [Column("category")]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Related entity ID (conversation ID, user ID, etc.)
    /// </summary>
    [Column("entity_id")]
    [StringLength(50)]
    public string? EntityId { get; set; }

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    [Column("metadata", TypeName = "jsonb")]
    public string? Metadata { get; set; }

    /// <summary>
    /// Created timestamp
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last updated timestamp
    /// </summary>
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Directory path for logical grouping
    /// </summary>
    [Column("directory")]
    [StringLength(500)]
    public string Directory { get; set; } = string.Empty;
}
