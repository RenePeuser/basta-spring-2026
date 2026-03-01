using Siemens.AspNet.ErrorHandling;
using Siemens.AspNet.MinimalApi.Sdk;
using StrategyPattern.Evolution.Api;

namespace StrategyPattern.Evolution
{
    /// <summary>
    /// V2 - Intermediate Startup Strategy
    /// Complete configuration with ProblemDetails and proper HTTP status code mapping.
    /// Shows proper REST API error handling with RFC 7807 compliance.
    /// </summary>
    public class V3_SwitchStrategy : IStartupStrategy
    {
        public string Description => "The switch solution";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain registrations
            services.AddApi(configuration);

            // Error handling - Intermediate strategy
            services.AddSwitchErrorHandlingStrategy();
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

            app.UseBastaErrorHandlingMiddleware();

            app.UseAllowedQueryParameter();

            // Register endpoints
            var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
            registerEndpoints.MapEndpoints(apiBasePath);
        }
    }
}
