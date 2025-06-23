using BotSharp.Abstraction.Tenants.Models;

namespace BotSharp.Abstraction.Tenants;

/// <summary>
/// 租户管理服务接口
/// </summary>
public interface ITenantService
{
    /// <summary>
    /// 创建新租户
    /// </summary>
    Task<Tenant> CreateTenant(CreateTenantRequest request);
    
    /// <summary>
    /// 获取租户信息
    /// </summary>
    Task<Tenant?> GetTenant(string tenantId);
    
    /// <summary>
    /// 根据域名获取租户
    /// </summary>
    Task<Tenant?> GetTenantByDomain(string domain);
    
    /// <summary>
    /// 更新租户信息
    /// </summary>
    Task<bool> UpdateTenant(Tenant tenant);
    
    /// <summary>
    /// 获取用户的租户列表
    /// </summary>
    Task<List<Tenant>> GetUserTenants(string userId);
    
    /// <summary>
    /// 添加用户到租户
    /// </summary>
    Task<bool> AddUserToTenant(string tenantId, string userId, TenantRole role);
    
    /// <summary>
    /// 从租户移除用户
    /// </summary>
    Task<bool> RemoveUserFromTenant(string tenantId, string userId);
    
    /// <summary>
    /// 更新用户租户角色
    /// </summary>
    Task<bool> UpdateUserRole(string tenantId, string userId, TenantRole role);
    
    /// <summary>
    /// 检查用户权限
    /// </summary>
    Task<bool> HasPermission(string tenantId, string userId, string permission);
    
    /// <summary>
    /// 更新使用量统计
    /// </summary>
    Task UpdateUsage(string tenantId, UsageType type, int increment = 1);
    
    /// <summary>
    /// 检查使用限制
    /// </summary>
    Task<bool> CheckUsageLimit(string tenantId, UsageType type);
    
    /// <summary>
    /// 获取租户使用统计
    /// </summary>
    Task<UsageMetrics> GetUsageStats(string tenantId);
}

public class CreateTenantRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Subdomain { get; set; }
    public string PlanId { get; set; } = "free";
    public string OwnerUserId { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
}

public enum UsageType
{
    AgentCreation,
    Conversation,
    APICall,
    Storage
}
