using System.Net.Mime;

namespace StrategyPattern.Evolution
{
    public static class BasicErrorHandlingExtensions
    {
        /// <summary>
        /// Registers the Basic error handling strategy (V2).
        /// Simple exception catching with basic error response.
        /// </summary>
        public static void AddBasicErrorHandling(this IServiceCollection services)
        {
            services.AddSingleton<IBastaErrorHandler, BasicErrorHandlingStrategy>();
        }
    }

    /// <summary>
    /// V2 - Basic Error Handling Strategy
    ///
    /// First instinct: catch exceptions and return 500 with error message.
    ///
    /// ✅ Capabilities:
    /// - Easy to understand and implement
    /// - Provides error message to client
    /// - Better than returning empty JSON
    /// - Prevents application crash
    ///
    /// ❌ Problems:
    /// - Always returns 500 (incorrect for client errors like validation)
    /// - No distinction between error types
    /// - Security risk: exposes exception details in production
    /// - Not RFC 9457 compliant
    /// - Clients can't distinguish between server errors and user errors
    /// - HTTP semantics violated (400 Bad Request vs 500 Internal Server Error)
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
