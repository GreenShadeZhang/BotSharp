using BotSharp.Abstraction.Agents.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class AgentResponseMappers
{
    public static AgentResponseElement ToEntity(this AgentResponse model)
    {
        return new AgentResponseElement
        {
            Prefix = model.Prefix,
            Intent = model.Intent,
            Content = model.Content
        };
    }

    public static AgentResponse ToModel(this AgentResponseElement model)
    {
        return new AgentResponse
        {
            Prefix = model.Prefix,
            Intent = model.Intent,
            Content = model.Content
        };
    }
}
