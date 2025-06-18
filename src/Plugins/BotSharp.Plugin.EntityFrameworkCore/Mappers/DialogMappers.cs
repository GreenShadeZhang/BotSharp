using BotSharp.Abstraction.Conversations.Models;
using BotSharp.Plugin.EntityFrameworkCore.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class DialogMappers
{
    public static Models.DialogEfElement ToEntity(this DialogElement dialog)
    {
        return new Models.DialogEfElement
        {
            MetaData = dialog.MetaData.ToEntity(),
            Content = dialog.Content,
            SecondaryContent = dialog.SecondaryContent,
            RichContent = dialog.RichContent,
            SecondaryRichContent = dialog.SecondaryRichContent,
            Payload = dialog.Payload
        };
    }

    public static DialogElement ToModel(this Models.DialogEfElement dialog)
    {
        return new DialogElement
        {
            MetaData = dialog.MetaData.ToModel(),
            Content = dialog.Content,
            SecondaryContent = dialog.SecondaryContent,
            RichContent = dialog.RichContent,
            SecondaryRichContent = dialog.SecondaryRichContent,
            Payload = dialog.Payload
        };
    }

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
