using Siemens.AspNet.MinimalApi.Sdk;
using StrategyPattern.Evolution.Api.User.V1.Create;
using StrategyPattern.Evolution.V06_Solid_Strategy;
using StrategyPattern.Evolution.V06_Solid_Strategy.Exceptions;
using StrategyPattern.Evolution.V07_Basta;
using IErrorHandlingStrategy = StrategyPattern.Evolution.V06_Solid_Strategy.IErrorHandlingStrategy;

namespace StrategyPattern.Evolution.Startup
{
    /// <summary>
    /// V7 - BASTA! Startup Strategy
    /// Special demo configuration with ASCII art error response.
    /// Shows the flexibility of the Strategy Pattern with a fun demo implementation.
    /// </summary>
    public class V7_Basta : IStartupStrategy
    {
        public string Description => "BASTA! Special (ASCII Art Demo Response)";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Domain - User endpoint registration
            services.AddSingleton<IEndpointRegistration, CreateUserEndpoint>();

            // Endpoint mapper
            services.AddSingleton<RegisterEndpoints>();

            // Error handling - Solid Strategy with full context
            services.AddSingleton<SolidStrategyErrorMiddleware>();

            services.AddSingleton<IErrorHandlingStrategy, BastaErrorHandlingStrategy>();
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

            // BASTA! error handling middleware
            app.UseMiddleware<SolidStrategyErrorMiddleware>();

            // Register endpoints
            var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
            registerEndpoints.MapEndpoints(apiBasePath);
        }
    }
}
