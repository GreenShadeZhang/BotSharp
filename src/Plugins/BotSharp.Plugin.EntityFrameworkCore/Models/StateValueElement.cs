namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class StateValueElement
{
    public string Data { get; set; }
    public string? MessageId { get; set; }
    public bool Active { get; set; }
    public int ActiveRounds { get; set; }
    public string DataType { get; set; }
    public string Source { get; set; }
    public DateTime UpdateTime { get; set; }
}
