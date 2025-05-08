using BotSharp.Abstraction.Crontab.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotSharp.Plugin.LiteDBStorage.Repository;

public partial class LiteDBRepository
{
    public bool UpsertCrontabItem(CrontabItem item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.ConversationId))
        {
            return false;
        }

        try
        {
            var cronDoc = CrontabItemDocument.ToLiteDBModel(item);
            cronDoc.Id = Guid.NewGuid().ToString();

            var existingItem = _dc.CrontabItems.Query().Where(x => x.ConversationId == item.ConversationId).FirstOrDefault();
            if (existingItem != null)
            {
                _dc.CrontabItems.Update(cronDoc);
            }
            else
            {
                _dc.CrontabItems.Insert(cronDoc);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error when saving crontab item: {ex.Message}\r\n{ex.InnerException}");
            return false;
        }
    }

    public bool DeleteCrontabItem(string conversationId)
    {
        if (string.IsNullOrWhiteSpace(conversationId))
        {
            return false;
        }

        var result = _dc.CrontabItems.DeleteMany(x => x.ConversationId == conversationId);
        return result > 0;
    }

    public PagedItems<CrontabItem> GetCrontabItems(CrontabItemFilter filter)
    {
        if (filter == null)
        {
            filter = CrontabItemFilter.Empty();
        }

        var query = _dc.CrontabItems.Query();

        if (filter?.AgentIds != null)
        {
            query = query.Where(x => filter.AgentIds.Contains(x.AgentId));
        }
        if (filter?.ConversationIds != null)
        {
            query = query.Where(x => filter.ConversationIds.Contains(x.ConversationId));
        }
        if (filter?.UserIds != null)
        {
            query = query.Where(x => filter.UserIds.Contains(x.UserId));
        }

        var total = query.Count();
        var items = query.OrderByDescending(x => x.CreatedTime)
                         .Skip(filter.Offset)
                         .Limit(filter.Size)
                         .ToList()
                         .Select(x => CrontabItemDocument.ToDomainModel(x))
                         .ToList();

        return new PagedItems<CrontabItem>
        {
            Items = items,
            Count = total
        };
    }
}
