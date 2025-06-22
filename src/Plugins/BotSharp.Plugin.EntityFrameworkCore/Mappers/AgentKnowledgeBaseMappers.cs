using BotSharp.Abstraction.Agents.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class AgentKnowledgeBaseMappers
{
    public static AgentKnowledgeBaseElement ToEntity(this AgentKnowledgeBase model)
    {
        return new AgentKnowledgeBaseElement
        {
            Name = model.Name,
            Type = model.Type,
            Disabled = model.Disabled,
            Confidence = model.Confidence
        };
    }

    public static AgentKnowledgeBase ToModel(this AgentKnowledgeBaseElement entity)
    {
        return new AgentKnowledgeBase
        {
            Name = entity.Name,
            Type = entity.Type,
            Disabled = entity.Disabled,
            Confidence = entity.Confidence
        };
    }
}
