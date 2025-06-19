using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class InstructionLog
{
    [Key]
    public string Id { get; set; } = string.Empty;
    public string? AgentId { get; set; }
    public string Provider { get; set; } = default!;
    public string Model { get; set; } = default!;
    public string? TemplateName { get; set; }
    public string UserMessage { get; set; } = default!;
    public string? SystemInstruction { get; set; }
    public string CompletionText { get; set; } = default!;
    public string? UserId { get; set; }

    public Dictionary<string, JsonDocument> States { get; set; } = new();
    public DateTime CreatedTime { get; set; }
}
