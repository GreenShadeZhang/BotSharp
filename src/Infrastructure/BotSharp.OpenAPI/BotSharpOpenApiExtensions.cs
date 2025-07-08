using BotSharp.Abstraction.Messaging.JsonConverters;
using BotSharp.Core.Users.Services;
using BotSharp.OpenAPI.BackgroundServices;
using BotSharp.OpenAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;

namespace BotSharp.OpenAPI;

public static class BotSharpOpenApiExtensions
{
    private static string policy = "BotSharpCorsPolicy";

    /// <summary>
    /// Add Swagger/OpenAPI without authentication
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddBotSharpOpenAPI(this IServiceCollection services, IConfiguration config, string[] origins, IHostEnvironment env)
    {
        services.AddScoped<IUserIdentity, UserIdentity>();
        services.AddHostedService<ConversationTimeoutService>();

        // Add services to the container.
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new RichContentJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new TemplateMessageJsonConverter());
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/

        services.AddHttpContextAccessor();

        services.AddCors(options =>
        {
            options.AddPolicy(policy,
                builder => builder.WithOrigins(origins)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials());
        });

        return services;
    }

    /// <summary>
    /// Use Swagger/OpenAPI
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    /// <param name="useAuthentication">是否使用认证中间件</param>
    /// <returns></returns>
    public static IApplicationBuilder UseBotSharpOpenAPI(this IApplicationBuilder app, IHostEnvironment env, bool useAuthentication = true)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        app.UseCors(policy);

        //app.UseSwagger();

        if (env.IsDevelopment())
        {
            IdentityModelEventSource.ShowPII = true;
            //app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        // 只在需要时添加认证中间件
        if (useAuthentication)
        {
            app.UseAuthentication();
        }

        app.UseRouting();

        // 只在需要时添加授权中间件
        if (useAuthentication)
        {
            app.UseAuthorization();
        }

        app.UseEndpoints(
            endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

        return app;
    }

    /// <summary>
    /// Host BotSharp UI built in adapter-static
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IApplicationBuilder UseBotSharpUI(this IApplicationBuilder app, bool isDevelopment = false)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        // app.UseFileServer();
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseEndpoints(
            endpoints =>
            {
                // For SPA static file routing
                endpoints.MapFallbackToFile("/index.html");
            });

        //app.UseSpa(config =>
        //{
        //    if (isDevelopment)
        //    {
        //        config.UseProxyToSpaDevelopmentServer("http://localhost:5015");
        //    }
        //});

        return app;
    }

    /// <summary>
    /// Add Swagger/OpenAPI with OIDC authentication
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <param name="origins"></param>
    /// <param name="env"></param>
    /// <param name="autoCreateUser">是否自动创建用户</param>
    /// <returns></returns>
    public static IServiceCollection AddBotSharpOpenAPIWithOidcAuth(this IServiceCollection services,
        IConfiguration config,
        string[] origins,
        IHostEnvironment env,
        bool autoCreateUser = true)
    {
        // 添加基础OpenAPI服务
        services.AddBotSharpOpenAPI(config, origins, env);

        // 添加OIDC认证
        services.AddBotSharpOidcAuthentication(config, env, autoCreateUser);

        //// 重新配置Swagger以支持OIDC认证
        //services.Configure<SwaggerGenOptions>(c =>
        //{
        //    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        //    {
        //        In = ParameterLocation.Header,
        //        Description = "Please insert JWT token with Bearer into field",
        //        Name = "Authorization",
        //        Type = SecuritySchemeType.ApiKey
        //    });
        //    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        //        {
        //            new OpenApiSecurityScheme
        //            {
        //                Reference = new OpenApiReference
        //                {
        //                    Type = ReferenceType.SecurityScheme,
        //                    Id = "Bearer"
        //                }
        //            },
        //            Array.Empty<string>()
        //        }
        //    });
        //});

        return services;
    }
}

