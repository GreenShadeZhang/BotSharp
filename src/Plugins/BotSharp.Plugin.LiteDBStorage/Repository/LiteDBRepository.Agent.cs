using BotSharp.Abstraction.Agents.Models;
using BotSharp.Abstraction.Functions.Models;
using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Abstraction.Routing.Models;
using System.Threading.Tasks;

namespace BotSharp.Plugin.LiteDBStorage.Repository;

public partial class LiteDBRepository
{
    public void UpdateAgent(Agent agent, AgentField field)
    {
        if (agent == null || string.IsNullOrWhiteSpace(agent.Id)) return;

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
            case AgentField.Type:
                UpdateAgentType(agent.Id, agent.Type);
                break;
            case AgentField.InheritAgentId:
                UpdateAgentInheritAgentId(agent.Id, agent.InheritAgentId);
                break;
            case AgentField.Profile:
                UpdateAgentProfiles(agent.Id, agent.Profiles);
                break;
            case AgentField.Label:
                UpdateAgentLabels(agent.Id, agent.Profiles);
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

        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.Name = name;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }

    private void UpdateAgentDescription(string agentId, string description)
    {
        if (string.IsNullOrWhiteSpace(description)) return;
        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();
        if (agent != null)
        {
            agent.Description = description;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }

    private void UpdateAgentIsPublic(string agentId, bool isPublic)
    {
        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.IsPublic = isPublic;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }

    private void UpdateAgentDisabled(string agentId, bool disabled)
    {
        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.Disabled = disabled;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }

    private void UpdateAgentType(string agentId, string type)
    {
        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.Type = type;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }

    private void UpdateAgentInheritAgentId(string agentId, string? inheritAgentId)
    {
        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.InheritAgentId = inheritAgentId;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }

    private void UpdateAgentProfiles(string agentId, List<string> profiles)
    {
        if (profiles == null) return;

        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.Profiles = profiles;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }
	
  public bool UpdateAgentLabels(string agentId, List<string> labels)
    {
        if (labels == null) return false;

        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();
        if (agent != null)
        {
            agent.Labels = labels;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
            return true;
        }
        return false;
    }

    private void UpdateAgentRoutingRules(string agentId, List<RoutingRule> rules)
    {
        if (rules == null) return;

        var ruleElements = rules.Select(x => RoutingRuleLiteDBElement.ToLiteDBElement(x)).ToList();

        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.RoutingRules = ruleElements;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }

    private void UpdateAgentInstructions(string agentId, string instruction, List<ChannelInstruction>? channelInstructions)
    {
        if (string.IsNullOrWhiteSpace(agentId)) return;

        var instructionElements = channelInstructions?.Select(x => ChannelInstructionLiteDBElement.ToLiteDBElement(x))?
                                                      .ToList() ?? new List<ChannelInstructionLiteDBElement>();

        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.Instruction = instruction;
            agent.ChannelInstructions = instructionElements;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }

    private void UpdateAgentFunctions(string agentId, List<FunctionDef> functions)
    {
        if (functions == null) return;

        var functionsToUpdate = functions.Select(f => FunctionDefLiteDBElement.ToLiteDBElement(f)).ToList();
        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.Functions = functionsToUpdate;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }

    private void UpdateAgentTemplates(string agentId, List<AgentTemplate> templates)
    {
        if (templates == null) return;

        var templatesToUpdate = templates.Select(t => AgentTemplateLiteDBElement.ToLiteDBElement(t)).ToList();

        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.Templates = templatesToUpdate;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }

    private void UpdateAgentResponses(string agentId, List<AgentResponse> responses)
    {
        if (responses == null) return;

        var responsesToUpdate = responses.Select(r => AgentResponseLiteDBElement.ToLiteDBElement(r)).ToList();

        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.Responses = responsesToUpdate;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }

    private void UpdateAgentSamples(string agentId, List<string> samples)
    {
        if (samples == null) return;
        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.Samples = samples;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }
	
  private void UpdateAgentUtilities(string agentId, bool mergeUtility, List<AgentUtility> utilities)
    {
        if (utilities == null) return;

        var elements = utilities.Select(x => AgentUtilityLiteDBElement.ToLiteDBElement(x)).ToList();

        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();
        if (agent != null)
        {
            agent.MergeUtility = mergeUtility;
            agent.Utilities = elements;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }
	
  private void UpdateAgentKnowledgeBases(string agentId, List<AgentKnowledgeBase> knowledgeBases)
    {
        if (knowledgeBases == null) return;

        var elements = knowledgeBases.Select(x => AgentKnowledgeBaseLiteDBElement.ToLiteDBElement(x)).ToList();

        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();
        if (agent != null)
        {
            agent.KnowledgeBases = elements;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }
	
  private void UpdateAgentRules(string agentId, List<AgentRule> rules)
    {
        if (rules == null) return;

        var elements = rules.Select(x => AgentRuleLiteDBElement.ToLiteDBElement(x)).ToList();

        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();
        if (agent != null)
        {
            agent.Rules = elements;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }
	

    private void UpdateAgentLlmConfig(string agentId, AgentLlmConfig? config)
    {
        var llmConfig = AgentLlmConfigLiteDBElement.ToLiteDBElement(config);

        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.LlmConfig = llmConfig;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }
	
  private void UpdateAgentMaxMessageCount(string agentId, int? maxMessageCount)
    {
        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();
        if (agent != null)
        {
            agent.MaxMessageCount = maxMessageCount;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }

    private void UpdateAgentAllFields(Agent agent)
    {
        var agentData = _dc.Agents.Query().Where(x => x.Id == agent.Id).FirstOrDefault();

        if (agentData != null)
        {
            agentData.Name = agent.Name;
            agentData.Description = agent.Description;
            agentData.Disabled = agent.Disabled;
            agentData.MergeUtility = agent.MergeUtility;
            agentData.Type = agent.Type;
            agentData.MaxMessageCount = agent.MaxMessageCount;
            agentData.Profiles = agent.Profiles;
            agentData.Labels = agent.Labels;
            agentData.RoutingRules = agent.RoutingRules.Select(r => RoutingRuleLiteDBElement.ToLiteDBElement(r)).ToList();
            agentData.Instruction = agent.Instruction;
            agentData.ChannelInstructions = agent.ChannelInstructions.Select(i => ChannelInstructionLiteDBElement.ToLiteDBElement(i)).ToList();
            agentData.Templates = agent.Templates.Select(t => AgentTemplateLiteDBElement.ToLiteDBElement(t)).ToList();
            agentData.Functions = agent.Functions.Select(f => FunctionDefLiteDBElement.ToLiteDBElement(f)).ToList();
            agentData.Responses = agent.Responses.Select(r => AgentResponseLiteDBElement.ToLiteDBElement(r)).ToList();
            agentData.Samples = agent.Samples;
            agentData.Utilities = agent.Utilities.Select(u => AgentUtilityLiteDBElement.ToLiteDBElement(u)).ToList();
            agentData.KnowledgeBases = agent.KnowledgeBases.Select(u => AgentKnowledgeBaseLiteDBElement.ToLiteDBElement(u)).ToList();
            agentData.Rules = agent.Rules.Select(e => AgentRuleLiteDBElement.ToLiteDBElement(e)).ToList();
            agentData.LlmConfig = AgentLlmConfigLiteDBElement.ToLiteDBElement(agent.LlmConfig);
            agentData.IsPublic = agent.IsPublic;
            agentData.UpdatedTime = DateTime.UtcNow;
        }

        _dc.Agents.Update(agentData);
    }
    #endregion

    public Agent? GetAgent(string agentId, bool basicsOnly = false)
    {
        var agent = _dc.Agents.FindOne(x => x.Id == agentId);
        if (agent == null) return null;

        return TransformAgentDocument(agent);
    }

    public Agent? GetAgent(string agentId)
    {
        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();
        if (agent == null) return null;

        return TransformAgentDocument(agent);
    }

    public List<Agent> GetAgents(AgentFilter filter)
    {
        var agents = new List<Agent>();
        var query = _dc.Agents.Query();
        if (filter.AgentIds != null)
        {
            query = query.Where(x => filter.AgentIds.Contains(x.Id));
        }
        if (!filter.AgentNames.IsNullOrEmpty())
        {
            query = query.Where(x => filter.AgentNames.Contains(x.Name));
        }

        if (!string.IsNullOrEmpty(filter.SimilarName))
        {
            query = query.Where(x => x.Name.Contains(filter.SimilarName, StringComparison.OrdinalIgnoreCase));
        }

        if (filter.Disabled.HasValue)
        {
            query = query.Where(x => x.Disabled == filter.Disabled.Value);
        }

        if (filter.Types != null)
        {
            query = query.Where(x => filter.Types.Contains(x.Type));
        }

        if (!filter.Labels.IsNullOrEmpty())
        {
            query = query.Where(x => x.Labels.Any(label => filter.Labels.Contains(label)));
        }



        var agentDocs = query.ToList();

        return agentDocs.Select(x => TransformAgentDocument(x)).ToList();
    }
	
    public List<UserAgent> GetUserAgents(string userId)
    {
        var user = _dc.Users.Query().Where(x => x.Id == userId || x.ExternalId == userId).FirstOrDefault();

        if (user == null)
        {
            return [];
        }
        var found = _dc.UserAgents.Query().Where(x => x.UserId == user.Id).ToList();

        if (found.IsNullOrEmpty()) return [];

        var res = found.Select(x => new UserAgent
        {
            Id = x.Id,
            UserId = x.UserId,
            AgentId = x.AgentId,
            Actions = x.Actions,
            CreatedTime = x.CreatedTime,
            UpdatedTime = x.UpdatedTime
        }).ToList();

        var agentIds = found.Select(x => x.AgentId).Distinct().ToList();
        var agents = GetAgents(new AgentFilter { AgentIds = agentIds });
        foreach (var item in res)
        {
            var agent = agents.FirstOrDefault(x => x.Id == item.AgentId);
            if (agent == null) continue;

            item.Agent = agent;
        }

        return res;
    }
	

    public Task<List<Agent>> GetAgentsByUserAsync(string userId)
    {
        var user = _dc.Users.Query().Where(x => x.Id == userId || x.ExternalId == userId).FirstOrDefault();

        if (user == null)
        {
            return Task.FromResult(new List<Agent>());
        }

        var agentIds = _dc.UserAgents.Query().Where(x => x.UserId == user.Id).Select(ua => ua.Id).ToList();

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
        var agent = _dc.Agents.FindOne(x => x.Id == agentId);
        if (agent == null) return responses;

        return agent.Responses.Where(x => x.Prefix == prefix && x.Intent == intent).Select(x => x.Content).ToList();
    }

    public string GetAgentTemplate(string agentId, string templateName)
    {
        var agent = _dc.Agents.FindOne(x => x.Id == agentId);
        if (agent == null) return string.Empty;

        return agent.Templates?.FirstOrDefault(x => x.Name == templateName.ToLower())?.Content ?? string.Empty;
    }

    public bool PatchAgentTemplate(string agentId, AgentTemplate template)
    {
        if (string.IsNullOrEmpty(agentId) || template == null) return false;

        var agent = _dc.Agents.FindOne(x => x.Id == agentId);
        if (agent == null || agent.Templates.IsNullOrEmpty()) return false;

        var foundTemplate = agent.Templates.FirstOrDefault(x => x.Name.IsEqualTo(template.Name));
        if (foundTemplate == null) return false;

        foundTemplate.Content = template.Content;
        agent.Templates = agent.Templates;
        _dc.Agents.Update(agent);
        return true;
    }
    public bool AppendAgentLabels(string agentId, List<string> labels)
    {
        if (labels == null || !labels.Any()) return false;

        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();
        if (agent == null) return false;

        var prevLabels = agent.Labels ?? new List<string>();
        var curLabels = prevLabels.Concat(labels).Distinct().ToList();
        agent.Labels = curLabels;
        agent.UpdatedTime = DateTime.UtcNow;

        _dc.Agents.Update(agent);
        return true;
    }

    public void BulkInsertAgents(List<Agent> agents)
    {
        if (agents.IsNullOrEmpty()) return;

        var agentDocs = agents.Select(x => new AgentDocument
        {
            Id = !string.IsNullOrEmpty(x.Id) ? x.Id : Guid.NewGuid().ToString(),
            Name = x.Name,
            IconUrl = x.IconUrl,
            Description = x.Description,
            Instruction = x.Instruction,
            Samples = x.Samples ?? [],
            IsPublic = x.IsPublic,
            Type = x.Type,
            InheritAgentId = x.InheritAgentId,
            Disabled = x.Disabled,
            MergeUtility = x.MergeUtility,
            MaxMessageCount = x.MaxMessageCount,
            Profiles = x.Profiles ?? [],
            Labels = x.Labels ?? [],
            LlmConfig = AgentLlmConfigLiteDBElement.ToLiteDBElement(x.LlmConfig),
            ChannelInstructions = x.ChannelInstructions?.Select(i => ChannelInstructionLiteDBElement.ToLiteDBElement(i))?.ToList() ?? [],
            Templates = x.Templates?.Select(t => AgentTemplateLiteDBElement.ToLiteDBElement(t))?.ToList() ?? [],
            Functions = x.Functions?.Select(f => FunctionDefLiteDBElement.ToLiteDBElement(f))?.ToList() ?? [],
            Responses = x.Responses?.Select(r => AgentResponseLiteDBElement.ToLiteDBElement(r))?.ToList() ?? [],
            RoutingRules = x.RoutingRules?.Select(r => RoutingRuleLiteDBElement.ToLiteDBElement(r))?.ToList() ?? [],
            Utilities = x.Utilities?.Select(u => AgentUtilityLiteDBElement.ToLiteDBElement(u))?.ToList() ?? [],
            KnowledgeBases = x.KnowledgeBases?.Select(k => AgentKnowledgeBaseLiteDBElement.ToLiteDBElement(k))?.ToList() ?? [],
            Rules = x.Rules?.Select(e => AgentRuleLiteDBElement.ToLiteDBElement(e))?.ToList() ?? [],
            CreatedTime = x.CreatedDateTime,
            UpdatedTime = x.UpdatedDateTime
        }).ToList();

        _dc.Agents.InsertBulk(agentDocs);
    }

    public void BulkInsertUserAgents(List<UserAgent> userAgents)
    {
        if (userAgents.IsNullOrEmpty()) return;

        var filtered = userAgents.Where(x => !string.IsNullOrEmpty(x.UserId) && !string.IsNullOrEmpty(x.AgentId)).ToList();
        if (filtered.IsNullOrEmpty()) return;

        var userAgentDocs = filtered.Select(x => new UserAgentDocument
        {
            Id = !string.IsNullOrEmpty(x.Id) ? x.Id : Guid.NewGuid().ToString(),
            UserId = x.UserId,
            AgentId = x.AgentId,
            Actions = x.Actions,
            CreatedTime = x.CreatedTime,
            UpdatedTime = x.UpdatedTime
        }).ToList();

        _dc.UserAgents.InsertBulk(userAgentDocs);
    }

    public bool DeleteAgents()
    {
        try
        {
            _dc.UserAgents.DeleteAll();
            _dc.Agents.DeleteAll();
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

            _dc.Agents.DeleteMany(x => x.Id == agentId);
            _dc.UserAgents.DeleteMany(x => x.AgentId == agentId);
            _dc.AgentTasks.DeleteMany(x => x.AgentId == agentId);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private Agent TransformAgentDocument(AgentDocument? agentDoc)
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
            InheritAgentId = agentDoc.InheritAgentId,
            Profiles = agentDoc.Profiles ?? [],
			Labels = agentDoc.Labels ?? [],
			MaxMessageCount = agentDoc.MaxMessageCount,
            LlmConfig = AgentLlmConfigLiteDBElement.ToDomainElement(agentDoc.LlmConfig),
            ChannelInstructions = agentDoc.ChannelInstructions?.Select(i => ChannelInstructionLiteDBElement.ToDomainElement(i))?.ToList() ?? [],
            Templates = agentDoc.Templates?.Select(t => AgentTemplateLiteDBElement.ToDomainElement(t))?.ToList() ?? [],
            Functions = agentDoc.Functions?.Select(f => FunctionDefLiteDBElement.ToDomainElement(f)).ToList() ?? [],
            Responses = agentDoc.Responses?.Select(r => AgentResponseLiteDBElement.ToDomainElement(r))?.ToList() ?? [],
            RoutingRules = agentDoc.RoutingRules?.Select(r => RoutingRuleLiteDBElement.ToDomainElement(agentDoc.Id, agentDoc.Name, r))?.ToList() ?? [],
            Utilities = agentDoc.Utilities?.Select(u => AgentUtilityLiteDBElement.ToDomainElement(u))?.ToList() ?? [],
            KnowledgeBases = agentDoc.KnowledgeBases?.Select(x => AgentKnowledgeBaseLiteDBElement.ToDomainElement(x))?.ToList() ?? [],
            Rules = agentDoc.Rules?.Select(e => AgentRuleLiteDBElement.ToDomainElement(e))?.ToList() ?? []
        };
    }
}
