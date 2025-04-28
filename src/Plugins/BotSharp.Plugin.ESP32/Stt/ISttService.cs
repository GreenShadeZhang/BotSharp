namespace BotSharp.Plugin.ESP32.Stt;
/// <summary>
/// STT服务接口
/// </summary>
public interface ISttService
{
    /// <summary>
    /// 获取服务提供商名称
    /// </summary>
    string GetProviderName();

    /// <summary>
    /// 处理音频数据（非流式）
    /// </summary>
    /// <param name="audioData">音频字节数组</param>
    /// <returns>识别的文本结果</returns>
    string Recognition(byte[] audioData);

    /// <summary>
    /// 流式处理音频数据
    /// </summary>
    /// <param name="audioStream">音频数据流</param>
    /// <returns>识别的文本结果流</returns>
    IObservable<string> StreamRecognition(IObservable<byte[]> audioStream);

    /// <summary>
    /// 检查服务是否支持流式处理
    /// </summary>
    /// <returns>是否支持流式处理</returns>
    bool SupportsStreaming() => false;
}