using BotSharp.Plugin.ESP32.Models;
using BotSharp.Plugin.ESP32.Stt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BotSharp.Plugin.ESP32.Tts;

public class AliyunTtsService : ITtsService
{
    private static readonly ILogger<AliyunTtsService> _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AliyunTtsService>();

    private static readonly string PROVIDER_NAME = "aliyun";

    // 阿里云配置
    private readonly string _apiKey;
    private readonly string _voiceName;
    private readonly string _outputPath;

    public AliyunTtsService()
    {

    }

    public AliyunTtsService(SysConfig config, string voiceName, string outputPath)
    {
        _apiKey = config.ApiKey;
        _voiceName = voiceName;
        _outputPath = outputPath;
    }

    public string GetProviderName()
    {
        return PROVIDER_NAME;
    }

    public string GetAudioFileName()
    {
        string uuid = Guid.NewGuid().ToString("N");
        return uuid + ".mp3";
    }

    public string TextToSpeech(string text)
    {
        //try
        //{
        //    DirectoryInfo outputDir = new DirectoryInfo(_outputPath);
        //    if (!outputDir.Exists)
        //    {
        //        outputDir.Create();
        //    }

        //    if (_voiceName.Contains("sambert"))
        //    {
        //        return TtsSambert(text);
        //    }
        //    else if (GetVoiceByName(_voiceName) != null)
        //    {
        //        return TtsQwen(text);
        //    }
        //    else
        //    {
        //        return TtsCosyvoice(text);
        //    }
        //}
        //catch (Exception e)
        //{
        //    _logger.LogError(e, $"语音合成aliyun -使用{_voiceName}模型语音合成失败：");
        //    throw new Exception("语音合成失败");
        //}
        return string.Empty;
    }

    private string TtsQwen(string text)
    {
        //try
        //{
        //    AudioParameters.Voice voice = GetVoiceByName(_voiceName);
        //    var param = MultiModalConversationParam.Builder()
        //        .Model("qwen-tts")
        //        .ApiKey(_apiKey)
        //        .Text(text)
        //        .Voice(voice)
        //        .Build();

        //    var conv = new MultiModalConversation();
        //    var result = conv.Call(param);
        //    string audioUrl = result.Output.Audio.Url;
        //    string outPath = Path.Combine(_outputPath, GetAudioFileName());

        //    // 下载音频文件到本地
        //    using (var client = new WebClient())
        //    {
        //        client.DownloadFile(audioUrl, outPath);
        //    }

        //    return outPath;
        //}
        //catch (Exception e)
        //{
        //    _logger.LogError(e, $"语音合成aliyun -使用{_voiceName}模型语音合成失败：");
        //    return string.Empty;
        //}
        return string.Empty;
    }

    //private AudioParameters.Voice GetVoiceByName(string voiceName)
    //{
    //    switch (voiceName)
    //    {
    //        case "Chelsie":
    //            return AudioParameters.Voice.CHELSIE;
    //        case "Cherry":
    //            return AudioParameters.Voice.CHERRY;
    //        case "Ethan":
    //            return AudioParameters.Voice.ETHAN;
    //        case "Serena":
    //            return AudioParameters.Voice.SERENA;
    //        default:
    //            return null;
    //    }
    //}

    private string TtsCosyvoice(string text)
    {
        //try
        //{
        //    var param = SpeechSynthesisParam.Builder()
        //        .ApiKey(_apiKey)
        //        .Model("cosyvoice-v1")
        //        .Voice(_voiceName)
        //        .Build();

        //    var synthesizer = new SpeechSynthesizer(param, null);
        //    byte[] audio = synthesizer.Call(text).ToArray();
        //    string outPath = Path.Combine(_outputPath, GetAudioFileName());

        //    File.WriteAllBytes(outPath, audio);
        //    return outPath;
        //}
        //catch (Exception e)
        //{
        //    _logger.LogError(e, $"语音合成aliyun -使用{_voiceName}模型语音合成失败：");
        //    return string.Empty;
        //}
        return string.Empty;
    }

    public string TtsSambert(string text)
    {
        //try
        //{
        //    var param = SpeechSynthesisParam.Builder()
        //        .ApiKey(_apiKey)
        //        .Model(_voiceName)
        //        .Text(text)
        //        .SampleRate(AudioUtils.SAMPLE_RATE)
        //        .Format(SpeechSynthesisAudioFormat.MP3)
        //        .Build();

        //    var synthesizer = new SpeechSynthesizer();
        //    byte[] audio = synthesizer.Call(param).ToArray();
        //    string outPath = Path.Combine(_outputPath, GetAudioFileName());

        //    File.WriteAllBytes(outPath, audio);
        //    return outPath;
        //}
        //catch (Exception e)
        //{
        //    _logger.LogError(e, $"语音合成aliyun - 使用{_voiceName}模型失败：");
        //    return string.Empty;
        //}
        return string.Empty;
    }

    public void StreamTextToSpeech(string text, Action<byte[]> audioDataConsumer)
    {
        // TODO: Auto-generated method stub
        throw new NotImplementedException("Unimplemented method 'StreamTextToSpeech'");
    }
}
