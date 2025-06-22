using BotSharp.Abstraction.Agents.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class AgentUtilityLiteDBElement
{
    public string Category { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool Disabled { get; set; }
    public string? VisibilityExpression { get; set; }
    public List<AgentUtilityItemLiteDBElement> Items { get; set; } = [];

    public static AgentUtilityLiteDBElement ToLiteDBElement(AgentUtility utility)
    {
        return new AgentUtilityLiteDBElement
        {
            Category = utility.Category,
            Name = utility.Name,
            Disabled = utility.Disabled,
            VisibilityExpression = utility.VisibilityExpression,
            Items = utility.Items?.Select(x => new AgentUtilityItemLiteDBElement
            {
                FunctionName = x.FunctionName,
                TemplateName = x.TemplateName,
                VisibilityExpression = x.VisibilityExpression
            })?.ToList() ?? []
        };
    }

    public static AgentUtility ToDomainElement(AgentUtilityLiteDBElement utility)
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
public class AgentUtilityItemLiteDBElement
{
    public string FunctionName { get; set; }
    public string? TemplateName { get; set; }
    public string? VisibilityExpression { get; set; }
}