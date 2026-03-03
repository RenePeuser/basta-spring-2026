using System.Collections.Immutable;
using System.Net.Mime;
using Extensions.Pack;
using Microsoft.AspNetCore.Mvc;
using Siemens.AspNet.ErrorHandling.Contracts;
using Siemens.AspNet.MinimalApi.Sdk.Contracts;

namespace StrategyPattern.Evolution.Strategies.V6_Solid_Strategy
{
    public interface IErrorResponseWriter
    {
        Task WriteResponseAsync(HttpContext context,
                                ProblemDetails problemDetails);
    }

    internal sealed class ErrorResponseWriter(IJsonSerializer jsonSerializer) : IErrorResponseWriter
    {
        public Task WriteResponseAsync(HttpContext context,
                                       ProblemDetails problemDetails)
        {
            // 1. Save the original headers to restore them after writing the response
            var originalHeaders = context.Response.Headers.ToImmutableList();

            // 2. We clean up anything to go sure only the current infos are written to the response
            context.Response.Clear();

            // 3. Restore the original headers
            context.Response.Headers.AddRange(originalHeaders);

            // 4. Set current response infos
            context.Response.ContentType = MediaTypeNames.Application.ProblemJson;
            context.Response.StatusCode = problemDetails.Status ?? 500;

            // 5. We have to cast the specific type to write the correct response
            var responseJson = problemDetails switch
            {
                ValidationProblemDetailsExtended httpValidationProblemDetails => jsonSerializer.Serialize(httpValidationProblemDetails),
                ValidationProblemDetails validationProblemDetails => jsonSerializer.Serialize(validationProblemDetails),
                _ => jsonSerializer.Serialize(problemDetails)
            };

            // 6. For better debug experience we serialized it first
            return context.Response.WriteAsync(responseJson);
        }
    }
}
