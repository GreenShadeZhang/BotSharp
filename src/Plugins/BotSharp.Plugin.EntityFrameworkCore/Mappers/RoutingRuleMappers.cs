using BotSharp.Abstraction.Routing.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class RoutingRuleMappers
{
    public static RoutingRuleElement ToEntity(this RoutingRule model)
    {
        return new RoutingRuleElement
        {
            Field = model.Field,
            Description = model.Description,
            Required = model.Required,
            RedirectTo = model.RedirectTo,
            Type = model.Type,
            FieldType = model.FieldType
        };
    }

    public static RoutingRule ToModel(this RoutingRuleElement rule, string agentId, string agentName)
    {
        return new RoutingRule
        {
            AgentId = agentId,
            AgentName = agentName,
            Field = rule.Field,
            Description = rule.Description,
            Required = rule.Required,
            RedirectTo = rule.RedirectTo,
            Type = rule.Type,
            FieldType = rule.FieldType
        };
    }
}
