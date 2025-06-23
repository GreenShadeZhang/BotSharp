# 订阅计划限制详细实施方案

## 🎯 **核心限制策略**

### 1. **多层次限制架构**

#### **硬限制 (Hard Limits)**
- **立即阻止**：超出限制立即拒绝操作
- **适用场景**：Agent数量、存储空间、团队成员数
- **用户体验**：提供清晰的错误信息和升级建议

#### **软限制 (Soft Limits)**  
- **警告提醒**：接近限制时发送通知
- **适用场景**：API调用次数、对话次数
- **用户体验**：渐进式提醒，引导升级

#### **计量限制 (Metered Limits)**
- **超量付费**：允许超出并按用量计费
- **适用场景**：高级功能、额外存储
- **用户体验**：透明的计费信息

### 2. **实时限制检查机制**

#### **检查点设计**
```csharp
// 关键业务操作前的检查点
public static class LimitCheckpoints
{
    public const string AGENT_CREATION = "agent.create";
    public const string CONVERSATION_START = "conversation.start";
    public const string API_CALL = "api.call";
    public const string FILE_UPLOAD = "file.upload";
    public const string KNOWLEDGE_BASE_QUERY = "knowledge.query";
    public const string WEBHOOK_CALL = "webhook.call";
    public const string TEAM_MEMBER_INVITE = "team.invite";
}
```

#### **缓存优化策略**
```csharp
// 使用Redis缓存使用量数据，减少数据库查询
public class CachedUsageTracker
{
    private readonly IMemoryCache _cache;
    private readonly IDatabase _redis;
    
    // 缓存策略：
    // - 热数据：内存缓存 5分钟
    // - 温数据：Redis缓存 1小时  
    // - 冷数据：数据库存储
    
    public async Task<UsageMetrics> GetUsageWithCache(string tenantId)
    {
        // 1. 检查内存缓存
        if (_cache.TryGetValue($"usage:{tenantId}", out UsageMetrics cached))
            return cached;
            
        // 2. 检查Redis缓存
        var redisData = await _redis.StringGetAsync($"usage:{tenantId}");
        if (redisData.HasValue)
        {
            var usage = JsonSerializer.Deserialize<UsageMetrics>(redisData);
            _cache.Set($"usage:{tenantId}", usage, TimeSpan.FromMinutes(5));
            return usage;
        }
        
        // 3. 从数据库加载
        var dbUsage = await LoadFromDatabase(tenantId);
        await _redis.StringSetAsync($"usage:{tenantId}", 
            JsonSerializer.Serialize(dbUsage), TimeSpan.FromHours(1));
        _cache.Set($"usage:{tenantId}", dbUsage, TimeSpan.FromMinutes(5));
        
        return dbUsage;
    }
}
```

### 3. **动态限制调整**

#### **基于使用模式的智能调整**
```csharp
public class IntelligentLimitAdjuster
{
    public async Task<AdjustedLimits> CalculatePersonalizedLimits(
        string tenantId, 
        SubscriptionPlan basePlan,
        UsageHistory history)
    {
        var adjustments = new AdjustedLimits(basePlan.Limits);
        
        // 分析使用模式
        var pattern = await AnalyzeUsagePattern(history);
        
        // 根据使用行为调整限制
        if (pattern.IsHighVolumeUser && pattern.ConsistentUsage)
        {
            // 高频稳定用户：临时提升限制
            adjustments.TemporaryBoost = new()
            {
                ConversationBoost = 0.2m, // 20% 提升
                APICallBoost = 0.15m,      // 15% 提升
                Duration = TimeSpan.FromDays(7)
            };
        }
        
        if (pattern.ApproachingUpgrade)
        {
            // 即将升级用户：提供试用高级功能
            adjustments.TrialFeatures = new()
            {
                Features = new[] { "advanced_analytics", "priority_support" },
                Duration = TimeSpan.FromDays(14)
            };
        }
        
        return adjustments;
    }
}
```

## 🔧 **具体限制实施细节**

### 1. **Agent创建限制**

