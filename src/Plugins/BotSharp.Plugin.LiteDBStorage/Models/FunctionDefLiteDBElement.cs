using BotSharp.Abstraction.Functions.Models;
using System.Text.Json;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class FunctionDefLiteDBElement
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string>? Channels { get; set; }
    public string? VisibilityExpression { get; set; }
    public string? Impact { get; set; }
    public FunctionParametersDefMongoElement Parameters { get; set; } = new FunctionParametersDefMongoElement();

    public FunctionDefLiteDBElement()
    {
        
    }

    public static FunctionDefLiteDBElement ToMongoElement(FunctionDef function)
    {
        return new FunctionDefLiteDBElement
        {
            Name = function.Name,
            Description = function.Description,
            Channels = function.Channels,
            VisibilityExpression = function.VisibilityExpression,
            Impact = function.Impact,
            Parameters = new FunctionParametersDefMongoElement
            {
                Type = function.Parameters.Type,
                Properties = JsonSerializer.Serialize(function.Parameters.Properties),
                Required = function.Parameters.Required,
            }
        };
    }

    public static FunctionDef ToDomainElement(FunctionDefLiteDBElement function)
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
                Properties = JsonSerializer.Deserialize<JsonDocument>(function.Parameters.Properties.IfNullOrEmptyAs("{}")),
                Required = function.Parameters.Required,
            }
        };
    }
}

public class FunctionParametersDefMongoElement
{
    public string Type { get; set; }
    public string Properties { get; set; }
    public List<string> Required { get; set; } = new List<string>();

    public FunctionParametersDefMongoElement()
    {
        
    }
}
