using BotSharp.Abstraction.Roles.Models;

namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class RoleAgentDocument : LiteDBBase
{
    public string RoleId { get; set; }
    public string AgentId { get; set; }
    public IEnumerable<string> Actions { get; set; } = [];
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }

    public RoleAgent ToRoleAgent()
    {
        return new RoleAgent
        {
            Id = Id,
            RoleId = RoleId,
            AgentId = AgentId,
            Actions = Actions,
            CreatedTime = CreatedTime,
            UpdatedTime = UpdatedTime
        };
    }
}