#### **检查逻辑**
```csharp
public async Task<LimitCheckResult> CheckAgentCreationLimit()
{
    var tenant = await GetCurrentTenant();
    var subscription = await GetActiveSubscription(tenant.Id);
    var currentCount = await GetAgentCount(tenant.Id);
    
    // 检查基础限制
    if (subscription.Plan.Limits.MaxAgents > 0 && 
        currentCount >= subscription.Plan.Limits.MaxAgents)
    {
        // 检查是否有临时提升
        var boost = await GetTemporaryBoost(tenant.Id, LimitType.MaxAgents);
        var effectiveLimit = subscription.Plan.Limits.MaxAgents + (boost?.AdditionalQuota ?? 0);
        
        if (currentCount >= effectiveLimit)
        {
            return LimitCheckResult.Denied(
                $"已达到Agent数量限制 ({effectiveLimit}个)",
                suggestedActions: new[]
                {
                    new SuggestedAction
                    {
                        Type = "upgrade",
                        Title = "升级订阅计划",
                        Description = "获得更多Agent配额",
                        Url = "/billing/upgrade",
                        Priority = 1
                    },
                    new SuggestedAction
                    {
                        Type = "optimize",
                        Title = "删除未使用的Agent",
                        Description = "释放配额创建新Agent",
                        Url = "/agents?filter=unused",
                        Priority = 2
                    }
                });
        }
    }
    
    return LimitCheckResult.Allowed();
}
```

#### **限制提升策略**
```csharp
public class AgentLimitBoostStrategy
{
    // 场景1：新用户试用期
    public static LimitBoost NewUserTrial => new()
    {
        AdditionalQuota = 2,
        Duration = TimeSpan.FromDays(14),
        Reason = "新用户试用"
    };
    
    // 场景2：节假日促销
    public static LimitBoost HolidayPromotion => new()
    {
        AdditionalQuota = 5,
        Duration = TimeSpan.FromDays(30),
        Reason = "节假日特别优惠"
    };
    
    // 场景3：忠实用户奖励
    public static LimitBoost LoyaltyReward => new()
    {
        AdditionalQuota = 3,
        Duration = TimeSpan.FromDays(7),
        Reason = "忠实用户奖励"
    };
}
```

### 2. **对话次数限制**

#### **月度重置机制**
```csharp
public class ConversationLimitManager
{
    public async Task<bool> CheckMonthlyConversationLimit(string tenantId)
    {
        var currentMonth = DateTime.UtcNow.ToString("yyyy-MM");
        var usage = await GetMonthlyUsage(tenantId, currentMonth);
        var subscription = await GetActiveSubscription(tenantId);
        
        var limit = subscription.Plan.Limits.MaxConversationsPerMonth;
        if (limit <= 0) return true; // 无限制
        
        // 检查是否接近限制（80%预警）
        if (usage.ConversationsThisMonth >= limit * 0.8m)
        {
            await SendUsageWarning(tenantId, "conversation", usage.ConversationsThisMonth, limit);
        }
        
        // 硬限制检查
        if (usage.ConversationsThisMonth >= limit)
        {
            // 检查是否允许超量使用
            var settings = await GetTenantSettings(tenantId);
            if (settings.AllowOverage && settings.OverageRatePerConversation > 0)
            {
                // 记录超量使用，将在下次账单中收费
                await RecordOverageUsage(tenantId, "conversation", 1, settings.OverageRatePerConversation);
                return true;
            }
            
            return false;
        }
        
        return true;
    }
    
    // 每月1号自动重置
    [Cron("0 0 1 * *")]
    public async Task ResetMonthlyUsage()
    {
        var tenants = await GetAllActiveTenants();
        foreach (var tenant in tenants)
        {
            await ResetTenantMonthlyUsage(tenant.Id);
            _logger.LogInformation("已重置租户 {TenantId} 的月度使用量", tenant.Id);
        }
    }
}
```

#### **智能流量控制**
```csharp
public class ConversationThrottleManager
{
    // 基于时间窗口的限流
    public async Task<bool> CheckRateLimit(string tenantId)
    {
        var windowSize = TimeSpan.FromMinutes(1);
        var maxRequestsPerWindow = await GetRateLimit(tenantId);
        
        var key = $"rate_limit:{tenantId}:{DateTime.UtcNow:yyyyMMddHHmm}";
        var currentCount = await _redis.StringIncrementAsync(key);
        
        if (currentCount == 1)
        {
            await _redis.KeyExpireAsync(key, windowSize);
        }
        
        return currentCount <= maxRequestsPerWindow;
    }
    
    // 动态速率限制
    private async Task<int> GetRateLimit(string tenantId)
    {
        var subscription = await GetActiveSubscription(tenantId);
        
        return subscription.Plan.Id switch
        {
            "free" => 10,      // 10 requests/minute
            "pro" => 100,      // 100 requests/minute
            "enterprise" => 1000, // 1000 requests/minute
            _ => 5
        };
    }
}
```

### 3. **API调用限制**

