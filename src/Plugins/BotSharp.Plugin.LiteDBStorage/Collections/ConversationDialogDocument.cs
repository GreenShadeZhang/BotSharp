namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class ConversationDialogDocument : LiteDBBase
{
    public string ConversationId { get; set; }
    public List<DialogMongoElement> Dialogs { get; set; }
}
