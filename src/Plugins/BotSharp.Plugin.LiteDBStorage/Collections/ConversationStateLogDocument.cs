namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class ConversationStateLogDocument : LiteDBBase
{
    public string ConversationId { get; set; }
    public string MessageId { get; set; }
    public Dictionary<string, string> States { get; set; }
    public DateTime CreateTime { get; set; }
}