#### **分级限制策略**
```csharp
public class APICallLimitManager
{
    private readonly Dictionary<string, APILimitConfig> _limitConfigs = new()
    {
        ["free"] = new() 
        { 
            MonthlyLimit = 1000,
            RateLimit = 10, // per minute
            BurstLimit = 20,
            OverageAllowed = false
        },
        ["pro"] = new() 
        { 
            MonthlyLimit = 50000,
            RateLimit = 100,
            BurstLimit = 200,
            OverageAllowed = true,
            OverageRate = 0.01m // $0.01 per call
        },
        ["enterprise"] = new() 
        { 
            MonthlyLimit = -1, // unlimited
            RateLimit = 1000,
            BurstLimit = 2000,
            OverageAllowed = true,
            OverageRate = 0.005m
        }
    };
    
    public async Task<APICallResult> CheckAPICallLimit(string tenantId, string endpoint)
    {
        var subscription = await GetActiveSubscription(tenantId);
        var config = _limitConfigs[subscription.Plan.Id];
        
        // 1. 检查月度限制
        if (config.MonthlyLimit > 0)
        {
            var monthlyUsage = await GetMonthlyAPIUsage(tenantId);
            if (monthlyUsage >= config.MonthlyLimit)
            {
                if (!config.OverageAllowed)
                {
                    return APICallResult.Denied("已达到月度API调用限制");
                }
                // 记录超量使用
                await RecordOverageUsage(tenantId, "api_call", 1, config.OverageRate);
            }
        }
        
        // 2. 检查速率限制
        var rateLimitResult = await CheckRateLimit(tenantId, config);
        if (!rateLimitResult.Allowed)
        {
            return APICallResult.RateLimited(rateLimitResult.RetryAfter);
        }
        
        // 3. 检查突发限制
        var burstResult = await CheckBurstLimit(tenantId, config);
        if (!burstResult.Allowed)
        {
            return APICallResult.BurstLimited();
        }
        
        return APICallResult.Allowed();
    }
}

public class APILimitConfig
{
    public int MonthlyLimit { get; set; }
    public int RateLimit { get; set; }      // per minute
    public int BurstLimit { get; set; }     // short burst allowance
    public bool OverageAllowed { get; set; }
    public decimal OverageRate { get; set; }
}
```

### 4. **存储空间限制**

#### **分层存储策略**
```csharp
public class StorageLimitManager
{
    public async Task<StorageCheckResult> CheckStorageLimit(
        string tenantId, 
        long additionalSizeMB,
        StorageType storageType)
    {
        var subscription = await GetActiveSubscription(tenantId);
        var currentUsage = await GetStorageUsage(tenantId);
        var totalUsage = currentUsage.TotalUsedMB + additionalSizeMB;
        
        var limit = subscription.Plan.Limits.MaxStorageMB;
        if (limit <= 0) return StorageCheckResult.Allowed(); // 无限制
        
        // 按存储类型检查
        var typeLimit = GetStorageTypeLimit(subscription.Plan.Id, storageType);
        var typeUsage = GetStorageUsageByType(currentUsage, storageType);
        
        if (typeUsage + additionalSizeMB > typeLimit)
        {
            return StorageCheckResult.Denied(
                $"{storageType}存储空间不足",
                typeUsage,
                typeLimit);
        }
        
        // 总存储限制检查
        if (totalUsage > limit)
        {
            // 检查是否支持存储扩展包
            var expansionPacks = await GetAvailableExpansionPacks(tenantId);
            if (expansionPacks.Any())
            {
                return StorageCheckResult.UpgradeRequired(
                    "存储空间不足，建议购买扩展包",
                    expansionPacks);
            }
            
            return StorageCheckResult.Denied(
                "存储空间不足，请升级订阅计划",
                totalUsage,
                limit);
        }
        
        return StorageCheckResult.Allowed();
    }
    
    private long GetStorageTypeLimit(string planId, StorageType type)
    {
        var limits = new Dictionary<string, Dictionary<StorageType, long>>
        {
            ["free"] = new()
            {
                [StorageType.UserFiles] = 50,     // 50MB
                [StorageType.KnowledgeBase] = 30, // 30MB  
                [StorageType.AgentConfigs] = 20   // 20MB
            },
            ["pro"] = new()
            {
                [StorageType.UserFiles] = 500,    // 500MB
                [StorageType.KnowledgeBase] = 300, // 300MB
                [StorageType.AgentConfigs] = 200   // 200MB
            },
            ["enterprise"] = new()
            {
                [StorageType.UserFiles] = -1,     // 无限制
                [StorageType.KnowledgeBase] = -1,
                [StorageType.AgentConfigs] = -1
            }
        };
        
        return limits[planId][type];
    }
}

public enum StorageType
{
    UserFiles,      // 用户上传文件
    KnowledgeBase,  // 知识库文档
    AgentConfigs,   // Agent配置和模板
    Backups,        // 备份文件
    Logs           // 日志文件
}
```

