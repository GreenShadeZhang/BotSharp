using System.Collections.Generic;

namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class FunctionDefElement
{
    public string Name { get; set; }
    public string Description { get; set; } = default!;
    public List<string>? Channels { get; set; }
    public string? VisibilityExpression { get; set; }
    public string? Impact { get; set; }
    public FunctionParametersDefElement Parameters { get; set; } = new ();
    public string? Output { get; set; }

}
