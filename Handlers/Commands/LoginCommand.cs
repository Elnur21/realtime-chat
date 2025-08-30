using MediatR;
using RealTimeChat.Models;

namespace RealTimeChat.Handlers.Commands
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public UserInfo? User { get; set; }
    }

    public class UserInfo
    {
        public string Email { get; set; } = string.Empty;
    }
}
