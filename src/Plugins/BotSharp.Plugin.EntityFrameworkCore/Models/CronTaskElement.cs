using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotSharp.Plugin.EntityFrameworkCore.Models;

public class CronTaskElement
{
    public string Topic { get; set; } = default!;
    public string Script { get; set; } = default!;
    public string Language { get; set; } = default!;
}
