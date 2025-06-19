using System.ComponentModel.DataAnnotations;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class GlobalStat
{
    public string Id { get; set; } = string.Empty;
    
    public string AgentId { get; set; } = string.Empty;
    
    public GlobalStatsCountElement Count { get; set; } = new();
    
    public GlobalStatsLlmCostElement LlmCost { get; set; } = new();
    
    public DateTime RecordTime { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public string Interval { get; set; } = string.Empty;
}
