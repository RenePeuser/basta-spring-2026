namespace StrategyPattern.Evolution
{
    /// <summary>
    /// Strategy interface for configuring services and middleware pipeline
    /// based on the selected error handling strategy type.
    /// Each strategy is responsible for its complete startup configuration.
    /// </summary>
    public interface IStartupStrategy
    {
        /// <summary>
        /// Configure all services for the specific strategy.
        /// This includes error handling, domain services, validation, etc.
        /// </summary>
        void ConfigureServices(IServiceCollection services, IConfiguration configuration);

        /// <summary>
        /// Configure the complete middleware pipeline for the specific strategy.
        /// This includes HTTPS redirection, error handling, routing, etc.
        /// </summary>
        void ConfigurePipeline(WebApplication app);

        /// <summary>
        /// Gets a description of this startup strategy for logging/demo purposes.
        /// </summary>
        string Description { get; }
    }
}
