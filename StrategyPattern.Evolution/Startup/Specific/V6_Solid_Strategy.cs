using Siemens.AspNet.MinimalApi.Sdk;
using StrategyPattern.Evolution.Api.User.V1.Create;
using StrategyPattern.Evolution.Strategies.V6_Solid_Strategy;
using StrategyPattern.Evolution.V6_Solid_Strategy;
using StrategyPattern.Evolution.V6_Solid_Strategy.Exceptions;

namespace StrategyPattern.Evolution.Startup
{
    /// <summary>
    /// V1 - Basic Startup Strategy
    /// Complete configuration with basic error handling.
    /// Shows the minimal viable error handling approach.
    /// </summary>
    public class V6_Solid_Strategy : IStartupStrategy
    {
        public string Description => "Extended error handling with full collected call infos context";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain - User endpoint registration
            services.AddSingleton<IEndpointRegistration, CreateUserEndpoint>();

            // Endpoint mapper
            services.AddSingleton<RegisterEndpoints>();

            // Error handling - Solid Strategy with full context
            services.AddSingleton<SolidStrategyErrorMiddleware>();


            services.AddSingleton<IErrorHandlingStrategy, ErrorHandlingStrategy>();
            services.AddSingleton<ISpecificExceptionHandler, BadHttpRequestExceptionHandler>();
            services.AddSingleton<ISpecificExceptionHandler, JsonExceptionHandler>();
            services.AddSingleton<ExceptionHelper>();
            services.AddSingleton<IErrorResponseWriter, ErrorResponseWriter>();
            services.AddJsonSerializer();

            // Common services
            services.AddJsonSerializeOptions();
        }

        public void ConfigurePipeline(WebApplication app)
        {
            app.UseHttpsRedirection();

            var apiBasePath = app.MapGroup("api/v1");

            // Solid strategy error middleware
            app.UseMiddleware<SolidStrategyErrorMiddleware>();

            

            // Register endpoints
            var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
            registerEndpoints.MapEndpoints(apiBasePath);
        }
    }
}
