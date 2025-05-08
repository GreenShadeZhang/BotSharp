namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class LlmCompletionLogDocument : LiteDBBase
{
    public string ConversationId { get; set; }
    public List<PromptLogLiteDBElement> Logs { get; set; }
}
