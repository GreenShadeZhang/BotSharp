using BotSharp.Abstraction.Agents.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class AgentLlmConfigLiteDBElement
{
    public string? Provider { get; set; }
    public string? Model { get; set; }
    public bool IsInherit { get; set; }
    public int MaxRecursionDepth { get; set; }

    public static AgentLlmConfigLiteDBElement? ToLiteDBElement(AgentLlmConfig? config)
    {
        if (config == null) return null;

        return new AgentLlmConfigLiteDBElement
        {
            Provider = config.Provider,
            Model = config.Model,
            IsInherit = config.IsInherit,
            MaxRecursionDepth = config.MaxRecursionDepth,
        };
    }

    public static AgentLlmConfig? ToDomainElement(AgentLlmConfigLiteDBElement? config)
    {
        if (config == null) return null;

        return new AgentLlmConfig
        {
            Provider = config.Provider,
            Model = config.Model,
            IsInherit = config.IsInherit,
            MaxRecursionDepth = config.MaxRecursionDepth,
        };
    }
}
