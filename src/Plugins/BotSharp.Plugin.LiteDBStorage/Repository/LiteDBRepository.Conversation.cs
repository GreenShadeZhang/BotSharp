using BotSharp.Abstraction.Conversations.Models;
using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Abstraction.SideCar.Attributes;

namespace BotSharp.Plugin.LiteDBStorage.Repository;

public partial class LiteDBRepository
{
    public void CreateNewConversation(Conversation conversation)
    {
        if (conversation == null) return;

        var utcNow = DateTime.UtcNow;
        var convDoc = new ConversationDocument
        {
            Id = !string.IsNullOrEmpty(conversation.Id) ? conversation.Id : Guid.NewGuid().ToString(),
            AgentId = conversation.AgentId,
            UserId = !string.IsNullOrEmpty(conversation.UserId) ? conversation.UserId : string.Empty,
            Title = conversation.Title,
            Channel = conversation.Channel,
            ChannelId = conversation.ChannelId,
            TaskId = conversation.TaskId,
            Status = conversation.Status,
            Tags = conversation.Tags ?? new(),
            CreatedTime = utcNow,
            UpdatedTime = utcNow
        };

        var dialogDoc = new ConversationDialogDocument
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = convDoc.Id,
            AgentId = conversation.AgentId,
            Dialogs = [],
            UpdatedTime = utcNow
        };

