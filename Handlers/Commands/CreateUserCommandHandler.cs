using MediatR;
using BCrypt.Net;
using MongoDB.Driver;
using RealTimeChat.Models;
using RealTimeChat.Services;

namespace RealTimeChat.Handlers.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
    {
        private readonly IMongoCollection<User> _users;

        public CreateUserCommandHandler(MongoCollectionService<User> userService)
        {
            _users = userService.Collection;
        }

        public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email))
                {
                    return new CreateUserResponse
                    {
                        Success = false,
                        Message = "Name and email are required"
                    };
                }

                // Check if user with this email already exists
                var existingUser = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync(cancellationToken);
                if (existingUser != null)
                {
                    return new CreateUserResponse
                    {
                        Success = false,
                        Message = "User with this email already exists"
                    };
                }

                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                };

                await _users.InsertOneAsync(user, cancellationToken: cancellationToken);

                return new CreateUserResponse
                {
                    Success = true,
                    User = user,
                    Message = "User created successfully"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateUser error: {ex.Message}");
                return new CreateUserResponse
                {
                    Success = false,
                    Message = "Failed to create user"
                };
            }
        }
    }
}
