using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class ConversationState
{
    public string Id { get; set; }
    public string ConversationId { get; set; }
    public List<State> States { get; set; } = new List<State>();

    [Column(TypeName = "json")]
    public List<BreakpointInfoElement> Breakpoints { get; set; } = new List<BreakpointInfoElement>();
}
