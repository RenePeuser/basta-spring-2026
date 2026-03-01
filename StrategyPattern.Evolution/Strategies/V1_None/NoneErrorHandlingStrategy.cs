namespace StrategyPattern.Evolution
{
    public static class NoneErrorHandlingStrategyExtensions
    {
        /// <summary>
        /// Registers the None error handling strategy (V1).
        /// Worst case scenario - returns empty JSON with wrong status code.
        /// </summary>
        public static void AddNoneErrorHandling(this IServiceCollection services)
        {
            services.AddSingleton<IBastaErrorHandler, NoneErrorHandlingStrategy>();
        }
    }

    /// <summary>
    /// V1 - None Error Handling Strategy (Worst Case)
    ///
    /// Educational demonstration of what NOT to do.
    ///
    /// ✅ Capabilities:
    /// - Prevents application crash
    /// - Returns valid JSON (technically)
    /// - Shows what default behavior might look like without proper error handling
    ///
    /// ❌ Problems:
    /// - No useful error information whatsoever
    /// - Always wrong HTTP status code (400 for everything, even server errors)
    /// - Terrible developer experience
    /// - Clients have no idea what went wrong -> passive-aggressive error handling
    /// - Violates HTTP semantics completely
    /// - No distinction between client errors (4xx) and server errors (5xx)
    /// - Far away from any RFC conventions for error responses
    /// </summary>
    public class NoneErrorHandlingStrategy : IBastaErrorHandler
    {
        public async Task HandleAsync(HttpContext httpContext, Exception exception)
        {
            // Ooops - an exception occurred, we are afraid to handle it.
            // Keep eyes closed send {} and hope the best -> And say The
            // User is guilty - As well it was Friday and we are tired.
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(new object());
        }
    }
}
