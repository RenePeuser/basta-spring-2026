using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace StrategyPattern.Evolution
{
    public static class IntermediateErrorHandlingExtensions
    {
        /// <summary>
        /// Registers the Intermediate error handling strategy (V2).
        /// Maps different exception types to appropriate HTTP status codes and ProblemDetails.
        /// </summary>
        public static void AddIntermediateErrorHandling(this IServiceCollection services)
        {
            services.AddSingleton<IBastaErrorHandler, IntermediateErrorHandlingStrategy>();
        }
    }

    /// <summary>
    /// V2 - Intermediate Error Handling Strategy
    /// Maps different exception types to appropriate HTTP status codes and ProblemDetails
    /// </summary>
    public class IntermediateErrorHandlingStrategy : IBastaErrorHandler
    {
        public async Task HandleAsync(HttpContext httpContext,
                                      Exception exception)
        {
            var problemDetails = exception switch
            {
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
                Type = $"https://httpstatuses.com/{statusCode}"
            };
        }
    }

}
