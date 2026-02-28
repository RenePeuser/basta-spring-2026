using StrategyPattern.Evolution.Api.User.V1;

namespace StrategyPattern.Evolution.Api.User
{
    internal static class Startup
    {
        internal static void AddUsers(this IServiceCollection services,
                                      IConfiguration configuration)
        {
            services.AddUsersV1(configuration);
        }
    }
}
