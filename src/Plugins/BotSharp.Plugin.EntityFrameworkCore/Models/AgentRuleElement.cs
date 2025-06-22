namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class AgentRuleElement
{
    public string TriggerName { get; set; } = default!;
    public bool Disabled { get; set; }
    public string Criteria { get; set; } = default!;
}
