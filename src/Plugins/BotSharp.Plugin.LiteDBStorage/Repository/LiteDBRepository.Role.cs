using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Abstraction.Roles.Models;

namespace BotSharp.Plugin.LiteDBStorage.Repository;

public partial class LiteDBRepository
{
    public bool RefreshRoles(IEnumerable<Role> roles)
    {
        if (roles == null || !roles.Any()) return false;

        var validRoles = roles.Where(x => !string.IsNullOrWhiteSpace(x.Id) && !string.IsNullOrWhiteSpace(x.Name)).ToList();
        if (!validRoles.Any()) return false;

        // Clear data
        _dc.RoleAgents.DeleteMany(x => true);
        _dc.Roles.DeleteMany(x => true);

        var roleDocs = validRoles.Select(x => new RoleDocument
        {
            Id = x.Id,
            Name = x.Name,
            Permissions = x.Permissions,
            CreatedTime = DateTime.UtcNow,
            UpdatedTime = DateTime.UtcNow
        }).ToList();
        _dc.Roles.InsertBulk(roleDocs);

        return true;
    }

    public IEnumerable<Role> GetRoles(RoleFilter filter)
    {
        if (filter == null)
        {
            filter = RoleFilter.Empty();
        }

        var query = _dc.Roles.Query();

        // Apply filters
        if (filter.Names != null && filter.Names.Any())
        {
            query = query.Where(x => filter.Names.Contains(x.Name));
        }

        if (filter.ExcludeRoles != null && filter.ExcludeRoles.Any())
        {
            query = query.Where(x => !filter.ExcludeRoles.Contains(x.Name));
        }

        // Search
        var roleDocs = query.ToList();
        var roles = roleDocs.Select(x => x.ToRole()).ToList();

        return roles;
    }

    public Role? GetRoleDetails(string roleId, bool includeAgent = false)
    {
        if (string.IsNullOrWhiteSpace(roleId)) return null;

        var roleDoc = _dc.Roles.FindById(roleId);
        if (roleDoc == null) return null;

        var agentActions = new List<RoleAgentAction>();
        var role = roleDoc.ToRole();
        var roleAgentDocs = _dc.RoleAgents.Find(x => x.RoleId == roleId).ToList();

        if (!includeAgent)
        {
            agentActions = roleAgentDocs.Select(x => new RoleAgentAction
            {
                Id = x.Id,
                AgentId = x.AgentId,
                Actions = x.Actions
            }).ToList();
            role.AgentActions = agentActions;
            return role;
        }

        var agentIds = roleAgentDocs.Select(x => x.AgentId).Distinct().ToList();
        if (agentIds.Any())
        {
            var agents = GetAgents(new AgentFilter { AgentIds = agentIds });

            foreach (var item in roleAgentDocs)
            {
                var found = agents.FirstOrDefault(x => x.Id == item.AgentId);
                if (found == null) continue;

                agentActions.Add(new RoleAgentAction
                {
                    Id = item.Id,
                    AgentId = found.Id,
                    Agent = found,
                    Actions = item.Actions
                });
            }
        }

        role.AgentActions = agentActions;
        return role;
    }

    public bool UpdateRole(Role role, bool updateRoleAgents = false)
    {
        if (string.IsNullOrEmpty(role?.Id)) return false;

        var roleDoc = new RoleDocument
        {
            Id = role.Id,
            Name = role.Name,
            Permissions = role.Permissions,
            CreatedTime = DateTime.UtcNow,
            UpdatedTime = DateTime.UtcNow
        };

        _dc.Roles.Upsert(roleDoc);

        if (updateRoleAgents)
        {
            var roleAgentDocs = role.AgentActions?.Select(x => new RoleAgentDocument
            {
                Id = !string.IsNullOrEmpty(x.Id) ? x.Id : Guid.NewGuid().ToString(),
                RoleId = role.Id,
                AgentId = x.AgentId,
                Actions = x.Actions,
                CreatedTime = DateTime.UtcNow,
                UpdatedTime = DateTime.UtcNow
            }).ToList() ?? new List<RoleAgentDocument>();

            var toDelete = _dc.RoleAgents.Find(x => x.RoleId == role.Id && !roleAgentDocs.Select(y => y.Id).Contains(x.Id)).ToList();
            _dc.RoleAgents.DeleteMany(x => toDelete.Select(y => y.Id).Contains(x.Id));

            foreach (var doc in roleAgentDocs)
            {
                _dc.RoleAgents.Upsert(doc);
            }
        }

        return true;
    }
}
