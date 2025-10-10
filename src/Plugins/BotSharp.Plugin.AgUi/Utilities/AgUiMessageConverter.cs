using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using BotSharp.Abstraction.Agents.Enums;
using BotSharp.Abstraction.Conversations.Models;
using BotSharp.Plugin.AgUi.ViewModels;

namespace BotSharp.Plugin.AgUi.Utilities;

/// <summary>
/// Utility class to convert between AG-UI and BotSharp message formats
/// </summary>
public static class AgUiMessageConverter
{
    /// <summary>
    /// Convert AG-UI messages to BotSharp RoleDialogModel list
    /// </summary>
    public static List<RoleDialogModel> ConvertToBotSharpMessages(List<AgUiMessage> aguiMessages)
    {
        var botSharpMessages = new List<RoleDialogModel>();

        foreach (var aguiMsg in aguiMessages)
        {
            if (aguiMsg.Role == "user" && !string.IsNullOrEmpty(aguiMsg.Content))
            {
                botSharpMessages.Add(new RoleDialogModel(AgentRole.User, aguiMsg.Content)
                {
                    MessageId = aguiMsg.Id ?? Guid.NewGuid().ToString()
                });
            }
            else if (aguiMsg.Role == "assistant")
            {
                if (!string.IsNullOrEmpty(aguiMsg.Content))
                {
                    botSharpMessages.Add(new RoleDialogModel(AgentRole.Assistant, aguiMsg.Content)
                    {
                        MessageId = aguiMsg.Id ?? Guid.NewGuid().ToString()
                    });
                }

                // Handle tool calls
                if (aguiMsg.ToolCalls != null && aguiMsg.ToolCalls.Any())
                {
                    foreach (var toolCall in aguiMsg.ToolCalls)
                    {
                        botSharpMessages.Add(new RoleDialogModel(AgentRole.Function, string.Empty)
                        {
                            MessageId = aguiMsg.Id ?? Guid.NewGuid().ToString(),
                            ToolCallId = toolCall.Id,
                            FunctionName = toolCall.Function.Name,
                            FunctionArgs = toolCall.Function.Arguments
                        });
                    }
                }
            }
            else if (aguiMsg.Role == "tool" && !string.IsNullOrEmpty(aguiMsg.ToolCallId))
            {
                // Tool result message
                botSharpMessages.Add(new RoleDialogModel(AgentRole.Function, aguiMsg.Content ?? string.Empty)
                {
                    MessageId = aguiMsg.Id ?? Guid.NewGuid().ToString(),
                    ToolCallId = aguiMsg.ToolCallId
                });
            }
        }

        return botSharpMessages;
    }

    /// <summary>
    /// Convert AG-UI tools to BotSharp function definitions
    /// </summary>
    public static Dictionary<string, object> ConvertToolsToFunctions(List<AgUiTool>? tools)
    {
        var functions = new Dictionary<string, object>();
        
        if (tools == null || !tools.Any())
        {
            return functions;
        }

        foreach (var tool in tools)
        {
            functions[tool.Name] = new
            {
                name = tool.Name,
                description = tool.Description ?? string.Empty,
                parameters = tool.Parameters ?? new Dictionary<string, object>()
            };
        }

        return functions;
    }

    /// <summary>
    /// Convert AG-UI context to BotSharp conversation state
    /// </summary>
    public static Dictionary<string, string> ConvertContextToState(List<AgUiContext>? context)
    {
        var state = new Dictionary<string, string>();
        
        if (context == null || !context.Any())
        {
            return state;
        }

        foreach (var ctx in context)
        {
            var value = ctx.Value switch
            {
                string str => str,
                _ => JsonSerializer.Serialize(ctx.Value)
            };
            
            state[$"context_{ctx.Name}"] = value;
        }

        return state;
    }

    /// <summary>
    /// Convert AG-UI state to BotSharp conversation state
    /// </summary>
    public static Dictionary<string, string> ConvertStateToConversationState(Dictionary<string, object>? aguiState)
    {
        var state = new Dictionary<string, string>();
        
        if (aguiState == null || !aguiState.Any())
        {
            return state;
        }

        foreach (var kvp in aguiState)
        {
            var value = kvp.Value switch
            {
                string str => str,
                _ => JsonSerializer.Serialize(kvp.Value)
            };
            
            state[kvp.Key] = value;
        }

        return state;
    }

    /// <summary>
    /// Parse JSON arguments safely
    /// </summary>
    public static string SafeSerializeArguments(string? args)
    {
        if (string.IsNullOrEmpty(args))
        {
            return "{}";
        }

        try
        {
            // Try to parse and re-serialize to ensure valid JSON
            var parsed = JsonSerializer.Deserialize<Dictionary<string, object>>(args);
            return JsonSerializer.Serialize(parsed);
        }
        catch
        {
            // If parsing fails, wrap as string value
            return JsonSerializer.Serialize(new { value = args });
        }
    }
}
