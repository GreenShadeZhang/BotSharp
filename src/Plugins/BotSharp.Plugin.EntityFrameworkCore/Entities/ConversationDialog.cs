using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class ConversationDialog
{
    public string Id { get; set; }
    public string ConversationId { get; set; }

    [Column(TypeName = "json")]
    public DialogMetaDataElement MetaData { get; set; }
    public string Content { get; set; }
    public string? SecondaryContent { get; set; }
    public string? RichContent { get; set; }
    public string? SecondaryRichContent { get; set; }
    public string? Payload { get; set; }
}
