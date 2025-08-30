using MediatR;
using RealTimeChat.Models;

namespace RealTimeChat.Handlers.Commands
{
    public class UpdateUserCommand : IRequest<UpdateUserResponse>
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateUserResponse
    {
        public bool Success { get; set; }
        public User? User { get; set; }
        public string? Message { get; set; }
    }
}
