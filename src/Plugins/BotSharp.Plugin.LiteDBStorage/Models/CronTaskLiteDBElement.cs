using BotSharp.Abstraction.Crontab.Models;

namespace BotSharp.Plugin.LiteDBStorage.Models;

public class CronTaskLiteDBElement
{
    public string Topic { get; set; }
    public string Script { get; set; }
    public string Language { get; set; }

    public static CronTaskLiteDBElement ToLiteDBElement(ScheduleTaskItemArgs model)
    {
        return new CronTaskLiteDBElement
        {
            Topic = model.Topic,
            Script = model.Script,
            Language = model.Language
        };
    }

    public static ScheduleTaskItemArgs ToDomainElement(CronTaskLiteDBElement model)
    {
        return new ScheduleTaskItemArgs
        {
            Topic = model.Topic,
            Script = model.Script,
            Language = model.Language
        };
    }
}
