using BotSharp.Abstraction.Agents.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class AgentKnowledgeBaseLiteDBElement
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool Disabled { get; set; }

    public static AgentKnowledgeBaseLiteDBElement ToLiteDBElement(AgentKnowledgeBase knowledgeBase)
    {
        return new AgentKnowledgeBaseLiteDBElement
        {
            Name = knowledgeBase.Name,
            Type = knowledgeBase.Type,
            Disabled = knowledgeBase.Disabled
        };
    }

    public static AgentKnowledgeBase ToDomainElement(AgentKnowledgeBaseLiteDBElement knowledgeBase)
    {
        return new AgentKnowledgeBase
        {
            Name = knowledgeBase.Name,
            Type = knowledgeBase.Type,
            Disabled = knowledgeBase.Disabled
        };
    }
}
