namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class ConversationDialogDocument : LiteDBBase
{
    public string ConversationId { get; set; }
    public string AgentId { get; set; }
    public DateTime UpdatedTime { get; set; }
    public List<DialogLiteDBElement> Dialogs { get; set; }
}
