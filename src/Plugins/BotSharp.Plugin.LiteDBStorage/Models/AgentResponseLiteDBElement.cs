using BotSharp.Abstraction.Agents.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class AgentResponseLiteDBElement
{
    public string Prefix { get; set; }
    public string Intent { get; set; }
    public string Content { get; set; }

    public static AgentResponseLiteDBElement ToMongoElement(AgentResponse response)
    {
        return new AgentResponseLiteDBElement
        {
            Prefix = response.Prefix,
            Intent = response.Intent,
            Content = response.Content
        };
    }

    public static AgentResponse ToDomainElement(AgentResponseLiteDBElement mongoResponse)
    {
        return new AgentResponse
        {
            Prefix = mongoResponse.Prefix,
            Intent = mongoResponse.Intent,
            Content = mongoResponse.Content
        };
    }
}
