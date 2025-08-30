using Microsoft.AspNetCore.Builder;
using RealTimeChat.Middlewares;

namespace RealTimeChat.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
