using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BotSharp.Plugin.AgUi.Models;

/// <summary>
/// Base AG-UI event
/// </summary>
public class AgUiEvent
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

/// <summary>
/// Text message start event
/// </summary>
public class TextMessageStartEvent : AgUiEvent
{
    public TextMessageStartEvent()
    {
        Type = AgUiEventType.TextMessageStart;
    }
    
    [JsonPropertyName("role")]
    public string Role { get; set; } = "assistant";
    
    [JsonPropertyName("message_id")]
    public string MessageId { get; set; } = string.Empty;
}

/// <summary>
/// Text message content event (streaming)
/// </summary>
public class TextMessageContentEvent : AgUiEvent
{
    public TextMessageContentEvent()
    {
        Type = AgUiEventType.TextMessageContent;
    }
    
    [JsonPropertyName("message_id")]
    public string MessageId { get; set; } = string.Empty;
    
    [JsonPropertyName("delta")]
    public string Delta { get; set; } = string.Empty;
}

/// <summary>
/// Text message end event
/// </summary>
public class TextMessageEndEvent : AgUiEvent
{
    public TextMessageEndEvent()
    {
        Type = AgUiEventType.TextMessageEnd;
    }
    
    [JsonPropertyName("message_id")]
    public string MessageId { get; set; } = string.Empty;
}

/// <summary>
/// Tool call start event
/// </summary>
public class ToolCallStartEvent : AgUiEvent
{
    public ToolCallStartEvent()
    {
        Type = AgUiEventType.ToolCallStart;
    }
    
    [JsonPropertyName("tool_call_id")]
    public string ToolCallId { get; set; } = string.Empty;
    
    [JsonPropertyName("tool_call_name")]
    public string ToolCallName { get; set; } = string.Empty;
    
    [JsonPropertyName("parent_message_id")]
    public string? ParentMessageId { get; set; }
}

/// <summary>
/// Tool call arguments event (streaming)
/// </summary>
public class ToolCallArgsEvent : AgUiEvent
{
    public ToolCallArgsEvent()
    {
        Type = AgUiEventType.ToolCallArgs;
    }
    
    [JsonPropertyName("tool_call_id")]
    public string ToolCallId { get; set; } = string.Empty;
    
    [JsonPropertyName("delta")]
    public string Delta { get; set; } = string.Empty;
}

/// <summary>
/// Tool call end event
/// </summary>
public class ToolCallEndEvent : AgUiEvent
{
    public ToolCallEndEvent()
    {
        Type = AgUiEventType.ToolCallEnd;
    }
    
    [JsonPropertyName("tool_call_id")]
    public string ToolCallId { get; set; } = string.Empty;
}

/// <summary>
/// Tool call result event
/// </summary>
public class ToolCallResultEvent : AgUiEvent
{
    public ToolCallResultEvent()
    {
        Type = AgUiEventType.ToolCallResult;
    }
    
    [JsonPropertyName("tool_call_id")]
    public string ToolCallId { get; set; } = string.Empty;
    
    [JsonPropertyName("result")]
    public string Result { get; set; } = string.Empty;
}

/// <summary>
/// State snapshot event
/// </summary>
public class StateSnapshotEvent : AgUiEvent
{
    public StateSnapshotEvent()
    {
        Type = AgUiEventType.StateSnapshot;
    }
    
    [JsonPropertyName("snapshot")]
    public Dictionary<string, object> Snapshot { get; set; } = new();
}

/// <summary>
/// Custom event
/// </summary>
public class CustomEvent : AgUiEvent
{
    public CustomEvent()
    {
        Type = AgUiEventType.Custom;
    }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("value")]
    public object? Value { get; set; }
}

/// <summary>
/// Error event
/// </summary>
public class ErrorEvent : AgUiEvent
{
    public ErrorEvent()
    {
        Type = AgUiEventType.Error;
    }
    
    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
