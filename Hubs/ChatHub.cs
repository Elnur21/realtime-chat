using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RealTimeChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
            var userEmail = Context.User?.Identity?.Name ?? "Anonymous";
            await Clients.All.SendAsync("ReceiveMessage", userEmail, message);
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