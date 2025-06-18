using BotSharp.Abstraction.Agents.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class AgentUtilityMappers
{
    public static AgentUtilityElement ToEntity(this AgentUtility model)
    {
        return new AgentUtilityElement
        {
            Name = model.Name,
        };
    }

    public static AgentUtility ToModel(this AgentUtilityElement entity)
    {
        return new AgentUtility
        {
            Name = entity.Name,
        };
    }
}
