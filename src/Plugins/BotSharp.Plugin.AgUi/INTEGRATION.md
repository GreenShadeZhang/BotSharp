# AG-UI Plugin Integration Guide

This guide explains how to integrate the BotSharp AG-UI plugin into your application.

## Prerequisites

- BotSharp instance running
- Authentication configured (the plugin uses `[Authorize]` attribute)
- At least one agent configured in BotSharp

## Installation

The plugin is included in the BotSharp Plugins directory. To enable it, ensure it's referenced in your project.

### 1. Add Project Reference

In your main application project (e.g., `WebStarter.csproj`), add a reference to the AG-UI plugin:

```xml
<ItemGroup>
  <ProjectReference Include="..\Plugins\BotSharp.Plugin.AgUi\BotSharp.Plugin.AgUi.csproj" />
</ItemGroup>
```

### 2. Register the Plugin

The plugin will be automatically discovered and registered by BotSharp's plugin system.

## Configuration

No special configuration is required. The plugin uses BotSharp's existing services:

- `IConversationService` - For conversation management
- `IRoutingService` - For agent routing
- Authentication middleware - For securing endpoints

### Optional: CORS Configuration

If you're accessing the AG-UI endpoint from a different origin (e.g., a React app), configure CORS in your `Startup.cs` or `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AgUiPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Your frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

app.UseCors("AgUiPolicy");
```

## API Endpoint

Once the plugin is installed, the following endpoint will be available:

```
POST /ag-ui/chat
```

### Authentication

The endpoint requires authentication. Include a bearer token in the request:

```
Authorization: Bearer YOUR_TOKEN
```

## Frontend Integration

### Option 1: CopilotKit (Recommended)

CopilotKit provides ready-made React components that work with the AG-UI protocol:

```bash
npm install @copilotkit/react-core @copilotkit/react-ui
```

```tsx
import { CopilotKit } from "@copilotkit/react-core";
import { CopilotSidebar } from "@copilotkit/react-ui";
import "@copilotkit/react-ui/styles.css";

function App() {
  return (
    <CopilotKit
      runtimeUrl="http://localhost:5000/ag-ui/chat"
      agent="YOUR_AGENT_ID"
      headers={{
        Authorization: "Bearer YOUR_TOKEN"
      }}
    >
      <CopilotSidebar>
        {/* Your app content */}
      </CopilotSidebar>
    </CopilotKit>
  );
}
```

### Option 2: Custom Implementation

If you prefer to build your own UI, implement an SSE (Server-Sent Events) client:

```javascript
const eventSource = new EventSource(
  'http://localhost:5000/ag-ui/chat',
  {
    headers: {
      'Authorization': 'Bearer YOUR_TOKEN',
      'Content-Type': 'application/json'
    }
  }
);

eventSource.onmessage = (event) => {
  const data = JSON.parse(event.data);
  handleAGUIEvent(data);
};

eventSource.onerror = (error) => {
  console.error('SSE Error:', error);
  eventSource.close();
};
```

Note: Most browsers don't support custom headers with EventSource, so you may need to use `fetch` with a readable stream instead. See [EXAMPLES.md](./EXAMPLES.md) for complete implementations.

## Agent Configuration

Ensure your BotSharp agents are properly configured to handle AG-UI requests.

### Agent Requirements

1. **Agent ID**: Each agent must have a unique ID
2. **Functions/Tools**: Define functions that the agent can call
3. **Instructions**: Provide clear instructions for the agent

### Example Agent Configuration

```json
{
  "id": "01fcc3e5-9af7-49e6-ad7a-a760bd12dc4a",
  "name": "Customer Support Agent",
  "description": "Helps customers with their inquiries",
  "instruction": "You are a helpful customer support agent...",
  "functions": [
    {
      "name": "search_knowledge_base",
      "description": "Search the knowledge base for information",
      "parameters": {
        "type": "object",
        "properties": {
          "query": {
            "type": "string",
            "description": "Search query"
          }
        },
        "required": ["query"]
      }
    }
  ]
}
```

## Testing the Integration

### 1. Using cURL

Test the endpoint directly:

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
        "content": "Hello"
      }
    ]
  }'
```

### 2. Using Postman

1. Create a new POST request to `http://localhost:5000/ag-ui/chat`
2. Add headers:
   - `Content-Type: application/json`
   - `Authorization: Bearer YOUR_TOKEN`
3. Add body (raw JSON):
```json
{
  "agent_id": "YOUR_AGENT_ID",
  "messages": [
    {
      "role": "user",
      "content": "Hello"
    }
  ]
}
```
4. Send the request and observe the streaming response

### 3. Using the CopilotKit Demo

