using BotSharp.Abstraction.Conversations.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class DialogLiteDBElement
{
    public DialogMetaDataMongoElement MetaData { get; set; }
    public string Content { get; set; }
    public string? SecondaryContent { get; set; }
    public string? RichContent { get; set; }
    public string? SecondaryRichContent { get; set; }
    public string? Payload { get; set; }

    public DialogLiteDBElement()
    {

    }

    public static DialogLiteDBElement ToLiteDBElement(DialogElement dialog)
    {
        return new DialogLiteDBElement
        {
            MetaData = DialogMetaDataMongoElement.ToLiteDBElement(dialog.MetaData),
            Content = dialog.Content,
            SecondaryContent = dialog.SecondaryContent,
            RichContent = dialog.RichContent,
            SecondaryRichContent = dialog.SecondaryRichContent,
            Payload = dialog.Payload
        };
    }

    public static DialogElement ToDomainElement(DialogLiteDBElement dialog)
    {
        return new DialogElement
        {
            MetaData = DialogMetaDataMongoElement.ToDomainElement(dialog.MetaData),
            Content = dialog.Content,
            SecondaryContent = dialog.SecondaryContent,
            RichContent = dialog.RichContent,
            SecondaryRichContent = dialog.SecondaryRichContent,
            Payload = dialog.Payload
        };
    }
}

public class DialogMetaDataMongoElement
{
    public string Role { get; set; }
    public string AgentId { get; set; }
    public string MessageId { get; set; }
    public string MessageType { get; set; }
    public string? FunctionName { get; set; }
    public string? SenderId { get; set; }
    public DateTime CreateTime { get; set; }

    public DialogMetaDataMongoElement()
    {

    }

    public static DialogMetaData ToDomainElement(DialogMetaDataMongoElement meta)
    {
        return new DialogMetaData
        {
            Role = meta.Role,
            AgentId = meta.AgentId,
            MessageId = meta.MessageId,
            MessageType = meta.MessageType,
            FunctionName = meta.FunctionName,
            SenderId = meta.SenderId,
            CreatedTime = meta.CreateTime,
        };
    }

    public static DialogMetaDataMongoElement ToLiteDBElement(DialogMetaData meta)
    {
        return new DialogMetaDataMongoElement
        { 
            Role = meta.Role,
            AgentId = meta.AgentId,
            MessageId = meta.MessageId,
            MessageType = meta.MessageType,
            FunctionName = meta.FunctionName,
            SenderId = meta.SenderId,
            CreateTime = meta.CreatedTime,
        };
    }
}
