using BotSharp.Plugin.EntityFrameworkCore.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class GlobalStat
{
    public string Id { get; set; } = string.Empty;
    
    public string AgentId { get; set; } = string.Empty;

    [Column(TypeName = "json")]
    public GlobalStatsCountElement Count { get; set; } = new();

    [Column(TypeName = "json")]

    public GlobalStatsLlmCostElement LlmCost { get; set; } = new();
    
    public DateTime RecordTime { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public string Interval { get; set; } = string.Empty;
}