The fastest way to test is using a CopilotKit example:

```bash
# Clone CopilotKit
git clone https://github.com/CopilotKit/CopilotKit.git
cd CopilotKit/examples/coagents-starter

# Install dependencies
npm install

# Update the runtime URL in the code to point to your BotSharp instance
# Edit src/app/page.tsx and change runtimeUrl to:
# "http://localhost:5000/ag-ui/chat"

# Run the example
npm run dev
```

## Monitoring and Debugging

### Enable Detailed Logging

Add logging configuration in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "BotSharp.Plugin.AgUi": "Debug"
    }
  }
}
```

### Common Issues

#### 1. 401 Unauthorized

**Problem**: Missing or invalid authentication token.

**Solution**: Ensure you're including a valid bearer token in the Authorization header.

#### 2. No Events Received

**Problem**: Client not properly handling SSE format.

**Solution**: 
- Use HTTP/1.1 (not HTTP/2)
- Set proper headers: `Accept: text/event-stream`
- Don't buffer the response
- Parse `data:` lines correctly

#### 3. Agent Not Responding

**Problem**: Agent configuration or routing issue.

**Solution**:
- Verify agent ID is correct
- Check agent is properly configured in BotSharp
- Review logs for routing errors
- Ensure conversation service is working

#### 4. CORS Errors

**Problem**: Browser blocking cross-origin requests.

**Solution**: Configure CORS as shown above in the Configuration section.

## Advanced Integration

### State Management

Maintain state across conversation turns:

```typescript
const [conversationId] = useState(() => uuidv4());
const [state, setState] = useState({});

function sendMessage(message: string) {
  return fetch('/ag-ui/chat', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer YOUR_TOKEN'
    },
    body: JSON.stringify({
      agent_id: 'YOUR_AGENT_ID',
      conversation_id: conversationId,
      messages: [{ role: 'user', content: message }],
      state: state
    })
  });
}
```

### Context Awareness

Provide application context to the agent:

```typescript
function sendMessageWithContext(message: string, documentContent: string) {
  return fetch('/ag-ui/chat', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer YOUR_TOKEN'
    },
    body: JSON.stringify({
      agent_id: 'YOUR_AGENT_ID',
      messages: [{ role: 'user', content: message }],
      context: [
        {
          name: 'document',
          description: 'Current document being edited',
          value: documentContent
        }
      ]
    })
  });
}
```

### Tool Integration

Allow the agent to call frontend tools:

```typescript
const tools = [
  {
    name: 'show_modal',
    description: 'Display a modal dialog to the user',
    parameters: {
      type: 'object',
      properties: {
        title: { type: 'string' },
        content: { type: 'string' }
      },
      required: ['title', 'content']
    }
  }
];

// Send with tools
fetch('/ag-ui/chat', {
  method: 'POST',
  body: JSON.stringify({
    agent_id: 'YOUR_AGENT_ID',
    messages: [{ role: 'user', content: message }],
    tools: tools
  })
});

// Handle tool call events
if (event.type === 'tool_call_start') {
  if (event.tool_call_name === 'show_modal') {
    // Execute the tool locally
    // Parse arguments and show modal
  }
}
```

## Performance Optimization

### 1. Connection Pooling

Reuse HTTP connections for better performance:

```javascript
const keepAliveAgent = new http.Agent({
  keepAlive: true,
  keepAliveMsecs: 30000,
  maxSockets: 50
});
```

### 2. Debouncing

Debounce rapid user inputs:

```javascript
const debouncedSend = debounce((message) => {
  sendMessage(message);
}, 300);
```

### 3. Caching

Cache static context that doesn't change:

```javascript
const cachedContext = useMemo(() => ({
  name: 'user_profile',
  value: userProfile
}), [userProfile]);
```

## Security Considerations

1. **Always use HTTPS** in production
2. **Validate agent IDs** on the server side
3. **Implement rate limiting** to prevent abuse
4. **Sanitize user inputs** before sending to agents
5. **Use secure authentication** (JWT, OAuth2, etc.)
6. **Don't expose sensitive data** in state or context

## Next Steps

- Read the [EXAMPLES.md](./EXAMPLES.md) for detailed usage examples
- Review the [README.md](./README.md) for protocol documentation
- Check out [CopilotKit Documentation](https://docs.copilotkit.ai)
- Explore [AG-UI Protocol Specification](https://ag-ui.com)

## Support

For issues or questions:
- Open an issue on [BotSharp GitHub](https://github.com/GreenShadeZhang/BotSharp)
- Join the discussion on Discord
- Check existing documentation

## Contributing

Contributions are welcome! Please follow the BotSharp contribution guidelines.
