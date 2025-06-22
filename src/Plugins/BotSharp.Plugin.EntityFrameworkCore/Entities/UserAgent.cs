using System.ComponentModel.DataAnnotations.Schema;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class UserAgent
{
    public string Id { get; set; }
    public string UserId { get; set; } = default!;
    public string AgentId { get; set; } = default!;    
    
    [Column(TypeName = "json")]
    public List<string> Actions { get; set; } = new();
    public bool Editable { get; set; } = true;
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
