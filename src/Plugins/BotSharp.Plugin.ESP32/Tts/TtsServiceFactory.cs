using BotSharp.Plugin.ESP32.Models;
using Microsoft.Extensions.Configuration;

namespace BotSharp.Plugin.ESP32.Tts;

public class TtsServiceFactory
{
    private readonly ILogger<TtsServiceFactory> _logger;

    // 语音生成文件保存地址
    private static readonly string OutputPath = "audio/";

    // 默认服务提供商名称
    private static readonly string DEFAULT_PROVIDER = "edge";

    // 默认 EDGE TTS 服务默认语音名称
    private static readonly string DEFAULT_VOICE = "zh-CN-XiaoyiNeural";

    private readonly IConfiguration _configuration;

    public TtsServiceFactory(ILogger<TtsServiceFactory> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// 根据配置获取TTS服务
    /// </summary>
    public ITtsService GetTtsService(SysConfig config, string voiceName)
    {
        return new AzureTtsService(_configuration);
        string provider;
        // 如果提供商为空，则使用默认提供商
        if (config == null)
        {
            provider = DEFAULT_PROVIDER;
        }
        else
        {
            provider = config.Provider;
        }

        // 如果是默认提供商且尚未初始化，则初始化

        // 创建新的服务实例
        try
        {
            // 创建其他API服务
            ITtsService service = CreateApiService(config, voiceName, OutputPath);
            return service;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "创建{0}服务失败", provider);
            return new AzureTtsService(_configuration);
        }

    }

    /// <summary>
    /// 根据配置创建API类型的TTS服务
    /// </summary>
    private ITtsService CreateApiService(SysConfig config, string voiceName, string outputPath)
    {
        return new AzureTtsService(_configuration);
        string provider = config.Provider;

        // 如果是Edge，直接返回Edge服务
        if ("aliyun".Equals(provider))
        {
            return new AzureTtsService(_configuration);
        }
        /*
        else if ("tencent".Equals(provider))
        {
            return new TencentTtsService(config, voiceName, outputPath);
        }
        */

        _logger.LogWarning("不支持的TTS服务提供商: {0}", provider);
        return null;
    }
}