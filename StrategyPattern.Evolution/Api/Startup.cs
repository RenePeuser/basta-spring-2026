using StrategyPattern.Evolution.Api.User;

namespace StrategyPattern.Evolution.Api
{
    internal static class Startup
    {
        internal static void AddApi(this IServiceCollection services,
                                    IConfiguration configuration)
        {
            services.AddUsers(configuration);
        }
    }
}
