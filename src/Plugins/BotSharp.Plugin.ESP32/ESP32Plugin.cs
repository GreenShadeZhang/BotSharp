using BotSharp.Abstraction.Plugins;
using BotSharp.Plugin.ESP32.Enums;
using BotSharp.Plugin.ESP32.LLM;
using BotSharp.Plugin.ESP32.Repository;
using BotSharp.Plugin.ESP32.Repository.Enums;
using BotSharp.Plugin.ESP32.Repository.LiteDB;
using BotSharp.Plugin.ESP32.Services;
using BotSharp.Plugin.ESP32.Services.Manager;
using BotSharp.Plugin.ESP32.Settings;
using BotSharp.Plugin.ESP32.Stt;
using BotSharp.Plugin.ESP32.Tts;
using BotSharp.Plugin.ESP32.Utils;
using BotSharp.Plugin.ESP32.Vad;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BotSharp.Plugin.ESP32;

public class ESP32Plugin : IBotSharpAppPlugin
{
    public string Id => "81909466-45a2-4c39-88c3-dcbc9fc87acc";
    public string Name => "ESP32";
    public string Description => "ESP32 xiaozhi";
    public string IconUrl => "https://w7.pngwing.com/pngs/918/671/png-transparent-twilio-full-logo-tech-companies.png";

    public string[] AgentIds => new string[]
{
        IoTAgentId.IoTDefaultAgent
};

    public void RegisterDI(IServiceCollection services, IConfiguration config)
    {
        var settings = new ESP32Setting();
        config.Bind("ESP32", settings);

        services.AddScoped(provider =>
        {
            return settings;
        });

        if (settings.DbDefault == IoTRepositoryEnum.LiteDBRepository)
        {
            services.AddScoped((IServiceProvider x) =>
            {
                var dbSettings = x.GetRequiredService<ESP32Setting>();
                return new LiteDBIoTDbContext(settings);
            });

            services.AddScoped<IIoTDeviceRepository, LiteDBIoTDeviceRepository>();
        }

        services.AddScoped<TarsosNoiseReducer>();
        services.AddScoped<AudioService>();
        services.AddScoped<DialogueService>();
        services.AddScoped<SessionManager>();
        services.AddScoped<VadService>();
        services.AddScoped<MessageService>();
        services.AddScoped<LlmManager>();

        services.AddScoped<ITtsProvider, AzureTtsProvider>();
        services.AddScoped<ISttProvider, AzureSttProvider>();

        services.AddScoped<IVadDetector, VadServiceAdapter>();

        services.AddScoped<IVadModel, SileroVadModel>();

        services.AddScoped<IIoTDeviceService, IoTDeviceService>();

        services.AddScoped<SttProviderFactory>();

        services.AddScoped<TtsProviderFactory>();

        services.AddScoped<OpusProcessor>();

        // 添加WebSocket支持
        services.AddWebSockets(options =>
        {
            options.KeepAliveInterval = TimeSpan.FromSeconds(120);
        });
    }


    public void Configure(IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

        var logger = app.ApplicationServices.GetRequiredService<ILogger<ESP32Plugin>>();

        // 启用WebSocket
        app.UseWebSockets();
        app.UseMiddleware<XiaoZhiStreamMiddleware>();

        logger.LogInformation("xiaozhi Message Handler is running on /ws/xiaozhi/v1/.");

    }
}
