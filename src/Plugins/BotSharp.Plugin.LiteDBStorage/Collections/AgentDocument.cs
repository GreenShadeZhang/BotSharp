namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class AgentDocument : LiteDBBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string? InheritAgentId { get; set; }
    public string? IconUrl { get; set; }
    public string Instruction { get; set; }
    public List<ChannelInstructionLiteDBElement> ChannelInstructions { get; set; }
    public List<AgentTemplateLiteDBElement> Templates { get; set; }
    public List<FunctionDefLiteDBElement> Functions { get; set; }
    public List<AgentResponseLiteDBElement> Responses { get; set; }
    public List<string> Samples { get; set; }
    public List<string> Utilities { get; set; }
    public bool IsPublic { get; set; }
    public bool Disabled { get; set; }
    public List<string> Profiles { get; set; }
    public List<RoutingRuleLiteDBElement> RoutingRules { get; set; }
    public AgentLlmConfigLiteDBElement? LlmConfig { get; set; }

    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}