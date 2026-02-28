using Siemens.AspNet.MinimalApi.Sdk;
using StrategyPattern.Evolution.Api;

namespace StrategyPattern.Evolution
{
    /// <summary>
    /// V2 - Intermediate Startup Strategy
    /// Complete configuration with ProblemDetails and proper HTTP status code mapping.
    /// Shows proper REST API error handling with RFC 7807 compliance.
    /// </summary>
    public class IntermediateStartupStrategy : IStartupStrategy
    {
        public string Description => "V2 - Intermediate Error Handling (ProblemDetails with status codes)";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain registrations
            services.AddApi(configuration);

            // Error handling - Intermediate strategy
            services.AddIntermediateErrorHandling();

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

            // Intermediate error handling middleware
            app.AddBastaErrorHandlingMiddleware();

            app.UseAllowedQueryParameter();

            // Register endpoints
            var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
            registerEndpoints.MapEndpoints(apiBasePath);
        }
    }
}
