using MediatR;
using RealTimeChat.Models;

namespace RealTimeChat.Handlers.Queries
{
    public class GetAllUsersQuery : IRequest<GetAllUsersResponse>
    {
    }

    public class GetAllUsersResponse
    {
        public bool Success { get; set; }
        public List<User>? Users { get; set; }
        public string? Message { get; set; }
    }
}
