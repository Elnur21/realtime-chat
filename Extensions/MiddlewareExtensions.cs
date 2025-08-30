using Microsoft.AspNetCore.Builder;
using MyApi.Middlewares;

namespace MyApi.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
