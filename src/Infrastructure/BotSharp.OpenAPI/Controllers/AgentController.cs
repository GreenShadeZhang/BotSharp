using BotSharp.Abstraction.Agents.Models;
using BotSharp.Abstraction.Files.Utilities;
using BotSharp.Abstraction.Infrastructures.Attributes;

namespace BotSharp.OpenAPI.Controllers;

[Authorize]
[ApiController]
public class AgentController : ControllerBase
{
    private readonly IAgentService _agentService;
    private readonly IUserIdentity _user;
    private readonly IServiceProvider _services;

    public AgentController(
        IAgentService agentService,
        IUserIdentity user,
        IServiceProvider services
        )
    {
        _agentService = agentService;
        _user = user;
        _services = services;
    }

    [HttpGet("/agent/settings")]
    public AgentSettings GetSettings()
    {
        var settings = _services.GetRequiredService<AgentSettings>();
        return settings;
    }

    [HttpGet("/agent/{id}")]
    public async Task<AgentViewModel?> GetAgent([FromRoute] string id)
    {
        var pagedAgents = await _agentService.GetAgents(new AgentFilter
        {
            AgentIds = new List<string> { id }
        });

        var foundAgent = pagedAgents.Items.FirstOrDefault();
        if (foundAgent == null) return null;

        await _agentService.InheritAgent(foundAgent);
        var targetAgent = AgentViewModel.FromAgent(foundAgent);
        var agentSetting = _services.GetRequiredService<AgentSettings>();
        targetAgent.IsHost = targetAgent.Id == agentSetting.HostAgentId;

        var redirectAgentIds = targetAgent.RoutingRules
                                          .Where(x => !string.IsNullOrEmpty(x.RedirectTo))
                                          .Select(x => x.RedirectTo)
                                          .ToList();

        var redirectAgents = await _agentService.GetAgents(new AgentFilter
        {
            AgentIds = redirectAgentIds
        });
        foreach (var rule in targetAgent.RoutingRules)
        {
            var found = redirectAgents.Items.FirstOrDefault(x => x.Id == rule.RedirectTo);
            if (found == null) continue;

            rule.RedirectToAgentName = found.Name;
        }

        var userService = _services.GetRequiredService<IUserService>();
        var auth = await userService.GetUserAuthorizations(new List<string> { targetAgent.Id });
        targetAgent.Actions = auth.GetAllowedAgentActions(targetAgent.Id);
        return targetAgent;
    }

    [HttpGet("/agents")]
    public async Task<PagedItems<AgentViewModel>> GetAgents([FromQuery] AgentFilter filter, [FromQuery] bool checkAuth = false)
    {
        var agentSetting = _services.GetRequiredService<AgentSettings>();
        var userService = _services.GetRequiredService<IUserService>();

        List<AgentViewModel> agents;
        var pagedAgents = await _agentService.GetAgents(filter);

        if (!checkAuth)
        {
            agents = pagedAgents?.Items?.Select(x => AgentViewModel.FromAgent(x))?.ToList() ?? [];
            return new PagedItems<AgentViewModel>
            {
                Items = agents,
                Count = pagedAgents?.Count ?? 0
            };
        }

        var auth = await userService.GetUserAuthorizations(pagedAgents.Items.Select(x => x.Id));
        agents = pagedAgents?.Items?.Select(x =>
        {
            var model = AgentViewModel.FromAgent(x);
            model.Actions = auth.GetAllowedAgentActions(x.Id);
            return model;
        })?.ToList() ?? [];

        return new PagedItems<AgentViewModel>
        {
            Items = agents,
            Count = pagedAgents?.Count ?? 0
        };
    }

    [HttpPost("/agent")]
    public async Task<AgentViewModel> CreateAgent(AgentCreationModel agent)
    {
        var createdAgent = await _agentService.CreateAgent(agent.ToAgent());
        return AgentViewModel.FromAgent(createdAgent);
    }

    [BotSharpAuth]
    [HttpPost("/refresh-agents")]
    public async Task<string> RefreshAgents()
    {
        return await _agentService.RefreshAgents();
    }

    [HttpPut("/agent/file/{agentId}")]
    public async Task<string> UpdateAgentFromFile([FromRoute] string agentId)
    {
        return await _agentService.UpdateAgentFromFile(agentId);
    }

    [HttpPut("/agent/{agentId}")]
    public async Task UpdateAgent([FromRoute] string agentId, [FromBody] AgentUpdateModel agent)
    {
        var model = agent.ToAgent();
        model.Id = agentId;
        await _agentService.UpdateAgent(model, AgentField.All);
    }

