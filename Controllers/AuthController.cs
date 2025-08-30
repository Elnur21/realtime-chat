using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RealTimeChat.Models;
using MediatR;
using RealTimeChat.Handlers.Commands;

namespace RealTimeChat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Auth controller is working", timestamp = DateTime.UtcNow });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Request body is required");
                }

                var command = new LoginCommand
                {
                    Email = request.Email ?? string.Empty,
                    Password = request.Password ?? string.Empty
                };

                var response = await _mediator.Send(command);

                if (response.Success)
                {
                    return Ok(new { 
                        token = response.Token,
                        message = response.Message,
                        user = response.User
                    });
                }

                return Unauthorized(new { message = response.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }



        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userEmail = User.Identity?.Name;
            
            return Ok(new { 
                email = userEmail,
                message = "Token is valid"
            });
        }
    }
}
