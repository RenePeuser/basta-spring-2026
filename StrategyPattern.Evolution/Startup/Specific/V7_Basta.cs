using Siemens.AspNet.ErrorHandling;
using Siemens.AspNet.MinimalApi.Sdk;
using StrategyPattern.Evolution.Api;

namespace StrategyPattern.Evolution
{
    /// <summary>
    /// V5 - BASTA! Startup Strategy
    /// Special demo configuration with ASCII art error response.
    /// Shows the flexibility of the Strategy Pattern with a fun demo implementation.
    /// </summary>
    public class V7_Basta : IStartupStrategy
    {
        public string Description => "BASTA! Special (ASCII Art Demo Response)";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain registrations
            services.AddApi(configuration);

            // Error handling - BASTA! special strategy
            services.AddBastaAdvancedErrorHandlingStrategy();

            services.AddBastaErrorHandlingMiddleware();

            // Common services, to avoid too much code changes later !
            services.AddErrorHandling(configuration);
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
            app.UseBastaErrorHandlingMiddleware();

            app.UseAllowedQueryParameter();

            // Register endpoints
            var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
            registerEndpoints.MapEndpoints(apiBasePath);
        }
    }
}
