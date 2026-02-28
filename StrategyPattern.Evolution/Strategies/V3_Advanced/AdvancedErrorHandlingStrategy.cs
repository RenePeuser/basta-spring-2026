//namespace StrategyPattern.Evolution.Strategies.V3_Advanced
//{
//    /// <summary>
//    /// V3 - Advanced Error Handling Strategy
//    /// Uses the full-featured error handling system with:
//    /// - Multiple specialized exception handlers
//    /// - Request body buffering for detailed diagnostics
//    /// - JSON validation
//    /// - Extensive error context and metadata
//    /// - Security-aware error responses
//    /// </summary>
//    public static class AdvancedErrorHandlingExtensions
//    {
//        public static void AddAdvancedErrorHandling(this IServiceCollection services, IConfiguration configuration)
//        {
//            // Add request body buffering to enable detailed error diagnostics
//            services.AddEnableRequestBufferMiddleware();

//            // Add the full error handling strategy with all specialized handlers
//            services.AddErrorResponseHandlingMiddleware(configuration);

//            return services;
//        }

//        public static IApplicationBuilder UseAdvancedErrorHandling(this IApplicationBuilder app)
//        {
//            // Enable request body buffering first
//            app.UseEnableRequestBufferMiddleware();

//            // Then add the error handling middleware
//            app.UseMiddleware<ErrorResponseHandlingMiddleware>();

//            return app;
//        }
//    }
//}


