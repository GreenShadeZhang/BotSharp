using BotSharp.Abstraction.Conversations.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class StateMappers
{
    public static StateElement ToEntity(this StateKeyValue state)
    {
        return new StateElement
        {
            Key = state.Key,
            Versioning = state.Versioning,
            Readonly = state.Readonly,
            Values = state.Values?.Select(x => x.ToEntity())?.ToList() ?? new List<StateValueElement>()
        };
    }

    public static StateElement ToEntity(this StateKeyValue state, Entities.ConversationState conversationState)
    {
        var stateId = Guid.NewGuid().ToString();
        return new StateElement
        {
            Key = state.Key,
            Versioning = state.Versioning,
            Readonly = state.Readonly,
            Values = state.Values?.Select(x => x.ToEntity(stateId))?.ToList() ?? new List<StateValueElement>()
        };
    }

    public static StateKeyValue ToModel(this StateElement state)
    {
        return new StateKeyValue
        {
            Key = state.Key,
            Versioning = state.Versioning,
            Readonly = state.Readonly,
            Values = state.Values?.Select(x => x.ToModel())?.ToList() ?? new List<StateValue>()
        };
    }

    public static StateValueElement ToEntity(this StateValue element)
    {
        return new StateValueElement
        {
            Data = element.Data,
            MessageId = element.MessageId,
            Active = element.Active,
            ActiveRounds = element.ActiveRounds,
            DataType = element.DataType,
            Source = element.Source,
            UpdateTime = element.UpdateTime
        };
    }


    public static StateValueElement ToEntity(this StateValue element, string stateId)
    {
        return new StateValueElement
        {
            Data = element.Data,
            MessageId = element.MessageId,
            Active = element.Active,
            ActiveRounds = element.ActiveRounds,
            DataType = element.DataType,
            Source = element.Source,
            UpdateTime = element.UpdateTime
        };
    }

    public static StateValue ToModel(this StateValueElement element)
    {
        return new StateValue
        {
            Data = element.Data,
            MessageId = element.MessageId,
            Active = element.Active,
            ActiveRounds = element.ActiveRounds,
            DataType = element.DataType,
            Source = element.Source,
            UpdateTime = element.UpdateTime
        };
    }
}
