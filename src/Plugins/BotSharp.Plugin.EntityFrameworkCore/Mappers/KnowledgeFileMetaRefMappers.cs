using BotSharp.Abstraction.Knowledges.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class KnowledgeFileMetaRefMappers
{
    public static KnowledgeFileMetaRefElement? ToEntity(this DocMetaRefData? model)
    {
        if (model == null) return null;

        return new KnowledgeFileMetaRefElement
        {
            Id = model.Id,
            Name = model.Name,
            Type = model.Type,
            Url = model.Url,
            Data = model.Data
        };
    }

    public static DocMetaRefData? ToModel(this KnowledgeFileMetaRefElement? model)
    {
        if (model == null) return null;

        return new DocMetaRefData
        {
            Id = model.Id,
            Name = model.Name,
            Type = model.Type,
            Url = model.Url,
            Data = model.Data
        };
    }
}
