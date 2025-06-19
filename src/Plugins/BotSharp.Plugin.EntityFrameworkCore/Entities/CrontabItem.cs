using System.ComponentModel.DataAnnotations;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class CrontabItem
{
    [Key]
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = default!;
    public string AgentId { get; set; } = default!;
    public string ConversationId { get; set; } = default!;
    public string ExecutionResult { get; set; } = default!;
    public string Cron { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int ExecutionCount { get; set; }
    public int MaxExecutionCount { get; set; }
    public int ExpireSeconds { get; set; }
    public DateTime? LastExecutionTime { get; set; }
    public bool LessThan60Seconds { get; set; } = false;
    public IEnumerable<CronTaskElement> Tasks { get; set; } = [];
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

}
