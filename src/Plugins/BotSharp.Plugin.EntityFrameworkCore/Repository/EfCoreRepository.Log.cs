using BotSharp.Abstraction.Loggers.Models;
using BotSharp.Abstraction.Repositories.Filters;

namespace BotSharp.Plugin.EntityFrameworkCore.Repository;

public partial class EfCoreRepository
{
    #region Execution Log
    public void AddExecutionLogs(string conversationId, List<string> logs)
    {
        if (string.IsNullOrEmpty(conversationId) || logs.IsNullOrEmpty()) return;

        var executionLog = _context.ExecutionLogs.FirstOrDefault(x => x.ConversationId == conversationId);

        if (executionLog == null)
        {
            executionLog = new Entities.ExecutionLog
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = conversationId,
                Logs = logs
            };

            _context.ExecutionLogs.Add(executionLog);
        }
        else
        {
            executionLog.Logs.AddRange(logs);
        }

        _context.SaveChanges();
    }

    public List<string> GetExecutionLogs(string conversationId)
    {
        var logs = new List<string>();
        if (string.IsNullOrEmpty(conversationId)) return logs;

        var logCollection = _context.ExecutionLogs.FirstOrDefault(x => x.ConversationId == conversationId);

        logs = logCollection?.Logs ?? new List<string>();
        return logs;
    }
    #endregion

    #region LLM Completion Log
    public void SaveLlmCompletionLog(LlmCompletionLog log)
    {
        if (log == null) return;

        var conversationId = log.ConversationId.IfNullOrEmptyAs(Guid.NewGuid().ToString());
        var messageId = log.MessageId.IfNullOrEmptyAs(Guid.NewGuid().ToString());

        var data = new Entities.LlmCompletionLog
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = conversationId,
            MessageId = messageId,
            AgentId = log.AgentId,
            Prompt = log.Prompt,
            Response = log.Response,
            CreatedTime = log.CreatedTime
        };

        _context.LlmCompletionLogs.Add(data);
        _context.SaveChanges();
    }

    #endregion

    #region Conversation Content Log
    public void SaveConversationContentLog(ContentLogOutputModel log)
    {
        if (log == null) return;

        var found = _context.Conversations.FirstOrDefault(x => x.Id == log.ConversationId);
        if (found == null) return;

        var logDoc = new Entities.ConversationContentLog
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = log.ConversationId,
            MessageId = log.MessageId,
            Name = log.Name,
            AgentId = log.AgentId,
            Role = log.Role,
            Source = log.Source,
            Content = log.Content,
            CreatedTime = log.CreatedTime
        };

        _context.ConversationContentLogs.Add(logDoc);
        _context.SaveChanges();
    }

    public DateTimePagination<ContentLogOutputModel> GetConversationContentLogs(string conversationId, ConversationLogFilter filter)
    {
        var query = _context.ConversationContentLogs
            .Where(x => x.ConversationId == conversationId);

        if (filter != null)
        {
            query = query.Where(x => x.CreatedTime >= filter.StartTime);
        }

        var totalCount = query.Count();
        var logs = query.OrderBy(x => x.CreatedTime)
                       .Skip(0)
                       .Take(filter?.Size ?? 10)
                       .Select(x => new ContentLogOutputModel
                       {
                           ConversationId = x.ConversationId,
                           MessageId = x.MessageId,
                           Name = x.Name,
                           AgentId = x.AgentId,
                           Role = x.Role,
                           Source = x.Source,
                           Content = x.Content,
                           CreatedTime = x.CreatedTime
                       })
                       .ToList();
        logs.Reverse();
        return new DateTimePagination<ContentLogOutputModel>
        {
            Items = logs,
            Count = totalCount,
            NextTime = logs.FirstOrDefault()?.CreatedTime
        };
    }
    #endregion

    #region Conversation State Log
    public void SaveConversationStateLog(ConversationStateLogModel log)
    {
        if (log == null) return;

        var found = _context.Conversations.FirstOrDefault(x => x.Id == log.ConversationId);
        if (found == null) return;

        var logDoc = new Entities.ConversationStateLog
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = log.ConversationId,
            MessageId = log.MessageId,
            States = log.States ?? new Dictionary<string, string>(),
            CreatedTime = log.CreatedTime
        };

        _context.ConversationStateLogs.Add(logDoc);
        _context.SaveChanges();
    }

    public DateTimePagination<ConversationStateLogModel> GetConversationStateLogs(string conversationId, ConversationLogFilter filter)
    {
        var query = _context.ConversationStateLogs
            .Where(x => x.ConversationId == conversationId);

        if (filter != null)
        {
            query = query.Where(x => x.CreatedTime >= filter.StartTime);
        }

        var totalCount = query.Count();
        var logs = query.OrderBy(x => x.CreatedTime)
                       .Skip(0)
                       .Take(filter?.Size ?? 10)
                       .Select(x => new ConversationStateLogModel
                       {
                           ConversationId = x.ConversationId,
                           MessageId = x.MessageId,
                           States = x.States,
                           CreatedTime = x.CreatedTime
                       })
                       .ToList();
        logs.Reverse();
        return new DateTimePagination<ConversationStateLogModel>
        {
            Items = logs,
            Count = totalCount,
            NextTime = logs.FirstOrDefault()?.CreatedTime
        };
    }
    #endregion

    #region Instruction Log
    public bool SaveInstructionLogs(IEnumerable<InstructionLogModel> logs)
    {
        if (logs?.Any() != true) return false;

        try
        {
            var logEntities = logs.Select(x => new Entities.InstructionLog
            {
                Id = Guid.NewGuid().ToString(),
                AgentId = x.AgentId,
                ConversationId = x.ConversationId,
                MessageId = x.MessageId,
                Instruction = x.Instruction,
                Response = x.Response,
                CreatedTime = x.CreateTime
            });

            _context.InstructionLogs.AddRange(logEntities);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving instruction logs");
            return false;
        }
    }

    public PagedItems<InstructionLogModel> GetInstructionLogs(InstructLogFilter filter)
    {
        var query = _context.InstructionLogs.AsQueryable();

        if (filter != null)
        {
            if (!string.IsNullOrWhiteSpace(filter.AgentId))
            {
                query = query.Where(x => x.AgentId == filter.AgentId);
            }

            if (!string.IsNullOrWhiteSpace(filter.ConversationId))
            {
                query = query.Where(x => x.ConversationId == filter.ConversationId);
            }

            if (!string.IsNullOrWhiteSpace(filter.MessageId))
            {
                query = query.Where(x => x.MessageId == filter.MessageId);
            }

            if (filter.StartTime.HasValue)
            {
                query = query.Where(x => x.CreatedTime >= filter.StartTime.Value);
            }

            if (filter.EndTime.HasValue)
            {
                query = query.Where(x => x.CreatedTime <= filter.EndTime.Value);
            }
        }

        var totalCount = query.Count();
        var logs = query.OrderBy(x => x.CreatedTime)
                       .Skip(filter?.Offset ?? 0)
                       .Take(filter?.Size ?? 10)
                       .Select(x => new InstructionLogModel
                       {
                           AgentId = x.AgentId,
                           ConversationId = x.ConversationId,
                           MessageId = x.MessageId,
                           Instruction = x.Instruction,
                           Response = x.Response,
                           CreateTime = x.CreatedTime
                       })
                       .ToList();

        return new PagedItems<InstructionLogModel>
        {
            Items = logs,
            Count = totalCount
        };
    }

    public List<string> GetInstructionLogSearchKeys(InstructLogKeysFilter filter)
    {
        var query = _context.InstructionLogs.AsQueryable();

        if (filter != null)
        {
            if (!string.IsNullOrWhiteSpace(filter.AgentId))
            {
                query = query.Where(x => x.AgentId == filter.AgentId);
            }

            if (!string.IsNullOrWhiteSpace(filter.ConversationId))
            {
                query = query.Where(x => x.ConversationId == filter.ConversationId);
            }

            if (filter.StartTime.HasValue)
            {
                query = query.Where(x => x.CreatedTime >= filter.StartTime.Value);
            }

            if (filter.EndTime.HasValue)
            {
                query = query.Where(x => x.CreatedTime <= filter.EndTime.Value);
            }
        }

        var keys = new List<string>();

        switch (filter?.KeyType?.ToLower())
        {
            case "agent":
                keys = query.Select(x => x.AgentId).Distinct().ToList();
                break;
            case "conversation":
                keys = query.Select(x => x.ConversationId).Distinct().ToList();
                break;
            case "message":
                keys = query.Select(x => x.MessageId).Distinct().ToList();
                break;
            default:
                break;
        }

        return keys;
    }
    #endregion
}
