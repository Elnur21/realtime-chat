# SignalR CORS Configuration Guide

## Overview

This guide explains how to properly configure CORS (Cross-Origin Resource Sharing) for SignalR connections between your frontend and backend.

## The Problem

SignalR requires specific CORS configuration because:
1. It uses WebSockets or other transport methods that require credentials
2. It needs to handle preflight requests properly
3. It requires authentication tokens to be passed correctly

## Solution

### 1. Updated CORS Configuration

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true) // Allow any origin
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // This is required for SignalR
    });
});
```

**Key Changes:**
- Replaced `.AllowAnyOrigin()` with `.SetIsOriginAllowed(_ => true)`
- Added `.AllowCredentials()` - **This is crucial for SignalR**

### 2. JWT Authentication for SignalR

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // ... existing token validation parameters ...

        // Configure JWT for SignalR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
```

**What this does:**
- Extracts the JWT token from the query parameter `access_token`
- Applies it to SignalR hub connections
- Allows authentication for SignalR methods

### 3. Enhanced ChatHub

```csharp
[Authorize]
public class ChatHub : Hub
{
    public async Task SendMessage(string message)
    {
        var userEmail = Context.User?.Identity?.Name ?? "Anonymous";
        await Clients.All.SendAsync("ReceiveMessage", userEmail, message);
    }

    // ... other methods for groups, etc.
}
```

**Features:**
- `[Authorize]` attribute ensures only authenticated users can connect
- Uses `Context.User` to get authenticated user information
- Supports group messaging and user presence

## Frontend Configuration

### 1. SignalR Connection with Authentication

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl(`http://localhost:5192/chatHub?access_token=${authToken}`, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
    })
    .withAutomaticReconnect()
    .build();
```

**Key Points:**
- Pass token as `access_token` query parameter
- Use `skipNegotiation: true` for WebSocket-only transport
- Enable automatic reconnection

### 2. Event Handlers

```javascript
connection.on("ReceiveMessage", (user, message) => {
    console.log(`${user}: ${message}`);
});

connection.on("UserConnected", (user) => {
    console.log(`${user} connected`);
});

connection.on("UserDisconnected", (user) => {
    console.log(`${user} disconnected`);
});
```

## Testing

### 1. Use the Test HTML File

Open `signalr-test.html` in your browser to test:
- Authentication
- SignalR connection
- Message sending/receiving
- Group functionality

### 2. Manual Testing Steps

1. **Start the API**: `dotnet run`
2. **Open the test page**: Navigate to `signalr-test.html`
3. **Login**: Use credentials (e.g., `admin@test.com` / `123456`)
4. **Connect to SignalR**: Click "Connect to SignalR"
5. **Send messages**: Test global and group messaging

## Common Issues and Solutions

### Issue 1: CORS Error - "No 'Access-Control-Allow-Origin' header"
**Solution**: Ensure `.AllowCredentials()` is added to CORS policy

### Issue 2: Authentication Failed
**Solution**: 
- Check that JWT token is valid
- Ensure token is passed as `access_token` query parameter
- Verify JWT configuration matches between API and SignalR

### Issue 3: WebSocket Connection Failed
**Solution**:
- Use `skipNegotiation: true` in frontend
- Ensure WebSocket transport is available
- Check firewall/proxy settings

### Issue 4: "AllowAnyOrigin cannot be used with AllowCredentials"
**Solution**: Use `.SetIsOriginAllowed(_ => true)` instead of `.AllowAnyOrigin()`

## Production Considerations

### 1. Specific Origins
Instead of allowing all origins, specify your frontend domain:

```csharp
policy
    .WithOrigins("https://yourdomain.com", "http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials();
```

### 2. HTTPS Only
In production, ensure all connections use HTTPS:

```csharp
options.RequireHttpsMetadata = true; // In JWT configuration
```

### 3. Token Validation
Implement proper token validation and refresh mechanisms.

## Troubleshooting

### Debug Steps:
1. Check browser console for CORS errors
2. Verify API is running on correct port
3. Ensure frontend URL matches CORS policy
4. Check JWT token expiration
5. Verify SignalR hub endpoint is correct

### Logging:
Add logging to see connection attempts:

```csharp
public override async Task OnConnectedAsync()
{
    var userEmail = Context.User?.Identity?.Name ?? "Anonymous";
    Console.WriteLine($"User connected: {userEmail}");
    await Clients.All.SendAsync("UserConnected", userEmail);
    await base.OnConnectedAsync();
}
```

## Summary

The key changes to fix CORS issues with SignalR are:
1. ✅ Use `.SetIsOriginAllowed(_ => true)` instead of `.AllowAnyOrigin()`
2. ✅ Add `.AllowCredentials()` to CORS policy
3. ✅ Configure JWT authentication for SignalR
4. ✅ Pass token as `access_token` query parameter in frontend
5. ✅ Use proper SignalR connection configuration

With these changes, your frontend should be able to connect to SignalR without CORS errors!
