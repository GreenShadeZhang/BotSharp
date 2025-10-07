# BotSharp AG-UI Plugin

This plugin implements the [AG-UI (Agent-User Interaction) protocol](https://github.com/ag-ui-protocol/ag-ui) for BotSharp, enabling standardized event-based communication between AI agents and user-facing applications.

## What is AG-UI?

AG-UI is an open, lightweight, event-based protocol that standardizes how AI agents connect to user-facing applications. Built for simplicity and flexibility, it enables seamless integration between AI agents, real-time user context, and user interfaces.

## Features

- ✅ **Real-time streaming** via Server-Sent Events (SSE)
- ✅ **Text message events** - Stream assistant responses
- ✅ **Tool call events** - Stream function/tool invocations
- ✅ **State synchronization** - Bi-directional state management
- ✅ **Error handling** - Structured error events
- ✅ **Integration with BotSharp** - Uses existing conversation and routing services

## AG-UI Event Types

The plugin implements the following AG-UI protocol events:

### Text Messages
- `text_message_start` - Start of an assistant message
- `text_message_content` - Streaming message content (delta)
- `text_message_end` - End of message

### Tool Calls
- `tool_call_start` - Start of a tool/function call
- `tool_call_args` - Streaming tool arguments (delta)
- `tool_call_end` - End of tool call

### State & Other
- `state_snapshot` - Current conversation state
- `custom` - Custom events
- `error` - Error messages

## API Endpoint

### POST `/ag-ui/chat`

Send a message to an agent and receive streaming responses.

**Request Body:**
```json
{
  "agent_id": "01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a",
  "conversation_id": "optional-conversation-id",
  "messages": [
    {
      "id": "msg-1",
      "role": "user",
      "content": "Hello, how can you help me?"
    }
  ],
  "state": {
    "key": "value"
  },
  "config": {
    "temperature": 0.7
  }
}
```

**Response:** Server-Sent Events stream

Example events:
```
data: {"type":"text_message_start","role":"assistant","message_id":"msg-2"}

data: {"type":"text_message_content","message_id":"msg-2","delta":"Hello! I"}

data: {"type":"text_message_content","message_id":"msg-2","delta":" can help"}

data: {"type":"text_message_end","message_id":"msg-2"}

data: {"type":"state_snapshot","snapshot":{"channel":"ag-ui"}}
```

## Integration with Frontend

This plugin is compatible with AG-UI frontend frameworks such as:

- [CopilotKit](https://github.com/CopilotKit/CopilotKit) - React components for AG-UI
- Any custom AG-UI-compatible frontend

### Example with CopilotKit

```tsx
import { CopilotKit } from "@copilotkit/react-core";

function App() {
  return (
    <CopilotKit
      runtimeUrl="http://your-botsharp-server/ag-ui/chat"
      agent="01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a"
    >
      {/* Your app components */}
    </CopilotKit>
  );
}
```

## Architecture

The plugin integrates with BotSharp's core services:

```
AG-UI Client (Frontend)
        ↓
    SSE Stream
        ↓
AgUiController
        ↓
IConversationService ← Manages conversation state
        ↓
IRoutingService ← Routes to appropriate agent
        ↓
Agent Execution
        ↓
Stream Events Back (SSE)
```

## Event Flow

1. Client sends POST request with messages and state
2. Plugin converts AG-UI messages to BotSharp format
3. Message sent to agent via ConversationService
4. Agent responses streamed back as AG-UI events
5. State synchronized bidirectionally

## Message Conversion

### User Messages
- AG-UI message → BotSharp `RoleDialogModel` with role "User"

### Assistant Messages
- BotSharp assistant response → AG-UI text message events (start/content/end)

### Tool Calls
- BotSharp function execution → AG-UI tool call events (start/args/end)

## State Management

The plugin integrates with BotSharp's state management:

- Input state from AG-UI → BotSharp conversation states
- BotSharp conversation states → AG-UI state snapshot events
- Automatic state synchronization throughout conversation

## Testing

You can test the plugin using:

1. **CopilotKit UI** - Use any CopilotKit example app
2. **curl** - Send requests manually:

```bash
curl -N -X POST http://localhost:5000/ag-ui/chat \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "agent_id": "01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a",
    "messages": [{"role": "user", "content": "Hello"}]
  }'
```

3. **EventSource** - JavaScript:

```javascript
const eventSource = new EventSource('/ag-ui/chat');
eventSource.onmessage = (event) => {
  const data = JSON.parse(event.data);
  console.log('AG-UI Event:', data);
};
```

## References

- [AG-UI Protocol](https://github.com/ag-ui-protocol/ag-ui)
- [AG-UI Documentation](https://ag-ui.com)
- [CopilotKit](https://github.com/CopilotKit/CopilotKit)
- [.NET SDK PR](https://github.com/ag-ui-protocol/ag-ui/pull/38)

## License

This plugin follows the same license as BotSharp.
