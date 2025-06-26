using BotSharp.Abstraction.Files.Utilities;
using BotSharp.Plugin.PostgreSqlFileStorage.DbContexts;
using BotSharp.Plugin.PostgreSqlFileStorage.Entities;
using System.Net.Mime;
using System.Text.Json;

namespace BotSharp.Plugin.PostgreSqlFileStorage.Services;

public partial class PostgreSqlFileStorageService : IFileStorageService
{
    private readonly PostgreSqlFileStorageDbContext _context;
    private readonly IServiceProvider _services;
    private readonly IUserIdentity _user;
    private readonly ILogger<PostgreSqlFileStorageService> _logger;

    private readonly IEnumerable<string> _imageTypes = new List<string>
    {
        MediaTypeNames.Image.Png,
        MediaTypeNames.Image.Jpeg
    };

    private const string CONVERSATION_FOLDER = "conversations";
    private const string FILE_FOLDER = "files";
    private const string USER_FILE_FOLDER = "user";
    private const string SCREENSHOT_FILE_FOLDER = "screenshot";
    private const string BOT_FILE_FOLDER = "bot";
    private const string USERS_FOLDER = "users";
    private const string USER_AVATAR_FOLDER = "avatar";
    private const string SESSION_FOLDER = "sessions";
    private const string TEXT_TO_SPEECH_FOLDER = "speeches";
    private const string KNOWLEDGE_FOLDER = "knowledgebase";
    private const string KNOWLEDGE_DOC_FOLDER = "document";

    public PostgreSqlFileStorageService(
        PostgreSqlFileStorageDbContext context,
        IUserIdentity user,
        ILogger<PostgreSqlFileStorageService> logger,
        IServiceProvider services)
    {
        _context = context;
        _user = user;
        _logger = logger;
        _services = services;
    }

    #region Common
    public string GetDirectory(string conversationId)
    {
        return $"{CONVERSATION_FOLDER}/{conversationId}/attachments/";
    }

    public IEnumerable<string> GetFiles(string relativePath, string? searchPattern = null)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            return Enumerable.Empty<string>();
        }

        try
        {
            var files = _context.FileStorages
                .Where(f => f.Directory == relativePath || f.FilePath.StartsWith(relativePath))
                .Select(f => f.FilePath)
                .ToList();

            return files;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error when getting files (path: {relativePath}).");
            return Enumerable.Empty<string>();
        }
    }

    public BinaryData GetFileBytes(string fileStorageUrl)
    {
        try
        {
            var file = _context.FileStorages.FirstOrDefault(f => f.FilePath == fileStorageUrl);
            if (file == null)
            {
                return BinaryData.Empty;
            }

            return BinaryData.FromBytes(file.FileData);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error when getting file bytes (url: {fileStorageUrl}).");
            return BinaryData.Empty;
        }
    }

    public bool SaveFileStreamToPath(string filePath, Stream stream)
    {
        if (string.IsNullOrEmpty(filePath)) return false;

        try
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var fileData = memoryStream.ToArray();

            return SaveFileBytesToPath(filePath, BinaryData.FromBytes(fileData));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error when saving file stream to path ({filePath}).");
            return false;
        }
    }

    public bool SaveFileBytesToPath(string filePath, BinaryData binary)
    {
        if (string.IsNullOrEmpty(filePath)) return false;

        try
        {
            var fileName = Path.GetFileName(filePath);
            var directory = Path.GetDirectoryName(filePath)?.Replace('\\', '/') ?? string.Empty;
            var contentType = FileUtility.GetFileContentType(fileName);
            var category = GetCategoryFromPath(filePath);
            var entityId = GetEntityIdFromPath(filePath);

            var existingFile = _context.FileStorages.FirstOrDefault(f => f.FilePath == filePath);
            if (existingFile != null)
            {
                existingFile.FileData = binary.ToArray();
                existingFile.FileSize = binary.ToArray().Length;
                existingFile.UpdatedAt = DateTime.UtcNow;
                _context.FileStorages.Update(existingFile);
            }
            else
            {
                var fileStorage = new FileStorage
                {
                    Id = Guid.NewGuid(),
                    FilePath = filePath,
                    FileName = fileName,
                    ContentType = contentType,
                    FileSize = binary.ToArray().Length,
                    FileData = binary.ToArray(),
                    Category = category,
                    EntityId = entityId,
                    Directory = directory,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.FileStorages.Add(fileStorage);
            }

            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error when saving file bytes to path ({filePath}).");
            return false;
        }
    }

    public string GetParentDir(string dir, int level = 1)
    {
        var segments = dir.Split('/');
        return string.Join("/", segments.SkipLast(level));
    }

    public bool ExistDirectory(string? dir)
    {
        if (string.IsNullOrEmpty(dir)) return false;

        return _context.FileStorages.Any(f => f.Directory == dir || f.FilePath.StartsWith(dir));
    }

    public void CreateDirectory(string dir)
    {
        // In database storage, directories are created implicitly when files are saved
    }

    public void DeleteDirectory(string dir)
    {
        try
        {
            var filesToDelete = _context.FileStorages
                .Where(f => f.Directory == dir || f.FilePath.StartsWith(dir))
                .ToList();

            if (filesToDelete.Any())
            {
                _context.FileStorages.RemoveRange(filesToDelete);
                _context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error when deleting directory ({dir}).");
        }
    }

    public string BuildDirectory(params string[] segments)
    {
        return string.Join("/", segments);
    }
    #endregion

    #region Private Methods
    private string GetCategoryFromPath(string filePath)
    {
        var segments = filePath.Split('/');
        if (segments.Length > 0)
        {
            return segments[0] switch
            {
                CONVERSATION_FOLDER => "conversation",
                USERS_FOLDER => "user",
                KNOWLEDGE_FOLDER => "knowledge",
                _ => "general"
            };
        }
        return "general";
    }

    private string? GetEntityIdFromPath(string filePath)
    {
        var segments = filePath.Split('/');
        return segments.Length > 1 ? segments[1] : null;
    }
    #endregion
}
