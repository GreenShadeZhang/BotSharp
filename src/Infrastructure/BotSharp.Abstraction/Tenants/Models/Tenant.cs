using System.ComponentModel.DataAnnotations;
using BotSharp.Abstraction.Users.Models;

namespace BotSharp.Abstraction.Tenants.Models;

/// <summary>
/// 多租户模型 - 支持SaaS化部署
/// </summary>
public class Tenant
{
    [Required]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Subdomain { get; set; }
    
    [Required]
    public string PlanId { get; set; } = "free";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public TenantStatus Status { get; set; } = TenantStatus.Active;
    
    public TenantSettings Settings { get; set; } = new();
    
    public List<TenantUser> Users { get; set; } = new();
    
    public SubscriptionInfo Subscription { get; set; } = new();
}

public enum TenantStatus
{
    Active,
    Suspended,
    Trial,
    Cancelled
}

public class TenantSettings
{
    public string? CustomDomain { get; set; }
    public string? BrandingLogo { get; set; }
    public string? PrimaryColor { get; set; }
    public bool AllowUserRegistration { get; set; } = true;
    public bool RequireEmailVerification { get; set; } = true;
    public int MaxAgents { get; set; } = 1;
    public int MaxConversationsPerMonth { get; set; } = 100;
    public int MaxAPICallsPerMonth { get; set; } = 1000;
    public List<string> EnabledFeatures { get; set; } = new();
}

public class TenantUser
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TenantRole Role { get; set; } = TenantRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

public enum TenantRole
{
    Owner,
    Admin,
    Member,
    Viewer
}

public class SubscriptionInfo
{
    public string PlanId { get; set; } = "free";
    public decimal MonthlyPrice { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public bool AutoRenew { get; set; } = true;
    public string? PaymentMethodId { get; set; }
    public DateTime? CancelledAt { get; set; }
    public UsageMetrics Usage { get; set; } = new();
}

public class UsageMetrics
{
    public int AgentsCreated { get; set; }
    public int ConversationsThisMonth { get; set; }
    public int APICallsThisMonth { get; set; }
    public long StorageUsedMB { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
