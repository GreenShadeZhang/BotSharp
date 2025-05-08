namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class TranslationMemoryDocument : LiteDBBase
{
    public string OriginalText { get; set; }
    public string HashText { get; set; }
    public List<TranslationMemoryLiteDBElement> Translations { get; set; } = new List<TranslationMemoryLiteDBElement>();
}
