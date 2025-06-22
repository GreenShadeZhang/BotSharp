using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class Conversation
{
    public string Id { get; set; }
    public string AgentId { get; set; }
    public string UserId { get; set; }
    public string? TaskId { get; set; }
    public string Title { get; set; }
    public string? TitleAlias { get; set; }
    public string Channel { get; set; }
    public string ChannelId { get; set; }
    public string Status { get; set; }
    public int DialogCount { get; set; }
    
    [Column(TypeName = "json")]
    public List<string> Tags { get; set; } = [];
    
    [Column(TypeName = "json")]
    public Dictionary<string, JsonDocument> LatestStates { get; set; } = new();
    
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
