namespace StrategyPattern.Evolution
{
    internal static class AddBastaAdvancedErrorHandlingStrategyExtension
    {
        internal static void AddBastaAdvancedErrorHandlingStrategy(this IServiceCollection services)
        {
            services.AddSingleton<IBastaErrorHandler, BastaAdvancedErrorHandlingStrategy>();
        }
    }

    /// <summary>
    /// V5 - Basta Advanced Error Handling Strategy
    /// This is a placeholder strategy for the Advanced mode.
    /// In Advanced mode, the Siemens error handling middleware is used directly,
    /// so this strategy should never be called.
    /// </summary>
    public class BastaAdvancedErrorHandlingStrategy : IBastaErrorHandler
    {
        private const string Response = """
                                        ██████╗  █████╗ ███████╗████████╗ █████╗
                                        ██╔══██╗██╔══██╗██╔════╝╚══██╔══╝██╔══██╗
                                        ██████╔╝███████║███████╗   ██║   ███████║
                                        ██╔══██╗██╔══██║╚════██║   ██║   ██╔══██║
                                        ██████╔╝██║  ██║███████║   ██║   ██║  ██║
                                        ╚═════╝ ╚═╝  ╚═╝╚══════╝   ╚═╝   ╚═╝  ╚═╝

                                        ╔═══════════════════════════════════════╗
                                        ║  EXCEPTION DETECTED                   ║
                                        ║                                       ║
                                        ║  → Strategy Resolver Activated        ║
                                        ║  → Domain Context Matched             ║
                                        ║  → Clean JSON Contract Generated      ║
                                        ║                                       ║
                                        ║  STATUS     : 400 BadRequest          ║
                                        ║  ERROR_CODE : VALIDATION_FAILED       ║
                                        ║  TRACE_ID   : BASTA-2026-STRATEGY     ║
                                        ║                                       ║
                                        ║  Clean Errors.                        ║ 
                                        ║  Clean Architecture.                  ║
                                        ║                                       ║
                                        ║  Viel Spaß auf der BASTA!             ║
                                        ╚═══════════════════════════════════════╝
                                        """;

        public Task HandleAsync(HttpContext httpContext, Exception exception)
        {
            return httpContext.Response.WriteAsync(Response);
        }
    }
}
