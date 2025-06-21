using System.ComponentModel.DataAnnotations.Schema;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public sealed class KnowledgeCollectionFileMeta
{
    public string Id { get; set; } = string.Empty;
    public string Collection { get; set; } = null!;
    public Guid FileId { get; set; }
    public string FileName { get; set; } = null!;
    public string FileSource { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string VectorStoreProvider { get; set; } = null!;

    [Column(TypeName = "json")]
    public IEnumerable<string> VectorDataIds { get; set; } = [];

    [Column(TypeName = "json")]
    public KnowledgeFileMetaRefElement? RefData { get; set; }
    public DateTime CreatedTime { get; set; }
    public string CreateUserId { get; set; } = null!;
}
