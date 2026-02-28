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
