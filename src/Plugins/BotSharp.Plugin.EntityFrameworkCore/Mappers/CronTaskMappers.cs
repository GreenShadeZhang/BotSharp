using BotSharp.Abstraction.Crontab.Models;

namespace BotSharp.Plugin.EntityFrameworkCore.Mappers;

public static class CronTaskMappers
{
    public static CronTaskElement ToEntity(this ScheduleTaskItemArgs model)
    {
        return new CronTaskElement
        {
            Topic = model.Topic,
            Script = model.Script,
            Language = model.Language
        };
    }

    public static ScheduleTaskItemArgs ToModel(this CronTaskElement model)
    {
        return new ScheduleTaskItemArgs
        {
            Topic = model.Topic,
            Script = model.Script,
            Language = model.Language
        };
    }
}
