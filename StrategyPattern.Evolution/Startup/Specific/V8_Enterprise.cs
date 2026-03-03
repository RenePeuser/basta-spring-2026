using Siemens.AspNet.ErrorHandling;
using Siemens.AspNet.MinimalApi.Sdk;
using StrategyPattern.Evolution.Api.User.V1.Create;

namespace StrategyPattern.Evolution.Startup
{
    /// <summary>
    /// V8 - Enterprise Startup Strategy
    /// Complete production-ready configuration with Siemens error handling system.
    /// Includes all features: specialized handlers, validation, buffering, security-aware responses, etc.
    /// This is what you would use in a real production environment.
    /// </summary>
    public class V8_Enterprise : IStartupStrategy
    {
        public string Description => "Enterprise ready";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain - User endpoint registration
            services.AddSingleton<IEndpointRegistration, CreateUserEndpoint>();

            // Endpoint mapper
            services.AddSingleton<RegisterEndpoints>();

            // Error handling - Enterprise ready (Nuget package)
            services.AddErrorHandling(configuration);

            // Common services
            services.AddValidation();
            services.AddJsonSerializeOptions();
            services.AddAllowedQueryParameter();
        }

        public void ConfigurePipeline(WebApplication app)
        {
            app.UseErrorHandling();

            app.UseHttpsRedirection();

            var apiBasePath = app.MapGroup("api/v1");

            // Register endpoints
            var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
            registerEndpoints.MapEndpoints(apiBasePath);
        }
    }
}
