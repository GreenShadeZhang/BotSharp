namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class AgentRule
{
    public string TriggerName { get; set; } = default!;
    public bool Disabled { get; set; }
    public string Criteria { get; set; } = default!;
}
