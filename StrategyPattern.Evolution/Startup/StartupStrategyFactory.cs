namespace StrategyPattern.Evolution
{
    /// <summary>
    /// Factory for creating the appropriate startup strategy based on StrategyType.
    /// </summary>
    public static class StartupStrategyFactory
    {
        /// <summary>
        /// Gets the startup strategy for the specified strategy type.
        /// </summary>
        public static IStartupStrategy GetStartupStrategy(StrategyType strategyType)
        {
            return strategyType switch
            {
                StrategyType.Basic => new BasicStartupStrategy(),
                StrategyType.Intermediate => new IntermediateStartupStrategy(),
                StrategyType.Basta => new BastaStartupStrategy(),
                StrategyType.FullBlown => new FullBlownStartupStrategy(),
                StrategyType.Advanced => new FullBlownStartupStrategy(), // Advanced maps to FullBlown
                _ => throw new ArgumentOutOfRangeException(nameof(strategyType), strategyType,
                                                           $"Unknown strategy type: {strategyType}")
            };
        }
    }

    /// <summary>
    /// Extension methods for WebApplicationBuilder to configure startup based on strategy.
    /// </summary>
    public static class StartupStrategyExtensions
    {
        /// <summary>
        /// Configures the application using the startup strategy for the specified strategy type.
        /// </summary>
        public static WebApplicationBuilder ConfigureForStrategy(this WebApplicationBuilder builder,
                                                                 StrategyType strategyType)
        {
            var strategy = StartupStrategyFactory.GetStartupStrategy(strategyType);

            // Log the selected strategy
            Console.WriteLine($"╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  Strategy: {strategyType,-50}                                    ║");
            Console.WriteLine($"║  {strategy.Description,-60}                                      ║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            // Configure services for this strategy
            strategy.ConfigureServices(builder.Services, builder.Configuration);

            // Store strategy in services for pipeline configuration
            builder.Services.AddSingleton(strategy);

            return builder;
        }

        /// <summary>
        /// Configures the middleware pipeline using the registered startup strategy.
        /// </summary>
        public static WebApplication UseStrategyPipeline(this WebApplication app)
        {
            var strategy = app.Services.GetRequiredService<IStartupStrategy>();
            strategy.ConfigurePipeline(app);
            return app;
        }
    }
}
