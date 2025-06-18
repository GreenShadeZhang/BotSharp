using BotSharp.Abstraction.Agents.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class McpToolMappers
{
    public static McpToolElement ToEntity(this McpTool model)
    {
        return new McpToolElement
        {
            Name = model.Name,
            ServerId = model.ServerId,
            Disabled = model.Disabled,
            Functions = model.Functions?.Select(f => new McpFunctionElement { Name = f.Name })?.ToList() ?? []
        };
    }

    public static McpTool ToModel(this McpToolElement entity)
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
