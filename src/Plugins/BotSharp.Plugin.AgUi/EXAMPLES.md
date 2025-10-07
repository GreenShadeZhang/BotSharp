# AG-UI Plugin Usage Examples

This document provides practical examples of using the BotSharp AG-UI plugin.

## Table of Contents

- [Basic Chat Request](#basic-chat-request)
- [Chat with State Management](#chat-with-state-management)
- [Chat with Context](#chat-with-context)
- [Tool/Function Calls](#toolfunction-calls)
- [Using with CopilotKit Frontend](#using-with-copilotkit-frontend)
- [cURL Examples](#curl-examples)
- [JavaScript/TypeScript Examples](#javascripttypescript-examples)

## Basic Chat Request

The simplest AG-UI request with just a user message:

```bash
curl -X POST http://localhost:5000/ag-ui/chat \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -N \
  -d '{
    "agent_id": "01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a",
    "messages": [
      {
        "role": "user",
        "content": "Hello, what can you help me with?"
      }
    ]
  }'
```

**Expected Response** (Server-Sent Events):

```
data: {"type":"text_message_start","role":"assistant","message_id":"msg-123"}

data: {"type":"text_message_content","message_id":"msg-123","delta":"Hello! I"}

data: {"type":"text_message_content","message_id":"msg-123","delta":" can help you"}

data: {"type":"text_message_end","message_id":"msg-123"}

data: {"type":"state_snapshot","snapshot":{"channel":"ag-ui"}}
```

## Chat with State Management

Maintain conversation state across requests:

```json
{
  "agent_id": "01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a",
  "conversation_id": "conv-456",
  "messages": [
    {
      "role": "user",
      "content": "What's my name?"
    }
  ],
  "state": {
    "user_name": "Alice",
    "user_preferences": {
      "language": "en",
      "theme": "dark"
    },
    "session_count": 5
  }
}
```

The state will be:
- Converted to BotSharp conversation states
- Available throughout the conversation
- Returned in state snapshot events

## Chat with Context

Provide context information to the agent:

```json
{
  "agent_id": "01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a",
  "messages": [
    {
      "role": "user",
      "content": "Summarize this document"
    }
  ],
  "context": [
    {
      "name": "document",
      "description": "Current document content",
      "value": "Lorem ipsum dolor sit amet..."
    },
    {
      "name": "user_location",
      "description": "User's current location",
      "value": {
        "city": "New York",
        "country": "USA"
      }
    }
  ]
}
```

Context items are stored as `context_{name}` in conversation state.

## Tool/Function Calls

Define tools that the agent can use:

```json
{
  "agent_id": "01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a",
  "messages": [
    {
      "role": "user",
      "content": "What's the weather in New York?"
    }
  ],
  "tools": [
    {
      "name": "get_weather",
      "description": "Get current weather for a location",
      "parameters": {
        "type": "object",
        "properties": {
          "location": {
            "type": "string",
            "description": "City name"
          },
          "unit": {
            "type": "string",
            "enum": ["celsius", "fahrenheit"]
          }
        },
        "required": ["location"]
      }
    }
  ]
}
```

**Tool Call Events Response:**

```
data: {"type":"tool_call_start","tool_call_id":"call-789","tool_call_name":"get_weather","parent_message_id":"msg-456"}

data: {"type":"tool_call_args","tool_call_id":"call-789","delta":"{\"location\":\"New York\",\"unit\":\"fahrenheit\"}"}

data: {"type":"tool_call_end","tool_call_id":"call-789"}
```

## Using with CopilotKit Frontend

### React Example

```tsx
import { CopilotKit } from "@copilotkit/react-core";
import { CopilotSidebar } from "@copilotkit/react-ui";

function App() {
  return (
    <CopilotKit
      // Point to your BotSharp AG-UI endpoint
      runtimeUrl="http://localhost:5000/ag-ui/chat"
      // Specify the agent ID
      agent="01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a"
      // Optional: provide initial state
      initialState={{
        user_name: "Alice",
        preferences: { theme: "dark" }
      }}
    >
      <CopilotSidebar>
        <YourAppComponents />
      </CopilotSidebar>
    </CopilotKit>
  );
}
```

### With Custom Context

```tsx
import { useCopilotContext } from "@copilotkit/react-core";

function DocumentEditor() {
  const [document, setDocument] = useState("Hello world");
  
  // Provide context to the agent
  useCopilotContext({
    name: "current_document",
    description: "The document being edited",
    value: document
  });

  return <textarea value={document} onChange={e => setDocument(e.target.value)} />;
}
```

### With Custom Tools

```tsx
import { useCopilotAction } from "@copilotkit/react-core";

function MyComponent() {
  useCopilotAction({
    name: "save_document",
    description: "Save the current document",
    parameters: [
      {
        name: "filename",
        type: "string",
        description: "Name of the file to save",
        required: true
      }
    ],
    handler: async ({ filename }) => {
      await saveToServer(filename);
      return "Document saved successfully";
    }
  });

  return <div>Your component</div>;
}
```

## cURL Examples

### Multi-turn Conversation

```bash
# First message
curl -X POST http://localhost:5000/ag-ui/chat \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -N \
  -d '{
    "agent_id": "01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a",
    "conversation_id": "conv-123",
    "messages": [
      {
        "role": "user",
        "content": "My name is Alice"
      }
    ]
  }'

# Second message (same conversation)
curl -X POST http://localhost:5000/ag-ui/chat \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -N \
  -d '{
    "agent_id": "01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a",
    "conversation_id": "conv-123",
    "messages": [
      {
        "role": "user",
        "content": "What is my name?"
      }
    ]
  }'
```

### With Config Parameters

```bash
curl -X POST http://localhost:5000/ag-ui/chat \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -N \
  -d '{
    "agent_id": "01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a",
    "messages": [
      {
        "role": "user",
        "content": "Tell me a story"
      }
    ],
    "config": {
      "temperature": 0.9,
      "max_tokens": 500,
      "stream": true
    }
  }'
```

## JavaScript/TypeScript Examples

### Using EventSource

```javascript
async function sendAGUIMessage(agentId, message, state = {}) {
  const response = await fetch('http://localhost:5000/ag-ui/chat', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer YOUR_TOKEN'
    },
    body: JSON.stringify({
      agent_id: agentId,
      messages: [
        { role: 'user', content: message }
      ],
      state: state
    })
  });

  const reader = response.body.getReader();
  const decoder = new TextDecoder();

  while (true) {
    const { done, value } = await reader.read();
    if (done) break;

    const chunk = decoder.decode(value);
    const lines = chunk.split('\n');

    for (const line of lines) {
      if (line.startsWith('data: ')) {
        const data = JSON.parse(line.slice(6));
        handleAGUIEvent(data);
      }
    }
  }
}

function handleAGUIEvent(event) {
  switch (event.type) {
    case 'text_message_start':
      console.log('Assistant started responding:', event.message_id);
      break;
    case 'text_message_content':
      console.log('Content chunk:', event.delta);
      break;
    case 'text_message_end':
      console.log('Assistant finished responding');
      break;
    case 'tool_call_start':
      console.log('Tool called:', event.tool_call_name);
      break;
    case 'state_snapshot':
      console.log('State updated:', event.snapshot);
      break;
    case 'error':
      console.error('Error:', event.message);
      break;
  }
}
```

### TypeScript with Proper Types

```typescript
interface AGUIMessage {
  id?: string;
  role: 'user' | 'assistant' | 'tool';
  content?: string;
  tool_calls?: Array<{
    id: string;
    type: 'function';
    function: {
      name: string;
      arguments: string;
    };
  }>;
  tool_call_id?: string;
}

interface AGUIRequest {
  agent_id: string;
  conversation_id?: string;
  messages: AGUIMessage[];
  state?: Record<string, any>;
  config?: Record<string, any>;
  tools?: Array<{
    name: string;
    description?: string;
    parameters?: Record<string, any>;
  }>;
  context?: Array<{
    name: string;
    description?: string;
    value: any;
  }>;
}

interface AGUIEvent {
  type: string;
  [key: string]: any;
}

async function* streamAGUIChat(
  request: AGUIRequest
): AsyncGenerator<AGUIEvent> {
  const response = await fetch('http://localhost:5000/ag-ui/chat', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer YOUR_TOKEN'
    },
    body: JSON.stringify(request)
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
        const event = JSON.parse(line.slice(6)) as AGUIEvent;
        yield event;
      }
    }
  }
}

// Usage
async function example() {
  for await (const event of streamAGUIChat({
    agent_id: '01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a',
    messages: [
      { role: 'user', content: 'Hello!' }
    ]
  })) {
    console.log('Received event:', event);
  }
}
```

## Error Handling

Handle errors gracefully:

```javascript
try {
  for await (const event of streamAGUIChat(request)) {
    if (event.type === 'error') {
      console.error('Agent error:', event.message);
      // Handle error in UI
      showErrorMessage(event.message);
      break;
    }
    
    // Handle normal events
    handleEvent(event);
  }
} catch (error) {
  console.error('Connection error:', error);
  // Handle network errors
  showConnectionError();
}
```

## Best Practices

1. **Always use conversation_id** for multi-turn conversations
2. **Include relevant context** to give agents better information
3. **Handle all event types** including errors
4. **Buffer partial state** until you receive complete events
5. **Implement timeout handling** for long-running requests
6. **Store and reuse state** across conversation turns
7. **Validate tool parameters** before sending requests

## Troubleshooting

### Events Not Streaming

Make sure your client:
- Uses HTTP/1.1 (not HTTP/2)
- Doesn't buffer the response
- Has proper headers set
- Handles SSE format correctly

### State Not Persisting

Ensure you:
- Use the same `conversation_id` across requests
- Include state in subsequent requests
- Check state_snapshot events for confirmation

### Tool Calls Not Working

Verify that:
- Tool definitions have proper JSON schema
- Tool names match exactly
- Parameters are properly serialized
- Your agent supports tool/function calling

## Additional Resources

- [AG-UI Protocol Documentation](https://ag-ui.com)
- [CopilotKit Documentation](https://docs.copilotkit.ai)
- [BotSharp Documentation](https://github.com/GreenShadeZhang/BotSharp)
