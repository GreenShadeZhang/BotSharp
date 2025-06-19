using BotSharp.Abstraction.Conversations.Enums;
using BotSharp.Abstraction.Conversations.Models;
using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Plugin.EntityFrameworkCore.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace BotSharp.Plugin.EntityFrameworkCore.Repository;

public partial class EfCoreRepository
{
    public void CreateNewConversation(Abstraction.Conversations.Models.Conversation conversation)
    {
        if (conversation == null) return;

        var utcNow = DateTime.UtcNow;

        var convDoc = new Entities.Conversation
        {
            Id = !string.IsNullOrEmpty(conversation.Id) ? conversation.Id : Guid.NewGuid().ToString(),
            AgentId = conversation.AgentId,
            UserId = !string.IsNullOrEmpty(conversation.UserId) ? conversation.UserId : string.Empty,
            Title = conversation.Title,
            TitleAlias = conversation.TitleAlias,
            Channel = conversation.Channel,
            ChannelId = conversation.ChannelId ?? ConversationChannel.OpenAPI,
            TaskId = conversation.TaskId,
            Status = conversation.Status,
            Tags = conversation.Tags ?? [],
            LatestStates = new(),
            CreatedTime = utcNow,
            UpdatedTime = utcNow
        };

        _context.Conversations.Add(convDoc);
        _context.SaveChanges();
    }

    public bool DeleteConversations(IEnumerable<string> conversationIds)
    {
        if (conversationIds?.Any() != true) return false;

        try
        {
            var conversations = _context.Conversations.Where(x => conversationIds.Contains(x.Id)).ToList();
            var dialogs = _context.ConversationDialogs.Where(x => conversationIds.Contains(x.ConversationId)).ToList();
            var states = _context.ConversationStates.Where(x => conversationIds.Contains(x.ConversationId)).ToList();

            _context.Conversations.RemoveRange(conversations);
            _context.ConversationDialogs.RemoveRange(dialogs);
            _context.ConversationStates.RemoveRange(states);

            var deleted = _context.SaveChanges();
            return deleted > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conversations");
            return false;
        }
    }

    public List<DialogElement> GetConversationDialogs(string conversationId)
    {
        var dialogs = new List<DialogElement>();
        if (string.IsNullOrEmpty(conversationId)) return dialogs;

        var foundDialog = _context.ConversationDialogs.Where(x => x.ConversationId == conversationId).ToList();

        if (foundDialog == null) return dialogs;

        var formattedDialog = foundDialog?.Select(x =>
        {
            return new DialogElement
            {
                MetaData = x.MetaData.ToModel(),
                Content = x.Content,
                SecondaryContent = x.SecondaryContent,
                RichContent = x.RichContent,
                SecondaryRichContent = x.SecondaryRichContent,
                Payload = x.Payload
            };
        })?.ToList();
        return formattedDialog ?? new List<DialogElement>();
    }

    public void AppendConversationDialogs(string conversationId, List<DialogElement> dialogs)
    {
        if (string.IsNullOrEmpty(conversationId)) return;

        var dialogElements = dialogs.Select(x =>
        {
            return new Entities.ConversationDialog
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = conversationId,
                MetaData = x.MetaData.ToEntity(),
                Content = x.Content,
                SecondaryContent = x.SecondaryContent,
                RichContent = x.RichContent,
                SecondaryRichContent = x.SecondaryRichContent,
                Payload = x.Payload
            };
        }).ToList();

        var conv = _context.Conversations.FirstOrDefault(x => x.Id == conversationId);
        if (conv != null)
        {
            conv.UpdatedTime = DateTime.UtcNow;
            conv.DialogCount += dialogs.Count;
        }

