using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RealTimeChat.Models;
using RealTimeChat.Services;

namespace RealTimeChat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IGeminiAiService _aiService;

        public AiController(IGeminiAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("ask")]
        [Authorize]
        public async Task<IActionResult> AskQuestion([FromBody] AiRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Request body is required" });
                }

                if (string.IsNullOrEmpty(request.Question))
                {
                    return BadRequest(new { message = "Question is required" });
                }

                // Get user ID from the authenticated user
                var userEmail = User.Identity?.Name;
                request.UserId = userEmail;

                var response = await _aiService.GetAnswerAsync(request);

                if (response.Success)
                {
                    return Ok(new
                    {
                        answer = response.Answer,
                        success = true,
                        timestamp = response.Timestamp,
                        modelUsed = response.ModelUsed
                    });
                }

                return BadRequest(new
                {
                    message = response.ErrorMessage,
                    success = false
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI service error: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("ask-public")]
        public async Task<IActionResult> AskQuestionPublic([FromBody] AiRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Request body is required" });
                }

                if (string.IsNullOrEmpty(request.Question))
                {
                    return BadRequest(new { message = "Question is required" });
                }

                var response = await _aiService.GetAnswerAsync(request);

                if (response.Success)
                {
                    return Ok(new
                    {
                        answer = response.Answer,
                        success = true,
                        timestamp = response.Timestamp,
                        modelUsed = response.ModelUsed
                    });
                }

                return BadRequest(new
                {
                    message = response.ErrorMessage,
                    success = false
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI service error: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { 
                message = "AI service is available", 
                timestamp = DateTime.UtcNow,
                service = "Gemini AI"
            });
        }
    }
}
