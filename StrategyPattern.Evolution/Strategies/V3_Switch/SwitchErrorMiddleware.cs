using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace StrategyPattern.Evolution.V3_Switch
{
    public class SwitchErrorMiddleware() : IMiddleware
    {
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception exception)
            {
                var problemDetails = exception switch
                {
                    BadHttpRequestException badHttpRequestException => CreateProblemDetails(StatusCodes.Status400BadRequest,
                                                                                            nameof(BadHttpRequestException),
                                                                                            badHttpRequestException.Message,
                                                                                            httpContext),

                    ArgumentNullException argEx => CreateProblemDetails(StatusCodes.Status500InternalServerError,
                                                                        "Argument was null",
                                                                        argEx.Message,
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
