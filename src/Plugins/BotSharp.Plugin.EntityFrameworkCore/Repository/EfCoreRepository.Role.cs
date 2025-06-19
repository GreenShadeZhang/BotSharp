using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Abstraction.Roles.Models;
using BotSharp.Plugin.EntityFrameworkCore.Mappers;
using Microsoft.Extensions.Logging;

namespace BotSharp.Plugin.EntityFrameworkCore.Repository;

public partial class EfCoreRepository
{
    #region Role
    public bool RefreshRoles(IEnumerable<Role> roles)
    {
        if (roles?.Any() != true) return false;

        var validRoles = roles.Where(x => !string.IsNullOrWhiteSpace(x.Id) &&
                                        !string.IsNullOrWhiteSpace(x.Name)).ToList();
        if (!validRoles.Any()) return false;

        try
        {
            // Clear existing data
            _context.RoleAgents.RemoveRange(_context.RoleAgents);
            _context.Roles.RemoveRange(_context.Roles);

            var roleEntities = validRoles.Select(x => new Entities.Role
            {
                Id = x.Id,
                Name = x.Name,
                Permissions = x.Permissions.ToList() ?? new List<string>(),
                CreatedTime = DateTime.UtcNow,
                UpdatedTime = DateTime.UtcNow
            });

            _context.Roles.AddRange(roleEntities);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing roles");
            return false;
        }
    }

    public IEnumerable<Role> GetRoles(RoleFilter filter)
    {
        filter ??= RoleFilter.Empty();

        var query = _context.Roles.AsQueryable();

        if (filter.Names?.Any() == true)
        {
            query = query.Where(x => filter.Names.Contains(x.Name));
        }

        var roles = query.ToList();
        return roles.Select(x => x.ToModel()).ToList();
    }

    public Role? GetRoleDetails(string roleId, bool includeAgent = false)
    {
        if (string.IsNullOrWhiteSpace(roleId)) return null;

        var role = _context.Roles.FirstOrDefault(x => x.Id == roleId);
        if (role == null) return null;

        var result = role.ToModel();

        if (includeAgent)
        {
            var roleAgents = _context.RoleAgents
                .Where(x => x.RoleId == roleId)
                .ToList();

            result.AgentActions = roleAgents.Select(x => new RoleAgentAction
            {
                Id = x.Id,
                AgentId = x.AgentId,
            }).ToList();
        }

        return result;
    }

    public bool UpdateRole(Role role, bool updateRoleAgents = false)
    {
        if (role == null || string.IsNullOrWhiteSpace(role.Id)) return false;

        try
        {
            var existingRole = _context.Roles.FirstOrDefault(x => x.Id == role.Id);
            if (existingRole == null) return false;

            existingRole.Name = role.Name;
            existingRole.Permissions = role.Permissions.ToList() ?? new List<string>();
            existingRole.UpdatedTime = DateTime.UtcNow;

            if (updateRoleAgents && role.AgentActions?.Any() == true)
            {
                // Remove existing role agents
                var existingRoleAgents = _context.RoleAgents.Where(x => x.RoleId == role.Id);
                _context.RoleAgents.RemoveRange(existingRoleAgents);

                // Add new role agents
                var newRoleAgents = role.AgentActions.Select(x => new Entities.RoleAgent
                {
                    Id = !string.IsNullOrEmpty(x.Id) ? x.Id : Guid.NewGuid().ToString(),
                    RoleId = role.Id,
                    AgentId = x.AgentId,
                    CreatedTime = DateTime.UtcNow,
                    UpdatedTime = DateTime.UtcNow
                });

                _context.RoleAgents.AddRange(newRoleAgents);
            }

            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", role.Id);
            return false;
        }
    }
    #endregion
}
