using Extensions.Pack;
using Siemens.AspNet.ErrorHandling;
using Siemens.AspNet.MinimalApi.Sdk;
using StrategyPattern.Evolution.Api.User.V1.Create;
using StrategyPattern.Evolution.V1_None;

namespace StrategyPattern.Evolution.Startup
{
    /// <summary>
    /// V1 - Basic Startup Strategy
    /// Complete configuration with basic error handling.
    /// Shows the minimal viable error handling approach.
    /// </summary>
    public class V1_None : IStartupStrategy
    {
        public string Description => "Ooops an error occured we keep our eyes closed";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain - User endpoint registration
            services.AddSingleton<IEndpointRegistration, CreateUserEndpoint>();

            // Endpoint mapper
            services.AddSingleton<RegisterEndpoints>();

            // Error handling - No error middleware
            services.AddSingleton<NoErrorMiddleware>();

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

            // No error middleware
            app.UseMiddleware<NoErrorMiddleware>();

            

            // Register endpoints
            var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
            registerEndpoints.MapEndpoints(apiBasePath);
        }
    }
}
