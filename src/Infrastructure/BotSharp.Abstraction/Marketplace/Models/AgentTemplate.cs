namespace BotSharp.Abstraction.Marketplace.Models;

/// <summary>
/// Agent模板市场模型
/// </summary>
public class AgentTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public string IconUrl { get; set; } = string.Empty;
    public List<string> Screenshots { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public List<UseCase> UseCases { get; set; } = new();
    
    public TemplateType Type { get; set; } = TemplateType.Free;
    public decimal Price { get; set; } = 0;
    
    public TemplateStats Stats { get; set; } = new();
    public TemplateConfiguration Configuration { get; set; } = new();
    
    public List<RequiredPlugin> RequiredPlugins { get; set; } = new();
    public List<string> SupportedLanguages { get; set; } = new();
    
    public bool IsPublished { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public TemplateStatus Status { get; set; } = TemplateStatus.Draft;
}

public class UseCase
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public List<string> Benefits { get; set; } = new();
}

public class TemplateStats
{
    public int Downloads { get; set; }
    public int Installs { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public int WeeklyActiveUsers { get; set; }
}

public class TemplateConfiguration
{
    public string AgentConfig { get; set; } = string.Empty; // JSON配置
    public List<string> DefaultInstructions { get; set; } = new();
    public List<FunctionTemplate> Functions { get; set; } = new();
    public Dictionary<string, object> Variables { get; set; } = new();
    public List<string> KnowledgeBases { get; set; } = new();
}

public class FunctionTemplate
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Implementation { get; set; } = string.Empty;
    public List<Parameter> Parameters { get; set; } = new();
}

public class Parameter
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Required { get; set; } = false;
    public object? DefaultValue { get; set; }
}

public class RequiredPlugin
{
    public string PluginId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string MinVersion { get; set; } = string.Empty;
    public bool IsOptional { get; set; } = false;
}

public enum TemplateType
{
    Free,
    Premium,
    Enterprise
}

public enum TemplateStatus
{
    Draft,
    Review,
    Published,
    Deprecated
}

/// <summary>
/// Agent模板分类
/// </summary>
public static class TemplateCategories
{
    public const string CustomerService = "customer-service";
    public const string Sales = "sales";
    public const string Marketing = "marketing";
    public const string DataAnalysis = "data-analysis";
    public const string ContentCreation = "content-creation";
    public const string Research = "research";
    public const string Education = "education";
    public const string Healthcare = "healthcare";
    public const string Finance = "finance";
    public const string Legal = "legal";
    public const string HR = "human-resources";
    public const string IT = "information-technology";
    public const string Manufacturing = "manufacturing";
    public const string Retail = "retail";
    public const string RealEstate = "real-estate";
    public const string Travel = "travel";
    public const string Entertainment = "entertainment";
    public const string Gaming = "gaming";
    public const string Social = "social";
    public const string Productivity = "productivity";
    public const string Utilities = "utilities";
}

/// <summary>
/// 预定义的Agent模板
/// </summary>
public static class PrebuiltTemplates
{
    public static readonly AgentTemplate CustomerServiceBot = new()
    {
        Id = "template-customer-service-001",
        Name = "智能客服助手",
        Description = "24/7在线客服，支持常见问题解答、订单查询、退款处理等",
        Category = TemplateCategories.CustomerService,
        Tags = new() { "客服", "FAQ", "订单查询", "多语言" },
        UseCases = new()
        {
            new UseCase
            {
                Title = "电商客服",
                Description = "处理订单查询、退款申请、物流跟踪等",
                Industry = "电子商务",
                Benefits = new() { "降低人工成本", "提升响应速度", "24小时服务" }
            }
        },
        SupportedLanguages = new() { "zh-CN", "en-US", "ja-JP" },
        IsPublished = true,
        IsFeatured = true
    };
    
    public static readonly AgentTemplate SalesAssistant = new()
    {
        Id = "template-sales-assistant-001",
        Name = "AI销售顾问",
        Description = "智能销售助手，协助线索培育、产品推荐、报价生成",
        Category = TemplateCategories.Sales,
        Tags = new() { "销售", "线索管理", "产品推荐", "CRM" },
        UseCases = new()
        {
            new UseCase
            {
                Title = "B2B销售",
                Description = "自动化线索筛选和培育，提供个性化产品方案",
                Industry = "企业服务",
                Benefits = new() { "提高转化率", "节约销售时间", "个性化服务" }
            }
        },
        SupportedLanguages = new() { "zh-CN", "en-US" },
        IsPublished = true,
        IsFeatured = true
    };
    
    public static readonly AgentTemplate ContentCreator = new()
    {
        Id = "template-content-creator-001",
        Name = "内容创作专家",
        Description = "AI驱动的内容创作助手，支持文章、社媒内容、营销文案生成",
        Category = TemplateCategories.ContentCreation,
        Tags = new() { "内容创作", "文案", "社媒", "SEO" },
        UseCases = new()
        {
            new UseCase
            {
                Title = "数字营销",
                Description = "批量生成高质量营销内容，提升品牌影响力",
                Industry = "数字营销",
                Benefits = new() { "提升创作效率", "保持内容一致性", "SEO优化" }
            }
        },
        SupportedLanguages = new() { "zh-CN", "en-US" },
        IsPublished = true,
        IsFeatured = false
    };
}
