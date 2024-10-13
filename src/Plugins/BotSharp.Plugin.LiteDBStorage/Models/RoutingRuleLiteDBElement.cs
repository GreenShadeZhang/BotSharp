using BotSharp.Abstraction.Routing.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class RoutingRuleLiteDBElement
{
    public string Field { get; set; }
    public string Description { get; set; }
    public bool Required { get; set; }
    public string? RedirectTo { get; set; }
    public string Type { get; set; }
    public string FieldType { get; set; }

    public RoutingRuleLiteDBElement()
    {
        
    }

    public static RoutingRuleLiteDBElement ToMongoElement(RoutingRule routingRule)
    {
        return new RoutingRuleLiteDBElement 
        {
            Field = routingRule.Field,
            Description = routingRule.Description,
            Required = routingRule.Required,
            RedirectTo = routingRule.RedirectTo,
            Type = routingRule.Type,
            FieldType = routingRule.FieldType
        };
    }

    public static RoutingRule ToDomainElement(string agentId, string agentName, RoutingRuleLiteDBElement rule)
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
            FieldType= rule.FieldType
        };
    }

    public override string ToString()
    {
        return $"{Field} - {FieldType}, Required: {Required} ({Type})";
    }
}
