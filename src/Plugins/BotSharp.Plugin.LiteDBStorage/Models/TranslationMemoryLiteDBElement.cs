namespace BotSharp.Plugin.LiteDBStorage.Models;

public class TranslationMemoryLiteDBElement
{
    public string TranslatedText { get; set; }
    public string Language { get; set; }

    public static TranslationMemoryLiteDBElement ToMongoElement(TranslationMemoryItem item)
    {
        return new TranslationMemoryLiteDBElement
        {
            TranslatedText = item.TranslatedText,
            Language = item.Language
        };
    }

    public static TranslationMemoryItem ToDomainElement(TranslationMemoryLiteDBElement element)
    {
        return new TranslationMemoryItem
        {
            TranslatedText = element.TranslatedText,
            Language = element.Language
        };
    }
}
