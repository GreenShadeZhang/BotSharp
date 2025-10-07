using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BotSharp.Abstraction.Agents.Enums;
using BotSharp.Abstraction.Conversations;
using BotSharp.Abstraction.Conversations.Models;
using BotSharp.Abstraction.Models;
using BotSharp.Abstraction.Routing;
using BotSharp.Plugin.AgUi.Models;
using BotSharp.Plugin.AgUi.Utilities;
using BotSharp.Plugin.AgUi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace BotSharp.Plugin.AgUi.Controllers;

[Authorize]
[ApiController]
public class AgUiController : ControllerBase
{
    private readonly IServiceProvider _services;
    private readonly ILogger<AgUiController> _logger;

    public AgUiController(ILogger<AgUiController> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    /// <summary>
    /// AG-UI streaming endpoint - sends agent responses as Server-Sent Events
    /// </summary>
    [HttpPost("/ag-ui/chat")]
    public async Task Chat([FromBody] AgUiMessageInput input)
    {
        Response.StatusCode = 200;
        Response.Headers[HeaderNames.ContentType] = "text/event-stream";
        Response.Headers[HeaderNames.CacheControl] = "no-cache";
        Response.Headers[HeaderNames.Connection] = "keep-alive";
        var outputStream = Response.Body;

        try
        {
            // Get the last user message
            var userMessage = input.Messages
                .Where(x => x.Role == "user")
                .LastOrDefault();

            if (userMessage == null || string.IsNullOrEmpty(userMessage.Content))
            {
                await SendErrorEvent(outputStream, "No user message found");
                return;
            }

            // Create BotSharp message
            var message = new RoleDialogModel(AgentRole.User, userMessage.Content)
            {
                MessageId = userMessage.Id ?? Guid.NewGuid().ToString()
            };

            // Get conversation service
            var conv = _services.GetRequiredService<IConversationService>();
            var routing = _services.GetRequiredService<IRoutingService>();

            // Set conversation ID
            var conversationId = input.ConversationId ?? Guid.NewGuid().ToString();
            routing.Context.SetMessageId(conversationId, message.MessageId);

            // Set conversation state
            var states = new List<MessageState>();
            
            // Add AG-UI state
            if (input.State != null)
            {
                var convertedState = AgUiMessageConverter.ConvertStateToConversationState(input.State);
                foreach (var kvp in convertedState)
                {
                    states.Add(new MessageState
                    {
                        Key = kvp.Key,
                        Value = kvp.Value
                    });
                }
            }
            
            // Add AG-UI context
            if (input.Context != null && input.Context.Any())
            {
                var contextState = AgUiMessageConverter.ConvertContextToState(input.Context);
                foreach (var kvp in contextState)
                {
                    states.Add(new MessageState
                    {
                        Key = kvp.Key,
                        Value = kvp.Value
                    });
                }
            }
            
            // Add AG-UI config
            if (input.Config != null)
            {
                foreach (var kvp in input.Config)
                {
                    states.Add(new MessageState
                    {
                        Key = $"config_{kvp.Key}",
                        Value = kvp.Value?.ToString() ?? string.Empty
                    });
                }
            }

            conv.SetConversationId(conversationId, states);
            conv.States.SetState("channel", "ag-ui");
            
            // Store tools information if provided
            if (input.Tools != null && input.Tools.Any())
            {
                var toolsJson = JsonSerializer.Serialize(input.Tools);
                conv.States.SetState("ag_ui_tools", toolsJson);
            }

            // Send state snapshot if state exists
            if (input.State != null && input.State.Any())
            {
                await SendStateSnapshotEvent(outputStream, input.State);
            }

            // Send message to agent and stream responses
            await conv.SendMessage(
                input.AgentId,
                message,
                replyMessage: null,
                async msg => await OnChunkReceived(outputStream, msg)
            );

            // Send final state snapshot
            var finalState = new Dictionary<string, object>();
            foreach (var state in conv.States.GetStates())
            {
                finalState[state.Key] = state.Value ?? string.Empty;
            }
            await SendStateSnapshotEvent(outputStream, finalState);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing AG-UI chat request");
            await SendErrorEvent(outputStream, ex.Message);
        }
        finally
        {
            await outputStream.FlushAsync();
        }
    }

    private async Task OnChunkReceived(Stream outputStream, RoleDialogModel message)
    {
        if (message.Role == AgentRole.Assistant && !string.IsNullOrEmpty(message.Content))
        {
            // Send text message events
            await SendTextMessageEvents(outputStream, message.MessageId, message.Content);
        }
        else if (message.Role == AgentRole.Function)
        {
            if (!string.IsNullOrEmpty(message.FunctionName))
            {
                // Send tool call events
                var args = AgUiMessageConverter.SafeSerializeArguments(message.FunctionArgs);
                await SendToolCallEvents(
                    outputStream,
                    message.ToolCallId ?? message.MessageId,
                    message.FunctionName,
                    args,
                    message.MessageId
                );
            }
            else if (!string.IsNullOrEmpty(message.Content))
            {
                // Send tool result event
                await SendToolResultEvent(
                    outputStream,
                    message.ToolCallId ?? message.MessageId,
                    message.Content
                );
            }
        }
    }

    private async Task SendTextMessageEvents(Stream outputStream, string messageId, string content)
    {
        // Start event
        var startEvent = new TextMessageStartEvent
        {
            MessageId = messageId,
            Role = "assistant"
        };
        await SendEvent(outputStream, startEvent);

        // Content event
        var contentEvent = new TextMessageContentEvent
        {
            MessageId = messageId,
            Delta = content
        };
        await SendEvent(outputStream, contentEvent);

        // End event
        var endEvent = new TextMessageEndEvent
        {
            MessageId = messageId
        };
        await SendEvent(outputStream, endEvent);
    }

    private async Task SendToolCallEvents(Stream outputStream, string toolCallId, string toolName, string args, string? parentMessageId)
    {
        // Start event
        var startEvent = new ToolCallStartEvent
        {
            ToolCallId = toolCallId,
            ToolCallName = toolName,
            ParentMessageId = parentMessageId
        };
        await SendEvent(outputStream, startEvent);

        // Args event
        var argsEvent = new ToolCallArgsEvent
        {
            ToolCallId = toolCallId,
            Delta = args
        };
        await SendEvent(outputStream, argsEvent);

        // End event
        var endEvent = new ToolCallEndEvent
        {
            ToolCallId = toolCallId
        };
        await SendEvent(outputStream, endEvent);
    }

    private async Task SendToolResultEvent(Stream outputStream, string toolCallId, string result)
    {
        var resultEvent = new ToolCallResultEvent
        {
            ToolCallId = toolCallId,
            Result = result
        };
        await SendEvent(outputStream, resultEvent);
    }

    private async Task SendStateSnapshotEvent(Stream outputStream, Dictionary<string, object> state)
    {
        var stateEvent = new StateSnapshotEvent
        {
            Snapshot = state
        };
        await SendEvent(outputStream, stateEvent);
    }

    private async Task SendErrorEvent(Stream outputStream, string errorMessage)
    {
        var errorEvent = new ErrorEvent
        {
            Error = "error",
            Message = errorMessage
        };
        await SendEvent(outputStream, errorEvent);
    }

    private async Task SendEvent(Stream outputStream, AgUiEvent eventData)
    {
        var json = JsonSerializer.Serialize(eventData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        var buffer = Encoding.UTF8.GetBytes($"data: {json}\n\n");
        await outputStream.WriteAsync(buffer, 0, buffer.Length);
        await outputStream.FlushAsync();
        await Task.Delay(10); // Small delay for SSE stability
    }
}
