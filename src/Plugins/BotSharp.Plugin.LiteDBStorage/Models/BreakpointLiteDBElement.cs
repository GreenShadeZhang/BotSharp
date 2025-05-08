namespace BotSharp.Plugin.LiteDBStorage.Models;

public class BreakpointLiteDBElement
{
    public string? MessageId { get; set; }
    public DateTime Breakpoint { get; set; }
    public DateTime CreatedTime { get; set; }
    public string? Reason { get; set; }
}
