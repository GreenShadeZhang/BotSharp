using BotSharp.Abstraction.Agents.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class AgentRuleLiteDBElement
{
    public string TriggerName { get; set; }
    public bool Disabled { get; set; }
    public string Criteria { get; set; }

    public static AgentRuleLiteDBElement ToLiteDBElement(AgentRule rule)
    {
        return new AgentRuleLiteDBElement
        {
            TriggerName = rule.TriggerName,
            Disabled = rule.Disabled,
            Criteria = rule.Criteria
        };
    }

    public static AgentRule ToDomainElement(AgentRuleLiteDBElement rule)
    {
        return new AgentRule
        {
            TriggerName = rule.TriggerName,
            Disabled = rule.Disabled,
            Criteria = rule.Criteria
        };
    }
}
