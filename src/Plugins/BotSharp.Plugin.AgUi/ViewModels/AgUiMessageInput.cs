using System.Collections.Generic;
using System.Text.Json.Serialization;
using BotSharp.Abstraction.Conversations.Models;

namespace BotSharp.Plugin.AgUi.ViewModels;

/// <summary>
/// AG-UI message input
/// </summary>
public class AgUiMessageInput
{
    [JsonPropertyName("agent_id")]
    public string AgentId { get; set; } = string.Empty;
    
    [JsonPropertyName("conversation_id")]
    public string? ConversationId { get; set; }
    
    [JsonPropertyName("threadId")]
    public string? ThreadId { get; set; }
    
    [JsonPropertyName("runId")]
    public string? RunId { get; set; }
    
    [JsonPropertyName("messages")]
    public List<AgUiMessage> Messages { get; set; } = new();
    
    [JsonPropertyName("state")]
    public Dictionary<string, object>? State { get; set; }
    
    [JsonPropertyName("config")]
    public Dictionary<string, object>? Config { get; set; }
    
    [JsonPropertyName("tools")]
    public List<AgUiTool>? Tools { get; set; }
    
    [JsonPropertyName("context")]
    public List<AgUiContext>? Context { get; set; }
}

/// <summary>
/// AG-UI message
/// </summary>
public class AgUiMessage
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("role")]
    public string Role { get; set; } = "user";
    
    [JsonPropertyName("content")]
    public string? Content { get; set; }
    
    [JsonPropertyName("tool_calls")]
    public List<AgUiToolCall>? ToolCalls { get; set; }
    
    [JsonPropertyName("tool_call_id")]
    public string? ToolCallId { get; set; }
}

/// <summary>
/// AG-UI tool call
/// </summary>
public class AgUiToolCall
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "function";
    
    [JsonPropertyName("function")]
    public AgUiFunction Function { get; set; } = new();
}

/// <summary>
/// AG-UI function
/// </summary>
public class AgUiFunction
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("arguments")]
    public string Arguments { get; set; } = string.Empty;
}

/// <summary>
/// AG-UI tool definition
/// </summary>
public class AgUiTool
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("parameters")]
    public Dictionary<string, object>? Parameters { get; set; }
}

/// <summary>
/// AG-UI context
/// </summary>
public class AgUiContext
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("value")]
    public object? Value { get; set; }
}
