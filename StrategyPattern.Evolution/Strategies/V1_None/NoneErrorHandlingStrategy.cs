namespace StrategyPattern.Evolution
{
    public static class NoneErrorHandlingStrategyExtensions
    {
        /// <summary>
        /// Registers the Basic error handling strategy (V1).
        /// Simple exception catching with basic error response.
        /// </summary>
        public static void AddNoneErrorHandling(this IServiceCollection services)
        {
            services.AddSingleton<IBastaErrorHandler, NoneErrorHandlingStrategy>();
        }
    }

    /// <summary>
    /// V1 - Basic Error Handling Strategy
    /// Simple exception catching with basic error response
    /// </summary>
    public class NoneErrorHandlingStrategy : IBastaErrorHandler
    {
        public async Task HandleAsync(HttpContext httpContext, Exception exception)
        {
            // Ooops - an exception occurred, we are afraid to handle it.
            // Keep eyes closed send {} and hope the best -> And say The
            // User is guilty - who else :) 
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(new object());
        }
    }
}
