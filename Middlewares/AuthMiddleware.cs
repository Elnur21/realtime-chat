using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MyApi.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Example: simple token check
            var token = context.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token) || token != "Bearer my-secret-token")
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            // Continue to the next middleware
            await _next(context);
        }
    }
}
