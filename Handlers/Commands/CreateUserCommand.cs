using MediatR;
using RealTimeChat.Models;

namespace RealTimeChat.Handlers.Commands
{
    public class CreateUserCommand : IRequest<CreateUserResponse>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class CreateUserResponse
    {
        public bool Success { get; set; }
        public User? User { get; set; }
        public string? Message { get; set; }
    }
}
