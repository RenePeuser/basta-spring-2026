using Microsoft.AspNetCore.Mvc;
using Siemens.AspNet.ErrorHandling.Contracts;

namespace StrategyPattern.Evolution.V08_Enterprise
{
    internal static class AddCustomJsonExceptionExtension
    {
        internal static void AddCustomJsonException(this IServiceCollection services)
        {
            services.AddSingleton<Siemens.AspNet.ErrorHandling.Contracts.ISpecificErrorHandler, CustomJsonException>();
        }
    }

    public class CustomJsonException : SpecificErrorHandler<BadHttpRequestException>
    {
        protected override bool CanHandle(BadHttpRequestException exception)
        {
            // Very hard do not do this at home :)
            return true;
        }

        protected override Task<ProblemDetails> HandleExceptionAsync(HttpCallInfos httpCallInfos, BadHttpRequestException exception)
        {
            var overrideProblemDetails = new ProblemDetails();
            overrideProblemDetails.Detail = "We have now intercepted an error handler library, without changing the original source (nuget)";
            overrideProblemDetails.Status = StatusCodes.Status422UnprocessableEntity;
            overrideProblemDetails.Extensions["Strategy"] = nameof(CustomJsonException);
            
            return Task.FromResult(overrideProblemDetails);
        }
    }
}
