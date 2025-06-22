using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class TranslationMemory
{
    public string Id { get; set; }
    public string OriginalText { get; set; }
    public string HashText { get; set; }

    [Column(TypeName = "json")]
    public List<TranslationMemoryElement> Translations { get; set; } = new List<TranslationMemoryElement>();
}
