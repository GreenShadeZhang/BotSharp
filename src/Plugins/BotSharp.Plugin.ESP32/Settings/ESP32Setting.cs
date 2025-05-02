using BotSharp.Plugin.ESP32.Repository.Enums;

namespace BotSharp.Plugin.ESP32.Settings;

public class ESP32Setting
{
    public string DbDefault { get; set; } = IoTRepositoryEnum.LiteDBRepository;
    public string MongoDb { get; set; } = string.Empty;

    public string LiteDB { get; set; } = string.Empty;

    public string TablePrefix { get; set; } = string.Empty;
    public AzureCognitiveServicesOptions AzureCognitiveServicesOptions { get; set; } = new();
}
