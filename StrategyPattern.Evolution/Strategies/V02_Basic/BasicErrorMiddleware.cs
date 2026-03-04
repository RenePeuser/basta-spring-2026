using System.Net.Mime;

namespace StrategyPattern.Evolution.V02_Basic
{
    public class BasicErrorMiddleware() : IMiddleware
    {
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception exception)
            {
                // We add exception message ....
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                httpContext.Response.ContentType = MediaTypeNames.Application.Json;

                var errorResponse = new
                {
                    error = "An error occurred",
                    message = exception.Message
                };

                await httpContext.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
