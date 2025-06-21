using BotSharp.Abstraction.Functions.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    public static FunctionDef ToModel(this FunctionDefElement function)
    {
        return new FunctionDef
        {
            Name = function.Name,
            Description = function.Description,
            Channels = function.Channels,
            VisibilityExpression = function.VisibilityExpression,
            Impact = function.Impact,
            Parameters = new FunctionParametersDef
            {
                Type = function.Parameters.Type,
                Properties = JsonDocument.Parse(function.Parameters.Properties.IfNullOrEmptyAs("{}")),
                Required = function.Parameters.Required,
            },
            Output = function.Output
        };
    }
}
