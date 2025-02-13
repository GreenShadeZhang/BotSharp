using BotSharp.Abstraction.Roles.Models;

namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class RoleDocument : LiteDBBase
{
    public string Name { get; set; }
    public IEnumerable<string> Permissions { get; set; } = [];
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
    

    public Role ToRole()
    {
        return new Role
        {
            Id = Id,
            Name = Name,
            Permissions = Permissions,
            CreatedTime = CreatedTime,
            UpdatedTime = UpdatedTime
        };
    }
}
