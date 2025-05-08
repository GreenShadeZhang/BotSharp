using BotSharp.Abstraction.Crontab.Models;

namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class CrontabItemDocument : LiteDBBase
{
    public string UserId { get; set; }
    public string AgentId { get; set; }
    public string ConversationId { get; set; }
    public string ExecutionResult { get; set; }
    public string Cron { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int ExecutionCount { get; set; }
    public int MaxExecutionCount { get; set; }
    public int ExpireSeconds { get; set; }
    public DateTime? LastExecutionTime { get; set; }
    public bool LessThan60Seconds { get; set; } = false;
    public IEnumerable<CronTaskLiteDBElement> Tasks { get; set; } = [];
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

    public static CrontabItem ToDomainModel(CrontabItemDocument item)
    {
        return new CrontabItem
        {
            UserId = item.UserId,
            AgentId = item.AgentId,
            ConversationId = item.ConversationId,
            ExecutionResult = item.ExecutionResult,
            Cron = item.Cron,
            Title = item.Title,
            Description = item.Description,
            ExecutionCount = item.ExecutionCount,
            MaxExecutionCount = item.MaxExecutionCount,
            ExpireSeconds = item.ExpireSeconds,
            LastExecutionTime = item.LastExecutionTime,
            LessThan60Seconds = item.LessThan60Seconds,
            Tasks = item.Tasks?.Select(x => CronTaskLiteDBElement.ToDomainElement(x))?.ToArray() ?? [],
            CreatedTime = item.CreatedTime
        };
    }

    public static CrontabItemDocument ToLiteDBModel(CrontabItem item)
    {
        return new CrontabItemDocument
        {
            UserId = item.UserId,
            AgentId = item.AgentId,
            ConversationId = item.ConversationId,
            ExecutionResult = item.ExecutionResult,
            Cron = item.Cron,
            Title = item.Title,
            Description = item.Description,
            ExecutionCount = item.ExecutionCount,
            MaxExecutionCount = item.MaxExecutionCount,
            ExpireSeconds = item.ExpireSeconds,
            LastExecutionTime = item.LastExecutionTime,
            LessThan60Seconds = item.LessThan60Seconds,
            Tasks = item.Tasks?.Select(x => CronTaskLiteDBElement.ToLiteDBElement(x))?.ToList() ?? [],
            CreatedTime = item.CreatedTime
        };
    }
}