        _context.ConversationDialogs.AddRange(dialogElements);
        _context.SaveChanges();
    }

    public void UpdateConversationTitle(string conversationId, string title)
    {
        if (string.IsNullOrEmpty(conversationId)) return;

        var conv = _context.Conversations.FirstOrDefault(x => x.Id == conversationId);

        if (conv != null)
        {
            conv.UpdatedTime = DateTime.UtcNow;
            conv.Title = title;
            _context.SaveChanges();
        }
    }

    public void UpdateConversationTitleAlias(string conversationId, string titleAlias)
    {
        if (string.IsNullOrEmpty(conversationId)) return;

        var conv = _context.Conversations.FirstOrDefault(x => x.Id == conversationId);

        if (conv != null)
        {
            conv.UpdatedTime = DateTime.UtcNow;
            conv.TitleAlias = titleAlias;
            _context.SaveChanges();
        }
    }

    public bool UpdateConversationMessage(string conversationId, UpdateMessageRequest request)
    {
        if (string.IsNullOrEmpty(conversationId)) return false;

        var foundDialogs = _context.ConversationDialogs.Where(x => x.ConversationId == conversationId).ToList();
        if (foundDialogs == null || foundDialogs.IsNullOrEmpty())
        {
            return false;
        }

        var dialogs = foundDialogs;
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


        _context.ConversationDialogs.Update(found);
        _context.SaveChanges();
        return true;
    }

    public void UpdateConversationBreakpoint(string conversationId, ConversationBreakpoint breakpoint)
    {
        if (string.IsNullOrEmpty(conversationId)) return;

        var newBreakpoint = new BreakpointInfoElement()
        {
            MessageId = breakpoint.MessageId,
            Breakpoint = breakpoint.Breakpoint,
            CreatedTime = DateTime.UtcNow,
            Reason = breakpoint.Reason
        };

        var state = _context.ConversationStates.FirstOrDefault(x => x.ConversationId == conversationId);
        if (state != null)
        {
            state.Breakpoints.Add(newBreakpoint);
            _context.SaveChanges();
        }
    }

    public ConversationBreakpoint? GetConversationBreakpoint(string conversationId)
    {
        if (string.IsNullOrEmpty(conversationId))
        {
            return null;
        }

        var state = _context.ConversationStates.FirstOrDefault(x => x.ConversationId == conversationId);
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

    public Abstraction.Conversations.Models.ConversationState GetConversationStates(string conversationId)
    {
        var states = new Abstraction.Conversations.Models.ConversationState();
        if (string.IsNullOrEmpty(conversationId)) return states;

        var foundStates = _context.ConversationStates.Include(c => c.States).ThenInclude(s => s.Values).FirstOrDefault(x => x.ConversationId == conversationId);

        if (foundStates == null || foundStates.States.IsNullOrEmpty()) return states;

        var savedStates = foundStates.States.Select(x => x.ToModel()).ToList();
        return new Abstraction.Conversations.Models.ConversationState(savedStates);
    }

    public void UpdateConversationStates(string conversationId, List<StateKeyValue> states)
    {
        if (string.IsNullOrEmpty(conversationId) || states == null) return;

        var foundStates = _context.ConversationStates.Include(c => c.States).ThenInclude(s => s.Values).Where(x => x.ConversationId == conversationId).ToList();

        foreach (var state in foundStates)
        {
            var saveStates = states.Select(x => x.ToEntity(state)).ToList();
            state.States = saveStates;
        }
        _context.SaveChanges();
    }

    public void UpdateConversationStatus(string conversationId, string status)
    {
        if (string.IsNullOrEmpty(conversationId) || string.IsNullOrEmpty(status)) return;

        var conv = _context.Conversations.FirstOrDefault(x => x.Id == conversationId);

        if (conv != null)
        {
            conv.Status = status;
            conv.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }

    public Conversation GetConversation(string conversationId, bool isLoadStates = false)
    {
        if (string.IsNullOrEmpty(conversationId)) return null;

        var conv = _context.Conversations.FirstOrDefault(x => x.Id == conversationId);
        if (conv == null) return null;

        var result = new Conversation
        {
            Id = conv.Id,
            AgentId = conv.AgentId,
            UserId = conv.UserId,
            Title = conv.Title,
            TitleAlias = conv.TitleAlias,
            Channel = conv.Channel,
            ChannelId = conv.ChannelId,
            Status = conv.Status,
            TaskId = conv.TaskId,
            Tags = conv.Tags ?? new List<string>(),
            DialogCount = conv.DialogCount,
            CreatedTime = conv.CreatedTime,
            UpdatedTime = conv.UpdatedTime
        };

        if (isLoadStates)
        {
            var states = _context.ConversationStates
                .Include(c => c.States)
                .ThenInclude(s => s.Values)
                .FirstOrDefault(x => x.ConversationId == conversationId);

            if (states?.States?.Any() == true)
            {
                var stateDict = new Dictionary<string, string>();
                foreach (var state in states.States)
                {
                    stateDict[state.Key] = state.Values?.LastOrDefault()?.Data ?? string.Empty;
                }
                result.States = stateDict;
            }
        }

        return result;
    }

    public PagedItems<Conversation> GetConversations(ConversationFilter filter)
    {
        var query = _context.Conversations.AsQueryable();

        var convStateQuery = _context.ConversationStates.Include(c => c.States).ThenInclude(s => s.Values).AsQueryable();

        // Filter conversations
        if (!string.IsNullOrEmpty(filter?.Id))
        {
            query = query.Where(x => x.Id == filter.Id);
        }
        if (!string.IsNullOrEmpty(filter?.Title))
        {
            query = query.Where(x => x.Title.Contains(filter.Title));
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

        // Filter states
        if (filter != null && string.IsNullOrEmpty(filter.Id) && !filter.States.IsNullOrEmpty())
        {
            foreach (var pair in filter.States)
            {
                convStateQuery = convStateQuery.Where(x => x.States.Any(s => s.Key == pair.Key && s.Values.Any(v => v.Data == pair.Value)));
            }

            var targetConvIds = convStateQuery.Select(x => x.ConversationId).ToList();

            query = query.Where(x => targetConvIds.Contains(x.Id));
        }

        // Sort and paginate
        var pager = filter?.Pager ?? new Pagination();

        // Apply sorting based on sort and order fields
        if (!string.IsNullOrEmpty(pager?.Sort))
        {
            var sortField = ConvertSnakeCaseToPascalCase(pager.Sort);

            var parameter = Expression.Parameter(typeof(Entities.Conversation));

            var property = Expression.Property(parameter, sortField);
            var lambda = Expression.Lambda(property, parameter);

            if (pager.Order == "asc")
            {
                if (property.Type == typeof(string))
                {
                    query = query.OrderBy((Expression<Func<Entities.Conversation, string>>)lambda);
                }
                else if (property.Type == typeof(DateTime))
                {
                    query = query.OrderBy((Expression<Func<Entities.Conversation, DateTime>>)lambda);
                }
                else if (property.Type == typeof(int))
                {
                    query = query.OrderBy((Expression<Func<Entities.Conversation, int>>)lambda);
                }
            }
            else if (pager.Order == "desc")
            {
                if (property.Type == typeof(string))
                {
                    query = query.OrderByDescending((Expression<Func<Entities.Conversation, string>>)lambda);
                }
                else if (property.Type == typeof(DateTime))
                {
                    query = query.OrderByDescending((Expression<Func<Entities.Conversation, DateTime>>)lambda);
                }
                else if (property.Type == typeof(int))
                {
                    query = query.OrderByDescending((Expression<Func<Entities.Conversation, int>>)lambda);
                }
            }
        }

        var count = query.Count();

        var conversations = query
            .Skip(pager.Offset)
            .Take(pager.Size)
            .Select(x => new Conversation
            {
                Id = x.Id.ToString(),
                AgentId = x.AgentId.ToString(),
                UserId = x.UserId.ToString(),
                TaskId = x.TaskId,
                Title = x.Title,
                TitleAlias = x.TitleAlias,
                Channel = x.Channel,
                Status = x.Status,
                DialogCount = x.DialogCount,
                CreatedTime = x.CreatedTime,
                UpdatedTime = x.UpdatedTime
            })
            .ToList();

        return new PagedItems<Conversation>
        {
            Items = conversations,
            Count = count
        };
    }

    public List<Conversation> GetLastConversations()
    {
        var records = new List<Conversation>();
        var conversations = _context.Conversations
            .GroupBy(c => c.UserId)
            .Select(g => g.OrderByDescending(x => x.CreatedTime).FirstOrDefault())
            .ToList();

        return conversations.Select(c => new Conversation()
        {
            Id = c.Id.ToString(),
            AgentId = c.AgentId.ToString(),
            UserId = c.UserId.ToString(),
            Title = c.Title,
            TitleAlias = c.TitleAlias,
            Channel = c.Channel,
            Status = c.Status,
            DialogCount = c.DialogCount,
            CreatedTime = c.CreatedTime,
            UpdatedTime = c.UpdatedTime
        }).ToList();
    }

    public bool UpdateConversationTags(string conversationId, List<string> toAddTags, List<string> toDeleteTags)
    {
        if (string.IsNullOrWhiteSpace(conversationId)) return false;

        try
        {
            var conv = _context.Conversations.FirstOrDefault(x => x.Id == conversationId);
            if (conv == null) return false;

            conv.Tags ??= new List<string>();

            if (toDeleteTags?.Any() == true)
            {
                conv.Tags = conv.Tags.Where(t => !toDeleteTags.Contains(t)).ToList();
            }

            if (toAddTags?.Any() == true)
            {
                foreach (var tag in toAddTags.Where(t => !conv.Tags.Contains(t)))
                {
                    conv.Tags.Add(tag);
                }
            }

            conv.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating conversation tags for {ConversationId}", conversationId);
            return false;
        }
    }

    public bool AppendConversationTags(string conversationId, List<string> tags)
    {
        if (string.IsNullOrWhiteSpace(conversationId) || tags?.Any() != true) return false;

        try
        {
            var conv = _context.Conversations.FirstOrDefault(x => x.Id == conversationId);
            if (conv == null) return false;

            conv.Tags ??= new List<string>();
            foreach (var tag in tags.Where(t => !conv.Tags.Contains(t)))
            {
                conv.Tags.Add(tag);
            }

            conv.UpdatedTime = DateTime.UtcNow;
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error appending conversation tags for {ConversationId}", conversationId);
            return false;
        }
    }

    public List<string> GetConversationStateSearchKeys(ConversationStateKeysFilter filter)
    {
        if (filter == null) return new List<string>();

        var query = _context.ConversationStates
            .Include(c => c.States)
            .AsQueryable();

        var keys = query.SelectMany(x => x.States.Select(s => s.Key))
                       .Distinct()
                       .ToList();

        return keys;
    }

    public List<string> GetConversationsToMigrate(int batchSize = 100)
    {
        var conversations = _context.Conversations
            .Where(x => x.LatestStates == null || x.LatestStates.Count == 0)
            .Take(batchSize)
            .Select(x => x.Id)
            .ToList();

        return conversations;
    }

    public bool MigrateConvsersationLatestStates(string conversationId)
    {
        if (string.IsNullOrWhiteSpace(conversationId)) return false;

        try
        {
            var conv = _context.Conversations.FirstOrDefault(x => x.Id == conversationId);
            if (conv == null) return false;

            var states = _context.ConversationStates
                .Include(c => c.States)
                .ThenInclude(s => s.Values)
                .FirstOrDefault(x => x.ConversationId == conversationId);

            if (states?.States?.Any() == true)
            {
                var latestStates = new Dictionary<string, object>();
                foreach (var state in states.States)
                {
                    var latestValue = state.Values?.OrderByDescending(v => v.UpdateTime).FirstOrDefault();
                    if (latestValue != null)
                    {
                        latestStates[state.Key] = latestValue.Data;
                    }
                }
                conv.LatestStates = latestStates;
                conv.UpdatedTime = DateTime.UtcNow;
                _context.SaveChanges();
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error migrating conversation latest states for {ConversationId}", conversationId);
            return false;
        }
    }

    public List<string> GetIdleConversations(int batchSize, int messageLimit, int bufferHours, IEnumerable<string> excludeAgentIds)
    {
        var bufferTime = DateTime.UtcNow.AddHours(-bufferHours);
        excludeAgentIds ??= Enumerable.Empty<string>();

        var conversations = _context.Conversations
            .Where(x => x.DialogCount <= messageLimit &&
                       x.UpdatedTime < bufferTime &&
                       !excludeAgentIds.Contains(x.AgentId))
            .OrderBy(x => x.UpdatedTime)
            .Take(batchSize)
            .Select(x => x.Id)
            .ToList();

        return conversations;
    }

    public List<string> TruncateConversation(string conversationId, string messageId, bool cleanLog = false)
    {
        var deletedMessageIds = new List<string>();
        if (string.IsNullOrEmpty(conversationId) || string.IsNullOrEmpty(messageId))
        {
            return deletedMessageIds;
        }

        var foundDialogs = _context.ConversationDialogs.Where(x => x.ConversationId == conversationId).ToList();
        if (foundDialogs == null || foundDialogs.IsNullOrEmpty())
        {
            return deletedMessageIds;
        }

        var foundIdx = foundDialogs.FindIndex(x => x.MetaData?.MessageId == messageId);
        if (foundIdx < 0)
        {
            return deletedMessageIds;
        }

        deletedMessageIds = foundDialogs.Where((x, idx) => idx >= foundIdx && !string.IsNullOrEmpty(x.MetaData?.MessageId))
                                               .Select(x => x.MetaData.MessageId).Distinct().ToList();

        // Handle truncated dialogs
        var truncatedDialogs = foundDialogs.Where((x, idx) => idx < foundIdx).ToList();

        // Handle truncated states
        var refTime = foundDialogs.ElementAt(foundIdx).MetaData.CreateTime;
        var foundStates = _context.ConversationStates.FirstOrDefault(x => x.ConversationId == conversationId);

        if (foundStates != null)
        {
            // Truncate states
            if (!foundStates.States.IsNullOrEmpty())
            {
                var truncatedStates = new List<Entities.State>();
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
                var breakpoints = foundStates.Breakpoints ?? new List<BreakpointInfoElement>();
                var truncatedBreakpoints = breakpoints.Where(x => x.CreatedTime < refTime).ToList();
                foundStates.Breakpoints = truncatedBreakpoints;
            }

            // Update
            _context.ConversationStates.Update(foundStates);
        }

        // Save dialogs
        foundDialogs = truncatedDialogs;
        _context.ConversationDialogs.UpdateRange(foundDialogs);

        // Update conversation
        var conv = _context.Conversations.FirstOrDefault(x => x.Id == conversationId);
        if (conv != null)
        {
            conv.UpdatedTime = DateTime.UtcNow;
            conv.DialogCount = truncatedDialogs.Count;
            _context.Conversations.Update(conv);
        }

        // Remove logs
        if (cleanLog)
        {
            var contentLogs = _context.ConversationContentLogs.Where(x => x.ConversationId == conversationId && x.CreatedTime >= refTime).ToList();

            var stateLogs = _context.ConversationStateLogs.Where(x => x.ConversationId == conversationId && x.CreatedTime >= refTime).ToList();

            _context.ConversationContentLogs.RemoveRange(contentLogs);
            _context.ConversationStateLogs.RemoveRange(stateLogs);
        }

        _context.SaveChanges();

        return deletedMessageIds;
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
