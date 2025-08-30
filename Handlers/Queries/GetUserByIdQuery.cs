using MediatR;
using RealTimeChat.Models;

namespace RealTimeChat.Handlers.Queries
{
    public class GetUserByIdQuery : IRequest<GetUserByIdResponse>
    {
        public string Id { get; set; } = string.Empty;
    }

    public class GetUserByIdResponse
    {
        public bool Success { get; set; }
        public User? User { get; set; }
        public string? Message { get; set; }
    }
}
