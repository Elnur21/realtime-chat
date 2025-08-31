using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RealTimeChat.Services;
using RealTimeChat.Models;

namespace RealTimeChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IGeminiAiService _aiService;

        public ChatHub(IGeminiAiService aiService)
        {
            _aiService = aiService;
        }

        public async Task SendMessage(string message)
        {
            var userEmail = Context.User?.Identity?.Name ?? "Anonymous";
            await Clients.All.SendAsync("ReceiveMessage", userEmail, message);
        }

        public async Task AskAI(string question, string? context = null, bool requireAnswer = true)
        {
            var userEmail = Context.User?.Identity?.Name ?? "Anonymous";
            
            // Send user's question to all clients
            await Clients.All.SendAsync("ReceiveMessage", userEmail, $"🤖 AI Question: {question}");
            
            try
            {
                // Create AI request
                var aiRequest = new AiRequest
                {
                    Question = question,
                    Context = context,
                    UserId = userEmail,
                    RequireAnswer = requireAnswer
                };

                // Get AI response
                var aiResponse = await _aiService.GetAnswerAsync(aiRequest);
                
                if (aiResponse.Success)
                {
                    // Send AI response to all clients
                    await Clients.All.SendAsync("ReceiveMessage", "🤖 AI Assistant", aiResponse.Answer);
                }
                else
                {
                    // Send error message
                    await Clients.All.SendAsync("ReceiveMessage", "🤖 AI Assistant", $"Sorry, I couldn't process your question: {aiResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                // Send error message
                await Clients.All.SendAsync("ReceiveMessage", "🤖 AI Assistant", $"Sorry, I encountered an error: {ex.Message}");
            }
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("UserJoined", Context.User?.Identity?.Name, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("UserLeft", Context.User?.Identity?.Name, groupName);
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            var userEmail = Context.User?.Identity?.Name ?? "Anonymous";
            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", userEmail, groupName, message);
        }

        public override async Task OnConnectedAsync()
        {
            var userEmail = Context.User?.Identity?.Name ?? "Anonymous";
            await Clients.All.SendAsync("UserConnected", userEmail);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userEmail = Context.User?.Identity?.Name ?? "Anonymous";
            await Clients.All.SendAsync("UserDisconnected", userEmail);
            await base.OnDisconnectedAsync(exception);
        }
    }
}