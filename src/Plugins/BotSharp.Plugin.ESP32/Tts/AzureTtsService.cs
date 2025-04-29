using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;

namespace BotSharp.Plugin.ESP32.Tts
{
    public class AzureTtsService : ITtsService
    {
        private readonly IConfiguration _configuration;
        private string _outputFolder;
        private string _subscriptionKey;
        private string _region;
        private string _voice;
        private string _outputFormat;

        public AzureTtsService(IConfiguration configuration)
        {
            _configuration = configuration;
            // 从配置中获取Azure语音服务的设置
            _subscriptionKey = _configuration["AzureSpeech:SubscriptionKey"] ?? "";
            _region = _configuration["AzureSpeech:Region"] ?? "eastus";
            _voice = _configuration["AzureSpeech:Voice"] ?? "zh-CN-XiaoxiaoNeural";
            _outputFormat = _configuration["AzureSpeech:OutputFormat"] ?? "riff-16khz-16bit-mono-pcm";
            _outputFolder = _configuration["AzureSpeech:OutputFolder"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tts_output");

            // 确保输出目录存在
            if (!Directory.Exists(_outputFolder))
            {
                Directory.CreateDirectory(_outputFolder);
            }
        }

        public string GetAudioFileName()
        {
            // 生成唯一的文件名
            return $"azure_tts_{Guid.NewGuid()}.wav";
        }

        public string GetProviderName()
        {
            return "Azure TTS";
        }

        public void StreamTextToSpeech(string text, Action<byte[]> audioDataConsumer)
        {

        }

        public string TextToSpeech(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            string fileName = GetAudioFileName();
            string filePath = Path.Combine(_outputFolder, fileName);

            // 配置语音合成
            var config = SpeechConfig.FromSubscription(_subscriptionKey, _region);
            config.SpeechSynthesisVoiceName = _voice;
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff16Khz16BitMonoPcm);

            // 创建音频输出文件
            using var audioConfig = AudioConfig.FromWavFileOutput(filePath);

            // 创建语音合成器
            using var synthesizer = new SpeechSynthesizer(config, audioConfig);

            // 开始合成语音
            var result = synthesizer.SpeakTextAsync(text).GetAwaiter().GetResult();

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                return filePath;
            }
            else
            {
                throw new Exception($"语音合成失败：{result.Reason}");
            }
        }

        public async Task<byte[]> TextToSpeechAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<byte>();

            // 配置语音合成
            var config = SpeechConfig.FromSubscription(_subscriptionKey, _region);
            config.SpeechSynthesisVoiceName = _voice;
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff16Khz16BitMonoPcm);

            // 创建内存流接收音频数据
            using var memoryStream = new MemoryStream();
            using var audioConfig = AudioConfig.FromStreamOutput(AudioOutputStream.CreatePullStream());

            // 创建语音合成器
            using var synthesizer = new SpeechSynthesizer(config, audioConfig);

            // 开始合成语音
            var result = await synthesizer.SpeakTextAsync(text);

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                // 将音频数据写入内存流
                var audioData = result.AudioData;
                return audioData;
            }
            else
            {
                throw new Exception($"语音合成失败：{result.Reason}");
            }
        }
    }
}
