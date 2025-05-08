namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class ConversationStateDocument : LiteDBBase
{
    public string ConversationId { get; set; }
    public string AgentId { get; set; }
    public DateTime UpdatedTime { get; set; }
    public List<StateLiteDBElement> States { get; set; } = new List<StateLiteDBElement>();
    public List<BreakpointLiteDBElement> Breakpoints { get; set; } = new List<BreakpointLiteDBElement>();
}
