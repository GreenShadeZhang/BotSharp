using System.ComponentModel.DataAnnotations;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class Role
{
    [Key]
    public string Id { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public List<string> Permissions { get; set; } = new();
    
    public DateTime CreatedTime { get; set; }
    
    public DateTime UpdatedTime { get; set; }
    
    // Navigation properties
    public virtual ICollection<RoleAgent> RoleAgents { get; set; } = new List<RoleAgent>();
}
