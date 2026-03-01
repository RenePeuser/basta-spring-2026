using System.Net.Mime;

namespace StrategyPattern.Evolution
{
    public static class BasicErrorHandlingExtensions
    {
        /// <summary>
        /// Registers the Basic error handling strategy (V1).
        /// Simple exception catching with basic error response.
        /// </summary>
        public static void AddBasicErrorHandling(this IServiceCollection services)
        {
            services.AddSingleton<IBastaErrorHandler, BasicErrorHandlingStrategy>();
        }
    }

    /// <summary>
    /// V1 - Basic Error Handling Strategy
    /// Simple exception catching with basic error response
    /// </summary>
    public class BasicErrorHandlingStrategy : IBastaErrorHandler
    {
        public async Task HandleAsync(HttpContext httpContext, Exception exception)
        {
            // Simple approach: Set status code and return error message
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = MediaTypeNames.Application.Json;

            var errorResponse = new
            {
                error = "An error occurred",
                message = exception.Message
            };

            await httpContext.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
