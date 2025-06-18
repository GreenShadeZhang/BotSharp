namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class RoutingRuleElement
{
    public string Field { get; set; }
    public string Description { get; set; }
    public bool Required { get; set; }
    public string? RedirectTo { get; set; }
    public string Type { get; set; }
    public string FieldType { get; set; }
}
