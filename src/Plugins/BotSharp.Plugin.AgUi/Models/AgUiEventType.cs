namespace BotSharp.Plugin.AgUi.Models;

/// <summary>
/// AG-UI protocol event types
/// </summary>
public static class AgUiEventType
{
    public const string TextMessageStart = "text_message_start";
    public const string TextMessageContent = "text_message_content";
    public const string TextMessageEnd = "text_message_end";
    
    public const string ToolCallStart = "tool_call_start";
    public const string ToolCallArgs = "tool_call_args";
    public const string ToolCallEnd = "tool_call_end";
    
    public const string ToolCallResult = "tool_call_result";
    
    public const string StateSnapshot = "state_snapshot";
    
    public const string Custom = "custom";
    
    public const string Error = "error";
}
