using BotSharp.Abstraction.Agents.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class AgentTemplateMappers
{

    public static AgentTemplateElement ToEntity(this AgentTemplate model)
    {
        return new AgentTemplateElement
        {
            Name = model.Name,
            Content = model.Content
        };
    }

    public static AgentTemplate ToModel(this AgentTemplateElement model)
    {
        return new AgentTemplate
        {
            Name = model.Name,
            Content = model.Content
        };
    }
}
