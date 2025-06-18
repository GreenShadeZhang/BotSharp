using System.ComponentModel.DataAnnotations.Schema;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class LlmCompletionLog
{
    public string Id { get; set; }
    public string ConversationId { get; set; }

    [Column(TypeName = "json")]
    public List<PromptLogElement> Logs { get; set; }
}
