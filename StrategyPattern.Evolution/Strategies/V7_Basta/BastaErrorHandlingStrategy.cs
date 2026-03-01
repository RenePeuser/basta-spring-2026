namespace StrategyPattern.Evolution
{
    internal static class AddBastaAdvancedErrorHandlingStrategyExtension
    {
        /// <summary>
        /// Registers the BASTA! ASCII Art error handling strategy (V7).
        /// For conference demonstrations and fun!
        /// </summary>
        internal static void AddBastaAdvancedErrorHandlingStrategy(this IServiceCollection services)
        {
            services.AddSingleton<IBastaErrorHandler, BastaAdvancedErrorHandlingStrategy>();
        }
    }

    /// <summary>
    /// V7 - Basta Advanced Error Handling Strategy (ASCII Art Demo)
    ///
    /// Demonstrates the ultimate flexibility of the Strategy Pattern.
    ///
    /// ✅ Capabilities:
    /// - Eye-catching ASCII art response
    /// - Shows the flexibility of the strategy pattern
    /// - Same interface (IBastaErrorHandler), completely different behavior
    /// - Great for live demos and audience engagement
    /// - Demonstrates that strategies can do ANYTHING
    /// - Proves the power of abstraction and polymorphism
    /// - Conference branding and marketing
    ///
    /// ❌ Problems:
    /// - NOT for production use! 😄
    /// - No actual error information
    /// - Not RFC 9457 compliant (but that's intentional!)
    /// - Would confuse API clients
    /// - Just for entertainment and education
    /// - No debugging help whatsoever
    ///
    /// Purpose: Show that with the same 3 lines of Program.cs, you can switch
    /// from production-ready enterprise error handling to ASCII art with one line change.
    /// That's the power of the Strategy Pattern!
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
