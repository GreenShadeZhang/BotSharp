namespace BotSharp.Abstraction.Billing.Models;

/// <summary>
/// 订阅计划模型
/// </summary>
public class SubscriptionPlan
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public string Currency { get; set; } = "USD";
    
    public PlanLimits Limits { get; set; } = new();
    public List<string> Features { get; set; } = new();
    public List<string> Integrations { get; set; } = new();
    
    public bool IsPopular { get; set; } = false;
    public bool IsCustom { get; set; } = false;
    public bool IsActive { get; set; } = true;
    
    public int SortOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class PlanLimits
{
    public int MaxAgents { get; set; } = 1;
    public int MaxConversationsPerMonth { get; set; } = 100;
    public int MaxAPICallsPerMonth { get; set; } = 1000;
    public long MaxStorageMB { get; set; } = 100;
    public int MaxTeamMembers { get; set; } = 1;
    public int MaxIntegrations { get; set; } = 3;
    public int MaxCustomDomains { get; set; } = 0;
    public bool SupportsSSO { get; set; } = false;
    public bool SupportsPriority { get; set; } = false;
    public bool SupportsWhiteLabel { get; set; } = false;
    public bool SupportsAPI { get; set; } = false;
    public bool SupportsWebhooks { get; set; } = false;
    public bool SupportsAnalytics { get; set; } = false;
}

/// <summary>
/// 账单记录
/// </summary>
public class Invoice
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    
    public DateTime BillingPeriodStart { get; set; }
    public DateTime BillingPeriodEnd { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    public DateTime? DueDate { get; set; }
    
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public string Currency { get; set; } = "USD";
    
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
    public List<InvoiceLineItem> LineItems { get; set; } = new();
    
    public string? PaymentIntentId { get; set; }
    public string? PaymentMethodId { get; set; }
    public string? FailureReason { get; set; }
}

public class InvoiceLineItem
{
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty; // subscription, usage, addon
}

public enum InvoiceStatus
{
    Pending,
    Paid,
    Failed,
    Cancelled,
    Refunded
}

/// <summary>
/// 使用量计费项
/// </summary>
public class UsageBillingItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // api_calls, conversations, storage
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime BillingDate { get; set; }
    
    public string? Description { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// 预定义的订阅计划
/// </summary>
public static class DefaultPlans
{
    public static readonly SubscriptionPlan FreePlan = new()
    {
        Id = "free",
        Name = "free",
        DisplayName = "免费版",
        Description = "适合个人用户和小团队试用",
        MonthlyPrice = 0,
        YearlyPrice = 0,
        Limits = new PlanLimits
        {
            MaxAgents = 1,
            MaxConversationsPerMonth = 100,
            MaxAPICallsPerMonth = 1000,
            MaxStorageMB = 100,
            MaxTeamMembers = 1,
            MaxIntegrations = 2,
            SupportsAPI = false,
            SupportsAnalytics = false
        },
        Features = new()
        {
            "1个AI助手",
            "100次对话/月",
            "基础模板库",
            "社区支持",
            "基础集成"
        }
    };
    
    public static readonly SubscriptionPlan ProPlan = new()
    {
        Id = "pro",
        Name = "pro",
        DisplayName = "专业版",
        Description = "适合中小企业和专业用户",
        MonthlyPrice = 29,
        YearlyPrice = 290, // 2个月免费
        Limits = new PlanLimits
        {
            MaxAgents = 10,
            MaxConversationsPerMonth = 5000,
            MaxAPICallsPerMonth = 50000,
            MaxStorageMB = 1000,
            MaxTeamMembers = 5,
            MaxIntegrations = 10,
            SupportsAPI = true,
            SupportsAnalytics = true,
            SupportsWebhooks = true
        },
        Features = new()
        {
            "10个AI助手",
            "5000次对话/月",
            "高级模板库",
            "API访问",
            "高级集成",
            "使用分析",
            "邮件支持",
            "Webhook支持"
        },
        IsPopular = true
    };
    
    public static readonly SubscriptionPlan EnterprisePlan = new()
    {
        Id = "enterprise",
        Name = "enterprise",
        DisplayName = "企业版",
        Description = "适合大型企业和高级用户",
        MonthlyPrice = 199,
        YearlyPrice = 1990,
        Limits = new PlanLimits
        {
            MaxAgents = -1, // 无限制
            MaxConversationsPerMonth = -1,
            MaxAPICallsPerMonth = -1,
            MaxStorageMB = -1,
            MaxTeamMembers = 50,
            MaxIntegrations = -1,
            MaxCustomDomains = 5,
            SupportsSSO = true,
            SupportsPriority = true,
            SupportsWhiteLabel = true,
            SupportsAPI = true,
            SupportsWebhooks = true,
            SupportsAnalytics = true
        },
        Features = new()
        {
            "无限AI助手",
            "无限对话",
            "完整模板库",
            "高级API",
            "所有集成",
            "高级分析",
            "单点登录(SSO)",
            "白标定制",
            "专属客户经理",
            "SLA保障",
            "优先支持"
        }
    };
}
