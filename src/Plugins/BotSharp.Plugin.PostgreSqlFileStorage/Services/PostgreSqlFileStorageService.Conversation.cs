using BotSharp.Abstraction.Files.Converters;
using BotSharp.Abstraction.Files.Enums;
using BotSharp.Abstraction.Files.Utilities;
using System.Net.Mime;

namespace BotSharp.Plugin.PostgreSqlFileStorage.Services;

public partial class PostgreSqlFileStorageService
{
    public async Task<IEnumerable<MessageFileModel>> GetMessageFileScreenshotsAsync(string conversationId, IEnumerable<string> messageIds)
    {
        var files = new List<MessageFileModel>();
        if (string.IsNullOrWhiteSpace(conversationId) || messageIds.IsNullOrEmpty()) return files;

        foreach (var messageId in messageIds)
        {
            var messageFiles = GetMessageFiles(conversationId, new[] { messageId }, FileSourceType.User);
            foreach (var messageFile in messageFiles)
            {
                var screenshots = await GetScreenshots(messageFile.FileStorageUrl, messageFile.MessageId, messageFile.FileSource);
                files.AddRange(screenshots);
            }
        }

        return files;
    }

    public IEnumerable<MessageFileModel> GetMessageFiles(string conversationId, IEnumerable<string> messageIds,
        string source, IEnumerable<string>? contentTypes = null)
    {
        var files = new List<MessageFileModel>();
        if (string.IsNullOrWhiteSpace(conversationId) || messageIds.IsNullOrEmpty()) return files;

        foreach (var messageId in messageIds)
        {
            var directoryPattern = $"{CONVERSATION_FOLDER}/{conversationId}/{FILE_FOLDER}/{messageId}/{source}";
            
            var messageFiles = _context.FileStorages
                .Where(f => f.Directory.StartsWith(directoryPattern))
                .ToList();

            foreach (var file in messageFiles)
            {
                if (!contentTypes.IsNullOrEmpty() && !contentTypes.Contains(file.ContentType))
                {
                    continue;
                }

                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var fileExtension = Path.GetExtension(file.FileName).TrimStart('.');
                var model = new MessageFileModel()
                {
                    MessageId = messageId,
                    FileUrl = BuildFileUrl(file.FilePath),
                    FileDownloadUrl = BuildFileUrl(file.FilePath),
                    FileStorageUrl = file.FilePath,
                    FileName = fileName,
                    FileExtension = fileExtension,
                    ContentType = file.ContentType,
                    FileSource = source
                };
                files.Add(model);
            }
        }

        return files;
    }

    public string GetMessageFile(string conversationId, string messageId, string source, string index, string fileName)
    {
        var directoryPattern = $"{CONVERSATION_FOLDER}/{conversationId}/{FILE_FOLDER}/{messageId}/{source}/{index}";
        
        var file = _context.FileStorages
            .FirstOrDefault(f => f.Directory == directoryPattern && 
                               Path.GetFileNameWithoutExtension(f.FileName).Equals(fileName, StringComparison.OrdinalIgnoreCase));
        
        return file?.FilePath ?? string.Empty;
    }

    public IEnumerable<MessageFileModel> GetMessagesWithFile(string conversationId, IEnumerable<string> messageIds)
    {
        var foundMsgs = new List<MessageFileModel>();
        if (string.IsNullOrWhiteSpace(conversationId) || messageIds.IsNullOrEmpty()) return foundMsgs;

        foreach (var messageId in messageIds)
        {
            var userDir = $"{CONVERSATION_FOLDER}/{conversationId}/{FILE_FOLDER}/{messageId}/{FileSourceType.User}/";
            if (ExistDirectory(userDir))
            {
                foundMsgs.Add(new MessageFileModel { MessageId = messageId, FileSource = FileSourceType.User });
            }

            var botDir = $"{CONVERSATION_FOLDER}/{conversationId}/{FILE_FOLDER}/{messageId}/{FileSourceType.Bot}";
            if (ExistDirectory(botDir))
            {
                foundMsgs.Add(new MessageFileModel { MessageId = messageId, FileSource = FileSourceType.Bot });
            }
        }

        return foundMsgs;
    }

