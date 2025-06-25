using BotSharp.Abstraction.Agents.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class AgentLlmConfigMappers
{
    public static AgentLlmConfigElement ToEntity(this AgentLlmConfig model)
    {
        return new AgentLlmConfigElement
        {
            Provider = model.Provider,
            Model = model.Model,
            IsInherit = model.IsInherit,
            MaxRecursionDepth = model.MaxRecursionDepth,
            MaxOutputTokens = model.MaxOutputTokens,
        };
    }

    public static AgentLlmConfig? ToModel(this AgentLlmConfigElement? model)
    {
        if(model == null)
        {
            return null;
        }
        return new AgentLlmConfig
        {
            Provider = model.Provider,
            Model = model.Model,
            IsInherit = model.IsInherit,
            MaxRecursionDepth = model.MaxRecursionDepth,
            MaxOutputTokens = model.MaxOutputTokens,
        };
    }
}
