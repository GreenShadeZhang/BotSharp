using BotSharp.Abstraction.Agents.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class McpToolMappers
{
    public static Entities.McpTool ToEntity(this McpTool model)
    {
        return new Entities.McpTool
        {
            Name = model.Name,
            ServerId = model.ServerId,
            Disabled = model.Disabled,
            Functions = model.Functions?.Select(f => new Entities.McpFunction { Name = f.Name })?.ToList() ?? []
        };
    }

    public static McpTool ToModel(this Entities.McpTool entity)
    {
        return new McpTool
        {
            Name = entity.Name,
            ServerId = entity.ServerId,
            Disabled = entity.Disabled,
            Functions = entity.Functions?.Select(f => new McpFunction(f.Name))?.ToList() ?? []
        };
    }
}
