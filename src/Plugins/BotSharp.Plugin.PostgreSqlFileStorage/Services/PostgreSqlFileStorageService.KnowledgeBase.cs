namespace BotSharp.Plugin.PostgreSqlFileStorage.Services;

public partial class PostgreSqlFileStorageService
{
    public bool SaveKnowledgeBaseFile(string collectionName, string vectorStoreProvider, Guid fileId, string fileName, BinaryData fileData)
    {
        if (string.IsNullOrWhiteSpace(collectionName) || string.IsNullOrWhiteSpace(vectorStoreProvider))
        {
            return false;
        }

        try
        {
            var docDir = BuildKnowledgeCollectionFileDir(collectionName, vectorStoreProvider);
            var directory = $"{docDir}/{fileId}";
            var filePath = $"{directory}/{fileName}";

            // Delete existing files for this fileId
            var existingFiles = _context.FileStorages
                .Where(f => f.Directory == directory)
                .ToList();

            if (existingFiles.Any())
            {
                _context.FileStorages.RemoveRange(existingFiles);
            }

            var result = SaveFileBytesToPath(filePath, fileData);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error when saving knowledge file " +
                $"(Vector store provider: {vectorStoreProvider}, Collection: {collectionName}, File name: {fileName}).");
            return false;
        }
    }

    public bool DeleteKnowledgeFile(string collectionName, string vectorStoreProvider, Guid? fileId = null)
    {
        if (string.IsNullOrWhiteSpace(collectionName) || string.IsNullOrWhiteSpace(vectorStoreProvider))
        {
            return false;
        }

        try
        {
            var docDir = BuildKnowledgeCollectionFileDir(collectionName, vectorStoreProvider);
            
            if (fileId == null)
            {
                // Delete all files in the collection
                DeleteDirectory(docDir);
            }
            else
            {
                // Delete specific file
                var fileDir = $"{docDir}/{fileId}";
                DeleteDirectory(fileDir);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error when deleting knowledge file " +
                $"(Vector store provider: {vectorStoreProvider}, Collection: {collectionName}, File ID: {fileId}).");
            return false;
        }
    }

    public string GetKnowledgeBaseFileUrl(string collectionName, string vectorStoreProvider, Guid fileId, string fileName)
    {
        if (string.IsNullOrWhiteSpace(collectionName) || 
            string.IsNullOrWhiteSpace(vectorStoreProvider) || 
            string.IsNullOrWhiteSpace(fileName))
        {
            return string.Empty;
        }

        var docDir = BuildKnowledgeCollectionFileDir(collectionName, vectorStoreProvider);
        var fileDir = $"{docDir}/{fileId}";
        var filePath = $"{fileDir}/{fileName}";

        var file = _context.FileStorages.FirstOrDefault(f => f.FilePath == filePath);
        if (file == null)
        {
            return string.Empty;
        }

        return BuildFileUrl(filePath);
    }

    public BinaryData GetKnowledgeBaseFileBinaryData(string collectionName, string vectorStoreProvider, Guid fileId, string fileName)
    {
        if (string.IsNullOrWhiteSpace(collectionName) || 
            string.IsNullOrWhiteSpace(vectorStoreProvider) || 
            string.IsNullOrWhiteSpace(fileName))
        {
            return BinaryData.Empty;
        }

        try
        {
            var docDir = BuildKnowledgeCollectionFileDir(collectionName, vectorStoreProvider);
            var fileDir = $"{docDir}/{fileId}";
            var filePath = $"{fileDir}/{fileName}";

            var file = _context.FileStorages.FirstOrDefault(f => f.FilePath == filePath);
            if (file == null)
            {
                return BinaryData.Empty;
            }

            return BinaryData.FromBytes(file.FileData);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error when downloading collection file ({collectionName}-{vectorStoreProvider}-{fileId}-{fileName})");
            return BinaryData.Empty;
        }
    }

    #region Private methods
    private string BuildKnowledgeCollectionFileDir(string collectionName, string vectorStoreProvider)
    {
        return $"{KNOWLEDGE_FOLDER}/{KNOWLEDGE_DOC_FOLDER}/{vectorStoreProvider.CleanStr()}/{collectionName.CleanStr()}";
    }
    #endregion
}
