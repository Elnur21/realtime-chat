using MediatR;
using MongoDB.Driver;
using RealTimeChat.Models;
using RealTimeChat.Services;

namespace RealTimeChat.Handlers.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
    {
        private readonly IMongoCollection<User> _users;

        public DeleteUserCommandHandler(MongoCollectionService<User> userService)
        {
            _users = userService.Collection;
        }

        public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _users.DeleteOneAsync(u => u.Id == request.Id, cancellationToken);
                
                if (result.DeletedCount == 0)
                {
                    return new DeleteUserResponse
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                return new DeleteUserResponse
                {
                    Success = true,
                    Message = "User deleted successfully"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteUser error: {ex.Message}");
                return new DeleteUserResponse
                {
                    Success = false,
                    Message = "Failed to delete user"
                };
            }
        }
    }
}
