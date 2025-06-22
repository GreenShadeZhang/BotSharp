using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class AgentUtilityElement
{
    public string Category { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool Disabled { get; set; }
    public string? VisibilityExpression { get; set; }

    public List<AgentUtilityItemElement> Items { get; set; } = [];
}


public class AgentUtilityItemElement
{
    public string FunctionName { get; set; }
    public string? TemplateName { get; set; }
    public string? VisibilityExpression { get; set; }
}