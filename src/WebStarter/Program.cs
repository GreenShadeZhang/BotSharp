using BotSharp.Abstraction.Messaging.JsonConverters;
using BotSharp.Core;
using BotSharp.Core.MCP;
using BotSharp.Logger;
using BotSharp.OpenAPI;
using BotSharp.Plugin.ChatHub;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

string[] allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[]
    {
        "http://0.0.0.0:5015",
        "https://botsharp.scisharpstack.org",
        "https://chat.scisharpstack.org"
    };

// Add BotSharp
builder.Services.AddBotSharpCore(builder.Configuration, options =>
{
    options.JsonSerializerOptions.Converters.Add(new RichContentJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new TemplateMessageJsonConverter());
}).AddBotSharpOpenAPIWithOidcAuth(builder.Configuration, allowedOrigins, builder.Environment)
  .AddBotSharpMCP(builder.Configuration)
  .AddBotSharpLogger(builder.Configuration);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add SignalR for WebSocket
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redis =>
    {
        var redisConfiguration = builder.Configuration["Database:Redis"];
        if (!string.IsNullOrEmpty(redisConfiguration))
        {
            var literal = builder.Environment.IsProduction() ? "ai-forge" : "ai-forge-dev";
            redis.Configuration.ChannelPrefix = RedisChannel.Literal(literal);
            redis.ConnectionFactory = async (writer) =>
            {
                var connection = await ConnectionMultiplexer.ConnectAsync(redisConfiguration);
                connection.ConnectionFailed += (_, e) =>
                {
                    Console.WriteLine("Connection to Redis failed.");
                };

                if (!connection.IsConnected)
                {
                    Console.WriteLine("Did not connect to Redis.");
                }
                return connection;
            };
        }
    });

var app = builder.Build();

app.UseWebSockets();

// Enable SignalR
app.MapHub<SignalRHub>("/chatHub");
app.UseMiddleware<ChatHubMiddleware>();
app.UseMiddleware<ChatStreamMiddleware>();

// Use BotSharp
app.UseBotSharp()
    .UseBotSharpOpenAPI(app.Environment, true)
    .UseBotSharpUI();

app.Run();