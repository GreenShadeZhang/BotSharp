namespace BotSharp.Plugin.AgUi.Models;

/// <summary>
/// AG-UI protocol event types (SCREAMING_SNAKE_CASE format as per AG UI protocol standard)
/// </summary>
public static class AgUiEventType
{
    // Text message events
    public const string TextMessageStart = "TEXT_MESSAGE_START";
    public const string TextMessageContent = "TEXT_MESSAGE_CONTENT";
    public const string TextMessageEnd = "TEXT_MESSAGE_END";
    public const string TextMessageChunk = "TEXT_MESSAGE_CHUNK";
    
    // Tool call events
    public const string ToolCallStart = "TOOL_CALL_START";
    public const string ToolCallArgs = "TOOL_CALL_ARGS";
    public const string ToolCallEnd = "TOOL_CALL_END";
    public const string ToolCallChunk = "TOOL_CALL_CHUNK";
    public const string ToolCallResult = "TOOL_CALL_RESULT";
    
    // State management events
    public const string StateSnapshot = "STATE_SNAPSHOT";
    public const string StateDelta = "STATE_DELTA";
    public const string MessagesSnapshot = "MESSAGES_SNAPSHOT";
    
    // Special events
    public const string Raw = "RAW";
    public const string Custom = "CUSTOM";
    
    // Lifecycle events
    public const string RunStarted = "RUN_STARTED";
    public const string RunFinished = "RUN_FINISHED";
    public const string RunError = "RUN_ERROR";
    public const string StepStarted = "STEP_STARTED";
    public const string StepFinished = "STEP_FINISHED";
    
    // Error event
    public const string Error = "ERROR";
}
