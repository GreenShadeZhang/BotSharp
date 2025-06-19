using BotSharp.Abstraction.Crontab.Models;
using BotSharp.Plugin.EntityFrameworkCore.Mappers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotSharp.Plugin.EntityFrameworkCore.Repository
{
    public partial class EfCoreRepository
    {
        #region Crontab
        public bool UpsertCrontabItem(CrontabItem cron)
        {
            if (cron == null || string.IsNullOrWhiteSpace(cron.ConversationId)) return false;

            try
            {
                var existing = _context.CrontabItems.FirstOrDefault(x => x.ConversationId == cron.ConversationId);

                if (existing != null)
                {
                    existing.UserId = cron.UserId;
                    existing.AgentId = cron.AgentId;
                    existing.ConversationId = cron.ConversationId;
                    existing.ExecutionResult = cron.AgentId;
                    existing.UserId = cron.UserId;
                    existing.Cron = cron.Cron;
                    existing.Title = cron.Title;
                    existing.Description = cron.Description;
                    existing.ExecutionCount = cron.ExecutionCount;
                    existing.MaxExecutionCount = cron.MaxExecutionCount;
                    existing.ExpireSeconds = cron.ExpireSeconds;
                    existing.LastExecutionTime = cron.LastExecutionTime;
                    existing.LessThan60Seconds = cron.LessThan60Seconds;
                    existing.Tasks = cron.Tasks?.Select(x => x.ToEntity())?.ToList() ?? [];
                }
                else
                {
                    var newCron = new Entities.CrontabItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = cron.UserId,
                        AgentId = cron.AgentId,
                        ConversationId = cron.ConversationId,
                        ExecutionResult = cron.ExecutionResult,
                        Cron = cron.Cron,
                        Title = cron.Title,
                        Description = cron.Description,
                        ExecutionCount = cron.ExecutionCount,
                        MaxExecutionCount = cron.MaxExecutionCount,
                        ExpireSeconds = cron.ExpireSeconds,
                        LastExecutionTime = cron.LastExecutionTime,
                        LessThan60Seconds = cron.LessThan60Seconds,
                        Tasks = cron.Tasks?.Select(x => x.ToEntity())?.ToList() ?? [],
                        CreatedTime = cron.CreatedTime
                    };
                    _context.CrontabItems.Add(newCron);
                }

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting crontab item for conversation {ConversationId}", cron.ConversationId);
                return false;
            }
        }

        public bool DeleteCrontabItem(string conversationId)
        {
            if (string.IsNullOrWhiteSpace(conversationId)) return false;

            try
            {
                var cron = _context.CrontabItems.FirstOrDefault(x => x.ConversationId == conversationId);
                if (cron != null)
                {
                    _context.CrontabItems.Remove(cron);
                    var deleted = _context.SaveChanges();
                    return deleted > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting crontab item for conversation {ConversationId}", conversationId);
                return false;
            }
        }

        public PagedItems<CrontabItem> GetCrontabItems(CrontabItemFilter filter)
        {
            var query = _context.CrontabItems.AsQueryable();

            if (filter != null)
            {
                foreach (var agentId in filter.AgentIds ?? [])
                {
                    query = query.Where(x => x.AgentId == agentId);
                }

                foreach (var conversationId in filter.ConversationIds ?? [])
                {
                    query = query.Where(x => x.ConversationId == conversationId);
                }

                foreach (var userId in filter.UserIds ?? [])
                {
                    query = query.Where(x => x.UserId == userId);
                }
            }

            var totalCount = query.Count();
            var items = query.OrderBy(x => x.CreatedTime)
                             .Skip(filter?.Offset ?? 0)
                             .Take(filter?.Size ?? 10)
                             .Select(x => new CrontabItem
                             {
                                 UserId = x.UserId,
                                 AgentId = x.AgentId,
                                 ConversationId = x.ConversationId,
                                 ExecutionResult = x.ExecutionResult,
                                 Cron = x.Cron,
                                 Title = x.Title,
                                 Description = x.Description,
                                 ExecutionCount = x.ExecutionCount,
                                 MaxExecutionCount = x.MaxExecutionCount,
                                 ExpireSeconds = x.ExpireSeconds,
                                 LastExecutionTime = x.LastExecutionTime,
                                 LessThan60Seconds = x.LessThan60Seconds,
                                 Tasks = x.Tasks.Select(x => x.ToModel()).ToArray(),
                                 CreatedTime = x.CreatedTime
                             })
                             .ToList();

            return new PagedItems<CrontabItem>
            {
                Items = items,
                Count = totalCount
            };
        }
        #endregion
    }
}
