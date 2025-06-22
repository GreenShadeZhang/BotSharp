using System;

namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class DialogMetaDataElement
{
    public string Role { get; set; }
    public string AgentId { get; set; }
    public string MessageId { get; set; }
    public string? FunctionName { get; set; }
    public string? SenderId { get; set; }
    public DateTime CreateTime { get; set; }
}
