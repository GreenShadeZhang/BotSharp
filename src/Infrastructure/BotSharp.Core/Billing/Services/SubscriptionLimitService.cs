using BotSharp.Abstraction.Billing.Models;
using BotSharp.Abstraction.Tenants.Keycloak;

namespace BotSharp.Core.Billing.Services;

/// <summary>
/// 订阅限制服务 - 基于Keycloak Organization的租户限制
/// </summary>
public class SubscriptionLimitService : ISubscriptionLimitService
{
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUsageTrackingService _usageTracking;
    private readonly ILogger<SubscriptionLimitService> _logger;

    public SubscriptionLimitService(
        ITenantContextAccessor tenantContext,
        ISubscriptionService subscriptionService,
        IUsageTrackingService usageTracking,
        ILogger<SubscriptionLimitService> logger)
    {
        _tenantContext = tenantContext;
        _subscriptionService = subscriptionService;
        _usageTracking = usageTracking;
        _logger = logger;
    }

    /// <summary>
    /// 检查Agent创建限制
    /// </summary>
    public async Task<LimitCheckResult> CheckAgentCreationLimit()
    {
        var tenantId = _tenantContext.Current?.TenantId;
        if (string.IsNullOrEmpty(tenantId))
        {
            return LimitCheckResult.Denied("未找到租户信息");
        }

        var subscription = await _subscriptionService.GetActiveSubscription(tenantId);
        if (subscription == null)
        {
            return LimitCheckResult.Denied("未找到有效订阅");
        }

        var currentUsage = await _usageTracking.GetCurrentUsage(tenantId);
        var plan = await _subscriptionService.GetPlan(subscription.PlanId);

        // 检查Agent数量限制
        if (plan.Limits.MaxAgents > 0 && currentUsage.AgentsCreated >= plan.Limits.MaxAgents)
        {
            return LimitCheckResult.Denied(
                $"已达到最大Agent数量限制 ({plan.Limits.MaxAgents})",
                LimitType.MaxAgents,
                currentUsage.AgentsCreated,
                plan.Limits.MaxAgents);
        }

        return LimitCheckResult.Allowed();
    }

    /// <summary>
    /// 检查对话次数限制
    /// </summary>
    public async Task<LimitCheckResult> CheckConversationLimit()
    {
        var tenantId = _tenantContext.Current?.TenantId;
        if (string.IsNullOrEmpty(tenantId))
        {
            return LimitCheckResult.Denied("未找到租户信息");
        }

        var subscription = await _subscriptionService.GetActiveSubscription(tenantId);
        var currentUsage = await _usageTracking.GetCurrentUsage(tenantId);
        var plan = await _subscriptionService.GetPlan(subscription.PlanId);

        // 检查月度对话限制
        if (plan.Limits.MaxConversationsPerMonth > 0 && 
            currentUsage.ConversationsThisMonth >= plan.Limits.MaxConversationsPerMonth)
        {
            return LimitCheckResult.Denied(
                $"已达到月度对话次数限制 ({plan.Limits.MaxConversationsPerMonth})",
                LimitType.MonthlyConversations,
                currentUsage.ConversationsThisMonth,
                plan.Limits.MaxConversationsPerMonth);
        }

        return LimitCheckResult.Allowed();
    }

    /// <summary>
    /// 检查API调用限制
    /// </summary>
    public async Task<LimitCheckResult> CheckAPICallLimit()
    {
        var tenantId = _tenantContext.Current?.TenantId;
        if (string.IsNullOrEmpty(tenantId))
        {
            return LimitCheckResult.Denied("未找到租户信息");
        }

        var subscription = await _subscriptionService.GetActiveSubscription(tenantId);
        var currentUsage = await _usageTracking.GetCurrentUsage(tenantId);
        var plan = await _subscriptionService.GetPlan(subscription.PlanId);

        // 检查月度API调用限制
        if (plan.Limits.MaxAPICallsPerMonth > 0 && 
            currentUsage.APICallsThisMonth >= plan.Limits.MaxAPICallsPerMonth)
        {
            return LimitCheckResult.Denied(
                $"已达到月度API调用次数限制 ({plan.Limits.MaxAPICallsPerMonth})",
                LimitType.MonthlyAPICalls,
                currentUsage.APICallsThisMonth,
                plan.Limits.MaxAPICallsPerMonth);
        }

        return LimitCheckResult.Allowed();
    }

