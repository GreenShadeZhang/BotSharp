using BotSharp.Abstraction.Agents.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class AgentUtilityMappers
{
    public static Entities.AgentUtility ToEntity(this AgentUtility model)
    {
        return new Entities.AgentUtility
        {
            Name = model.Name,
        };
    }

    public static AgentUtility ToModel(this Entities.AgentUtility entity)
    {
        return new AgentUtility
        {
            Name = entity.Name,
        };
    }
}
