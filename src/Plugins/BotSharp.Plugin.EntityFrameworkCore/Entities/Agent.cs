using BotSharp.Abstraction.Agents.Models;
using BotSharp.Abstraction.Routing.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class Agent
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string? Mode { get; set; }
    public string? InheritAgentId { get; set; }
    public string? IconUrl { get; set; }
    public string Instruction { get; set; }
    public bool IsPublic { get; set; }
    public bool Disabled { get; set; }
    public bool MergeUtility { get; set; }
    public int? MaxMessageCount { get; set; }

    [Column(TypeName = "json")]
    public List<ChannelInstructionElement> ChannelInstructions { get; set; }

    [Column(TypeName = "json")]
    public List<AgentTemplateElement> Templates { get; set; }

    [Column(TypeName = "json")]
    public List<FunctionDefElement> Functions { get; set; }

    [Column(TypeName = "json")]
    public List<AgentResponseElement> Responses { get; set; }

    [Column(TypeName = "json")]
    public List<string> Samples { get; set; }

    [Column(TypeName = "json")]
    public List<AgentUtilityElement> Utilities { get; set; }

    [Column(TypeName = "json")]
    public List<McpToolElement> McpTools { get; set; }

    [Column(TypeName = "json")]
    public List<AgentKnowledgeBaseElement> KnowledgeBases { get; set; }

    [Column(TypeName = "json")]
    public List<string> Profiles { get; set; }

    [Column(TypeName = "json")]
    public List<string> Labels { get; set; }

    [Column(TypeName = "json")]
    public List<RoutingRuleElement> RoutingRules { get; set; }

    [Column(TypeName = "json")]
    public List<AgentRuleElement> Rules { get; set; }

    [Column(TypeName = "json")]
    public AgentLlmConfigElement? LlmConfig { get; set; }
    
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
