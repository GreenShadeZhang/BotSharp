namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class TranslationMemoryDocument : LiteDBBase
{
    public string OriginalText { get; set; }
    public string HashText { get; set; }
    public List<TranslationMemoryMongoElement> Translations { get; set; } = new List<TranslationMemoryMongoElement>();
}
