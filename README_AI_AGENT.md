# AI Agent with Gemini AI Integration

This project now includes an AI agent powered by Google's Gemini AI that can answer user questions. The AI agent is designed to be flexible and can handle both authenticated and public requests.

## Features

- 🤖 **Gemini AI Integration**: Powered by Google's Gemini 1.5 Flash model
- 🔐 **Authentication Support**: Both authenticated and public endpoints
- 📝 **Context Support**: Optional context can be provided for better answers
- ⚙️ **Configurable**: Model parameters can be adjusted via configuration
- 🧪 **Test Interface**: Built-in HTML test page for easy testing

## Setup

### 1. Get Gemini API Key

1. Go to [Google AI Studio](https://makersuite.google.com/app/apikey)
2. Create a new API key
3. Copy the API key

### 2. Configure the API Key

Update your `appsettings.json` file:

```json
{
  "Gemini": {
    "ApiKey": "YOUR_ACTUAL_GEMINI_API_KEY_HERE",
    "ModelName": "gemini-1.5-flash",
    "MaxTokens": 1000,
    "Temperature": 0.7
  }
}
```

### 3. Install Dependencies

The project automatically includes the required NuGet package:
- `Google.AI.GenerativeAI` (v0.3.0)

## API Endpoints

### 1. Ask Question (Authenticated)
```
POST /ai/ask
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "question": "What is the capital of France?",
  "context": "Optional context for better answers",
  "requireAnswer": true
}
```

### 2. Ask Question (Public)
```
POST /ai/ask-public
Content-Type: application/json

{
  "question": "What is the capital of France?",
  "context": "Optional context for better answers",
  "requireAnswer": true
}
```

### 3. Health Check
```
GET /ai/health
```

## Request Models

### AiRequest
```csharp
{
  "question": "string",        // Required: The question to ask
  "context": "string",         // Optional: Additional context
  "userId": "string",          // Auto-filled for authenticated requests
  "requireAnswer": boolean     // Whether to require a definitive answer
}
```

### AiResponse
```csharp
{
  "answer": "string",          // The AI's response
  "success": boolean,          // Whether the request was successful
  "errorMessage": "string",    // Error message if failed
  "timestamp": "datetime",     // When the response was generated
  "modelUsed": "string"        // Which Gemini model was used
}
```

## Usage Examples

### Basic Question
```json
{
  "question": "What is the capital of France?"
}
```

### Question with Context
```json
{
  "question": "What should I do next?",
  "context": "I'm learning C# and just finished a basic console application. I want to build a web API next."
}
```

### Question that Doesn't Require Answer
```json
{
  "question": "What are the latest developments in quantum computing?",
  "requireAnswer": false
}
```

## Testing

### Using the Test Interface

1. Open `TestUI/ai-test.html` in your browser
2. Enter your JWT token (if testing authenticated endpoints)
3. Type your question
4. Optionally add context
5. Choose whether to require an answer
6. Click "Ask" to test

### Using curl

```bash
# Public request
curl -X POST https://localhost:7001/ai/ask-public \
  -H "Content-Type: application/json" \
  -d '{"question": "What is the capital of France?"}'

# Authenticated request
curl -X POST https://localhost:7001/ai/ask \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"question": "What is the capital of France?"}'
```

## Configuration Options

### GeminiSettings
- **ApiKey**: Your Gemini API key (required)
- **ModelName**: Gemini model to use (default: "gemini-1.5-flash")
- **MaxTokens**: Maximum tokens in response (default: 1000)
- **Temperature**: Creativity level 0.0-1.0 (default: 0.7)

## Error Handling

The AI service handles various error scenarios:

- **Missing API Key**: Returns configuration error
- **Empty Question**: Returns validation error
- **API Errors**: Returns detailed error messages
- **Network Issues**: Returns network error messages

## Security Considerations

1. **API Key Security**: Never commit your API key to version control
2. **Rate Limiting**: Consider implementing rate limiting for production use
3. **Input Validation**: All inputs are validated before processing
4. **Authentication**: Use JWT tokens for authenticated requests

## Troubleshooting

### Common Issues

1. **"Gemini API key is not configured"**
   - Make sure you've added your API key to `appsettings.json`

2. **"Question cannot be empty"**
   - Ensure you're sending a question in the request body

3. **Authentication errors**
   - Verify your JWT token is valid and not expired

4. **Network errors**
   - Check your internet connection
   - Verify the API endpoint is accessible

### Getting Help

If you encounter issues:
1. Check the application logs for detailed error messages
2. Verify your Gemini API key is valid
3. Test the health endpoint: `GET /ai/health`
4. Ensure all required dependencies are installed

## Future Enhancements

Potential improvements for the AI agent:
- Conversation history support
- Multiple model selection
- Response streaming
- Custom prompt templates
- Usage analytics
- Rate limiting
- Response caching
