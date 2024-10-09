using BotSharp.Abstraction.Agents.Models;
using BotSharp.Abstraction.Users.Models;

namespace BotSharp.Plugin.LiteDBStorage.Repository;

public partial class MongoRepository
{
    public void Add<TTableInterface>(object entity)
    {
        if (entity is Agent agent)
        {
            _agents.Add(agent);
            _changedTableNames.Add(nameof(Agent));
        }
        else if (entity is User user)
        {
            _users.Add(user);
            _changedTableNames.Add(nameof(User));
        }
        else if (entity is UserAgent userAgent)
        {
            _userAgents.Add(userAgent);
            _changedTableNames.Add(nameof(UserAgent));
        }
    }

    public int Transaction<TTableInterface>(Action action)
    {
        _changedTableNames.Clear();
        action();

        foreach (var table in _changedTableNames)
        {
            if (table == nameof(Agent))
            {
                var agents = _agents.Select(x => new AgentDocument
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

                foreach (var agent in agents)
                {
                    var agentData = _dc.Agents.Find(x => x.Id == agent.Id)?.FirstOrDefault();
                    if (agentData != null)
                    {
                        agentData.Name = agent.Name;
                        agentData.Description = agent.Description;
                        agentData.Instruction = agent.Instruction;
                        agentData.ChannelInstructions = agent.ChannelInstructions;
                        agentData.Templates = agent.Templates;
                        agentData.Functions = agent.Functions;
                        agentData.Responses = agent.Responses;
                        agentData.Samples = agent.Samples;
                        agentData.Utilities = agent.Utilities;
                        agentData.IsPublic = agent.IsPublic;
                        agentData.Type = agent.Type;
                        agentData.InheritAgentId = agent.InheritAgentId;
                        agentData.Disabled = agent.Disabled;
                        agentData.Profiles = agent.Profiles;
                        agentData.RoutingRules = agent.RoutingRules;
                        agentData.LlmConfig = agent.LlmConfig;
                        agentData.CreatedTime = agent.CreatedTime;
                        agentData.UpdatedTime = agent.UpdatedTime;
                        _dc.Agents.Update(agentData);
                    }              
                }
            }
            else if (table == nameof(User))
            {
                var users = _users.Select(x => new UserDocument
                {
                    Id = !string.IsNullOrEmpty(x.Id) ? x.Id : Guid.NewGuid().ToString(),
                    UserName = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Salt = x.Salt,
                    Password = x.Password,
                    Email = x.Email,
                    ExternalId = x.ExternalId,
                    Role = x.Role,
                    CreatedTime = x.CreatedTime,
                    UpdatedTime = x.UpdatedTime
                }).ToList();

                foreach (var user in users)
                {
                    var userData = _dc.Users.Find(x => x.Id == user.Id)?.FirstOrDefault();
                    if (userData != null)
                    {
                        userData.UserName = user.UserName;
                        userData.FirstName = user.FirstName;
                        userData.LastName = user.LastName;
                        userData.Email = user.Email;
                        userData.Salt = user.Salt;
                        userData.Password = user.Password;
                        userData.ExternalId = user.ExternalId;
                        userData.Role = user.Role;
                        userData.CreatedTime = user.CreatedTime;
                        userData.UpdatedTime = user.UpdatedTime;
                        _dc.Users.Update(userData);
                    }
                }
            }
            else if (table == nameof(UserAgent))
            {
                var userAgents = _userAgents.Select(x => new UserAgentDocument
                {
                    Id = !string.IsNullOrEmpty(x.Id) ? x.Id : Guid.NewGuid().ToString(),
                    AgentId = x.AgentId,
                    UserId = !string.IsNullOrEmpty(x.UserId) ? x.UserId : string.Empty,
                    Editable = x.Editable,
                    CreatedTime = x.CreatedTime,
                    UpdatedTime = x.UpdatedTime
                }).ToList();

                foreach (var userAgent in userAgents)
                {
                    var userAgentData = _dc.UserAgents.Find(x => x.Id == userAgent.Id)?.FirstOrDefault();
                    if (userAgentData != null)
                    {
                        userAgentData.AgentId = userAgent.AgentId;
                        userAgentData.UserId = userAgent.UserId;
                        userAgentData.Editable = userAgent.Editable;
                        userAgentData.CreatedTime = userAgent.CreatedTime;
                        userAgentData.UpdatedTime = userAgent.UpdatedTime;
                        _dc.UserAgents.Update(userAgentData);
                    }
                }
            }
        }

        return _changedTableNames.Count;
    }
}
