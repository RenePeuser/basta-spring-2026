using System.Text.Json;
using Extensions.Pack;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Siemens.AspNet.ErrorHandling.Contracts;

namespace StrategyPattern.Evolution.V6_Solid_Strategy.Exceptions
{
    internal class JsonExceptionHandler : ISpecificExceptionHandler
    {
        public bool CanHandle(Exception exception)
        {
            var canHandle = exception.GetType() == typeof(JsonException); // System.Text.Json.JsonException
            return canHandle;
        }

        public Task<ProblemDetails> HandleAsync(HttpContext httpContext, Exception exception)
        {
            // Safety first !! Do not trust your caller - or your own code ;)
            if (CanHandle(exception).IsFalse())
            {
                // Critical throw an error during error exception handling !
                throw new InternalServerErrorDetailsException("Your specific exception handler is not able to handle",
                                                              $"Your specific exceptions handler: {nameof(JsonExceptionHandler)} ca not handle: {exception.GetType()}");
            }

            var status400BadRequest = StatusCodes.Status400BadRequest;

            var problemDetails = new ProblemDetails
            {
                Status = status400BadRequest, // Json exeption > Created by client
                Title = "Your json is not OK",
                Detail = exception.Message,
                Instance = httpContext.Request.GetDisplayUrl(),
                Type = $"https://http.cat/status/{status400BadRequest}"
            };

            return Task.FromResult(problemDetails);
        }
    }
}
