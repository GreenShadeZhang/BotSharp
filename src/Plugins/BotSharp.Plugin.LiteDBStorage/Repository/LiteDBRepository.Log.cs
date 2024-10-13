using BotSharp.Abstraction.Loggers.Models;

namespace BotSharp.Plugin.LiteDBStorage.Repository;

public partial class LiteDBRepository
{
    #region Execution Log
    public void AddExecutionLogs(string conversationId, List<string> logs)
    {
        if (string.IsNullOrEmpty(conversationId) || logs.IsNullOrEmpty()) return;

        var excutionLog = _dc.ExectionLogs.Query().Where(x => x.ConversationId == conversationId).FirstOrDefault();

        if (excutionLog == null)
        {
            excutionLog = new ExecutionLogDocument
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = conversationId,
                Logs = logs
            };

            _dc.ExectionLogs.Insert(excutionLog);
        }
        else
        {
            excutionLog.Logs = logs;
            _dc.ExectionLogs.Update(excutionLog);
        }
    }

    public List<string> GetExecutionLogs(string conversationId)
    {
        var logs = new List<string>();
        if (string.IsNullOrEmpty(conversationId)) return logs;

        var logCollection = _dc.ExectionLogs.Query().Where(x => x.ConversationId==conversationId).FirstOrDefault();

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

        var logElement = new PromptLogLiteDBElement
        {
            MessageId = messageId,
            AgentId = log.AgentId,
            Prompt = log.Prompt,
            Response = log.Response,
            CreateDateTime = log.CreateDateTime
        };

        var llmCompletionLog = _dc.LlmCompletionLogs.Query().Where(x => x.ConversationId == conversationId).FirstOrDefault();

        if (llmCompletionLog == null)
        {
            llmCompletionLog = new LlmCompletionLogDocument
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = conversationId,
                Logs = new List<PromptLogLiteDBElement> { logElement }
            };

            _dc.LlmCompletionLogs.Insert(llmCompletionLog);
        }
        else
        {
            var found = llmCompletionLog.Logs.FirstOrDefault(x => x.MessageId == messageId);
            if (found != null) return;

            llmCompletionLog.Logs.Add(logElement);
            _dc.LlmCompletionLogs.Update(llmCompletionLog);
        }
    }

    #endregion

    #region Conversation Content Log
    public void SaveConversationContentLog(ContentLogOutputModel log)
    {
        if (log == null) return;

        var found = _dc.Conversations.Query().Where(x => x.Id == log.ConversationId).FirstOrDefault();
        if (found == null) return;

        var logDoc = new ConversationContentLogDocument
        {
            ConversationId = log.ConversationId,
            MessageId = log.MessageId,
            Name = log.Name,
            AgentId = log.AgentId,
            Role = log.Role,
            Source = log.Source,
            Content = log.Content,
            CreateTime = log.CreateTime
        };

        _dc.ContentLogs.Insert(logDoc);
    }

    public List<ContentLogOutputModel> GetConversationContentLogs(string conversationId)
    {
        var logs = _dc.ContentLogs
                      .Query()
                      .Where(x => x.ConversationId == conversationId)
                      .OrderBy(x => x.CreateTime)
                      .Select(x => new ContentLogOutputModel
                      {
                          ConversationId = x.ConversationId,
                          MessageId = x.MessageId,
                          Name = x.Name,
                          AgentId = x.AgentId,
                          Role = x.Role,
                          Source = x.Source,
                          Content = x.Content,
                          CreateTime = x.CreateTime
                      })   
                      .ToList();
        return logs;
    }
    #endregion

    #region Conversation State Log
    public void SaveConversationStateLog(ConversationStateLogModel log)
    {
        if (log == null) return;

        var found = _dc.Conversations.Query().Where(x => x.Id == log.ConversationId).FirstOrDefault();
        if (found == null) return;

        var logDoc = new ConversationStateLogDocument
        {
            ConversationId = log.ConversationId,
            MessageId = log.MessageId,
            States = log.States,
            CreateTime = log.CreateTime
        };

        _dc.StateLogs.Insert(logDoc);
    }

    public List<ConversationStateLogModel> GetConversationStateLogs(string conversationId)
    {
        var logs = _dc.StateLogs
                      .Query()
                      .Where(x => x.ConversationId == conversationId)
                      .OrderBy(x => x.CreateTime)
                      .Select(x => new ConversationStateLogModel
                      {
                          ConversationId = x.ConversationId,
                          MessageId = x.MessageId,
                          States = x.States,
                          CreateTime = x.CreateTime
                      })
                      .ToList();
        return logs;
    }
    #endregion
}
