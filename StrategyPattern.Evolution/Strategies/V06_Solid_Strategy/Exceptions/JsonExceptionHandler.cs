using System.Text.Json;
using Extensions.Pack;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Siemens.AspNet.ErrorHandling.Contracts;

namespace StrategyPattern.Evolution.V06_Solid_Strategy.Exceptions
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

            var status422UnprocessableEntity = StatusCodes.Status422UnprocessableEntity;

            var acceptsMetadata = httpContext.GetEndpoint()?.Metadata.GetMetadata<AcceptsMetadata>();
            var requestType = acceptsMetadata?.RequestType?.Name ?? "Unknown";

            var problemDetails = new ProblemDetails()
            {
                Status = status422UnprocessableEntity, // Json exeption > Created by client
                Title = "Your json is not OK - Please check the error details for more infos",
                Detail = exception.Message,
                Instance = httpContext.Request.GetDisplayUrl(),
                Type = $"https://http.cat/status/{status422UnprocessableEntity}",
                Extensions = new Dictionary<string, object?>()
                {
                    {"RequestType", requestType}
                }

            };

            return Task.FromResult(problemDetails);
        }
    }
}
