using Siemens.AspNet.ErrorHandling;
using Siemens.AspNet.MinimalApi.Sdk;
using StrategyPattern.Evolution.Api.User.V1.Create;
using StrategyPattern.Evolution.V3_Switch;

namespace StrategyPattern.Evolution.Startup
{
    /// <summary>
    /// V2 - Intermediate Startup Strategy
    /// Complete configuration with ProblemDetails and proper HTTP status code mapping.
    /// Shows proper REST API error handling with RFC 7807 compliance.
    /// </summary>
    public class V3_Switch : IStartupStrategy
    {
        public string Description => "The switch solution";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain - User endpoint registration
            services.AddSingleton<IEndpointRegistration, CreateUserEndpoint>();

            // Endpoint mapper
            services.AddSingleton<RegisterEndpoints>();

            // Error handling - Switch strategy
            services.AddSingleton<SwitchErrorMiddleware>();

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

            // Switch error middleware
            app.UseMiddleware<SwitchErrorMiddleware>();

            

            // Register endpoints
            var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
            registerEndpoints.MapEndpoints(apiBasePath);
        }
    }
}
