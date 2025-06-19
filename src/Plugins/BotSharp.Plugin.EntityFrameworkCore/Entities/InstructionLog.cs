using System.ComponentModel.DataAnnotations;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class InstructionLog
{
    [Key]
    public string Id { get; set; } = string.Empty;
    
    public string AgentId { get; set; } = string.Empty;
    
    public string ConversationId { get; set; } = string.Empty;
    
    public string MessageId { get; set; } = string.Empty;
    
    public string Instruction { get; set; } = string.Empty;
    
    public string Response { get; set; } = string.Empty;
    
    public DateTime CreatedTime { get; set; }
}
