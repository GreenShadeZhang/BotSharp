using BotSharp.Abstraction.Agents.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class AgentUtilityLiteDBElement
{
    public string Name { get; set; }
    public bool Disabled { get; set; }
    public List<UtilityFunctionLiteDBElement> Functions { get; set; } = [];
    public List<UtilityTemplateLiteDBElement> Templates { get; set; } = [];

    public static AgentUtilityLiteDBElement ToLiteDBElement(AgentUtility utility)
    {
        return new AgentUtilityLiteDBElement
        {
            Name = utility.Name,
            Disabled = utility.Disabled,
            Functions = utility.Functions?.Select(x => new UtilityFunctionLiteDBElement(x.Name))?.ToList() ?? [],
            Templates = utility.Templates?.Select(x => new UtilityTemplateLiteDBElement(x.Name))?.ToList() ?? []
        };
    }

    public static AgentUtility ToDomainElement(AgentUtilityLiteDBElement utility)
    {
        return new AgentUtility
        {
            Name = utility.Name,
            Disabled = utility.Disabled,
            Functions = utility.Functions?.Select(x => new UtilityFunction(x.Name))?.ToList() ?? [],
            Templates = utility.Templates?.Select(x => new UtilityTemplate(x.Name))?.ToList() ?? []
        };
    }
}

public class UtilityFunctionLiteDBElement
{
    public string Name { get; set; }

    public UtilityFunctionLiteDBElement()
    {

    }

    public UtilityFunctionLiteDBElement(string name)
    {
        Name = name;
    }
}

public class UtilityTemplateLiteDBElement
{
    public string Name { get; set; }

    public UtilityTemplateLiteDBElement()
    {

    }

    public UtilityTemplateLiteDBElement(string name)
    {
        Name = name;
    }
}