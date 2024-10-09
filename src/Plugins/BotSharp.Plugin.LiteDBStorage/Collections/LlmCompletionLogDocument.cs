namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class LlmCompletionLogDocument : LiteDBBase
{
    public string ConversationId { get; set; }
    public List<PromptLogMongoElement> Logs { get; set; }
}
