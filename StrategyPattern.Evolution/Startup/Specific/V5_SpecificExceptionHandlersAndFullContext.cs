using Siemens.AspNet.ErrorHandling;
using Siemens.AspNet.MinimalApi.Sdk;
using StrategyPattern.Evolution.Api;

namespace StrategyPattern.Evolution
{
    /// <summary>
    /// V1 - Basic Startup Strategy
    /// Complete configuration with basic error handling.
    /// Shows the minimal viable error handling approach.
    /// </summary>
    public class V5_SpecificExceptionHandlersAndFullContext : IStartupStrategy
    {
        public string Description => "Extended error handling with full collected call infos context";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain registrations
            services.AddApi(configuration);

            // Error handling - Basic strategy - Scope is here
            services.AddSpecificExceptionHandlerWithContextStrategy();

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
            app.UseErrorResponseHandling();

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
