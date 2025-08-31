namespace RealTimeChat.Models
{
    public class AiRequest
    {
        public string Question { get; set; } = string.Empty;
        public string? Context { get; set; }
        public string? UserId { get; set; }
        public bool RequireAnswer { get; set; } = true;
    }
}
