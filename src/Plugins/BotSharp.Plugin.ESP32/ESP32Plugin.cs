using BotSharp.Abstraction.Plugins;
using BotSharp.Abstraction.Settings;
using BotSharp.Plugin.ESP32.Settings;
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

    public void RegisterDI(IServiceCollection services, IConfiguration config)
    {
        services.AddScoped(provider =>
        {
            var settingService = provider.GetRequiredService<ISettingService>();
            return settingService.Bind<ESP32Setting>("ESP32");
        });

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
