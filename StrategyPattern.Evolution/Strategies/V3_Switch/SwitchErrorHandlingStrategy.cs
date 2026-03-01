using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace StrategyPattern.Evolution
{
    public static class SwitchErrorHandlingStrategyExtensions
    {
        /// <summary>
        /// Registers the Switch error handling strategy (V3).
        /// Uses C# switch expression with RFC 9457 ProblemDetails format.
        /// </summary>
        public static void AddSwitchErrorHandlingStrategy(this IServiceCollection services)
        {
            services.AddSingleton<IBastaErrorHandler, SwitchErrorHandlingStrategy>();
        }
    }

    /// <summary>
    /// V3 - Switch Pattern (RFC 9457 ProblemDetails)
    ///
    /// Uses C# switch expression for exception type mapping with proper HTTP status codes.
    ///
    /// ✅ Capabilities:
    /// - Industry standard format (RFC 9457 - successor to RFC 7807)
    /// - Correct HTTP semantics (proper status codes: 400, 403, 404, 409, 500)
    /// - Better client experience with structured errors
    /// - Type, title, detail, status, and instance fields
    /// - Human-readable and machine-parseable
    /// - Fun type link to http.cat for visual representation
    ///
    /// ❌ Problems:
    /// - Manual exception mapping in switch (doesn't scale with many exception types)
    /// - No extensibility: adding custom handlers requires modifying the switch statement
    /// - Inner exceptions not handled (only top-level exception is evaluated)
    /// - Violates Open/Closed Principle (not open for extension without modification)
    /// - Maintenance burden grows linearly with number of exception types
    /// </summary>
    public class SwitchErrorHandlingStrategy : IBastaErrorHandler
    {
        public async Task HandleAsync(HttpContext httpContext,
                                      Exception exception)
        {
            var problemDetails = exception switch
            {
                BadHttpRequestException badHttpRequestException => CreateProblemDetails(StatusCodes.Status400BadRequest,
                                                                                        nameof(BadHttpRequestException),
                                                                                        badHttpRequestException.Message,
                                                                                        httpContext),

                ArgumentNullException argEx => CreateProblemDetails(StatusCodes.Status400BadRequest,
                                                                    "Bad Request",
                                                                    argEx.Message,
                                                                    httpContext),

                ArgumentException argEx => CreateProblemDetails(StatusCodes.Status400BadRequest,
                                                                "Bad Request",
                                                                argEx.Message,
                                                                httpContext),

                InvalidOperationException invalidEx => CreateProblemDetails(StatusCodes.Status409Conflict,
                                                                            "Conflict",
                                                                            invalidEx.Message,
                                                                            httpContext),

                UnauthorizedAccessException => CreateProblemDetails(StatusCodes.Status403Forbidden,
                                                                    "Forbidden",
                                                                    "You don't have permission to access this resource",
                                                                    httpContext),

                KeyNotFoundException notFoundEx => CreateProblemDetails(StatusCodes.Status404NotFound,
                                                                        "Not Found",
                                                                        notFoundEx.Message,
                                                                        httpContext),

                _ => CreateProblemDetails(StatusCodes.Status500InternalServerError,
                                          "Internal Server Error",
                                          "An unexpected error occurred",
                                          httpContext)
            };

            httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = MediaTypeNames.Application.ProblemJson;

            await httpContext.Response.WriteAsJsonAsync(problemDetails);
        }

        private static ProblemDetails CreateProblemDetails(int statusCode, string title, string detail, HttpContext context)
        {
            return new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path,
                Type = $"https://http.cat/status/{statusCode}"
            };
        }
    }

}
