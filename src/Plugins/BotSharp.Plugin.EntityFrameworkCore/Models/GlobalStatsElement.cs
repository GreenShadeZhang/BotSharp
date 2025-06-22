namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class GlobalStatsCountElement
{
    public long AgentCallCount { get; set; }
}

public class GlobalStatsLlmCostElement
{
    public long PromptTokens { get; set; }
    public long CompletionTokens { get; set; }
    public float PromptTotalCost { get; set; }
    public float CompletionTotalCost { get; set; }
}
