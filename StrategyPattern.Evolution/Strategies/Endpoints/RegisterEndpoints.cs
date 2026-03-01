namespace StrategyPattern.Evolution
{
    internal static class RegisterEndpointsExtensions
    {
        internal static void AddRegisterEndpoints(this IServiceCollection services)
        {
            services.AddSingleton<RegisterEndpoints>();
        }
    }

    internal interface IEndpointRegistration
    {
        void Map(IEndpointRouteBuilder versionBasePath);
    }


    // Also the way how the "Behavior" for your endpoint registrations is a "Strategy"
    internal sealed class RegisterEndpoints(IEnumerable<IEndpointRegistration> endpoints)
    {
        public void MapEndpoints(IEndpointRouteBuilder routeBuilder)
        {
            foreach (var endpoint in endpoints)
            {
                endpoint.Map(routeBuilder);
            }
        }
    }
}
