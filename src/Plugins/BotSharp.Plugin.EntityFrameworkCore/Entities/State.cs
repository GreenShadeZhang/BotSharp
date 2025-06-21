namespace BotSharp.Plugin.EntityFrameworkCore.Entities;

public class State
{
    public string Key { get; set; }
    public bool Versioning { get; set; }
    public bool Readonly { get; set; }
    public List<StateValue> Values { get; set; }
}
