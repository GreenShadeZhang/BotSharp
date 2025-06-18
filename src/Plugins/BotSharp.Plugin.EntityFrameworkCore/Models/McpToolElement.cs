namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class McpToolElement
{
    public string Name { get; set; } = default!;
    public string ServerId { get; set; } = default!;
    public bool Disabled { get; set; }
    public List<McpFunctionElement> Functions { get; set; } = [];
}

public class McpFunctionElement
{
    public string Name { get; set; } = default!;
}
