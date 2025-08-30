using MediatR;
using MongoDB.Driver;
using RealTimeChat.Models;
using RealTimeChat.Services;

namespace RealTimeChat.Handlers.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
    {
        private readonly IMongoCollection<User> _users;

        public UpdateUserCommandHandler(MongoCollectionService<User> userService)
        {
            _users = userService.Collection;
        }

        public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email))
                {
                    return new UpdateUserResponse
                    {
                        Success = false,
                        Message = "Name and email are required"
                    };
                }

                var updatedUser = new User
                {
                    Id = request.Id,
                    Name = request.Name,
                    Email = request.Email
                };

                var result = await _users.ReplaceOneAsync(u => u.Id == request.Id, updatedUser, cancellationToken: cancellationToken);
                
                if (result.MatchedCount == 0)
                {
                    return new UpdateUserResponse
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                return new UpdateUserResponse
                {
                    Success = true,
                    User = updatedUser,
                    Message = "User updated successfully"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateUser error: {ex.Message}");
                return new UpdateUserResponse
                {
                    Success = false,
                    Message = "Failed to update user"
                };
            }
        }
    }
}
