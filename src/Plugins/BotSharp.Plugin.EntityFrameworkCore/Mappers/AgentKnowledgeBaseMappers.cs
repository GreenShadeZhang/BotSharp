using BotSharp.Abstraction.Agents.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class AgentKnowledgeBaseMappers
{
    public static Entities.AgentKnowledgeBase ToEntity(this AgentKnowledgeBase model)
    {
        return new Entities.AgentKnowledgeBase
        {
            Name = model.Name,
            Type = model.Type,
            Disabled = model.Disabled,
            Confidence = model.Confidence
        };
    }

    public static AgentKnowledgeBase ToModel(this Entities.AgentKnowledgeBase entity)
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
