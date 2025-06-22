using BotSharp.Abstraction.Roles.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class RoleMappers
{
    public static Role ToModel(this Entities.Role entity)
    {
        return new Role
        {
            Id = entity.Id,
            Name = entity.Name,
            Permissions = entity.Permissions ?? new List<string>(),
            CreatedTime = entity.CreatedTime,
            UpdatedTime = entity.UpdatedTime
        };
    }

    public static Entities.Role ToEntity(this Role model)
    {
        return new Entities.Role
        {
            Id = model.Id,
            Name = model.Name,
            Permissions = model.Permissions.ToList() ?? new List<string>(),
            CreatedTime = model.CreatedTime,
            UpdatedTime = model.UpdatedTime
        };
    }
}