    /// <summary>
    /// 检查存储空间限制
    /// </summary>
    public async Task<LimitCheckResult> CheckStorageLimit(long additionalSizeMB = 0)
    {
        var tenantId = _tenantContext.Current?.TenantId;
        if (string.IsNullOrEmpty(tenantId))
        {
            return LimitCheckResult.Denied("未找到租户信息");
        }

        var subscription = await _subscriptionService.GetActiveSubscription(tenantId);
        var currentUsage = await _usageTracking.GetCurrentUsage(tenantId);
        var plan = await _subscriptionService.GetPlan(subscription.PlanId);

        var totalUsage = currentUsage.StorageUsedMB + additionalSizeMB;

        // 检查存储空间限制
        if (plan.Limits.MaxStorageMB > 0 && totalUsage >= plan.Limits.MaxStorageMB)
        {
            return LimitCheckResult.Denied(
                $"存储空间不足，已使用 {totalUsage}MB，限制 {plan.Limits.MaxStorageMB}MB",
                LimitType.Storage,
                (int)totalUsage,
                (int)plan.Limits.MaxStorageMB);
        }

        return LimitCheckResult.Allowed();
    }

    /// <summary>
    /// 检查功能权限
    /// </summary>
    public async Task<bool> CheckFeatureAccess(string featureName)
    {
        var tenantId = _tenantContext.Current?.TenantId;
        if (string.IsNullOrEmpty(tenantId))
        {
            return false;
        }

        var subscription = await _subscriptionService.GetActiveSubscription(tenantId);
        var plan = await _subscriptionService.GetPlan(subscription.PlanId);

        return plan.Features.Contains(featureName, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 记录使用量
    /// </summary>
    public async Task RecordUsage(UsageType type, int quantity = 1, Dictionary<string, object>? metadata = null)
    {
        var tenantId = _tenantContext.Current?.TenantId;
        if (string.IsNullOrEmpty(tenantId))
        {
            _logger.LogWarning("无法记录使用量：未找到租户信息");
            return;
        }

        await _usageTracking.RecordUsage(tenantId, type, quantity, metadata);
        
        _logger.LogInformation("记录使用量: TenantId={TenantId}, Type={Type}, Quantity={Quantity}", 
            tenantId, type, quantity);
    }

    /// <summary>
    /// 获取当前使用情况
    /// </summary>
    public async Task<UsageStatus> GetUsageStatus()
    {
        var tenantId = _tenantContext.Current?.TenantId;
        if (string.IsNullOrEmpty(tenantId))
        {
            return new UsageStatus { IsValid = false, Message = "未找到租户信息" };
        }

        var subscription = await _subscriptionService.GetActiveSubscription(tenantId);
        var currentUsage = await _usageTracking.GetCurrentUsage(tenantId);
        var plan = await _subscriptionService.GetPlan(subscription.PlanId);

        return new UsageStatus
        {
            IsValid = true,
            TenantId = tenantId,
            PlanName = plan.DisplayName,
            CurrentUsage = currentUsage,
            Limits = plan.Limits,
            SubscriptionStatus = subscription.Status.ToString(),
            PeriodEnd = subscription.PeriodEnd
        };
    }
}

/// <summary>
/// 限制检查结果
/// </summary>
public class LimitCheckResult
{
    public bool IsAllowed { get; set; }
    public string Message { get; set; } = string.Empty;
    public LimitType? LimitType { get; set; }
    public int? CurrentValue { get; set; }
    public int? LimitValue { get; set; }

    public static LimitCheckResult Allowed() => new() { IsAllowed = true };
    
    public static LimitCheckResult Denied(string message, LimitType? limitType = null, 
        int? currentValue = null, int? limitValue = null) => new()
    {
        IsAllowed = false,
        Message = message,
        LimitType = limitType,
        CurrentValue = currentValue,
        LimitValue = limitValue
    };
}

/// <summary>
/// 限制类型
/// </summary>
public enum LimitType
{
    MaxAgents,
    MonthlyConversations,
    MonthlyAPICalls,
    Storage,
    TeamMembers,
    Integrations
}

/// <summary>
/// 使用状态
/// </summary>
public class UsageStatus
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public UsageMetrics CurrentUsage { get; set; } = new();
    public PlanLimits Limits { get; set; } = new();
    public string SubscriptionStatus { get; set; } = string.Empty;
    public DateTime? PeriodEnd { get; set; }
}

/// <summary>
/// 使用类型
/// </summary>
public enum UsageType
{
    AgentCreation,
    Conversation,
    APICall,
    Storage,
    FileUpload,
    KnowledgeBaseQuery,
    WebhookCall
}

public interface ISubscriptionLimitService
{
    Task<LimitCheckResult> CheckAgentCreationLimit();
    Task<LimitCheckResult> CheckConversationLimit();
    Task<LimitCheckResult> CheckAPICallLimit();
    Task<LimitCheckResult> CheckStorageLimit(long additionalSizeMB = 0);
    Task<bool> CheckFeatureAccess(string featureName);
    Task RecordUsage(UsageType type, int quantity = 1, Dictionary<string, object>? metadata = null);
    Task<UsageStatus> GetUsageStatus();
}
