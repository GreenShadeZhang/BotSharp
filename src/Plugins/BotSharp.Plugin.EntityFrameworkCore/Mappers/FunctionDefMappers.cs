using BotSharp.Abstraction.Functions.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;
using System.Text.Json;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class FunctionDefMappers
{
    public static FunctionDefElement ToEntity(this FunctionDef model)
    {
        return new FunctionDefElement
        {
            Name = model.Name,
            Description = model.Description,
            Channels = model.Channels,
            VisibilityExpression = model.VisibilityExpression,
            Impact = model.Impact,
            Parameters = new FunctionParametersDefElement
            {
                Type = model.Parameters.Type,
                Properties = JsonSerializer.Serialize(model.Parameters.Properties),
                Required = model.Parameters.Required,
            }
        };
    }

    public static FunctionDef ToModel(this FunctionDefElement model)
    {
        return new FunctionDef
        {
            Name = model.Name,
            Description = model.Description,
            Channels = model.Channels,
            VisibilityExpression = model.VisibilityExpression,
            Impact = model.Impact,
            Parameters = new FunctionParametersDef
            {
                Type = model.Parameters.Type,
                Properties = JsonDocument.Parse(model.Parameters.Properties),
                Required = model.Parameters.Required,
            }
        };
    }
}
