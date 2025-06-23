using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BotSharp.Abstraction.Tenants.Keycloak;

/// <summary>
/// Keycloak Organization 多租户中间件
/// </summary>
public class KeycloakOrganizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<KeycloakOrganizationMiddleware> _logger;

    public KeycloakOrganizationMiddleware(RequestDelegate next, ILogger<KeycloakOrganizationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 从JWT Token中提取Organization信息
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var organizationId = ExtractOrganizationId(context.User);
            var organizationName = ExtractOrganizationName(context.User);
            var userRoles = ExtractOrganizationRoles(context.User);

            if (!string.IsNullOrEmpty(organizationId))
            {
                // 设置当前租户上下文
                var tenantContext = new TenantContext
                {
                    TenantId = organizationId,
                    TenantName = organizationName,
                    UserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "",
                    UserRoles = userRoles,
                    IsMultiTenant = true
                };

                context.Items["TenantContext"] = tenantContext;
                _logger.LogInformation("Tenant context set: {TenantId}", organizationId);
            }
        }

        await _next(context);
    }

    private string? ExtractOrganizationId(ClaimsPrincipal user)
    {
        // Keycloak Organization ID 通常在这些Claims中
        return user.FindFirst("organization_id")?.Value ??
               user.FindFirst("org_id")?.Value ??
               user.FindFirst("https://keycloak.org/organization/id")?.Value;
    }

    private string? ExtractOrganizationName(ClaimsPrincipal user)
    {
        return user.FindFirst("organization_name")?.Value ??
               user.FindFirst("org_name")?.Value ??
               user.FindFirst("https://keycloak.org/organization/name")?.Value;
    }

    private List<string> ExtractOrganizationRoles(ClaimsPrincipal user)
    {
        var roles = new List<string>();

        // 提取组织级别的角色
        var orgRoles = user.FindFirst("organization_roles")?.Value;
        if (!string.IsNullOrEmpty(orgRoles))
        {
            roles.AddRange(orgRoles.Split(',').Select(r => r.Trim()));
        }

        // 提取realm角色
        var realmRoles = user.FindAll("realm_access")?.Select(c => c.Value);
        if (realmRoles != null)
        {
            roles.AddRange(realmRoles);
        }

        return roles.Distinct().ToList();
    }
}

/// <summary>
/// 租户上下文
/// </summary>
public class TenantContext
{
    public string TenantId { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public List<string> UserRoles { get; set; } = new();
    public bool IsMultiTenant { get; set; } = false;
}

/// <summary>
/// 租户上下文访问器
/// </summary>
public interface ITenantContextAccessor
{
    TenantContext? Current { get; }
    bool HasTenant { get; }
    bool HasRole(string role);
}

public class TenantContextAccessor : ITenantContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public TenantContext? Current =>
        _httpContextAccessor.HttpContext?.Items["TenantContext"] as TenantContext;

    public bool HasTenant => !string.IsNullOrEmpty(Current?.TenantId);

    public bool HasRole(string role) =>
        Current?.UserRoles.Contains(role, StringComparer.OrdinalIgnoreCase) == true;
}
