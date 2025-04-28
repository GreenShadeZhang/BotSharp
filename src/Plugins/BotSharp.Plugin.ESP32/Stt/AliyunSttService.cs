using BotSharp.Plugin.ESP32.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Reactive.Linq;
using System.Reactive.Subjects;


namespace BotSharp.Plugin.ESP32.Stt;

public class AliyunSttService : ISttService
{
    private static readonly ILogger<AliyunSttService> _logger;
    private const string PROVIDER_NAME = "aliyun";

    private readonly string _apiKey = "";
    private readonly string _region = "eastus"; // 默认区域，可以从配置中读取

    public AliyunSttService()
    {
    }

    public AliyunSttService(SysConfig config)
    {
        //_apiKey = config.ApiKey;
        // 可以从 config.ApiUrl 中提取区域信息，如果有的话
        if (!string.IsNullOrEmpty(config.ApiUrl) && config.ApiUrl.Contains("."))
        {
            var parts = config.ApiUrl.Split('.');
            if (parts.Length > 1)
            {
                _region = parts[0];
            }
        }
    }

    public string GetProviderName()
    {
        return PROVIDER_NAME;
    }

    public bool SupportsStreaming()
    {
        return true;
    }

    public string Recognition(byte[] audioData)
    {
        // 单次识别暂未实现，可以根据需要添加
        _logger?.LogWarning("阿里云单次识别未实现，请使用流式识别");
        return null;
    }

    IObservable<string> ISttService.StreamRecognition(IObservable<byte[]> audioStream)
    {
        var resultSubject = new Subject<string>();

        // 创建语音配置
        var config = SpeechConfig.FromSubscription(_apiKey, _region);
        config.SpeechRecognitionLanguage = "zh-CN"; // 默认使用中文识别，可以根据需求调整

        // 创建推送流
        using var pushStream = AudioInputStream.CreatePushStream();
        using var audioConfig = AudioConfig.FromStreamInput(pushStream);

        // 创建语音识别器
        using var recognizer = new SpeechRecognizer(config, audioConfig);

        // 处理识别结果
        recognizer.Recognized += (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                var text = e.Result.Text;
                _logger?.LogInformation("识别结果（完成）: {0}", text);
                resultSubject.OnNext(text);
            }
        };

        // 处理中间识别结果
        recognizer.Recognizing += (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizingSpeech)
            {
                var text = e.Result.Text;
                _logger?.LogDebug("识别结果（中间）: {0}", text);
                resultSubject.OnNext(text);
            }
        };

        // 处理错误
        recognizer.Canceled += (s, e) =>
        {
            if (e.Reason == CancellationReason.Error)
            {
                var error = new Exception($"语音识别错误: {e.ErrorCode}, {e.ErrorDetails}");
                _logger?.LogError(error, "流式语音识别失败");
                resultSubject.OnError(error);
            }
        };

        // 处理会话结束
        recognizer.SessionStopped += (s, e) =>
        {
            _logger?.LogInformation("语音识别会话结束");
            resultSubject.OnCompleted();
        };

        // 开始连续识别
        recognizer.StartContinuousRecognitionAsync().GetAwaiter().GetResult();

        // 订阅音频流
        var subscription = audioStream.Subscribe(
            bytes =>
            {
                try
                {
                    // 将音频数据写入推送流
                    pushStream.Write(bytes);
                    try
                    {
                        // 添加数据有效性检查
                        if (bytes != null && bytes.Length > 0)
                        {
                            // 将音频数据写入推送流
                            pushStream.Write(bytes);
                        }
                        else
                        {
                            _logger?.LogWarning("收到空音频数据，已跳过");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "写入音频数据时发生错误: {0}, 数据长度: {1}",
                            ex.Message, bytes?.Length ?? 0);
                        resultSubject.OnError(ex);
                    }

                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "写入音频数据时发生错误");
                    resultSubject.OnError(ex);
                }
            },
            error =>
            {
                _logger?.LogError(error, "音频流发生错误");
                recognizer.StopContinuousRecognitionAsync().GetAwaiter().GetResult();
                resultSubject.OnError(error);
            },
            () =>
            {
                // 音频流结束，停止识别
                pushStream.Close();
                recognizer.StopContinuousRecognitionAsync().GetAwaiter().GetResult();
            }
        );

        // 当订阅被取消时，清理资源
        return resultSubject.AsObservable()
            .Finally(() =>
            {
                subscription.Dispose();
                recognizer.StopContinuousRecognitionAsync().GetAwaiter().GetResult();
            });
    }
}

// 为了兼容性保留的占位符类
public class ByteBuffer
{
    public static ByteBuffer Wrap(byte[] data) => new ByteBuffer();
}

public class RecognitionParam
{
    public class Builder
    {
        public Builder Model(string model) => this;
        public Builder Format(string format) => this;
        public Builder SampleRate(int sampleRate) => this;
        public Builder ApiKey(string apiKey) => this;
        public RecognitionParam Build() => new RecognitionParam();
    }
}

public class RecognitionResult
{
    public bool IsSentenceEnd() => false;
    public Sentence GetSentence() => new Sentence();
}

public class Sentence
{
    public string GetText() => "";
}

public static class AudioUtils
{
    public const int SAMPLE_RATE = 16000;
}
