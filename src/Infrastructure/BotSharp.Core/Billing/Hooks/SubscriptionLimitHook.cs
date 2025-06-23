using BotSharp.Abstraction.Agents;
using BotSharp.Abstraction.Conversations;
using BotSharp.Core.Billing.Services;

namespace BotSharp.Core.Billing.Hooks;

/// <summary>
/// 订阅限制拦截器 - 在关键操作前检查限制
/// </summary>
public class SubscriptionLimitHook : IAgentHook, IConversationHook
{
    private readonly ISubscriptionLimitService _limitService;
    private readonly ILogger<SubscriptionLimitHook> _logger;

    public SubscriptionLimitHook(ISubscriptionLimitService limitService, ILogger<SubscriptionLimitHook> logger)
    {
        _limitService = limitService;
        _logger = logger;
    }

    /// <summary>
    /// Agent创建前检查限制
    /// </summary>
    public async Task OnAgentCreating(Agent agent)
    {
        var result = await _limitService.CheckAgentCreationLimit();
        if (!result.IsAllowed)
        {
            _logger.LogWarning("Agent创建被拒绝: {Message}", result.Message);
            throw new SubscriptionLimitException(result.Message, result.LimitType, result.CurrentValue, result.LimitValue);
        }

        _logger.LogInformation("Agent创建检查通过: {AgentName}", agent.Name);
    }

    /// <summary>
    /// Agent创建后记录使用量
    /// </summary>
    public async Task OnAgentCreated(Agent agent)
    {
        await _limitService.RecordUsage(UsageType.AgentCreation, 1, new Dictionary<string, object>
        {
            ["agent_id"] = agent.Id,
            ["agent_name"] = agent.Name,
            ["agent_type"] = agent.Type
        });

        _logger.LogInformation("Agent创建使用量已记录: {AgentId}", agent.Id);
    }

    /// <summary>
    /// 对话开始前检查限制
    /// </summary>
    public async Task OnConversationStarting(Conversation conversation)
    {
        var result = await _limitService.CheckConversationLimit();
        if (!result.IsAllowed)
        {
            _logger.LogWarning("对话创建被拒绝: {Message}", result.Message);
            throw new SubscriptionLimitException(result.Message, result.LimitType, result.CurrentValue, result.LimitValue);
        }

        // 同时检查API调用限制
        var apiResult = await _limitService.CheckAPICallLimit();
        if (!apiResult.IsAllowed)
        {
            _logger.LogWarning("API调用被拒绝: {Message}", apiResult.Message);
            throw new SubscriptionLimitException(apiResult.Message, apiResult.LimitType, apiResult.CurrentValue, apiResult.LimitValue);
        }

        _logger.LogInformation("对话创建检查通过: {ConversationId}", conversation.Id);
    }

    /// <summary>
    /// 对话创建后记录使用量
    /// </summary>
    public async Task OnConversationCreated(Conversation conversation)
    {
        await _limitService.RecordUsage(UsageType.Conversation, 1, new Dictionary<string, object>
        {
            ["conversation_id"] = conversation.Id,
            ["agent_id"] = conversation.AgentId ?? "",
            ["channel"] = conversation.Channel ?? "web"
        });

        _logger.LogInformation("对话使用量已记录: {ConversationId}", conversation.Id);
    }

    /// <summary>
    /// API调用时记录使用量
    /// </summary>
    public async Task OnAPICallMade(string endpoint, string method, int responseCode)
    {
        // 只记录成功的API调用
        if (responseCode >= 200 && responseCode < 300)
        {
            await _limitService.RecordUsage(UsageType.APICall, 1, new Dictionary<string, object>
            {
                ["endpoint"] = endpoint,
                ["method"] = method,
                ["response_code"] = responseCode,
                ["timestamp"] = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// 文件上传前检查存储限制
    /// </summary>
    public async Task OnFileUploading(string fileName, long fileSizeMB)
    {
        var result = await _limitService.CheckStorageLimit(fileSizeMB);
        if (!result.IsAllowed)
        {
            _logger.LogWarning("文件上传被拒绝: {Message}", result.Message);
            throw new SubscriptionLimitException(result.Message, result.LimitType, result.CurrentValue, result.LimitValue);
        }

        _logger.LogInformation("文件上传检查通过: {FileName}, Size: {Size}MB", fileName, fileSizeMB);
    }

    /// <summary>
    /// 文件上传后记录存储使用量
    /// </summary>
    public async Task OnFileUploaded(string fileName, long fileSizeMB)
    {
        await _limitService.RecordUsage(UsageType.Storage, (int)fileSizeMB, new Dictionary<string, object>
        {
            ["file_name"] = fileName,
            ["file_size_mb"] = fileSizeMB,
            ["upload_timestamp"] = DateTime.UtcNow
        });

        _logger.LogInformation("存储使用量已记录: {FileName}, Size: {Size}MB", fileName, fileSizeMB);
    }
}

/// <summary>
/// 订阅限制异常
/// </summary>
public class SubscriptionLimitException : Exception
{
    public LimitType? LimitType { get; }
    public int? CurrentValue { get; }
    public int? LimitValue { get; }

    public SubscriptionLimitException(string message, LimitType? limitType = null, 
        int? currentValue = null, int? limitValue = null) : base(message)
    {
        LimitType = limitType;
        CurrentValue = currentValue;
        LimitValue = limitValue;
    }
}

/// <summary>
/// 使用量跟踪服务接口
/// </summary>
public interface IUsageTrackingService
{
    Task<UsageMetrics> GetCurrentUsage(string tenantId);
    Task RecordUsage(string tenantId, UsageType type, int quantity, Dictionary<string, object>? metadata = null);
    Task ResetMonthlyUsage(string tenantId);
    Task<List<UsageRecord>> GetUsageHistory(string tenantId, DateTime from, DateTime to);
}

/// <summary>
/// 使用记录
/// </summary>
public class UsageRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public UsageType Type { get; set; }
    public int Quantity { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// 订阅服务接口
/// </summary>
public interface ISubscriptionService
{
    Task<Subscription?> GetActiveSubscription(string tenantId);
    Task<SubscriptionPlan> GetPlan(string planId);
    Task<bool> UpgradePlan(string tenantId, string newPlanId);
    Task<bool> CancelSubscription(string tenantId);
}

/// <summary>
/// 订阅信息
/// </summary>
public class Subscription
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public bool AutoRenew { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum SubscriptionStatus
{
    Active,
    Cancelled,
    Expired,
    Trial,
    Suspended
}
