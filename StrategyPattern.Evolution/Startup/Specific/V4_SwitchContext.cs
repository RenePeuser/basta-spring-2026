using Siemens.AspNet.ErrorHandling;
using Siemens.AspNet.MinimalApi.Sdk;
using StrategyPattern.Evolution.Api.User.V1.Create;
using StrategyPattern.Evolution.V4_SwitchContext;

namespace StrategyPattern.Evolution.Startup
{
    /// <summary>
    /// V2 - Intermediate Startup Strategy
    /// Complete configuration with ProblemDetails and proper HTTP status code mapping.
    /// Shows proper REST API error handling with RFC 7807 compliance.
    /// </summary>
    public class V4_SwitchContext : IStartupStrategy
    {
        public string Description => "The switch solution";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain - User endpoint registration
            services.AddSingleton<IEndpointRegistration, CreateUserEndpoint>();

            // Endpoint mapper
            services.AddSingleton<RegisterEndpoints>();

            // Error handling - Switch with context strategy
            services.AddSingleton<SwitchContextErrorMiddleware>();

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

            // Switch context error middleware
            app.UseMiddleware<SwitchContextErrorMiddleware>();

            

            // Register endpoints
            var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
            registerEndpoints.MapEndpoints(apiBasePath);
        }
    }
}
