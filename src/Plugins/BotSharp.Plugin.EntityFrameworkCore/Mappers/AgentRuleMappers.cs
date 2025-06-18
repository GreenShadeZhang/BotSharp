using BotSharp.Abstraction.Agents.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class AgentRuleMappers
{
    public static Entities.AgentRule ToEntity(this AgentRule model)
    {
        return new Entities.AgentRule
        {
            TriggerName = model.TriggerName,
            Disabled = model.Disabled,
            Criteria = model.Criteria
        };
    }

    public static AgentRule ToModel(this Entities.AgentRule entity)
    {
        return new AgentRule
        {
            TriggerName = entity.TriggerName,
            Disabled = entity.Disabled,
            Criteria = entity.Criteria
        };
    }
}
