using BotSharp.Abstraction.Tasks.Models;

namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class AgentTaskDocument : LiteDBBase
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Content { get; set; }
    public bool Enabled { get; set; }
    public string AgentId { get; set; }
    public string Status { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }

    public static AgentTask ToDomainModel(AgentTaskDocument model)
    {
        return new AgentTask
        {
            Id = model.Id,
            Description = model.Description,
            Content = model.Content,
            Enabled = model.Enabled,
            AgentId = model.AgentId,
            Status = model.Status,
            CreatedTime = model.CreatedTime,
            UpdatedTime = model.UpdatedTime
        };
    }

    public static AgentTaskDocument ToLiteDBModel(AgentTask model)
    {
        return new AgentTaskDocument
        {
            Id = model.Id,
            Description = model.Description,
            Content = model.Content,
            Enabled = model.Enabled,
            AgentId = model.AgentId,
            Status = model.Status,
            CreatedTime = model.CreatedTime,
            UpdatedTime = model.UpdatedTime
        };
    }
}
