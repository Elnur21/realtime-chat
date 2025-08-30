using MediatR;
using MongoDB.Driver;
using RealTimeChat.Models;
using RealTimeChat.Services;

namespace RealTimeChat.Handlers.Queries
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, GetAllUsersResponse>
    {
        private readonly IMongoCollection<User> _users;

        public GetAllUsersQueryHandler(MongoCollectionService<User> userService)
        {
            _users = userService.Collection;
        }

        public async Task<GetAllUsersResponse> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _users.Find(u => true).ToListAsync(cancellationToken);
                
                return new GetAllUsersResponse
                {
                    Success = true,
                    Users = users,
                    Message = $"Found {users.Count} users"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllUsers error: {ex.Message}");
                return new GetAllUsersResponse
                {
                    Success = false,
                    Message = "Failed to retrieve users"
                };
            }
        }
    }
}
