namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class ConversationStateDocument : LiteDBBase
{
    public string ConversationId { get; set; }
    public List<StateMongoElement> States { get; set; } = new List<StateMongoElement>();
    public List<BreakpointMongoElement> Breakpoints { get; set; } = new List<BreakpointMongoElement>();
}
