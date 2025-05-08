namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class ExecutionLogDocument : LiteDBBase
{
    public string ConversationId { get; set; }
    public List<string> Logs { get; set; }
}
