using BotSharp.Plugin.ESP32.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotSharp.Plugin.ESP32.Stt;
public class SttServiceFactory
{
    private readonly ILogger<SttServiceFactory> _logger;

    // 缓存已初始化的服务：对于API服务，键为"provider:configId"格式；对于本地服务，键为provider名称
    private readonly ConcurrentDictionary<string, ISttService> _serviceCache = new ConcurrentDictionary<string, ISttService>();

    // 默认服务提供商名称
    private static readonly string DEFAULT_PROVIDER = "vosk";

    // 标记Vosk是否初始化成功
    private bool _voskInitialized = false;

    // 备选默认提供商（当Vosk初始化失败时使用）
    private string _fallbackProvider = null;

    public SttServiceFactory(ILogger<SttServiceFactory> logger)
    {
        _logger = logger;
        InitializeDefaultSttService();
    }

    /// <summary>
    /// 应用启动时自动初始化Vosk服务
    /// </summary>
    private void InitializeDefaultSttService()
    {
        _logger.LogInformation("正在初始化默认语音识别服务(Vosk)...");
        if (_voskInitialized)
        {
            _logger.LogInformation("默认语音识别服务(Vosk)初始化成功，可直接使用");
        }
        else
        {
            _logger.LogWarning("默认语音识别服务(Vosk)初始化失败，将在需要时尝试使用备选服务");
        }
    }

    /// <summary>
    /// 获取默认STT服务
    /// 如果Vosk可用则返回Vosk，否则返回备选服务
    /// </summary>
    public ISttService GetDefaultSttService()
    {
        // 如果Vosk已初始化成功，直接返回
        if (_voskInitialized && _serviceCache.ContainsKey(DEFAULT_PROVIDER))
        {
            return _serviceCache[DEFAULT_PROVIDER];
        }

        // 否则返回备选服务
        if (_fallbackProvider != null && _serviceCache.ContainsKey(_fallbackProvider))
        {
            return _serviceCache[_fallbackProvider];
        }

        // 如果没有备选服务，尝试创建一个API类型的服务作为备选
        if (_serviceCache.Count == 0)
        {
            _logger.LogWarning("没有可用的STT服务，将尝试创建默认API服务");
            try
            {
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "创建默认API服务失败");
                return new AliyunSttService();
            }
        }

        return null;
    }

    /// <summary>
    /// 根据配置获取STT服务
    /// </summary>
    public ISttService GetSttService(SysConfig config)
    {
        //if (config == null)
        //{
        //    return GetDefaultSttService();
        //}

        //string provider = config.Provider;

        //// 如果是Vosk，直接使用全局共享的实例
        //if (DEFAULT_PROVIDER.Equals(provider))
        //{
        //    // 如果Vosk还未初始化，尝试初始化
        //    // Vosk初始化失败的情况
        //    if (!_voskInitialized)
        //    {
        //        return null;
        //    }
        //    return _serviceCache[DEFAULT_PROVIDER];
        //}

        //// 对于API服务，使用"provider:configId"作为缓存键，确保每个配置使用独立的服务实例
        //int? configId = config.ConfigId;
        //string cacheKey = provider + ":" + (configId != null ? configId.ToString() : "default");

        //// 检查是否已有该配置的服务实例
        //if (_serviceCache.ContainsKey(cacheKey))
        //{
        //    return _serviceCache[cacheKey];
        //}

        //// 创建新的API服务实例
        //try
        //{
        //    ISttService service = CreateApiService(config);
        //    if (service != null)
        //    {
        //        _serviceCache[cacheKey] = service;

        //        // 如果没有备选默认服务，将此服务设为备选
        //        if (_fallbackProvider == null)
        //        {
        //            _fallbackProvider = cacheKey;
        //        }
        //        return service;
        //    }
        //}
        //catch (Exception e)
        //{
        //    _logger.LogError(e, "创建{Provider}服务失败, configId={ConfigId}", provider, configId);
        //}

        return new AliyunSttService();
    }

    /// <summary>
    /// 根据配置创建API类型的STT服务
    /// </summary>
    private ISttService CreateApiService(SysConfig config)
    {
        if (config == null)
        {
            return null;
        }

        string provider = config.Provider;

        // 根据提供商类型创建对应的服务实例
        if ("aliyun".Equals(provider))
        {
            return new AliyunSttService(config);
        }
        // 可以添加其他服务提供商的支持

        _logger.LogWarning("不支持的STT服务提供商: {Provider}", provider);
        return null;
    }
}