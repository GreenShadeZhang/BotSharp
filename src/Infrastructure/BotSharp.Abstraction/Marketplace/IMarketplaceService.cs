using BotSharp.Abstraction.Marketplace.Models;
using AgentTemplate = BotSharp.Abstraction.Marketplace.Models.AgentTemplate;

namespace BotSharp.Abstraction.Marketplace;

/// <summary>
/// Agent模板市场服务接口
/// </summary>
public interface IMarketplaceService
{
    /// <summary>
    /// 获取推荐模板
    /// </summary>
    Task<List<AgentTemplate>> GetFeaturedTemplates(int limit = 10);
    
    /// <summary>
    /// 按分类获取模板
    /// </summary>
    Task<List<AgentTemplate>> GetTemplatesByCategory(string category, int page = 1, int size = 20);
    
    /// <summary>
    /// 搜索模板
    /// </summary>
    Task<List<AgentTemplate>> SearchTemplates(string query, string? category = null);
    
    /// <summary>
    /// 获取模板详情
    /// </summary>
    Task<AgentTemplate?> GetTemplate(string templateId);
    
    /// <summary>
    /// 安装模板到租户
    /// </summary>
    Task<string> InstallTemplate(string tenantId, string templateId, InstallTemplateRequest request);
    
    /// <summary>
    /// 获取租户已安装的模板
    /// </summary>
    Task<List<InstalledTemplate>> GetInstalledTemplates(string tenantId);
    
    /// <summary>
    /// 卸载模板
    /// </summary>
    Task<bool> UninstallTemplate(string tenantId, string installedTemplateId);
    
    /// <summary>
    /// 发布新模板
    /// </summary>
    Task<string> PublishTemplate(PublishTemplateRequest request);
    
    /// <summary>
    /// 更新模板
    /// </summary>
    Task<bool> UpdateTemplate(string templateId, AgentTemplate template);
    
    /// <summary>
    /// 获取模板评价
    /// </summary>
    Task<List<TemplateReview>> GetTemplateReviews(string templateId, int page = 1, int size = 10);
    
    /// <summary>
    /// 添加模板评价
    /// </summary>
    Task<bool> AddTemplateReview(string templateId, string userId, TemplateReview review);
    
    /// <summary>
    /// 记录模板使用统计
    /// </summary>
    Task RecordTemplateUsage(string templateId, TemplateUsageEvent eventType);
    
    /// <summary>
    /// 获取个性化推荐
    /// </summary>
    Task<List<AgentTemplate>> GetPersonalizedRecommendations(string userId, int limit = 5);
}

public class InstallTemplateRequest
{
    public string AgentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Dictionary<string, object> Configuration { get; set; } = new();
    public List<string> EnabledFeatures { get; set; } = new();
}

public class InstalledTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TemplateId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string AgentId { get; set; } = string.Empty;
    public string AgentName { get; set; } = string.Empty;
    public DateTime InstalledAt { get; set; } = DateTime.UtcNow;
    public string InstalledBy { get; set; } = string.Empty;
    public Dictionary<string, object> Configuration { get; set; } = new();
    public string Status { get; set; } = "active";
}

public class PublishTemplateRequest
{
    public AgentTemplate Template { get; set; } = new();
    public string PublisherId { get; set; } = string.Empty;
    public string? ReleaseNotes { get; set; }
}

public class TemplateReview
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TemplateId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsVerified { get; set; } = false;
    public int HelpfulVotes { get; set; } = 0;
}

public enum TemplateUsageEvent
{
    View,
    Download,
    Install,
    Uninstall,
    Review,
    Share
}
