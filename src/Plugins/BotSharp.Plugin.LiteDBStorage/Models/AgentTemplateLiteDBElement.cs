using BotSharp.Abstraction.Agents.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class AgentTemplateLiteDBElement
{
    public string Name { get; set; }
    public string Content { get; set; }

    public static AgentTemplateLiteDBElement ToLiteDBElement(AgentTemplate template)
    {
        return new AgentTemplateLiteDBElement
        {
            Name = template.Name,
            Content = template.Content
        };
    }

    public static AgentTemplate ToDomainElement(AgentTemplateLiteDBElement mongoTemplate)
    {
        return new AgentTemplate
        {
            Name = mongoTemplate.Name,
            Content = mongoTemplate.Content
        };
    }
}
