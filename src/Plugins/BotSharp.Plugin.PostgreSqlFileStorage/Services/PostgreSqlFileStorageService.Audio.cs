namespace BotSharp.Plugin.PostgreSqlFileStorage.Services;

public partial class PostgreSqlFileStorageService
{
    public bool SaveSpeechFile(string conversationId, string fileName, BinaryData data)
    {
        try
        {
            var filePath = $"{CONVERSATION_FOLDER}/{conversationId}/{TEXT_TO_SPEECH_FOLDER}/{fileName}";
            
            var existingFile = _context.FileStorages.FirstOrDefault(f => f.FilePath == filePath);
            if (existingFile != null)
            {
                return false; // File already exists
            }

            return SaveFileBytesToPath(filePath, data);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error when saving speech file. {fileName} (conv id: {conversationId})");
            return false;
        }
    }

    public BinaryData GetSpeechFile(string conversationId, string fileName)
    {
        var filePath = $"{CONVERSATION_FOLDER}/{conversationId}/{TEXT_TO_SPEECH_FOLDER}/{fileName}";
        
        var file = _context.FileStorages.FirstOrDefault(f => f.FilePath == filePath);
        if (file == null)
        {
            return BinaryData.Empty;
        }

        return BinaryData.FromBytes(file.FileData);
    }
}
