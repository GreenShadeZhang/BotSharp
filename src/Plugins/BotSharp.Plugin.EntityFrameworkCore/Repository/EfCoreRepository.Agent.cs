using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Plugin.EntityFrameworkCore.Mappers;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BotSharp.Plugin.EntityFrameworkCore.Repository;

public partial class EfCoreRepository
{
    public void UpdateAgent(Agent agent, AgentField field)
    {
        if (agent == null || string.IsNullOrEmpty(agent.Id)) return;

        switch (field)
        {
            case AgentField.Name:
                UpdateAgentName(agent.Id, agent.Name);
                break;
            case AgentField.Description:
                UpdateAgentDescription(agent.Id, agent.Description);
                break;
            case AgentField.IsPublic:
                UpdateAgentIsPublic(agent.Id, agent.IsPublic);
                break;
            case AgentField.Disabled:
                UpdateAgentDisabled(agent.Id, agent.Disabled);
                break;
            case AgentField.RoutingMode:
                UpdateAgentRoutingMode(agent.Id, agent.Mode);
                break;
            case AgentField.InheritAgentId:
                UpdateAgentInheritAgentId(agent.Id, agent.InheritAgentId);
                break;
            case AgentField.Label:
                UpdateAgentLabels(agent.Id, agent.Labels);
                break;
            case AgentField.RoutingRule:
                UpdateAgentRoutingRules(agent.Id, agent.RoutingRules);
                break;
            case AgentField.Instruction:
                UpdateAgentInstructions(agent.Id, agent.Instruction, agent.ChannelInstructions);
                break;
            case AgentField.Function:
                UpdateAgentFunctions(agent.Id, agent.Functions);
                break;
            case AgentField.Template:
                UpdateAgentTemplates(agent.Id, agent.Templates);
                break;
            case AgentField.Response:
                UpdateAgentResponses(agent.Id, agent.Responses);
                break;
            case AgentField.Sample:
                UpdateAgentSamples(agent.Id, agent.Samples);
                break;
            case AgentField.LlmConfig:
                UpdateAgentLlmConfig(agent.Id, agent.LlmConfig);
                break;
            case AgentField.Utility:
                UpdateAgentUtilities(agent.Id, agent.MergeUtility, agent.Utilities);
                break;
            case AgentField.McpTool:
                UpdateAgentMcpTools(agent.Id, agent.McpTools);
                break;
            case AgentField.KnowledgeBase:
                UpdateAgentKnowledgeBases(agent.Id, agent.KnowledgeBases);
                break;
            case AgentField.Rule:
                UpdateAgentRules(agent.Id, agent.Rules);
                break;
            case AgentField.MaxMessageCount:
                UpdateAgentMaxMessageCount(agent.Id, agent.MaxMessageCount);
                break;
            case AgentField.All:
                UpdateAgentAllFields(agent);
                break;
            default:
                break;
        }
    }

