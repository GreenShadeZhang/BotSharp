using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using BotSharp.Abstraction.Users;
using BotSharp.Abstraction.Users.Models;
using BotSharp.OpenAPI.Settings;

namespace BotSharp.OpenAPI.Extensions;

public static class OidcAuthenticationExtensions
{    /// <summary>
    /// 添加OIDC JWT认证配置
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="config">配置</param>
    /// <param name="env">环境信息</param>
    /// <param name="autoCreateUser">是否自动创建用户</param>
    /// <returns></returns>
    public static AuthenticationBuilder AddBotSharpOidcAuthentication(
        this IServiceCollection services,
        IConfiguration config,
        IHostEnvironment env,
        bool autoCreateUser = true)
    {
        // 绑定OIDC配置
        var oidcSettings = new OidcSettings();
        config.GetSection("Oidc").Bind(oidcSettings);
        
        
        if (!oidcSettings.IsValid())
        {
            throw new InvalidOperationException("OIDC configuration is invalid. Please check Authority, Realm, and ClientId settings.");
        }

        // 注册配置服务
        services.Configure<OidcSettings>(config.GetSection("Oidc"));
        
        var issuer = oidcSettings.GetIssuerUrl();
        
        var builder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = issuer;
                options.Audience = oidcSettings.Audience;
                options.RequireHttpsMetadata = env.IsDevelopment() ? oidcSettings.RequireHttpsMetadata : true;
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = !string.IsNullOrEmpty(oidcSettings.Audience),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = oidcSettings.Audience,
                    ClockSkew = TimeSpan.FromMinutes(oidcSettings.ClockSkewMinutes),
                    // 映射标准Claims
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role
                };

                // 添加事件处理
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        if (autoCreateUser)
                        {
                            await HandleUserSynchronization(context, oidcSettings);
                        }
                    },
                    OnAuthenticationFailed = context =>
                    {
                        // 记录认证失败的详细信息
                        return Task.CompletedTask;
                    }
                };
            });

        return builder;
    }    /// <summary>
    /// 处理用户同步
    /// </summary>
    private static async Task HandleUserSynchronization(TokenValidatedContext context, OidcSettings oidcSettings)
    {
        try
        {
            var services = context.HttpContext.RequestServices;
            var userService = services.GetRequiredService<IUserService>();
            var claims = context.Principal.Claims.ToList();
            var userInfo = ExtractUserInfoFromClaims(claims, oidcSettings);

            if (string.IsNullOrEmpty(userInfo.Id))
            {
                Console.WriteLine("Cannot extract user ID from OIDC token claims");
                return;
            }

            // 尝试获取现有用户
            var existingUser = await userService.GetUser(userInfo.Id);
            
            if (existingUser == null)
            {
                // 创建新用户
                Console.WriteLine("Creating new user from OIDC token: {UserId}", userInfo.Id);
                await userService.CreateUser(userInfo);
            }
            else
            {
                // 更新现有用户信息
                Console.WriteLine("User already exists, updating info: {UserId}", userInfo.Id);
                
                // 更新用户信息（如邮箱、姓名等可能会变化）
                existingUser.Email = userInfo.Email ?? existingUser.Email;
                existingUser.FirstName = userInfo.FirstName ?? existingUser.FirstName;
                existingUser.LastName = userInfo.LastName ?? existingUser.LastName;
                existingUser.UserName = userInfo.UserName ?? existingUser.UserName;
                
                await userService.UpdateUser(existingUser);
            }

            // 调用认证Hook
            var hooks = services.GetServices<IAuthenticationHook>();
            foreach (var hook in hooks)
            {
                var user = existingUser ?? userInfo;
                hook.UserAuthenticated(user, null); // Token为null因为这里不是BotSharp颁发的token
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error during user synchronization from OIDC token");
        }
    }    /// <summary>
    /// 从JWT Claims中提取用户信息
    /// </summary>
    private static User ExtractUserInfoFromClaims(List<Claim> claims, OidcSettings oidcSettings)
    {
        var user = new User();

        // 标准OIDC Claims映射
        user.Id = GetClaimValue(claims, "sub") ?? GetClaimValue(claims, ClaimTypes.NameIdentifier);
        user.UserName = GetClaimValue(claims, "preferred_username") ?? GetClaimValue(claims, ClaimTypes.Name) ?? user.Id;
        user.Email = GetClaimValue(claims, "email") ?? GetClaimValue(claims, ClaimTypes.Email);
        user.FirstName = GetClaimValue(claims, "given_name") ?? GetClaimValue(claims, ClaimTypes.GivenName);
        user.LastName = GetClaimValue(claims, "family_name") ?? GetClaimValue(claims, ClaimTypes.Surname);
        
        // 其他可能的Claims
        user.Phone = GetClaimValue(claims, "phone_number");
        user.Source = "OIDC";
        user.Type = GetClaimValue(claims, "user_type") ?? "Client";
        user.Verified = true; // OIDC认证的用户默认已验证
        
        // 角色映射
        var roles = claims.Where(c => c.Type == "realm_access" || c.Type == ClaimTypes.Role || c.Type == "roles")
                         .SelectMany(c => ExtractRoles(c.Value))
                         .ToList();
        
        user.Role = DetermineUserRole(roles, oidcSettings.RoleMapping);

        // 外部ID
        user.ExternalId = user.Id;

        return user;
    }

    /// <summary>
    /// 获取Claim值
    /// </summary>
    private static string? GetClaimValue(List<Claim> claims, string claimType)
    {
        return claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }

    /// <summary>
    /// 从角色Claim值中提取角色列表
    /// </summary>
    private static IEnumerable<string> ExtractRoles(string roleValue)
    {
        if (string.IsNullOrEmpty(roleValue))
            return Enumerable.Empty<string>();

        try
        {
            // 尝试解析JSON格式的角色（Keycloak格式）
            if (roleValue.StartsWith("{") && roleValue.Contains("roles"))
            {
                var json = System.Text.Json.JsonDocument.Parse(roleValue);
                if (json.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    return rolesElement.EnumerateArray().Select(r => r.GetString()).Where(r => r != null)!;
                }
            }
            
            // 简单字符串或逗号分隔
            return roleValue.Split(',', ';').Select(r => r.Trim()).Where(r => !string.IsNullOrEmpty(r));
        }
        catch
        {
            return new[] { roleValue };
        }
    }    /// <summary>
    /// 根据OIDC角色确定BotSharp用户角色
    /// </summary>
    private static string DetermineUserRole(List<string> oidcRoles, Dictionary<string, string> roleMapping)
    {
        foreach (var oidcRole in oidcRoles)
        {
            if (roleMapping.TryGetValue(oidcRole.ToLower(), out var botSharpRole))
            {
                return botSharpRole;
            }
        }

        return "User"; // 默认角色
    }
}
