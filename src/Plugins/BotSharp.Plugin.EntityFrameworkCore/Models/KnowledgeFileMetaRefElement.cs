namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class KnowledgeFileMetaRefElement
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Url { get; set; } = null!;
    public IDictionary<string, string>? Data { get; set; }
}
