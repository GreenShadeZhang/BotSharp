using System.ComponentModel.DataAnnotations;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class RoleAgent
{
    [Key]
    public string Id { get; set; } = string.Empty;
    
    public string RoleId { get; set; } = string.Empty;
    
    public string AgentId { get; set; } = string.Empty;
    
    public DateTime CreatedTime { get; set; }
    
    public DateTime UpdatedTime { get; set; }
    
    // Navigation properties
    public virtual Role Role { get; set; } = null!;
    public virtual Agent Agent { get; set; } = null!;
}
