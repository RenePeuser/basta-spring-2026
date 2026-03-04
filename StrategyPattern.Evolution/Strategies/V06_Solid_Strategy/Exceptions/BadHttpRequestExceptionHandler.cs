using System.Net;
using Extensions.Pack;
using Microsoft.AspNetCore.Mvc;
using Siemens.AspNet.ErrorHandling.Contracts;

namespace StrategyPattern.Evolution.V06_Solid_Strategy.Exceptions
{
    internal class BadHttpRequestExceptionHandler : ISpecificExceptionHandler
    {
        public bool CanHandle(Exception exception)
        {
            var canHandle = exception.GetType() == typeof(BadHttpRequestException);
            return canHandle;
        }

        public Task<ProblemDetails> HandleAsync(HttpContext httpContext, Exception exception)
        {
            // Safety first !! Do not trust your caller - or your own code ;)
            if (CanHandle(exception).IsFalse())
            {
                // Critical throw an error during error exception handling !
                throw new InternalServerErrorDetailsException("Your specific exception handler is not able to handle",
                                                              $"Your specific exceptions handler: {nameof(BadHttpRequestExceptionHandler)} ca not handle: {exception.GetType()}");
            }

            var problemDetails = CreateProblemDetails(HttpStatusCode.BadRequest,
                                                      $"{nameof(BadHttpRequestException)} occured",
                                                      exception.Message,
                                                      httpContext);

            return Task.FromResult(problemDetails);
        }

        private static ProblemDetails CreateProblemDetails(HttpStatusCode statusCode, string title, string detail, HttpContext context)
        {
            var statusAsInteger = statusCode.ToInt();

            return new ProblemDetails
            {
                Status = statusAsInteger,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path,
                Type = $"https://http.cat/status/{statusAsInteger}"
            };
        }
    }
}
