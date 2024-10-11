using BotSharp.Abstraction.Agents.Models;
using BotSharp.Abstraction.Functions.Models;
using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Abstraction.Routing.Models;

namespace BotSharp.Plugin.LiteDBStorage.Repository;

public partial class MongoRepository
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
            case AgentField.Type:
                UpdateAgentType(agent.Id, agent.Type);
                break;
            case AgentField.InheritAgentId:
                UpdateAgentInheritAgentId(agent.Id, agent.InheritAgentId);
                break;
            case AgentField.Profiles:
                UpdateAgentProfiles(agent.Id, agent.Profiles);
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
                UpdateAgentUtilities(agent.Id, agent.Utilities);
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

    private void UpdateAgentRoutingRules(string agentId, List<RoutingRule> rules)
    {
        if (rules == null) return;

        var ruleElements = rules.Select(x => RoutingRuleMongoElement.ToMongoElement(x)).ToList();

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

        var instructionElements = channelInstructions?.Select(x => ChannelInstructionMongoElement.ToMongoElement(x))?
                                                      .ToList() ?? new List<ChannelInstructionMongoElement>();

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

        var functionsToUpdate = functions.Select(f => FunctionDefMongoElement.ToMongoElement(f)).ToList();
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

        var templatesToUpdate = templates.Select(t => AgentTemplateMongoElement.ToMongoElement(t)).ToList();

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

        var responsesToUpdate = responses.Select(r => AgentResponseMongoElement.ToMongoElement(r)).ToList();

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

    private void UpdateAgentUtilities(string agentId, List<string> utilities)
    {
        if (utilities == null) return;

        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.Utilities = utilities;
            agent.UpdatedTime = DateTime.UtcNow;
            _dc.Agents.Update(agent);
        }
    }

    private void UpdateAgentLlmConfig(string agentId, AgentLlmConfig? config)
    {
        var llmConfig = AgentLlmConfigMongoElement.ToMongoElement(config);

        var agent = _dc.Agents.Query().Where(x => x.Id == agentId).FirstOrDefault();

        if (agent != null)
        {
            agent.LlmConfig = llmConfig;
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
            agentData.Type = agent.Type;
            agentData.Profiles = agent.Profiles;
            agentData.RoutingRules = agent.RoutingRules.Select(r => RoutingRuleMongoElement.ToMongoElement(r)).ToList();
            agentData.Instruction = agent.Instruction;
            agentData.ChannelInstructions = agent.ChannelInstructions.Select(i => ChannelInstructionMongoElement.ToMongoElement(i)).ToList();
            agentData.Templates = agent.Templates.Select(t => AgentTemplateMongoElement.ToMongoElement(t)).ToList();
            agentData.Functions = agent.Functions.Select(f => FunctionDefMongoElement.ToMongoElement(f)).ToList();
            agentData.Responses = agent.Responses.Select(r => AgentResponseMongoElement.ToMongoElement(r)).ToList();
            agentData.Samples = agent.Samples;
            agentData.Utilities = agent.Utilities;
            agentData.LlmConfig = AgentLlmConfigMongoElement.ToMongoElement(agent.LlmConfig);
            agentData.IsPublic = agent.IsPublic;
            agentData.UpdatedTime = DateTime.UtcNow;
        }

        _dc.Agents.Update(agentData);
    }
    #endregion


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

        if (!string.IsNullOrEmpty(filter.AgentName))
        {
            query = query.Where(x => x.Name == filter.AgentName);
        }

        if (filter.Disabled.HasValue)
        {
            query = query.Where(x => x.Disabled == filter.Disabled.Value);
        }

        if (filter.Type != null)
        {
            var types = filter.Type.Split(",");
            query = query.Where(x => types.Contains(x.Type));
        }

        if (filter.IsPublic.HasValue)
        {
            query = query.Where(x => x.IsPublic == filter.IsPublic.Value);
        }

        if (filter.AgentIds != null)
        {
            query = query.Where(x => filter.AgentIds.Contains(x.Id));
        }

        var agentDocs = query.ToList();

        return agentDocs.Select(x => TransformAgentDocument(x)).ToList();
    }

    public List<Agent> GetAgentsByUser(string userId)
    {
        var user = _dc.Users.Query().Where(x => x.Id == userId || x.ExternalId == userId).FirstOrDefault();

        if (user == null)
        {
            return new List<Agent>();
        }

        var agentIds = _dc.UserAgents.Query().Where(x => x.UserId == user.Id).Select(ua => ua.Id).ToList();

        var filter = new AgentFilter
        {
            AgentIds = agentIds
        };
        var agents = GetAgents(filter);
        return agents;
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
            ChannelInstructions = x.ChannelInstructions?
                            .Select(i => ChannelInstructionMongoElement.ToMongoElement(i))?
                            .ToList() ?? new List<ChannelInstructionMongoElement>(),
            Templates = x.Templates?
                            .Select(t => AgentTemplateMongoElement.ToMongoElement(t))?
                            .ToList() ?? new List<AgentTemplateMongoElement>(),
            Functions = x.Functions?
                            .Select(f => FunctionDefMongoElement.ToMongoElement(f))?
                            .ToList() ?? new List<FunctionDefMongoElement>(),
            Responses = x.Responses?
                            .Select(r => AgentResponseMongoElement.ToMongoElement(r))?
                            .ToList() ?? new List<AgentResponseMongoElement>(),
            Samples = x.Samples ?? new List<string>(),
            Utilities = x.Utilities ?? new List<string>(),
            IsPublic = x.IsPublic,
            Type = x.Type,
            InheritAgentId = x.InheritAgentId,
            Disabled = x.Disabled,
            Profiles = x.Profiles,
            RoutingRules = x.RoutingRules?
                            .Select(r => RoutingRuleMongoElement.ToMongoElement(r))?
                            .ToList() ?? new List<RoutingRuleMongoElement>(),
            LlmConfig = AgentLlmConfigMongoElement.ToMongoElement(x.LlmConfig),
            CreatedTime = x.CreatedDateTime,
            UpdatedTime = x.UpdatedDateTime
        }).ToList();

        _dc.Agents.InsertBulk(agentDocs);
    }

    public void BulkInsertUserAgents(List<UserAgent> userAgents)
    {
        if (userAgents.IsNullOrEmpty()) return;

        var userAgentDocs = userAgents.Select(x => new UserAgentDocument
        {
            Id = !string.IsNullOrEmpty(x.Id) ? x.Id : Guid.NewGuid().ToString(),
            AgentId = x.AgentId,
            UserId = !string.IsNullOrEmpty(x.UserId) ? x.UserId : string.Empty,
            Editable = x.Editable,
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
            ChannelInstructions = !agentDoc.ChannelInstructions.IsNullOrEmpty() ? agentDoc.ChannelInstructions
                              .Select(i => ChannelInstructionMongoElement.ToDomainElement(i))
                              .ToList() : new List<ChannelInstruction>(),
            Templates = !agentDoc.Templates.IsNullOrEmpty() ? agentDoc.Templates
                             .Select(t => AgentTemplateMongoElement.ToDomainElement(t))
                             .ToList() : new List<AgentTemplate>(),
            Functions = !agentDoc.Functions.IsNullOrEmpty() ? agentDoc.Functions
                             .Select(f => FunctionDefMongoElement.ToDomainElement(f))
                             .ToList() : new List<FunctionDef>(),
            Responses = !agentDoc.Responses.IsNullOrEmpty() ? agentDoc.Responses
                             .Select(r => AgentResponseMongoElement.ToDomainElement(r))
                             .ToList() : new List<AgentResponse>(),
            RoutingRules = !agentDoc.RoutingRules.IsNullOrEmpty() ? agentDoc.RoutingRules
                                .Select(r => RoutingRuleMongoElement.ToDomainElement(agentDoc.Id, agentDoc.Name, r))
                                .ToList() : new List<RoutingRule>(),
            LlmConfig = AgentLlmConfigMongoElement.ToDomainElement(agentDoc.LlmConfig),
            Samples = agentDoc.Samples ?? new List<string>(),
            Utilities = agentDoc.Utilities ?? new List<string>(),
            IsPublic = agentDoc.IsPublic,
            Disabled = agentDoc.Disabled,
            Type = agentDoc.Type,
            InheritAgentId = agentDoc.InheritAgentId,
            Profiles = agentDoc.Profiles,
        };
    }
}