    [HttpPatch("/agent/{agentId}/{field}")]
    public async Task PatchAgentByField([FromRoute] string agentId, AgentField field, [FromBody] AgentUpdateModel agent)
    {
        var model = agent.ToAgent();
        model.Id = agentId;
        await _agentService.UpdateAgent(model, field);
    }

    [HttpPatch("/agent/{agentId}/templates")]
    public async Task<string> PatchAgentTemplates([FromRoute] string agentId, [FromBody] AgentTemplatePatchModel agent)
    {
        var model = agent.ToAgent();
        model.Id = agentId;
        return await _agentService.PatchAgentTemplate(model);
    }

    [HttpDelete("/agent/{agentId}")]
    public async Task<bool> DeleteAgent([FromRoute] string agentId)
    {
        return await _agentService.DeleteAgent(agentId);
    }

    [HttpGet("/agent/options")]
    public async Task<List<IdName>> GetAgentOptions()
    {
        return await _agentService.GetAgentOptions();
    }

    [HttpGet("/agent/utility/options")]
    public async Task<IEnumerable<AgentUtility>> GetAgentUtilityOptions()
    {
        var agentService = _services.GetRequiredService<IAgentService>();
        return await agentService.GetAgentUtilityOptions();
    }

    [HttpGet("/agent/labels")]
    public async Task<IEnumerable<string>> GetAgentLabels()
    {
        var agentService = _services.GetRequiredService<IAgentService>();
        var agents = await agentService.GetAgents(new AgentFilter
        {
            Pager = new Pagination { Size = 1000 }
        });

        var labels = agents.Items?.SelectMany(x => x.Labels)
                                  .Distinct()
                                  .OrderBy(x => x)
                                  .ToList() ?? [];
        return labels;
    }

    #region Agent Icon
    [HttpPost("/agent/{agentId}/icon")]
    public string UploadAgentIcon([FromRoute] string agentId, [FromBody] AgentIconModel input)
    {
        if (string.IsNullOrEmpty(agentId) || input == null || string.IsNullOrEmpty(input.FileData))
        {
            return string.Empty;
        }

        try
        {
            var fileStorage = _services.GetRequiredService<IFileStorageService>();
            var (_, binary) = FileUtility.GetFileInfoFromData(input.FileData);
            var extension = Path.GetExtension(input.FileName);
            var fileName = $"{agentId}{extension}";
            var dir = $"agents/{agentId}/icon/";

            var filePath = Path.Combine(dir, fileName);

            var result = fileStorage.SaveFileBytesToPath(filePath, BinaryData.FromBytes(binary.ToArray()));

            if (!result)
            {
                return string.Empty;
            }
            return filePath;
        }
        catch (Exception ex)
        {
            var logger = _services.GetService<ILogger<AgentController>>();
            logger?.LogError(ex, "Error uploading icon for agent {AgentId}", agentId);
            return string.Empty;
        }
    }

    [AllowAnonymous]
    [HttpGet("/agents/{agentId}/icon/{fileName}")]
    public IActionResult GetAgentIconByAgentId([FromRoute] string agentId, [FromRoute] string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(agentId) || string.IsNullOrEmpty(fileName))
            {
                return NotFound();
            }

            var fileStorage = _services.GetRequiredService<IFileStorageService>();

            // 构建图标文件路径
            var iconPath = $"agents/{agentId}/icon/{fileName}";
            var iconDirectory = $"agents/{agentId}/icon";

            // 检查目录是否存在
            if (!fileStorage.ExistDirectory(iconDirectory))
            {
                return NotFound();
            }

            // 获取文件字节数据
            var fileBytes = fileStorage.GetFileBytes(iconPath);
            if (fileBytes == null || fileBytes.Length == 0)
            {
                return NotFound();
            }

            // 根据文件扩展名确定MIME类型
            var contentType = GetImageContentType(fileName);

            // 返回图片文件
            return File(fileBytes.ToArray(), contentType);
        }
        catch (Exception ex)
        {
            // 记录错误日志
            var logger = _services.GetService<ILogger<AgentController>>();
            logger?.LogError(ex, "Error retrieving icon for agent {AgentId}, file {FileName}", agentId, fileName);

            return NotFound();
        }
    }
    #endregion

    #region Private methods
    private string GetImageContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        return extension switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            ".ico" => "image/x-icon",
            _ => "application/octet-stream"
        };
    }
    #endregion
}