using MediatR;
using MongoDB.Driver;
using RealTimeChat.Models;
using RealTimeChat.Services;

namespace RealTimeChat.Handlers.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
    {
        private readonly IMongoCollection<User> _users;

        public GetUserByIdQueryHandler(MongoCollectionService<User> userService)
        {
            _users = userService.Collection;
        }

        public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _users.Find(u => u.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
                
                if (user == null)
                {
                    return new GetUserByIdResponse
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                return new GetUserByIdResponse
                {
                    Success = true,
                    User = user,
                    Message = "User found"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetUserById error: {ex.Message}");
                return new GetUserByIdResponse
                {
                    Success = false,
                    Message = "Failed to retrieve user"
                };
            }
        }
    }
}
