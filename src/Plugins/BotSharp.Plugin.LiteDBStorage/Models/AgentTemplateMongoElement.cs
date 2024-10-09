using BotSharp.Abstraction.Agents.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class AgentTemplateMongoElement
{
    public string Name { get; set; }
    public string Content { get; set; }

    public static AgentTemplateMongoElement ToMongoElement(AgentTemplate template)
    {
        return new AgentTemplateMongoElement
        {
            Name = template.Name,
            Content = template.Content
        };
    }

    public static AgentTemplate ToDomainElement(AgentTemplateMongoElement mongoTemplate)
    {
        return new AgentTemplate
        {
            Name = mongoTemplate.Name,
            Content = mongoTemplate.Content
        };
    }
}
