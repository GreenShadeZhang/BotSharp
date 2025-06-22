namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class AgentKnowledgeBaseElement
{
    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!;
    public bool Disabled { get; set; }
    public decimal? Confidence { get; set; }
}
