using BotSharp.Abstraction.Files.Utilities;

namespace BotSharp.Plugin.PostgreSqlFileStorage.Services;

public partial class PostgreSqlFileStorageService
{
    public string GetUserAvatar()
    {
        var db = _services.GetRequiredService<IBotSharpRepository>();
        var user = db.GetUserById(_user.Id);
        var dir = GetUserAvatarDir(user?.Id);

        if (string.IsNullOrEmpty(dir)) return string.Empty;

        var avatarFile = _context.FileStorages
            .FirstOrDefault(f => f.Directory == dir);

        return avatarFile?.FilePath ?? string.Empty;
    }

    public bool SaveUserAvatar(FileDataModel file)
    {
        if (file == null || string.IsNullOrEmpty(file.FileData)) return false;

        try
        {
            var db = _services.GetRequiredService<IBotSharpRepository>();
            var user = db.GetUserById(_user.Id);
            var dir = GetUserAvatarDir(user?.Id);

            if (string.IsNullOrEmpty(dir)) return false;

            var (_, binary) = FileUtility.GetFileInfoFromData(file.FileData);
            var extension = Path.GetExtension(file.FileName);
            var fileName = user?.Id == null ? file.FileName : $"{user?.Id}{extension}";
            var filePath = $"{dir}/{fileName}";

            return SaveFileBytesToPath(filePath, BinaryData.FromBytes(binary.ToArray()));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Error when saving user avatar (user id: {_user.Id}).");
            return false;
        }
    }

    #region Private methods
    private string GetUserAvatarDir(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return string.Empty;
        }

        return $"{USERS_FOLDER}/{userId}/{USER_AVATAR_FOLDER}/";
    }
    #endregion
}
