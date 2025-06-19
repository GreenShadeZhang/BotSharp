using BotSharp.Abstraction.Conversations.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class DialogMappers
{
    public static DialogMetaData ToModel(this DialogMetaDataElement meta)
    {
        return new DialogMetaData
        {
            Role = meta.Role,
            AgentId = meta.AgentId,
            MessageId = meta.MessageId,
            FunctionName = meta.FunctionName,
            SenderId = meta.SenderId,
            CreatedTime = meta.CreateTime,
        };
    }

    public static DialogMetaDataElement ToEntity(this DialogMetaData meta)
    {
        return new DialogMetaDataElement
        {
            Role = meta.Role,
            AgentId = meta.AgentId,
            MessageId = meta.MessageId,
            FunctionName = meta.FunctionName,
            SenderId = meta.SenderId,
            CreateTime = meta.CreatedTime,
        };
    }
}
