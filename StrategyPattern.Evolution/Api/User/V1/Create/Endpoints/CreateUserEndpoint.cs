using System.Net.Mime;
using Extensions.Pack;
using Microsoft.AspNetCore.Mvc;
using Siemens.AspNet.ErrorHandling.Contracts;
using Siemens.AspNet.MinimalApi.Sdk.Contracts;

namespace StrategyPattern.Evolution.Api.User.V1.Create
{
    internal static class AddCreateUserEndpointExtension
    {
        internal static void AddCreateUserEndpoint(this IServiceCollection services,
                                                   IConfiguration configuration)
        {
            services.AddCreateUserRequestSampleValueProvider();

            services.AddSingletonIfNotExists<IEndpointRegistration, CreateUserEndpoint>();
        }
    }

    internal sealed class CreateUserEndpoint(IAsyncRequestValidator asyncRequestValidator) : IEndpointRegistration
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("users", HandleAsync)
                     .Accepts<CreateUserRequest>(MediaTypeNames.Application.Json)
                     .Produces<CreateUserResponse>()
                     .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                     .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
                     .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
                     .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                     .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
                     .Produces<ValidationProblemDetailsExtended>(StatusCodes.Status422UnprocessableEntity)
                     .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                     .Produces<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)
                     .WithTags("Users")
                     .WithName("CreateUserV1");

            async Task<CreateUserResponse> HandleAsync(CreateUserRequest createUserRequest,
                                                       HttpContext httpContext,
                                                       CancellationToken cancellationToken = default)
            {
                await asyncRequestValidator.ValidateAndThrowAsync(createUserRequest).ConfigureAwait(false);

                var user = new User(Id: Guid.NewGuid(),
                                    FirstName: createUserRequest.FirstName,
                                    LastName: createUserRequest.LastName,
                                    Email: createUserRequest.Email,
                                    UserType: createUserRequest.UserType);

                var response = new CreateUserResponse(user);

                return response;
            }
        }
    }
}
