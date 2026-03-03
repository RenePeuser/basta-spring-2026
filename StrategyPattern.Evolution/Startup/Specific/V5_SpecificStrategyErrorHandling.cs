using Extensions.Pack;
using Siemens.AspNet.ErrorHandling;
using Siemens.AspNet.MinimalApi.Sdk;
using StrategyPattern.Evolution.Api.User.V1.Create;
using StrategyPattern.Evolution.V5_SpecificStrategyErrorHandling;
using StrategyPattern.Evolution.V5_SpecificStrategyErrorHandling.Exceptions;

namespace StrategyPattern.Evolution.Startup
{
    /// <summary>
    /// V1 - Basic Startup Strategy
    /// Complete configuration with basic error handling.
    /// Shows the minimal viable error handling approach.
    /// </summary>
    public class V5_SpecificStrategyErrorHandling : IStartupStrategy
    {
        public string Description => "Specific error handling with full collected call infos context";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain - User endpoint registration
            services.AddSingleton<IEndpointRegistration, CreateUserEndpoint>();

            services.AddSingleton<SpecificStrategyErrorHandling>();

            // Endpoint mapper
            services.AddSingleton<RegisterEndpoints>();

            // Error handling - Specific strategy error handling
            services.AddSingleton<IExceptionHandler, BadHttpRequestExceptionHandler>();

            // Common services
            services.AddErrorHandling(configuration);
            services.AddValidation();
            services.AddJsonSerializeOptions();
            services.AddAllowedQueryParameter();
        }

        public void ConfigurePipeline(WebApplication app)
        {
            app.UseHttpsRedirection();

            var apiBasePath = app.MapGroup("api/v1");

            // Specific strategy error handling middleware
            app.UseMiddleware<SpecificStrategyErrorHandling>();


            // Register endpoints
            var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
            registerEndpoints.MapEndpoints(apiBasePath);
        }
    }
}
