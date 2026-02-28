using Siemens.AspNet.MinimalApi.Sdk;
using StrategyPattern.Evolution.Api;

namespace StrategyPattern.Evolution
{
    /// <summary>
    /// V1 - Basic Startup Strategy
    /// Complete configuration with basic error handling.
    /// Shows the minimal viable error handling approach.
    /// </summary>
    public class BasicStartupStrategy : IStartupStrategy
    {
        public string Description => "V1 - Basic Error Handling (Simple 500 responses)";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain registrations
            services.AddApi(configuration);

            // Error handling - Basic strategy
            services.AddBasicErrorHandling();

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

            // Basic error handling middleware
            app.AddBastaErrorHandlingMiddleware();

            app.UseAllowedQueryParameter();

            // Register endpoints
            var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
            registerEndpoints.MapEndpoints(apiBasePath);
        }
    }
}
