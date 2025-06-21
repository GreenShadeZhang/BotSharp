namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class StateElement
{
    public string Key { get; set; }
    public bool Versioning { get; set; }
    public bool Readonly { get; set; }
    public List<StateValueElement> Values { get; set; }
}
