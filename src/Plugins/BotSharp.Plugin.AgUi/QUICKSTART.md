# AG-UI Plugin Quick Start Guide

Get started with the BotSharp AG-UI plugin in 5 minutes.

## What is AG-UI?

AG-UI is an open protocol that standardizes how AI agents communicate with user interfaces. Think of it as a common language between your BotSharp agents and frontend applications.

## Quick Setup

### 1. Prerequisites

- BotSharp running
- An agent configured (e.g., `01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a`)
- Authentication token

### 2. Test the Endpoint

```bash
curl -X POST http://localhost:5000/ag-ui/chat \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -N \
  -d '{
    "agent_id": "01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a",
    "messages": [
      {"role": "user", "content": "Hello!"}
    ]
  }'
```

**Expected output:**
```
data: {"type":"text_message_start","role":"assistant","message_id":"msg-123"}

data: {"type":"text_message_content","message_id":"msg-123","delta":"Hello!"}

data: {"type":"text_message_end","message_id":"msg-123"}
```

✅ **Success!** If you see this, your AG-UI plugin is working.

## Frontend Integration

### Option A: Using CopilotKit (Easiest)

```bash
npm install @copilotkit/react-core @copilotkit/react-ui
```

```tsx
import { CopilotKit } from "@copilotkit/react-core";
import { CopilotSidebar } from "@copilotkit/react-ui";

function App() {
  return (
    <CopilotKit
      runtimeUrl="http://localhost:5000/ag-ui/chat"
      agent="01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a"
      headers={{ Authorization: "Bearer YOUR_TOKEN" }}
    >
      <CopilotSidebar>
        <YourApp />
      </CopilotSidebar>
    </CopilotKit>
  );
}
```

### Option B: Custom Implementation

```typescript
async function* streamChat(message: string) {
  const response = await fetch('http://localhost:5000/ag-ui/chat', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer YOUR_TOKEN'
    },
    body: JSON.stringify({
      agent_id: 'YOUR_AGENT_ID',
      messages: [{ role: 'user', content: message }]
    })
  });

  const reader = response.body!.getReader();
  const decoder = new TextDecoder();
  let buffer = '';

  while (true) {
    const { done, value } = await reader.read();
    if (done) break;

    buffer += decoder.decode(value, { stream: true });
    const lines = buffer.split('\n');
    buffer = lines.pop() || '';

    for (const line of lines) {
      if (line.startsWith('data: ')) {
        yield JSON.parse(line.slice(6));
      }
    }
  }
}

// Usage
for await (const event of streamChat('Hello!')) {
  if (event.type === 'text_message_content') {
    console.log(event.delta); // Print each chunk
  }
}
```

## Common Use Cases

### 1. Simple Chat

```json
{
  "agent_id": "YOUR_AGENT_ID",
  "messages": [
    {"role": "user", "content": "What's the weather?"}
  ]
}
```

### 2. Multi-turn Conversation

```json
{
  "agent_id": "YOUR_AGENT_ID",
  "conversation_id": "conv-123",
  "messages": [
    {"role": "user", "content": "My name is Alice"},
    {"role": "assistant", "content": "Nice to meet you, Alice!"},
    {"role": "user", "content": "What's my name?"}
  ]
}
```

### 3. With Context

```json
{
  "agent_id": "YOUR_AGENT_ID",
  "messages": [
    {"role": "user", "content": "Summarize this"}
  ],
  "context": [
    {
      "name": "document",
      "value": "Long document content here..."
    }
  ]
}
```

### 4. With State

```json
{
  "agent_id": "YOUR_AGENT_ID",
  "conversation_id": "conv-123",
  "messages": [
    {"role": "user", "content": "What's my preference?"}
  ],
  "state": {
    "user_preference": "dark_mode",
    "language": "en"
  }
}
```

## Event Types

You'll receive these events via Server-Sent Events:

| Event Type | Description | Example |
|------------|-------------|---------|
| `text_message_start` | Assistant starts responding | Message begins |
| `text_message_content` | Streaming text chunk | "Hello", " world", "!" |
| `text_message_end` | Assistant finishes message | Message complete |
| `tool_call_start` | Agent calls a function | Function execution begins |
| `tool_call_args` | Function arguments (streaming) | `{"location": "NYC"}` |
| `tool_call_end` | Function call complete | Ready for result |
| `state_snapshot` | Current conversation state | Latest state |
| `error` | Error occurred | Error details |

## Troubleshooting

### "401 Unauthorized"
- ✅ Check your authorization token
- ✅ Ensure authentication is configured in BotSharp

### "No events received"
- ✅ Use `-N` flag with curl (no buffering)
- ✅ Set proper headers in your client
- ✅ Use HTTP/1.1 (not HTTP/2)

### "Agent not responding"
- ✅ Verify agent ID is correct
- ✅ Check agent is properly configured
- ✅ Review BotSharp logs

### CORS errors in browser
- ✅ Configure CORS in BotSharp
- ✅ Add your frontend origin to allowed origins

## Next Steps

📖 **Learn More:**
- [README.md](./README.md) - Full protocol documentation
- [EXAMPLES.md](./EXAMPLES.md) - Detailed code examples
- [INTEGRATION.md](./INTEGRATION.md) - Complete integration guide

🎮 **Try Examples:**
- [CopilotKit Demo](https://github.com/CopilotKit/CopilotKit/tree/main/examples)
- [AG-UI Dojo](https://dojo.ag-ui.com/)

🔗 **External Resources:**
- [AG-UI Protocol](https://ag-ui.com)
- [CopilotKit Docs](https://docs.copilotkit.ai)

## Need Help?

- 📝 Open an issue on [BotSharp GitHub](https://github.com/GreenShadeZhang/BotSharp)
- 💬 Join the Discord community
- 📚 Check the documentation

---

**That's it!** You're now ready to build AG-UI powered applications with BotSharp. 🚀
