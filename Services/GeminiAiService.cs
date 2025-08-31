using RealTimeChat.Models;
using RealTimeChat.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace RealTimeChat.Services
{
    public interface IGeminiAiService
    {
        Task<AiResponse> GetAnswerAsync(AiRequest request);
    }

    public class GeminiAiService : IGeminiAiService
    {
        private readonly GeminiSettings _settings;
        private readonly HttpClient _httpClient;

        public GeminiAiService(IOptions<GeminiSettings> settings, HttpClient httpClient)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
            
            if (string.IsNullOrEmpty(_settings.ApiKey))
            {
                throw new InvalidOperationException("Gemini API key is not configured");
            }
        }

        public async Task<AiResponse> GetAnswerAsync(AiRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Question))
                {
                    return new AiResponse
                    {
                        Success = false,
                        ErrorMessage = "Question cannot be empty"
                    };
                }

                // Build the prompt with context if provided
                var prompt = BuildPrompt(request);

                // Call Gemini API
                var response = await CallGeminiApiAsync(prompt);

                return new AiResponse
                {
                    Answer = response,
                    Success = true,
                    ModelUsed = _settings.ModelName
                };
            }
            catch (Exception ex)
            {
                return new AiResponse
                {
                    Success = false,
                    ErrorMessage = $"AI service error: {ex.Message}"
                };
            }
        }

        private string BuildPrompt(AiRequest request)
        {
            var prompt = "You are a helpful AI assistant. Please provide a clear and concise answer to the following question.";

            if (!string.IsNullOrEmpty(request.Context))
            {
                prompt += $"\n\nContext: {request.Context}";
            }

            if (request.RequireAnswer)
            {
                prompt += "\n\nPlease provide a direct answer to the question.";
            }
            else
            {
                prompt += "\n\nIf you cannot provide a definitive answer, please indicate that clearly.";
            }

            prompt += $"\n\nQuestion: {request.Question}";

            return prompt;
        }

        private async Task<string> CallGeminiApiAsync(string prompt)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = _settings.Temperature,
                    maxOutputTokens = _settings.MaxTokens
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.ModelName}:generateContent?key={_settings.ApiKey}";

            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Gemini API error: {response.StatusCode} - {responseContent}");
            }

            var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
            return result?.candidates?[0]?.content?.parts?[0]?.text?.ToString() ?? "No response generated";
        }
    }
}
