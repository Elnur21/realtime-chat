using MediatR;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using RealTimeChat.Models;
using RealTimeChat.Services;

namespace RealTimeChat.Handlers.Commands
{
    public class LoginCommandHandler(IConfiguration config, MongoCollectionService<User> userService) : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IConfiguration _config = config;

        private readonly IMongoCollection<User> _users = userService.Collection;

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Email and password are required"
                    };
                }

                Console.WriteLine($"Login attempt for {request.Email}");



                var user = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync(cancellationToken);
                if (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    var token = await Task.Run(() => GenerateJwtToken(request.Email), cancellationToken);
                    return new LoginResponse
                    {
                        Success = true,
                        Token = token,
                        Message = "Login successful",
                        User = new UserInfo
                        {
                            Email = request.Email,
                        }
                    };
                }

                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return new LoginResponse
                {
                    Success = false,
                    Message = "Internal server error"
                };
            }
        }

        private string GenerateJwtToken(string email)
        {
            try
            {
                var key =_config.GetSection("Key").Value ?? "MySecretKey123456789012345678901234567890";
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, email),
                };

                var token = new JwtSecurityToken(
                    issuer: _config.GetSection("Issuer").Value ?? "RealTimeChatIssuer",
                    audience: _config.GetSection("Audience").Value ?? "RealTimeChatAudience",
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JWT generation error: {ex.Message}");
                throw;
            }
        }
    }
}
