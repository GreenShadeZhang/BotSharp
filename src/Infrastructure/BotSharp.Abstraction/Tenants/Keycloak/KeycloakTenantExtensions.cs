using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BotSharp.Abstraction.Tenants.Keycloak;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using System.Security.Claims;

namespace BotSharp.Infrastructure.Tenants.Keycloak;

/// <summary>
/// Keycloak 多租户配置扩展
/// </summary>
public static class KeycloakTenantExtensions
{
    public static IServiceCollection AddKeycloakMultiTenant(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var keycloakSettings = configuration.GetSection("Keycloak").Get<KeycloakSettings>();
        if (keycloakSettings == null)
        {
            throw new InvalidOperationException("Keycloak configuration is required");
        }

        services.AddSingleton(keycloakSettings);
        services.AddScoped<ITenantContextAccessor, TenantContextAccessor>();
        services.AddScoped<IKeycloakTenantService, KeycloakTenantService>();

        // 配置JWT认证
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = keycloakSettings.Authority;
                options.Audience = keycloakSettings.Audience;
                options.RequireHttpsMetadata = keycloakSettings.RequireHttpsMetadata;
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                // 自定义Claims映射
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        await MapKeycloakClaims(context);
                    }
                };
            });

        return services;
    }

    public static IApplicationBuilder UseKeycloakMultiTenant(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseMiddleware<KeycloakOrganizationMiddleware>();
        app.UseAuthorization();
        return app;
    }

    private static async Task MapKeycloakClaims(TokenValidatedContext context)
    {
        var identity = context.Principal?.Identity as ClaimsIdentity;
        if (identity == null) return;

        // 映射Keycloak特定的Claims
        var resourceAccess = context.Principal?.FindFirst("resource_access")?.Value;
        if (!string.IsNullOrEmpty(resourceAccess))
        {
            // 解析并添加资源访问权限Claims
            // 这里可以根据具体的JWT结构进行解析
        }

        // 添加组织相关Claims
        var orgId = context.Principal?.FindFirst("organization_id")?.Value;
        if (!string.IsNullOrEmpty(orgId))
        {
            identity.AddClaim(new Claim("tenant_id", orgId));
        }

        await Task.CompletedTask;
    }
}

/// <summary>
/// Keycloak 配置
/// </summary>
public class KeycloakSettings
{
    public string Authority { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public bool RequireHttpsMetadata { get; set; } = true;
    public string AdminApiUrl { get; set; } = string.Empty;
    public string AdminUsername { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
    public string Realm { get; set; } = "master";
}

/// <summary>
/// Keycloak 租户管理服务
/// </summary>
public interface IKeycloakTenantService
{
    Task<KeycloakOrganization?> GetOrganization(string organizationId);
    Task<List<KeycloakOrganization>> GetUserOrganizations(string userId);
    Task<bool> CheckOrganizationPermission(string organizationId, string userId, string permission);
    Task<KeycloakOrganization> CreateOrganization(CreateOrganizationRequest request);
    Task<bool> AddUserToOrganization(string organizationId, string userId, List<string> roles);
}

public class KeycloakOrganization
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Dictionary<string, object> Attributes { get; set; } = new();
    public bool Enabled { get; set; } = true;
    public List<KeycloakOrganizationMember> Members { get; set; } = new();
}

public class KeycloakOrganizationMember
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

public class CreateOrganizationRequest
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string OwnerUserId { get; set; } = string.Empty;
    public Dictionary<string, object> Attributes { get; set; } = new();
}
