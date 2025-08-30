using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealTimeChat.Handlers.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IConfiguration _config;

        public LoginCommandHandler(IConfiguration config)
        {
            _config = config;
        }

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

                // Example: hardcoded user check
                if (request.Email == "admin@test.com" && request.Password == "123456")
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
                            Role = "User"
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
                // Use a simple, hardcoded key for testing
                var key = "MySecretKey123456789012345678901234567890";
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Role, "User")
                };

                var token = new JwtSecurityToken(
                    issuer: "RealTimeChat",
                    audience: "RealTimeChatUsers",
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
