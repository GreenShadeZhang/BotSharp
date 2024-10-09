namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class UserAgentDocument : LiteDBBase
{
    public string UserId { get; set; }
    public string AgentId { get; set; }
    public bool Editable { get; set; }

    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
