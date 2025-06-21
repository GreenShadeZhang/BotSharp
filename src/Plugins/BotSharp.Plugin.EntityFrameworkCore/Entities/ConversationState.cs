using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class ConversationState
{
    public string Id { get; set; }
    public string ConversationId { get; set; }
    public string AgentId { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public DateTime UpdatedTime { get; set; }

    [Column(TypeName = "json")]
    public List<StateElement> States { get; set; } = new List<StateElement>();

    [Column(TypeName = "json")]
    public List<BreakpointInfoElement> Breakpoints { get; set; } = new List<BreakpointInfoElement>();
}