    public bool SaveMessageFiles(string conversationId, string messageId, string source, List<FileDataModel> files)
    {
        if (files.IsNullOrEmpty()) return false;

        try
        {
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (string.IsNullOrEmpty(file.FileData))
                {
                    continue;
                }

                var (_, binary) = FileUtility.GetFileInfoFromData(file.FileData);
                var directory = $"{CONVERSATION_FOLDER}/{conversationId}/{FILE_FOLDER}/{messageId}/{source}/{i + 1}";
                var filePath = $"{directory}/{file.FileName}";

                SaveFileBytesToPath(filePath, BinaryData.FromBytes(binary.ToArray()));
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error when saving message files (conv id: {conversationId}).");
            return false;
        }
    }

    public bool DeleteMessageFiles(string conversationId, IEnumerable<string> messageIds, string targetMessageId, string? newMessageId = null)
    {
        if (messageIds.IsNullOrEmpty()) return false;

        try
        {
            foreach (var messageId in messageIds)
            {
                var directoryPattern = $"{CONVERSATION_FOLDER}/{conversationId}/{FILE_FOLDER}/{messageId}";
                var filesToDelete = _context.FileStorages
                    .Where(f => f.Directory.StartsWith(directoryPattern))
                    .ToList();

                if (filesToDelete.Any())
                {
                    _context.FileStorages.RemoveRange(filesToDelete);
                }
            }

            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error when deleting message files (conv id: {conversationId}).");
            return false;
        }
    }

    public bool DeleteConversationFiles(IEnumerable<string> conversationIds)
    {
        if (conversationIds.IsNullOrEmpty()) return false;

        try
        {
            foreach (var conversationId in conversationIds)
            {
                var convDir = $"{CONVERSATION_FOLDER}/{conversationId}";
                DeleteDirectory(convDir);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error when deleting conversation files.");
            return false;
        }
    }

    #region Private Methods
    private async Task<IEnumerable<MessageFileModel>> GetScreenshots(string filePath, string messageId, string source)
    {
        var files = new List<MessageFileModel>();

        try
        {
            var file = _context.FileStorages.FirstOrDefault(f => f.FilePath == filePath);
            if (file == null) return files;

            var contentType = file.ContentType;
            var parentDir = Path.GetDirectoryName(file.FilePath)?.Replace('\\', '/') ?? string.Empty;
            var screenshotDir = $"{parentDir}/{SCREENSHOT_FILE_FOLDER}/";

            if (_imageTypes.Contains(contentType))
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var fileExtension = Path.GetExtension(file.FileName).TrimStart('.');
                var model = new MessageFileModel
                {
                    MessageId = messageId,
                    FileName = fileName,
                    FileExtension = fileExtension,
                    FileUrl = BuildFileUrl(file.FilePath),
                    FileStorageUrl = file.FilePath,
                    ContentType = contentType,
                    FileSource = source
                };
                files.Add(model);
            }
            else if (contentType == MediaTypeNames.Application.Pdf)
            {
                var images = await ConvertPdfToImages(file.FilePath, screenshotDir);
                foreach (var image in images)
                {
                    var fileName = Path.GetFileNameWithoutExtension(image);
                    var fileExtension = Path.GetExtension(image).TrimStart('.');
                    var model = new MessageFileModel
                    {
                        MessageId = messageId,
                        FileName = fileName,
                        FileExtension = fileExtension,
                        FileUrl = BuildFileUrl(image),
                        FileStorageUrl = image,
                        ContentType = contentType,
                        FileSource = source
                    };
                    files.Add(model);
                }
            }
            return files;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error when getting message file screenshots {filePath} (messageId: {messageId}).");
            return files;
        }
    }

    private async Task<IEnumerable<string>> ConvertPdfToImages(string pdfPath, string imagePath)
    {
        var converters = _services.GetServices<IPdf2ImageConverter>();
        if (converters.IsNullOrEmpty()) return Enumerable.Empty<string>();

        var converter = GetPdf2ImageConverter();
        if (converter == null)
        {
            return Enumerable.Empty<string>();
        }
        return await converter.ConvertPdfToImages(pdfPath, imagePath);
    }

    private IPdf2ImageConverter? GetPdf2ImageConverter()
    {
        var settings = _services.GetRequiredService<FileCoreSettings>();
        var converter = _services.GetServices<IPdf2ImageConverter>()
            .FirstOrDefault(x => x.Provider == settings.Pdf2ImageConverter.Provider);
        return converter;
    }

    private string BuildFileUrl(string filePath)
    {
        return $"/file-storage/{filePath}";
    }
    #endregion
}
