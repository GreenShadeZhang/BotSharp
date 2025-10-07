using BotSharp.Abstraction.Plugins;
using BotSharp.Abstraction.Plugins.Models;
using BotSharp.Abstraction.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BotSharp.Plugin.AgUi;

public class AgUiPlugin : IBotSharpPlugin
{
    public string Id => "d8e3f7a9-4b2c-4a1d-9e5f-6c7d8e9f0a1b";
    public string Name => "AG-UI Protocol";
    public string Description => "AG-UI (Agent-User Interaction) protocol implementation for BotSharp. Provides standardized event-based communication between AI agents and user-facing applications.";
    public string? IconUrl => "https://github.com/user-attachments/assets/ebc0dd08-8732-4519-9b6c-452ce54d8058";
    
    public void RegisterDI(IServiceCollection services, IConfiguration config)
    {
        // No additional services needed for now
    }
}
