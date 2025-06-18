namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class AgentKnowledgeBase
{
    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!;
    public bool Disabled { get; set; }
    public decimal? Confidence { get; set; }
}
