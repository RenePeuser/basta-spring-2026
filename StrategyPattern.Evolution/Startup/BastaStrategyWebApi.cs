namespace StrategyPattern.Evolution
{
    public enum StrategyType
    {
        V1_None,
        V2_Basic,
        V3_Switch,
        V4_SwitchContext,
        V5_SpecificStrategyErrorHandling,
        V6_Solid_Strategy,
        V7_Basta,
        V8_Enterprise
    }

    /// <summary>
    /// Facade for creating a web application with environment-based strategy configuration.
    /// Inspired by Siemens ServerlessMinimalWebApi pattern.
    /// Internally uses the Strategy Pattern to configure services and pipeline based on environment.
    /// </summary>
    public class BastaStrategyWebApi(string[] args)
    {
        /// <summary>
        /// Optional: Override the strategy type. If not set, it will be determined from the environment name.
        /// </summary>
        public StrategyType? StrategyType { get; set; }

        /// <summary>
        /// Optional: Additional service registration action that runs AFTER the strategy configuration.
        /// Use this to add custom services on top of the strategy's base configuration.
        /// </summary>
        public Action<IServiceCollection, IConfiguration>? RegisterServices { get; set; }

        /// <summary>
        /// Optional: Additional application setup action that runs AFTER the strategy pipeline configuration.
        /// Use this to add custom middleware on top of the strategy's base pipeline.
        /// </summary>
        public Action<WebApplication>? SetupApplication { get; set; }

        /// <summary>
        /// Runs the web application with the configured strategy.
        /// </summary>
        public void Run()
        {
            var builder = WebApplication.CreateBuilder(args);

            var strategyTypeToUse = StrategyType ?? Evolution.StrategyType.V8_Enterprise;

            var strategy = StartupStrategyFactory.GetStartupStrategy(strategyTypeToUse);

            strategy.ConfigureServices(builder.Services, builder.Configuration);

            RegisterServices?.Invoke(builder.Services, builder.Configuration);

            var app = builder.Build();

            strategy.ConfigurePipeline(app);

            SetupApplication?.Invoke(app);

            app.Run();
        }
    }
}
