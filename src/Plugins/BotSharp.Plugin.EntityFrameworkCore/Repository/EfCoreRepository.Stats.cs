using BotSharp.Abstraction.Statistics.Enums;
using BotSharp.Abstraction.Statistics.Models;
using BotSharp.Plugin.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BotSharp.Plugin.EntityFrameworkCore.Repository;

public partial class EfCoreRepository
{
    #region Statistics
    public BotSharpStats? GetGlobalStats(string agentId, DateTime recordTime, StatsInterval interval)
    {
        if (string.IsNullOrWhiteSpace(agentId))
        {
            return null;
        }

        var (startTime, endTime) = BotSharpStats.BuildTimeInterval(recordTime, interval);

        var found = _context.GlobalStats
            .FirstOrDefault(x => x.AgentId == agentId && 
                               x.StartTime == startTime && 
                               x.EndTime == endTime);

        return found != null ? new BotSharpStats
        {
            AgentId = agentId,
            Count = new()
            {
                AgentCallCount = found.Count?.AgentCallCount ?? 0
            },
            LlmCost = new()
            {
                PromptTokens = found.LlmCost?.PromptTokens ?? 0,
                CompletionTokens = found.LlmCost?.CompletionTokens ?? 0,
                PromptTotalCost = found.LlmCost?.PromptTotalCost ?? 0,
                CompletionTotalCost = found.LlmCost?.CompletionTotalCost ?? 0
            },
            RecordTime = found.RecordTime,
            StartTime = startTime,
            EndTime = endTime,
            Interval = interval.ToString()
        } : null;
    }

    public bool SaveGlobalStats(BotSharpStatsDelta delta)
    {
        if (delta == null || string.IsNullOrWhiteSpace(delta.AgentId))
        {
            return false;
        }

        try
        {
            var (startTime, endTime) = BotSharpStats.BuildTimeInterval(delta.RecordTime, delta.IntervalType);

            var existingStats = _context.GlobalStats
                .FirstOrDefault(x => x.AgentId == delta.AgentId && 
                                   x.StartTime == startTime && 
                                   x.EndTime == endTime);

            if (existingStats == null)
            {
                existingStats = new GlobalStat
                {
                    Id = Guid.NewGuid().ToString(),
                    AgentId = delta.AgentId,
                    StartTime = startTime,
                    EndTime = endTime,
                    RecordTime = delta.RecordTime,
                    Count = new Models.GlobalStatsCountElement
                    {
                        AgentCallCount = 0
                    },
                    LlmCost = new Models.GlobalStatsLlmCostElement
                    {
                        PromptTokens = 0,
                        CompletionTokens = 0,
                        PromptTotalCost = 0,
                        CompletionTotalCost = 0
                    }
                };
                _context.GlobalStats.Add(existingStats);
            }
            else
            {
                // Update stats with delta values
                existingStats.RecordTime = delta.RecordTime;

            }



            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving global stats for agent {AgentId}", delta.AgentId);
            return false;
        }
    }
    #endregion
}
