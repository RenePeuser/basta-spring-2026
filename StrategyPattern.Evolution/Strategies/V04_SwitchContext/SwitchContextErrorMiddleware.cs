using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Siemens.AspNet.ErrorHandling.Contracts;

namespace StrategyPattern.Evolution.V04_SwitchContext
{
    public class SwitchContextErrorMiddleware() : IMiddleware
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
                    // NEW: Validation exception handling with 422 status code and RFC 7807 compliant response
                    ValidationProblemDetailsException validationEx => CreateValidationProblems(StatusCodes.Status422UnprocessableEntity,
                                                                                               "Validation Failed",
                                                                                               validationEx.Message,
                                                                                               httpContext),

                    ArgumentNullException argEx => CreateProblemDetailsWithoutContext(StatusCodes.Status500InternalServerError,
                                                                                      "Argument was null",
                                                                                      argEx.Message,
                                                                                      httpContext),

                    BadHttpRequestException badHttpRequestException => CreateValidationProblems(StatusCodes.Status422UnprocessableEntity,
                                                                                                nameof(BadHttpRequestException),
                                                                                                badHttpRequestException.Message,
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

        private static ProblemDetails CreateProblemDetailsWithoutContext(int statusCode, string title, string detail, HttpContext context)
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

        private static ValidationProblemDetails CreateValidationProblems(int statusCode, string title, string detail, HttpContext context)
        {
            return new ValidationProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path,
                Type = $"https://http.cat/status/{statusCode}",
                Errors = new Dictionary<string, string[]>()
                {
                    {
                        "myProperty", ["Has multiple errors"]
                    }
                }
            };
        }
    }
}
