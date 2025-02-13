using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Abstraction.Tasks.Models;

namespace BotSharp.Plugin.LiteDBStorage.Repository;

public partial class LiteDBRepository
{
    #region Task
    public PagedItems<AgentTask> GetAgentTasks(AgentTaskFilter filter)
    {
        var pager = filter.Pager ?? new Pagination();
        var query = _dc.AgentTasks.Query();

        if (!string.IsNullOrEmpty(filter.AgentId))
        {
            query = query.Where(x => x.AgentId == filter.AgentId);
        }

        if (filter.Enabled.HasValue)
        {
            query = query.Where(x => x.Enabled == filter.Enabled.Value);
        }

        var totalTasks = query.Count();
        var taskDocs = query.OrderByDescending(x => x.CreatedTime).Skip(pager.Offset).Limit(pager.Size).ToList();

        var agentIds = taskDocs.Select(x => x.AgentId).Distinct().ToList();
        var agents = GetAgents(new AgentFilter { AgentIds = agentIds });

        var tasks = taskDocs.Select(x =>
        {
            var task = AgentTaskDocument.ToDomainModel(x);
            task.Agent = agents.FirstOrDefault(a => a.Id == x.AgentId);
            return task;
        }).ToList();

        return new PagedItems<AgentTask>
        {
            Items = tasks,
            Count = (int)totalTasks
        };
    }

    public AgentTask? GetAgentTask(string agentId, string taskId)
    {
        if (string.IsNullOrEmpty(taskId)) return null;

        var taskDoc = _dc.AgentTasks.Query().Where(x => x.Id == taskId).FirstOrDefault();
        if (taskDoc == null) return null;

        var agentDoc = _dc.Agents.Query().Where(x => x.Id == taskDoc.AgentId).FirstOrDefault();
        var agent = TransformAgentDocument(agentDoc);

        var task = AgentTaskDocument.ToDomainModel(taskDoc);
        task.Agent = agent;

        return task;
    }

    public void InsertAgentTask(AgentTask task)
    {
        var taskDoc = AgentTaskDocument.ToLiteDBModel(task);
        taskDoc.Id = Guid.NewGuid().ToString();

        _dc.AgentTasks.Insert(taskDoc);
    }

    public void BulkInsertAgentTasks(List<AgentTask> tasks)
    {
        if (tasks.IsNullOrEmpty()) return;

        var taskDocs = tasks.Select(x =>
        {
            var task = AgentTaskDocument.ToLiteDBModel(x);
            task.Id = !string.IsNullOrEmpty(x.Id) ? x.Id : Guid.NewGuid().ToString();
            return task;
        }).ToList();

        _dc.AgentTasks.InsertBulk(taskDocs);
    }

    public void UpdateAgentTask(AgentTask task, AgentTaskField field)
    {
        if (task == null || string.IsNullOrEmpty(task.Id)) return;

        var taskDoc = _dc.AgentTasks.Find(x => x.Id == task.Id).FirstOrDefault();
        if (taskDoc == null) return;

        switch (field)
        {
            case AgentTaskField.Name:
                taskDoc.Name = task.Name;
                break;
            case AgentTaskField.Description:
                taskDoc.Description = task.Description;
                break;
            case AgentTaskField.Enabled:
                taskDoc.Enabled = task.Enabled;
                break;
            case AgentTaskField.Content:
                taskDoc.Content = task.Content;
                break;
            case AgentTaskField.Status:
                taskDoc.Status = task.Status;
                break;
            case AgentTaskField.All:
                taskDoc.Name = task.Name;
                taskDoc.Description = task.Description;
                taskDoc.Enabled = task.Enabled;
                taskDoc.Content = task.Content;
                taskDoc.Status = task.Status;
                break;
        }

        taskDoc.UpdatedTime = DateTime.UtcNow;
        _dc.AgentTasks.Update(taskDoc);
    }

    public bool DeleteAgentTask(string agentId, List<string> taskIds)
    {
        if (taskIds.IsNullOrEmpty()) return false;

        var taskDeleted = _dc.AgentTasks.DeleteMany(x=>taskIds.Contains(x.Id));
        return taskDeleted > 0;
    }

    public bool DeleteAgentTasks()
    {
        try
        {
            _dc.AgentTasks.DeleteAll();
            return true;
        }
        catch
        {
            return false;
        }
    }
    #endregion
}
