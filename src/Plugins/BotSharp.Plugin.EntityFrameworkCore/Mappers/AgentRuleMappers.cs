using BotSharp.Abstraction.Agents.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class AgentRuleMappers
{
    public static AgentRuleElement ToEntity(this AgentRule model)
    {
        return new AgentRuleElement
        {
            TriggerName = model.TriggerName,
            Disabled = model.Disabled,
            Criteria = model.Criteria
        };
    }

    public static AgentRule ToModel(this AgentRuleElement entity)
    {
        return new AgentRule
        {
            TriggerName = entity.TriggerName,
            Disabled = entity.Disabled,
            Criteria = entity.Criteria
        };
    }
}