        var stateDoc = new ConversationStateDocument
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = convDoc.Id,
            AgentId = conversation.AgentId,
            States = [],
            Breakpoints = [],
            UpdatedTime = utcNow
        };

        _dc.Conversations.Insert(convDoc);
        _dc.ConversationDialogs.Insert(dialogDoc);
        _dc.ConversationStates.Insert(stateDoc);
    }

    public bool DeleteConversations(IEnumerable<string> conversationIds)
    {
        if (conversationIds.IsNullOrEmpty()) return false;

        var exeLogDeleted = _dc.ExectionLogs.DeleteMany(x => conversationIds.Contains(x.ConversationId));
        var promptLogDeleted = _dc.LlmCompletionLogs.DeleteMany(x => conversationIds.Contains(x.ConversationId));
        var contentLogDeleted = _dc.ContentLogs.DeleteMany(x => conversationIds.Contains(x.ConversationId));
        var stateLogDeleted = _dc.StateLogs.DeleteMany(x => conversationIds.Contains(x.ConversationId));
        var statesDeleted = _dc.ConversationStates.DeleteMany(x => conversationIds.Contains(x.ConversationId));
        var dialogDeleted = _dc.ConversationDialogs.DeleteMany(x => conversationIds.Contains(x.ConversationId));
		var cronDeleted = _dc.CrontabItems.DeleteMany(x => conversationIds.Contains(x.ConversationId));
        var convDeleted = _dc.Conversations.DeleteMany(x => conversationIds.Contains(x.Id));

        return convDeleted > 0 || dialogDeleted > 0 || statesDeleted > 0
            || exeLogDeleted > 0 || promptLogDeleted > 0
            || contentLogDeleted > 0 || stateLogDeleted > 0 || convDeleted > 0;
    }

    public List<DialogElement> GetConversationDialogs(string conversationId)
    {
        var dialogs = new List<DialogElement>();
        if (string.IsNullOrEmpty(conversationId)) return dialogs;

        var foundDialog = _dc.ConversationDialogs.FindOne(x => x.ConversationId == conversationId);
        if (foundDialog == null) return dialogs;

        var formattedDialog = foundDialog.Dialogs?.Select(x => DialogLiteDBElement.ToDomainElement(x))?.ToList();
        return formattedDialog ?? new List<DialogElement>();
    }

    [SideCar]
    public void AppendConversationDialogs(string conversationId, List<DialogElement> dialogs)
    {
        if (string.IsNullOrEmpty(conversationId)) return;

        var dialogElements = dialogs.Select(x => DialogLiteDBElement.ToLiteDBElement(x)).ToList();

        var conv = _dc.Conversations.FindOne(x => x.Id == conversationId);
        if (conv != null)
        {
            conv.DialogCount += dialogs.Count;
            conv.UpdatedTime = DateTime.UtcNow;
            _dc.Conversations.Update(conv);
        }

        var dialog = _dc.ConversationDialogs.FindOne(x => x.ConversationId == conversationId);
        if(dialog != null)
        {
            dialog.Dialogs.AddRange(dialogElements);
            _dc.ConversationDialogs.Update(dialog);
        }
    }

    public void UpdateConversationTitle(string conversationId, string title)
    {
        if (string.IsNullOrEmpty(conversationId)) return;

        var conv = _dc.Conversations.FindOne(x => x.Id == conversationId);
        if (conv != null)
        {
            conv.Title = title;
            conv.UpdatedTime = DateTime.UtcNow;
            _dc.Conversations.Update(conv);
        }
    }
	
  public void UpdateConversationTitleAlias(string conversationId, string titleAlias)
    {
        if (string.IsNullOrEmpty(conversationId)) return;

        var conv = _dc.Conversations.FindById(conversationId);
        if (conv != null)
        {
            conv.TitleAlias = titleAlias;
            conv.UpdatedTime = DateTime.UtcNow;
            _dc.Conversations.Update(conv);
        }
    }

    public bool UpdateConversationTags(string conversationId, List<string> tags)
    {
        if (string.IsNullOrEmpty(conversationId)) return false;

        var conv = _dc.Conversations.FindById(conversationId);
        if (conv != null)
        {
            conv.Tags = tags ?? new List<string>();
            conv.UpdatedTime = DateTime.UtcNow;
            _dc.Conversations.Update(conv);
            return true;
        }
        return false;
    }

    public bool AppendConversationTags(string conversationId, List<string> tags)
    {
        if (string.IsNullOrEmpty(conversationId) || tags.IsNullOrEmpty()) return false;

        var conv = _dc.Conversations.FindById(conversationId);
        if (conv != null)
        {
            var curTags = conv.Tags ?? new List<string>();
            var newTags = curTags.Concat(tags).Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();
            conv.Tags = newTags;
            conv.UpdatedTime = DateTime.UtcNow;
            _dc.Conversations.Update(conv);
            return true;
        }
        return false;
    }


    public bool UpdateConversationMessage(string conversationId, UpdateMessageRequest request)
    {
        if (string.IsNullOrEmpty(conversationId)) return false;

        var foundDialog = _dc.ConversationDialogs.FindOne(x => x.ConversationId == conversationId);
        if (foundDialog == null || foundDialog.Dialogs.IsNullOrEmpty())
        {
            return false;
        }

        var dialogs = foundDialog.Dialogs;
        var candidates = dialogs.Where(x => x.MetaData.MessageId == request.Message.MetaData.MessageId
                                            && x.MetaData.Role == request.Message.MetaData.Role).ToList();

        var found = candidates.Where((_, idx) => idx == request.InnderIndex).FirstOrDefault();
        if (found == null) return false;

        found.Content = request.Message.Content;
        found.RichContent = request.Message.RichContent;

        if (!string.IsNullOrEmpty(found.SecondaryContent))
        {
            found.SecondaryContent = request.Message.Content;
        }

        if (!string.IsNullOrEmpty(found.SecondaryRichContent))
        {
            found.SecondaryRichContent = request.Message.RichContent;
        }

        foundDialog.UpdatedTime = DateTime.UtcNow;
        _dc.ConversationDialogs.Update(foundDialog);
        return true;
    }

    [SideCar]
    public void UpdateConversationBreakpoint(string conversationId, ConversationBreakpoint breakpoint)
    {
        if (string.IsNullOrEmpty(conversationId)) return;

        var newBreakpoint = new BreakpointLiteDBElement()
        {
            MessageId = breakpoint.MessageId,
            Breakpoint = breakpoint.Breakpoint,
            CreatedTime = DateTime.UtcNow,
            Reason = breakpoint.Reason
        };

        var state = _dc.ConversationStates.FindOne(x => x.ConversationId == conversationId);

        if (state != null)
        {
            state.Breakpoints.Add(newBreakpoint);
            _dc.ConversationStates.Update(state);
        }
    }

    public ConversationBreakpoint? GetConversationBreakpoint(string conversationId)
    {
        if (string.IsNullOrEmpty(conversationId))
        {
            return null;
        }

        var state = _dc.ConversationStates.FindOne(x => x.ConversationId == conversationId);
        var leafNode = state?.Breakpoints?.LastOrDefault();

        if (leafNode == null)
        {
            return null;
        }

        return new ConversationBreakpoint
        {
            Breakpoint = leafNode.Breakpoint,
            MessageId = leafNode.MessageId,
            Reason = leafNode.Reason,
            CreatedTime = leafNode.CreatedTime,
        };
    }

    public ConversationState GetConversationStates(string conversationId)
    {
        var states = new ConversationState();
        if (string.IsNullOrEmpty(conversationId)) return states;

        var foundStates = _dc.ConversationStates.FindOne(x => x.ConversationId == conversationId);
        if (foundStates == null || foundStates.States.IsNullOrEmpty()) return states;

        var savedStates = foundStates.States.Select(x => StateLiteDBElement.ToDomainElement(x)).ToList();
        return new ConversationState(savedStates);
    }

    public void UpdateConversationStates(string conversationId, List<StateKeyValue> states)
    {
        if (string.IsNullOrEmpty(conversationId) || states == null) return;

        var saveStates = states.Select(x => StateLiteDBElement.ToLiteDBElement(x)).ToList();
        var state = _dc.ConversationStates.FindOne(x => x.ConversationId == conversationId);

        if (state != null)
        {
            state.States = saveStates;
            _dc.ConversationStates.Update(state);
        }
    }

    public void UpdateConversationStatus(string conversationId, string status)
    {
        if (string.IsNullOrEmpty(conversationId) || string.IsNullOrEmpty(status)) return;

        var conv = _dc.Conversations.FindOne(x => x.Id == conversationId);
        if (conv != null)
        {
            conv.Status = status;
            conv.UpdatedTime = DateTime.UtcNow;
            _dc.Conversations.Update(conv);
        }
    }

    public Conversation GetConversation(string conversationId)
    {
        if (string.IsNullOrEmpty(conversationId)) return null;

        var conv = _dc.Conversations.FindOne(x => x.Id == conversationId);
        var dialog = _dc.ConversationDialogs.FindOne(x => x.ConversationId == conversationId);
        var states = _dc.ConversationStates.FindOne(x => x.ConversationId == conversationId);

        if (conv == null) return null;

        var dialogElements = dialog?.Dialogs?.Select(x => DialogLiteDBElement.ToDomainElement(x))?.ToList() ?? new List<DialogElement>();
        var curStates = new Dictionary<string, string>();
        states.States.ForEach(x =>
        {
            curStates[x.Key] = x.Values?.LastOrDefault()?.Data ?? string.Empty;
        });

        return new Conversation
        {
            Id = conv.Id.ToString(),
            AgentId = conv.AgentId.ToString(),
            UserId = conv.UserId.ToString(),
            Title = conv.Title,
            Channel = conv.Channel,
            Status = conv.Status,
            Dialogs = dialogElements,
            States = curStates,
            DialogCount = conv.DialogCount,
            Tags = conv.Tags,
            CreatedTime = conv.CreatedTime,
            UpdatedTime = conv.UpdatedTime
        };
    }

    public PagedItems<Conversation> GetConversations(ConversationFilter filter)
    {
        var query = _dc.Conversations.Query();
        // Filter conversations
        if (!string.IsNullOrEmpty(filter?.Id))
        {
            query = query.Where(x => x.Id == filter.Id);
        }
        if (!string.IsNullOrEmpty(filter?.Title))
        {
            query = query.Where(x => x.Title.Contains(filter.Title));
        }
        if (!string.IsNullOrEmpty(filter?.TitleAlias))
        {
            query = query.Where(x => x.TitleAlias.Contains(filter.TitleAlias));
        }
        if (!string.IsNullOrEmpty(filter?.AgentId))
        {
            query = query.Where(x => x.AgentId == filter.AgentId);
        }
        if (!string.IsNullOrEmpty(filter?.Status))
        {
            query = query.Where(x => x.Status == filter.Status);
        }
        if (!string.IsNullOrEmpty(filter?.Channel))
        {
            query = query.Where(x => x.Channel == filter.Channel);
        }
        if (!string.IsNullOrEmpty(filter?.UserId))
        {
            query = query.Where(x => x.UserId == filter.UserId);
        }
        if (!string.IsNullOrEmpty(filter?.TaskId))
        {
            query = query.Where(x => x.TaskId == filter.TaskId);
        }
        if (filter?.StartTime != null)
        {
            query = query.Where(x => x.CreatedTime >= filter.StartTime.Value);
        }
        if (filter?.Tags != null && filter.Tags.Any())
        {
            foreach (var tag in filter.Tags)
            {
                query = query.Where(x => x.Tags.Contains(tag));
            }
        }

        //todo: Filter states
        // Filter states
        //var stateFilters = new List<FilterDefinition<ConversationStateDocument>>();
        //if (filter != null && string.IsNullOrEmpty(filter.Id) && !filter.States.IsNullOrEmpty())
        //{
        //    foreach (var pair in filter.States)
        //    {
        //        var elementFilters = new List<FilterDefinition<StateMongoElement>> { Builders<StateMongoElement>.Filter.Eq(x => x.Key, pair.Key) };
        //        if (!string.IsNullOrEmpty(pair.Value))
        //        {
        //            elementFilters.Add(Builders<StateMongoElement>.Filter.Eq("Values.Data", pair.Value));
        //        }
        //        stateFilters.Add(Builders<ConversationStateDocument>.Filter.ElemMatch(x => x.States, Builders<StateMongoElement>.Filter.And(elementFilters)));
        //    }

        //    var targetConvIds = _dc.ConversationStates.Find(Builders<ConversationStateDocument>.Filter.And(stateFilters)).ToEnumerable().Select(x => x.ConversationId).Distinct().ToList();
        //    convFilters.Add(convBuilder.In(x => x.Id, targetConvIds));
        //}

        // Sort and paginate
        //var filterDef = convBuilder.And(convFilters);
        //var sortDef = Builders<ConversationDocument>.Sort.Descending(x => x.CreatedTime);
        var pager = filter?.Pager ?? new Pagination();

        // todo: Apply sorting based on sort and order fields
        // Apply sorting based on sort and order fields
        //if (!string.IsNullOrEmpty(pager?.Sort))
        //{
        //    var sortField = ConvertSnakeCaseToPascalCase(pager.Sort);

        //    if (pager.Order == "asc")
        //    {
        //        sortDef = Builders<ConversationDocument>.Sort.Ascending(sortField);
        //    }
        //    else if (pager.Order == "desc")
        //    {
        //        sortDef = Builders<ConversationDocument>.Sort.Descending(sortField);
        //    }
        //}

        var conversationDocs = query.OrderByDescending(x => x.CreatedTime).Skip(pager.Offset).Limit(pager.Size).ToList();
        var count = query.Count();

        var conversations = conversationDocs.Select(x => new Conversation
        {
            Id = x.Id.ToString(),
            AgentId = x.AgentId.ToString(),
            UserId = x.UserId.ToString(),
            TaskId = x.TaskId,
            Title = x.Title,
            Channel = x.Channel,
            Status = x.Status,
            DialogCount = x.DialogCount,
            Tags = x.Tags ?? new(),
            CreatedTime = x.CreatedTime,
            UpdatedTime = x.UpdatedTime
        }).ToList();

        return new PagedItems<Conversation>
        {
            Items = conversations,
            Count = (int)count
        };
    }

    public List<Conversation> GetLastConversations()
    {
        //todo: Group by user id and get the latest conversation
        var records = new List<Conversation>();
        var conversations = _dc.Conversations.Query()
            .OrderByDescending(x => x.CreatedTime)
                                             //.GroupBy(c => c.UserId, g => g.First(x => x.CreatedTime == g.Select(y => y.CreatedTime).Max()))
                                             .ToList();
        return conversations.Select(c => new Conversation()
        {
            Id = c.Id.ToString(),
            AgentId = c.AgentId.ToString(),
            UserId = c.UserId.ToString(),
            Title = c.Title,
            Channel = c.Channel,
            Status = c.Status,
            DialogCount = c.DialogCount,
            Tags = c.Tags ?? new(),
            CreatedTime = c.CreatedTime,
            UpdatedTime = c.UpdatedTime
        }).ToList();
    }

    public List<string> GetIdleConversations(int batchSize, int messageLimit, int bufferHours, IEnumerable<string> excludeAgentIds)
    {
        var page = 1;
        var batchLimit = 100;
        var utcNow = DateTime.UtcNow;
        var conversationIds = new List<string>();

        if (batchSize <= 0 || batchSize > batchLimit)
        {
            batchSize = batchLimit;
        }

        if (bufferHours <= 0)
        {
            bufferHours = 12;
        }

        if (messageLimit <= 0)
        {
            messageLimit = 2;
        }

        while (true)
        {
            var skip = (page - 1) * batchSize;
            var candidates = _dc.Conversations.Query()
                                              .Where(x => !excludeAgentIds.Contains(x.AgentId) && x.DialogCount <= messageLimit && x.UpdatedTime <= utcNow.AddHours(-bufferHours))
                                              .Skip(skip)
                                              .Limit(batchSize)
                                              .ToList()
                                              .Select(x => x.Id);
                                              
            if (candidates.IsNullOrEmpty())
            {
                break;
            }

            conversationIds = conversationIds.Concat(candidates).Distinct().ToList();
            if (conversationIds.Count >= batchSize)
            {
                break;
            }

            page++;
        }

        return conversationIds.Take(batchSize).ToList();
    }

    public List<string> TruncateConversation(string conversationId, string messageId, bool cleanLog = false)
    {
        var deletedMessageIds = new List<string>();
        if (string.IsNullOrEmpty(conversationId) || string.IsNullOrEmpty(messageId))
        {
            return deletedMessageIds;
        }

        var foundDialog = _dc.ConversationDialogs.FindOne(x => x.ConversationId == conversationId);
        if (foundDialog == null || foundDialog.Dialogs.IsNullOrEmpty())
        {
            return deletedMessageIds;
        }

        var foundIdx = foundDialog.Dialogs.FindIndex(x => x.MetaData?.MessageId == messageId);
        if (foundIdx < 0)
        {
            return deletedMessageIds;
        }

        deletedMessageIds = foundDialog.Dialogs.Where((x, idx) => idx >= foundIdx && !string.IsNullOrEmpty(x.MetaData?.MessageId))
                                               .Select(x => x.MetaData.MessageId).Distinct().ToList();

        // Handle truncated dialogs
        var truncatedDialogs = foundDialog.Dialogs.Where((x, idx) => idx < foundIdx).ToList();

        // Handle truncated states
        var refTime = foundDialog.Dialogs.ElementAt(foundIdx).MetaData.CreateTime;
      
        var foundStates = _dc.ConversationStates.FindOne(x => x.ConversationId == conversationId);

        if (foundStates != null)
        {
            // Truncate states
            if (!foundStates.States.IsNullOrEmpty())
            {
                var truncatedStates = new List<StateLiteDBElement>();
                foreach (var state in foundStates.States)
                {
                    if (!state.Versioning)
                    {
                        truncatedStates.Add(state);
                        continue;
                    }

                    var values = state.Values.Where(x => x.MessageId != messageId)
                                             .Where(x => x.UpdateTime < refTime)
                                             .ToList();
                    if (values.Count == 0) continue;

                    state.Values = values;
                    truncatedStates.Add(state);
                }
                foundStates.States = truncatedStates;
            }

            // Truncate breakpoints
            if (!foundStates.Breakpoints.IsNullOrEmpty())
            {
                var breakpoints = foundStates.Breakpoints ?? new List<BreakpointLiteDBElement>();
                var truncatedBreakpoints = breakpoints.Where(x => x.CreatedTime < refTime).ToList();
                foundStates.Breakpoints = truncatedBreakpoints;
            }

            // Update
            _dc.ConversationStates.Update(foundStates);
        }

        // Save dialogs
        foundDialog.Dialogs = truncatedDialogs;
        _dc.ConversationDialogs.Update(foundDialog);

        // Update conversation
        var conv = _dc.Conversations.FindOne(x => x.Id == conversationId);
        if (conv != null)
        {
            conv.UpdatedTime = DateTime.UtcNow;
            conv.DialogCount = truncatedDialogs.Count;
            _dc.Conversations.Update(conv);
        }

        // Remove logs
        if (cleanLog)
        {
            _dc.ContentLogs.DeleteMany(x => x.ConversationId == conversationId && x.CreateTime >= refTime);
            _dc.StateLogs.DeleteMany(x => x.ConversationId == conversationId && x.CreateTime >= refTime);
        }

        return deletedMessageIds;
    }

#if !DEBUG
    [SharpCache(10)]
#endif
    public List<string> GetConversationStateSearchKeys(int messageLowerLimit = 2, int convUpperlimit = 100)
    {
        var conversations = _dc.Conversations
            .Find(x => x.DialogCount >= messageLowerLimit)
            .OrderByDescending(x => x.UpdatedTime)
            .Take(convUpperlimit)
            .ToList();

        if (conversations.Count == 0) return new List<string>();

        var convIds = conversations.Select(x => x.Id).ToList();
        var states = _dc.ConversationStates
            .Find(x => convIds.Contains(x.ConversationId))
            .ToList();

        var keys = states.SelectMany(x => x.States.Select(s => s.Key)).Distinct().ToList();
        return keys;
    }

    private string ConvertSnakeCaseToPascalCase(string snakeCase)
    {
        string[] words = snakeCase.Split('_');
        StringBuilder pascalCase = new();

        foreach (string word in words)
        {
            if (!string.IsNullOrEmpty(word))
            {
                string firstLetter = word[..1].ToUpper();
                string restOfWord = word[1..].ToLower();
                pascalCase.Append(firstLetter + restOfWord);
            }
        }

        return pascalCase.ToString();
    }
}
