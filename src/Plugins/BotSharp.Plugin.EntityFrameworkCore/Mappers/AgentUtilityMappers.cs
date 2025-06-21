using BotSharp.Abstraction.Agents.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;
using Microsoft.IdentityModel.Tokens;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class AgentUtilityMappers
{
    public static AgentUtilityElement ToEntity(this AgentUtility utility)
    {
        return new AgentUtilityElement
        {
            Category = utility.Category,
            Name = utility.Name,
            Disabled = utility.Disabled,
            VisibilityExpression = utility.VisibilityExpression,
            Items = utility.Items?.Select(x => new AgentUtilityItemElement
            {
                FunctionName = x.FunctionName,
                TemplateName = x.TemplateName,
                VisibilityExpression = x.VisibilityExpression
            })?.ToList() ?? []
        };
    }

    public static AgentUtility ToModel(this AgentUtilityElement utility)
    {
        return new AgentUtility
        {
            Category = utility.Category,
            Name = utility.Name,
            Disabled = utility.Disabled,
            VisibilityExpression = utility.VisibilityExpression,
            Items = utility.Items?.Select(x => new UtilityItem
            {
                FunctionName = x.FunctionName,
                TemplateName = x.TemplateName,
                VisibilityExpression = x.VisibilityExpression
            })?.ToList() ?? [],
        };
    }
}
