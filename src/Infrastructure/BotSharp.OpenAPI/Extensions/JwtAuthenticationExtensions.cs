using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace BotSharp.OpenAPI.Extensions;

public static class JwtAuthenticationExtensions
{
    /// <summary>
    /// 添加JWT认证配置
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="config">配置</param>
    /// <param name="env">环境信息</param>
    /// <param name="enableValidation">是否启用验证</param>
    /// <param name="enableMixedAuth">是否启用混合认证(JWT + Cookie)</param>
    /// <returns></returns>
    public static AuthenticationBuilder AddBotSharpJwtAuthentication(
        this IServiceCollection services,
        IConfiguration config,
        IHostEnvironment env,
        bool enableValidation = true,
        bool enableMixedAuth = true)
    {
        var schema = enableMixedAuth ? "MIXED_SCHEME" : JwtBearerDefaults.AuthenticationScheme;
        
        var builder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = schema;
            options.DefaultChallengeScheme = schema;
            options.DefaultAuthenticateScheme = schema;
        });

        // 添加JWT Bearer认证
        builder.AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                ValidateIssuer = enableValidation,
                ValidateAudience = enableValidation,
                ValidateLifetime = enableValidation && !env.IsDevelopment(),
                ValidateIssuerSigningKey = enableValidation
            };

            if (!enableValidation)
            {
                options.TokenValidationParameters.SignatureValidator = (string token, TokenValidationParameters parameters) =>
                    new JsonWebToken(token);
            }
        });

        // 如果启用混合认证，添加Cookie认证和策略方案
        if (enableMixedAuth)
        {
            builder.AddCookie(options =>
            {
                // 跨域Cookie支持
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddPolicyScheme(schema, "Mixed authentication", options =>
            {
                // 每个请求都会运行
                options.ForwardDefaultSelector = context =>
                {
                    // 根据认证类型过滤
                    string authorization = context.Request.Headers[HeaderNames.Authorization];
                    if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                        return JwtBearerDefaults.AuthenticationScheme;
                    else if (context.Request.Cookies.ContainsKey(".AspNetCore.Cookies"))
                        return CookieAuthenticationDefaults.AuthenticationScheme;
                    else if (context.Request.Path.StartsWithSegments("/sso") && context.Request.Method == "GET")
                        return CookieAuthenticationDefaults.AuthenticationScheme;
                    else if (context.Request.Path.ToString().StartsWith("/signin-") && context.Request.Method == "GET")
                        return CookieAuthenticationDefaults.AuthenticationScheme;

                    // 否则总是检查cookie认证
                    return JwtBearerDefaults.AuthenticationScheme;
                };
            });
        }

        return builder;
    }

    /// <summary>
    /// 添加OAuth提供商认证
    /// </summary>
    /// <param name="builder">认证构建器</param>
    /// <param name="config">配置</param>
    /// <param name="onTicketReceived">票据接收回调</param>
    /// <returns></returns>
    public static AuthenticationBuilder AddOAuthProviders(
        this AuthenticationBuilder builder,
        IConfiguration config,
        Func<TicketReceivedContext, Task> onTicketReceived)
    {
        // GitHub OAuth
        if (!string.IsNullOrWhiteSpace(config["OAuth:GitHub:ClientId"]) && 
            !string.IsNullOrWhiteSpace(config["OAuth:GitHub:ClientSecret"]))
        {
            builder.AddGitHub(options =>
            {
                options.ClientId = config["OAuth:GitHub:ClientId"];
                options.ClientSecret = config["OAuth:GitHub:ClientSecret"];
                options.Events.OnTicketReceived = onTicketReceived;
            });
        }

        // Google Identity OAuth
        if (!string.IsNullOrWhiteSpace(config["OAuth:Google:ClientId"]) && 
            !string.IsNullOrWhiteSpace(config["OAuth:Google:ClientSecret"]))
        {
            builder.AddGoogle(options =>
            {
                options.ClientId = config["OAuth:Google:ClientId"];
                options.ClientSecret = config["OAuth:Google:ClientSecret"];
                options.Events.OnTicketReceived = onTicketReceived;
            });
        }

        // Keycloak Identity OAuth
        if (!string.IsNullOrWhiteSpace(config["OAuth:Keycloak:ClientId"]) && 
            !string.IsNullOrWhiteSpace(config["OAuth:Keycloak:ClientSecret"]))
        {
            builder.AddKeycloak(options =>
            {
                options.BaseAddress = new Uri(config["OAuth:Keycloak:BaseAddress"]);
                options.Realm = config["OAuth:Keycloak:Realm"];
                options.ClientId = config["OAuth:Keycloak:ClientId"];
                options.ClientSecret = config["OAuth:Keycloak:ClientSecret"];
                options.AccessType = AspNet.Security.OAuth.Keycloak.KeycloakAuthenticationAccessType.Confidential;
                int version = Convert.ToInt32(config["OAuth:Keycloak:Version"] ?? "22");
                options.Version = new Version(version, 0);
                options.Events.OnTicketReceived = onTicketReceived;
            });
        }

        // Weixin OAuth
        if (!string.IsNullOrWhiteSpace(config["OAuth:Wexin:ClientId"]) && 
            !string.IsNullOrWhiteSpace(config["OAuth:Wexin:ClientSecret"]))
        {
            builder.AddWeixin(options =>
            {
                options.ClientId = config["OAuth:GitHub:ClientId"];
                options.ClientSecret = config["OAuth:GitHub:ClientSecret"];
                options.Scope.Add("user:email");
                options.Backchannel = builder.Services.BuildServiceProvider()
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient();
                options.Events.OnTicketReceived = onTicketReceived;
            });
        }

        return builder;
    }
}
