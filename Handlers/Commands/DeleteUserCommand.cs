using MediatR;

namespace RealTimeChat.Handlers.Commands
{
    public class DeleteUserCommand : IRequest<DeleteUserResponse>
    {
        public string Id { get; set; } = string.Empty;
    }

    public class DeleteUserResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
