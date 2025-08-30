using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyApi.Models;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Auth controller is working", timestamp = DateTime.UtcNow });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Request body is required");
                }
                
                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest("Email is required");
                }
                
                if (string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest("Password is required");
                }

                Console.WriteLine($"Login attempt for {request.Email}");
                
                // Example: hardcoded user check
                if (request.Email == "admin@test.com" && request.Password == "123456")
                {
                    var token = GenerateSimpleJwtToken(request.Email);
                    return Ok(new { 
                        token,
                        message = "Login successful",
                        user = new { email = request.Email }
                    });
                }

                return Unauthorized(new { message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        private string GenerateSimpleJwtToken(string email)
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
                    issuer: "MyApi",
                    audience: "MyApiUsers",
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Simple JWT generation error: {ex.Message}");
                throw;
            }
        }

        private string GenerateJwtToken(string email)
        {
            try
            {
                var jwtKey = _config["Jwt:Key"];
                Console.WriteLine($"JWT Key length: {jwtKey?.Length}");
                
                if (string.IsNullOrEmpty(jwtKey))
                {
                    throw new InvalidOperationException("JWT Key is not configured");
                }

                // Use a shorter key for testing
                var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
                var securityKey = new SymmetricSecurityKey(keyBytes);
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Role, "User")
                };

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"] ?? "MyApi",
                    audience: _config["Jwt:Audience"] ?? "MyApiUsers",
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials
                );

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenString = tokenHandler.WriteToken(token);
                Console.WriteLine($"Token generated successfully, length: {tokenString.Length}");
                return tokenString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JWT generation error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userEmail = User.Identity?.Name;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            
            return Ok(new { 
                email = userEmail,
                role = userRole,
                message = "Token is valid"
            });
        }
    }
}
