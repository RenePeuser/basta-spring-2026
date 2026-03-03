using System.Net.Mime;
using System.Text;
using StrategyPattern.Evolution.V6_Solid_Strategy;

namespace StrategyPattern.Evolution.V7_Basta
{
    /// <summary>
    /// V7 - Basta Advanced Error Handling Strategy (ASCII Art Demo)
    ///
    /// Shows the Strategy Pattern architecture visually with ASCII art.
    /// Displays all available strategies dynamically.
    /// Perfect for conference demos!
    /// </summary>
    internal class BastaErrorHandlingStrategy(IEnumerable<ISpecificExceptionHandler> errorHandlers) : IErrorHandlingStrategy
    {
        public async Task HandleAsync(HttpContext httpContext, Exception exception)
        {
            // Get all available strategy types dynamically
            var strategyTypes = Enum.GetNames<StrategyType>();
            var strategyList = string.Join("\n                                        ║    • ", strategyTypes);

            var exceptionType = exception.GetType().Name;
            var exceptionMessage = exception.Message.Length > 50
                ? exception.Message[..47] + "..."
                : exception.Message;

            var response = $"""
                                        ██████╗  █████╗ ███████╗████████╗ █████╗
                                        ██╔══██╗██╔══██╗██╔════╝╚══██╔══╝██╔══██╗
                                        ██████╔╝███████║███████╗   ██║   ███████║
                                        ██╔══██╗██╔══██║╚════██║   ██║   ██╔══██║
                                        ██████╔╝██║  ██║███████║   ██║   ██║  ██║
                                        ╚═════╝ ╚═╝  ╚═╝╚══════╝   ╚═╝   ╚═╝  ╚═╝

                                        ╔════════════════════════════════════════╗
                                        ║        🔴 EXCEPTION DETECTED           ║
                                        ║        → {exceptionType,-30} ║
                                        ╚════════════════════════════════════════╝

                                        ┌────────────────────────────────────────┐
                                        │    📐 STRATEGY PATTERN Architecture    │
                                        └────────────────────────────────────────┘

                                                   ┌─────────────────┐
                                                   │ IErrorHandling  │ 
                                                   │    Strategy     │ 
                                                   └────────┬────────┘
                                                            │
                                                ┌───────────┴───────────┐
                                                │                       │
                                                ▼                       ▼
                                          ┌───────────┐         ┌───────────┐
                                          │ BadRequest│   ...   │   Json    │
                                          │  Handler  │         │  Handler  │
                                          └───────────┘         └───────────┘
                                            

                                        ┌────────────────────────────────────────┐
                                        │ 📋 Verfügbare Strategien:              │
                                        │    • {strategyList}                    │ 
                                        └────────────────────────────────────────┘
                                        """;

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.ContentType = MediaTypeNames.Text.Plain;
            await httpContext.Response.WriteAsync(response, Encoding.UTF8);
        }
    }
}