### 5. **使用量统计和告警**

#### **实时统计面板**
```csharp
public class UsageDashboardService
{
    public async Task<UsageDashboard> GetDashboard(string tenantId)
    {
        var subscription = await GetActiveSubscription(tenantId);
        var usage = await GetCurrentUsage(tenantId);
        var plan = subscription.Plan;
        
        return new UsageDashboard
        {
            TenantInfo = new()
            {
                Name = subscription.TenantName,
                Plan = plan.DisplayName,
                BillingCycle = subscription.BillingCycle,
                NextBillingDate = subscription.NextBillingDate
            },
            Usage = new()
            {
                Agents = CreateUsageItem(usage.AgentsCreated, plan.Limits.MaxAgents, "个"),
                Conversations = CreateUsageItem(usage.ConversationsThisMonth, plan.Limits.MaxConversationsPerMonth, "次"),
                APICalls = CreateUsageItem(usage.APICallsThisMonth, plan.Limits.MaxAPICallsPerMonth, "次"),
                Storage = CreateUsageItem((int)usage.StorageUsedMB, (int)plan.Limits.MaxStorageMB, "MB")
            },
            Alerts = await GenerateUsageAlerts(tenantId, usage, plan)
        };
    }
    
    private UsageItem CreateUsageItem(int used, int limit, string unit)
    {
        var percentage = limit > 0 ? (double)used / limit * 100 : 0;
        var status = percentage switch
        {
            >= 100 => "exceeded",
            >= 90 => "critical", 
            >= 80 => "warning",
            _ => "normal"
        };
        
        return new UsageItem
        {
            Used = used,
            Limit = limit,
            Unit = unit,
            Percentage = Math.Round(percentage, 1),
            Status = status,
            IsUnlimited = limit <= 0
        };
    }
}
```

#### **智能告警系统**
```csharp
public class IntelligentAlertSystem
{
    public async Task<List<UsageAlert>> GenerateAlerts(string tenantId, UsageMetrics usage, SubscriptionPlan plan)
    {
        var alerts = new List<UsageAlert>();
        
        // 使用量告警
        alerts.AddRange(await CheckUsageAlerts(usage, plan));
        
        // 预测性告警
        alerts.AddRange(await CheckPredictiveAlerts(tenantId, usage, plan));
        
        // 账单告警
        alerts.AddRange(await CheckBillingAlerts(tenantId));
        
        // 功能推荐告警
        alerts.AddRange(await CheckFeatureRecommendations(tenantId, usage));
        
        return alerts.OrderByDescending(a => a.Priority).ToList();
    }
    
    private async Task<List<UsageAlert>> CheckPredictiveAlerts(string tenantId, UsageMetrics usage, SubscriptionPlan plan)
    {
        var alerts = new List<UsageAlert>();
        var history = await GetUsageHistory(tenantId, TimeSpan.FromDays(30));
        
        // 预测对话使用量
        var conversationTrend = CalculateTrend(history.Select(h => h.ConversationsDaily));
        var predictedMonthlyConversations = PredictMonthlyUsage(usage.ConversationsThisMonth, conversationTrend);
        
        if (predictedMonthlyConversations > plan.Limits.MaxConversationsPerMonth * 0.9)
        {
            alerts.Add(new UsageAlert
            {
                Type = AlertType.PredictiveWarning,
                Title = "预计本月对话次数将达到限制",
                Message = $"按当前使用趋势，预计本月将使用 {predictedMonthlyConversations} 次对话",
                Priority = AlertPriority.Medium,
                SuggestedActions = new[]
                {
                    new SuggestedAction { Type = "upgrade", Title = "提前升级计划" },
                    new SuggestedAction { Type = "optimize", Title = "优化Agent配置" }
                }
            });
        }
        
        return alerts;
    }
}
```

## 📊 **监控和优化**

### 1. **性能监控指标**
- 限制检查响应时间 < 50ms
- 使用量更新延迟 < 100ms  
- 缓存命中率 > 95%
- 数据库查询优化

### 2. **业务监控指标**
- 限制触发频率和类型
- 用户升级转化率
- 使用量预测准确度
- 客户满意度评分

### 3. **自动化优化策略**
- 动态调整缓存策略
- 智能预加载热数据
- 自动清理过期数据
- 负载均衡优化

这个详细的实施方案提供了完整的订阅限制机制，包括实时检查、智能调整、友好提示和预测性告警，确保用户在使用过程中有良好的体验同时有效控制资源使用。