    #region Update Agent Fields
    private void UpdateAgentName(string agentId, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.Name = name;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentDescription(string agentId, string description)
    {
        if (string.IsNullOrWhiteSpace(description)) return;

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.Description = description;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentIsPublic(string agentId, bool isPublic)
    {
        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.IsPublic = isPublic;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentDisabled(string agentId, bool disabled)
    {
        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.Disabled = disabled;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentType(string agentId, string type)
    {
        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);
        if (agent != null)
        {
            agent.Type = type;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentRoutingMode(string agentId, string? mode)
    {
        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);
        if (agent != null)
        {
            agent.Mode = mode;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentInheritAgentId(string agentId, string? inheritAgentId)
    {
        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);
        if (agent != null)
        {
            agent.InheritAgentId = inheritAgentId;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentProfiles(string agentId, List<string> profiles)
    {
        if (profiles == null) return;

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.Profiles = profiles;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    public void UpdateAgentLabels(string agentId, List<string> labels)
    {
        if (labels == null) return;
        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.Labels = labels;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentRoutingRules(string agentId, List<RoutingRule> rules)
    {
        if (rules == null) return;

        var ruleElements = rules.Select(x => x.ToEntity()).ToList();

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.RoutingRules = ruleElements;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentInstructions(string agentId, string instruction, List<ChannelInstruction>? channelInstructions)
    {
        if (string.IsNullOrWhiteSpace(agentId)) return;

        var instructionElements = channelInstructions?.Select(x => x.ToEntity())?
                                                      .ToList() ?? new List<ChannelInstructionElement>();

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.Instruction = instruction;
            agent.ChannelInstructions = instructionElements;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentFunctions(string agentId, List<FunctionDef> functions)
    {
        if (functions == null) return;

        var functionsToUpdate = functions.Select(f => f.ToEntity()).ToList();

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.Functions = functionsToUpdate;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentTemplates(string agentId, List<AgentTemplate> templates)
    {
        if (templates == null) return;

        var templatesToUpdate = templates.Select(t => t.ToEntity()).ToList();

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.Templates = templatesToUpdate;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentResponses(string agentId, List<AgentResponse> responses)
    {
        if (responses == null) return;

        var responsesToUpdate = responses.Select(r => r.ToEntity()).ToList();

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.Responses = responsesToUpdate;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentSamples(string agentId, List<string> samples)
    {
        if (samples == null) return;

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.Samples = samples;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
    private void UpdateAgentUtilities(string agentId, bool mergeUtility, List<AgentUtility> utilities)
    {
        if (utilities == null) return;

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.MergeUtility = mergeUtility;
            agent.Utilities = utilities?.Select(x => x.ToEntity()).ToList() ?? new List<AgentUtilityElement>();
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
    private void UpdateAgentMcpTools(string agentId, List<McpTool> mcpTools)
    {
        if (mcpTools == null) return;

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.McpTools = mcpTools.Select(x => x.ToEntity()).ToList();
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
    private void UpdateAgentKnowledgeBases(string agentId, List<AgentKnowledgeBase> knowledgeBases)
    {
        if (knowledgeBases == null) return;

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.KnowledgeBases = knowledgeBases.Select(x => x.ToEntity()).ToList();
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
    private void UpdateAgentRules(string agentId, List<AgentRule> rules)
    {
        if (rules == null) return;

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.Rules = rules.Select(x => x.ToEntity()).ToList();
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentMaxMessageCount(string agentId, int? maxMessageCount)
    {
        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.MaxMessageCount = maxMessageCount;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    private void UpdateAgentLlmConfig(string agentId, AgentLlmConfig? config)
    {
        var llmConfig = config?.ToEntity();

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent != null)
        {
            agent.LlmConfig = llmConfig;
            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
    private void UpdateAgentAllFields(Agent agent)
    {
        var agentData = _context.Agents.FirstOrDefault(x => x.Id == agent.Id);

        if (agentData != null)
        {
            agentData.Name = agent.Name;
            agentData.Description = agent.Description;
            agentData.Disabled = agent.Disabled;
            agentData.MergeUtility = agent.MergeUtility;
            agentData.Type = agent.Type;
            agentData.Mode = agent.Mode;
            agentData.MaxMessageCount = agent.MaxMessageCount;
            agentData.InheritAgentId = agent.InheritAgentId;
            agentData.IconUrl = agent.IconUrl;
            agentData.Profiles = agent.Profiles;
            agentData.Labels = agent.Labels; agentData.RoutingRules = agent.RoutingRules?.Select(x => x.ToEntity()).ToList() ?? new List<RoutingRuleElement>();
            agentData.Instruction = agent.Instruction;
            agentData.ChannelInstructions = agent.ChannelInstructions?.Select(x => x.ToEntity()).ToList() ?? new List<ChannelInstructionElement>();
            agentData.Templates = agent.Templates?.Select(x => x.ToEntity()).ToList() ?? new List<AgentTemplateElement>();
            agentData.Functions = agent.Functions?.Select(x => x.ToEntity()).ToList() ?? new List<FunctionDefElement>();
            agentData.Responses = agent.Responses?.Select(x => x.ToEntity()).ToList() ?? new List<AgentResponseElement>();
            agentData.Samples = agent.Samples ?? new List<string>();
            agentData.Utilities = agent.Utilities?.Select(x => x.ToEntity()).ToList() ?? new List<AgentUtilityElement>();
            agentData.McpTools = agent.McpTools?.Select(x => x.ToEntity()).ToList() ?? new List<McpToolElement>();
            agentData.KnowledgeBases = agent.KnowledgeBases?.Select(x => x.ToEntity()).ToList() ?? new List<AgentKnowledgeBaseElement>();
            agentData.Rules = agent.Rules?.Select(x => x.ToEntity()).ToList() ?? new List<AgentRuleElement>();
            agentData.LlmConfig = agent.LlmConfig?.ToEntity();
            agentData.IsPublic = agent.IsPublic;
            agentData.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
    #endregion


    public Agent? GetAgent(string agentId, bool basicsOnly = false)
    {
        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);
        if (agent == null) return null;

        if (basicsOnly)
        {
            return new Agent
            {
                Id = agent.Id,
                Name = agent.Name,
                IconUrl = agent.IconUrl,
                Description = agent.Description,
                IsPublic = agent.IsPublic,
                Disabled = agent.Disabled,
                Type = agent.Type,
                Mode = agent.Mode,
                CreatedDateTime = agent.CreatedTime,
                UpdatedDateTime = agent.UpdatedTime
            };
        }

        return TransformAgentDocument(agent);
    }

    public List<Agent> GetAgents(AgentFilter filter)
    {
        var agents = new List<Agent>();

        var query = _context.Agents.AsQueryable();


        if (filter.Disabled.HasValue)
        {
            query = query.Where(x => x.Disabled == filter.Disabled.Value);
        }

        if (filter.IsPublic.HasValue)
        {
            query = query.Where(x => x.IsPublic == filter.IsPublic.Value);
        }

        if (filter.AgentIds != null)
        {
            query = query.Where(x => filter.AgentIds.Contains(x.Id));
        }

        if (filter.Types != null)
        {
            query = query.Where(x => filter.Types.Contains(x.Type));
        }

        if (filter.SimilarName != null)
        {
            query = query.Where(x => x.Name.ToLower().Contains(filter.SimilarName) || x.Description.ToLower().Contains(filter.SimilarName));
        }

        if (!string.IsNullOrEmpty(filter.Pager?.Sort))
        {

        }
        else
        {
            query = query.OrderByDescending(c => c.CreatedTime);
        }

        var agentDocs = query.ToList();
        return agentDocs.Select(x => TransformAgentDocument(x)).ToList();
    }

    public Task<List<Agent>> GetAgentsByUserAsync(string userId)
    {
        var agentIds = (from ua in _context.UserAgents.AsQueryable()
                        join u in _context.Users.AsQueryable() on ua.UserId equals u.Id
                        where ua.UserId == userId || u.ExternalId == userId
                        select ua.AgentId).ToList();

        var filter = new AgentFilter
        {
            AgentIds = agentIds
        };
        var agents = GetAgents(filter);
        return Task.FromResult(agents);
    }

    public List<string> GetAgentResponses(string agentId, string prefix, string intent)
    {
        var responses = new List<string>();

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

        if (agent == null) return responses;

        return agent.Responses.Where(x => x.Prefix == prefix && x.Intent == intent).Select(x => x.Content).ToList();
    }

    public string GetAgentTemplate(string agentId, string templateName)
    {
        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);
        if (agent == null) return string.Empty;

        return agent.Templates?.FirstOrDefault(x => x.Name == templateName.ToLower())?.Content ?? string.Empty;
    }

    public bool PatchAgentTemplate(string agentId, AgentTemplate template)
    {
        if (string.IsNullOrEmpty(agentId) || template == null) return false;

        var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);
        if (agent == null || agent.Templates.IsNullOrEmpty()) return false;

        var foundTemplate = agent.Templates.FirstOrDefault(x => x.Name.IsEqualTo(template.Name));
        if (foundTemplate == null) return false;

        foundTemplate.Content = template.Content;

        _context.Agents.Update(agent);
        _context.SaveChanges();
        return true;
    }
    public void BulkInsertAgents(List<Agent> agents)
    {
        if (agents.IsNullOrEmpty()) return;

        var agentDocs = agents.Select(x => new Entities.Agent
        {
            Id = !string.IsNullOrEmpty(x.Id) ? x.Id : Guid.NewGuid().ToString(),
            Name = x.Name,
            IconUrl = x.IconUrl,
            Description = x.Description,
            Instruction = x.Instruction,
            ChannelInstructions = x.ChannelInstructions?.Select(x => x.ToEntity()).ToList() ?? [],
            Templates = x.Templates?.Select(x => x.ToEntity()).ToList() ?? [],
            Functions = x.Functions?.Select(x => x.ToEntity()).ToList() ?? [],
            Responses = x.Responses?.Select(x => x.ToEntity()).ToList() ?? [],
            Samples = x.Samples ?? [],
            Utilities = x.Utilities?.Select(x => x.ToEntity()).ToList() ?? [],
            McpTools = x.McpTools?.Select(x => x.ToEntity()).ToList() ?? [],
            KnowledgeBases = x.KnowledgeBases?.Select(x => x.ToEntity()).ToList() ?? [],
            Rules = x.Rules?.Select(x => x.ToEntity()).ToList() ?? [],
            IsPublic = x.IsPublic,
            Disabled = x.Disabled,
            MergeUtility = x.MergeUtility,
            Type = x.Type,
            Mode = x.Mode,
            InheritAgentId = x.InheritAgentId,
            Profiles = x.Profiles ?? [],
            Labels = x.Labels ?? [],
            RoutingRules = x.RoutingRules?.Select(x => x.ToEntity()).ToList() ?? [],
            LlmConfig = x.LlmConfig?.ToEntity(),
            MaxMessageCount = x.MaxMessageCount,
            CreatedTime = x.CreatedDateTime,
            UpdatedTime = x.UpdatedDateTime
        }).ToList();

        _context.Agents.AddRange(agentDocs);
        _context.SaveChanges();
    }

    public List<UserAgent> GetUserAgents(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return new List<UserAgent>();

        var userAgents = _context.UserAgents.Where(x => x.UserId == userId).ToList();
        return userAgents.Select(x => new UserAgent
        {
            Id = x.Id,
            UserId = x.UserId,
            AgentId = x.AgentId,
            Actions = x.Actions,
            CreatedTime = x.CreatedTime,
            UpdatedTime = x.UpdatedTime
        }).ToList();
    }

    public void BulkInsertUserAgents(List<UserAgent> userAgents)
    {
        if (userAgents?.Any() != true) return;

        var userAgentEntities = userAgents.Select(x => new Entities.UserAgent
        {
            Id = !string.IsNullOrEmpty(x.Id) ? x.Id : Guid.NewGuid().ToString(),
            UserId = x.UserId,
            AgentId = x.AgentId,
            Actions = x.Actions.ToList(),
            CreatedTime = DateTime.UtcNow,
            UpdatedTime = DateTime.UtcNow
        });

        _context.UserAgents.AddRange(userAgentEntities);
        _context.SaveChanges();
    }

    public bool AppendAgentLabels(string agentId, List<string> labels)
    {
        try
        {
            var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);
            if (agent == null || labels?.Any() != true) return false;

            agent.Labels ??= new List<string>();
            foreach (var label in labels.Where(l => !agent.Labels.Contains(l)))
            {
                agent.Labels.Add(label);
            }

            agent.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error appending agent labels for {AgentId}", agentId);
            return false;
        }
    }

    public bool DeleteAgents()
    {
        try
        {
            _context.UserAgents.RemoveRange(_context.UserAgents);
            _context.Agents.RemoveRange(_context.Agents);
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool DeleteAgent(string agentId)
    {
        try
        {
            if (string.IsNullOrEmpty(agentId)) return false;

            var agent = _context.Agents.FirstOrDefault(x => x.Id == agentId);

            if (agent != null)
            {
                _context.Agents.Remove(agent);
            }

            var agentUser = _context.UserAgents.FirstOrDefault(x => x.AgentId == agentId);

            if (agentUser != null)
            {
                _context.UserAgents.Remove(agentUser);
            }

            var agentTask = _context.AgentTasks.FirstOrDefault(x => x.AgentId == agentId);

            if (agentTask != null)
            {
                _context.AgentTasks.Remove(agentTask);
            }
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
    private Agent TransformAgentDocument(Entities.Agent? agentDoc)
    {
        if (agentDoc == null) return new Agent();

        return new Agent
        {
            Id = agentDoc.Id,
            Name = agentDoc.Name,
            IconUrl = agentDoc.IconUrl,
            Description = agentDoc.Description,
            Instruction = agentDoc.Instruction,
            Samples = agentDoc.Samples ?? [],
            IsPublic = agentDoc.IsPublic,
            Disabled = agentDoc.Disabled,
            MergeUtility = agentDoc.MergeUtility,
            Type = agentDoc.Type,
            Mode = agentDoc.Mode,
            InheritAgentId = agentDoc.InheritAgentId,
            Profiles = agentDoc.Profiles ?? [],
            Labels = agentDoc.Labels ?? [],
            MaxMessageCount = agentDoc.MaxMessageCount,
            LlmConfig = agentDoc.LlmConfig?.ToModel() ?? new(),
            ChannelInstructions = agentDoc.ChannelInstructions?.Select(x => x.ToModel()).ToList() ?? [],
            Templates = agentDoc.Templates?.Select(x => x.ToModel()).ToList() ?? [],
            Functions = agentDoc.Functions?.Select(x => x.ToModel()).ToList() ?? [],
            Responses = agentDoc.Responses?.Select(x => x.ToModel()).ToList() ?? [],
            RoutingRules = agentDoc.RoutingRules?.Select(x => x.ToModel(agentDoc.Id, agentDoc.Name)).ToList() ?? [],
            Utilities = agentDoc.Utilities?.Select(x => x.ToModel()).ToList() ?? [],
            McpTools = agentDoc.McpTools?.Select(x => x.ToModel()).ToList() ?? [],
            KnowledgeBases = agentDoc.KnowledgeBases?.Select(x => x.ToModel()).ToList() ?? [],
            Rules = agentDoc.Rules?.Select(x => x.ToModel()).ToList() ?? [],
            CreatedDateTime = agentDoc.CreatedTime,
            UpdatedDateTime = agentDoc.UpdatedTime
        };
    }
}
