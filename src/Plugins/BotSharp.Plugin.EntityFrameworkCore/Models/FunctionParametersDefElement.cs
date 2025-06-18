using System.Collections.Generic;

namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class FunctionParametersDefElement
{
    public string Type { get; set; }
    public string Properties { get; set; }
    public List<string> Required { get; set; } = new List<string>();
}
