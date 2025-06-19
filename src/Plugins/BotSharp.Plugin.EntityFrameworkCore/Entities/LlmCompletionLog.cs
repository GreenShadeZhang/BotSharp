using System.ComponentModel.DataAnnotations.Schema;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class LlmCompletionLog
{
    public string Id { get; set; } = default!;
    public string ConversationId { get; set; } = default!;
    public string MessageId { get; set; } = default!;
    public string AgentId { get; set; } = default!;
    public string Prompt { get; set; } = default!;
    public string? Response { get; set; }
    public DateTime CreatedTime { get; set; }
}
