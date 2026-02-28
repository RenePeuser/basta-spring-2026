using Siemens.AspNet.ErrorHandling;
using Siemens.AspNet.MinimalApi.Sdk;
using StrategyPattern.Evolution.Api;

namespace StrategyPattern.Evolution
{
    /// <summary>
    /// Full-Blown Startup Strategy
    /// Complete production-ready configuration with Siemens error handling system.
    /// Includes all features: specialized handlers, validation, buffering, security-aware responses, etc.
    /// This is what you would use in a real production environment.
    /// </summary>
    public class FullBlownStartupStrategy : IStartupStrategy
    {
        public string Description => "Full-Blown - Production Ready (Siemens Error Handling System)";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain registrations
            services.AddApi(configuration);

            // Error handling - Full Siemens error handling system
            services.AddErrorHandling(configuration);

            // Common services
            services.AddRegisterEndpoints();
            services.AddValidation();
            services.AddJsonSerializeOptions();
            services.AddAllowedQueryParameter();
        }

        public void ConfigurePipeline(WebApplication app)
        {
            app.UseHttpsRedirection();

            var apiBasePath = app.MapGroup("api/v1");

            // Siemens error handling middleware (production-ready)
            app.UseErrorHandling();

            app.UseAllowedQueryParameter();

            // Register endpoints
            var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
            registerEndpoints.MapEndpoints(apiBasePath);
        }
    }
}
