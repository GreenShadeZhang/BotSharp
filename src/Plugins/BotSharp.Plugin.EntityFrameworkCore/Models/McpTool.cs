namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class McpTool
{
    public string Name { get; set; } = default!;
    public string ServerId { get; set; } = default!;
    public bool Disabled { get; set; }
    public List<McpFunction> Functions { get; set; } = [];
}

public class McpFunction
{
    public string Name { get; set; } = default!;
}
