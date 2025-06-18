namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class AgentLlmConfigElement
{
    public string? Provider { get; set; }
    public string? Model { get; set; }
    public bool IsInherit { get; set; }
    public int MaxRecursionDepth { get; set; }
    public int? MaxOutputTokens { get; set; }
}
