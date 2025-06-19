using System.ComponentModel.DataAnnotations;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class KnowledgeDocument
{
    [Key]
    public string Id { get; set; } = string.Empty;
    
    public string Collection { get; set; } = string.Empty;
    
    public Guid FileId { get; set; }
    
    public string FileName { get; set; } = string.Empty;
    
    public string FileSource { get; set; } = string.Empty;
    
    public string ContentType { get; set; } = string.Empty;
    
    public string VectorStoreProvider { get; set; } = string.Empty;
    
    public List<string> VectorDataIds { get; set; } = new();
    
    public DateTime CreateDate { get; set; }
    
    public string CreateUserId { get; set; } = string.Empty;
}
