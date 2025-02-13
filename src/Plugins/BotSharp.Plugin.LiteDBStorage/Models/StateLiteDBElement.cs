using BotSharp.Abstraction.Conversations.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class StateLiteDBElement
{
    public string Key { get; set; }
    public bool Versioning { get; set; }
    public bool Readonly { get; set; }
    public List<StateValueLiteDBElement> Values { get; set; }

    public static StateLiteDBElement ToLiteDBElement(StateKeyValue state)
    {
        return new StateLiteDBElement
        {
            Key = state.Key,
            Versioning = state.Versioning,
            Readonly = state.Readonly,
            Values = state.Values?.Select(x => StateValueLiteDBElement.ToLiteDBElement(x))?.ToList() ?? new List<StateValueLiteDBElement>()
        };
    }

    public static StateKeyValue ToDomainElement(StateLiteDBElement state)
    {
        return new StateKeyValue
        {
            Key = state.Key,
            Versioning = state.Versioning,
            Readonly = state.Readonly,
            Values = state.Values?.Select(x => StateValueLiteDBElement.ToDomainElement(x))?.ToList() ?? new List<StateValue>()
        };
    }
}

public class StateValueLiteDBElement
{
    public string Data { get; set; }
    public string? MessageId { get; set; }
    public bool Active { get; set; }
    public int ActiveRounds { get; set; }
    public string DataType { get; set; }
    public string Source { get; set; }

    public DateTime UpdateTime { get; set; }

    public static StateValueLiteDBElement ToLiteDBElement(StateValue element)
    {
        return new StateValueLiteDBElement
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

    public static StateValue ToDomainElement(StateValueLiteDBElement element)
    {
        return new StateValue
        {
            Data = element.Data,
            MessageId = element.MessageId,
            Active = element.Active,
            ActiveRounds = element.ActiveRounds,
            DataType= element.DataType,
            Source = element.Source,
            UpdateTime = element.UpdateTime
        };
    }
}