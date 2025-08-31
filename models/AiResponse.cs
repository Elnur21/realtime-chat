namespace RealTimeChat.Models
{
    public class AiResponse
    {
        public string Answer { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? ModelUsed { get; set; }
    }
}
