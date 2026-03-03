namespace StrategyPattern.Evolution
{
    // This is a pure helper ! so we do not escalate strategies as well here -> but we could :)
    public static class StartupStrategyFactory
    {
        public static IStartupStrategy GetStartupStrategy(StrategyType strategyType)
        {
            return strategyType switch
            {
                StrategyType.V1_None => new Startup.V1_None(),
                StrategyType.V2_Basic => new Startup.V2_Basic(),
                StrategyType.V3_Switch => new Startup.V3_Switch(),
                StrategyType.V4_SwitchContext => new Startup.V4_SwitchContext(),
                StrategyType.V5_SpecificStrategyErrorHandling => new Startup.V5_SpecificStrategyErrorHandling(),
                StrategyType.V6_Solid_Strategy => new Startup.V6_Solid_Strategy(),
                StrategyType.V7_Basta => new Startup.V7_Basta(),
                StrategyType.V8_Enterprise => new Startup.V8_Enterprise(),
                _ => throw new ArgumentOutOfRangeException(nameof(strategyType), strategyType,
                                                           $"Unknown strategy type: {strategyType}")
            };
        }
    }

    public static class StartupStrategyExtensions
    {
        public static WebApplicationBuilder ConfigureForStrategy(this WebApplicationBuilder builder,
                                                                 StrategyType strategyType)
        {
            var strategy = StartupStrategyFactory.GetStartupStrategy(strategyType);

            // Configure services for this strategy
            strategy.ConfigureServices(builder.Services, builder.Configuration);

            // Store strategy in services for pipeline configuration
            builder.Services.AddSingleton(strategy);

            return builder;
        }

        public static WebApplication UseStrategyPipeline(this WebApplication app)
        {
            var strategy = app.Services.GetRequiredService<IStartupStrategy>();
            strategy.ConfigurePipeline(app);
            return app;
        }
    }
}
