namespace StrategyPattern.Evolution
{
    // ═══════════════════════════════════════════════════════════════════════════════════════
    // 🎯 STRATEGY PATTERN: Endpoint Registration
    // ═══════════════════════════════════════════════════════════════════════════════════════
    //
    // This is ALSO a Strategy Pattern! Each endpoint defines its OWN mapping behavior
    // by implementing IEndpointRegistration.
    //
    // 🎁 BENEFITS:
    //
    // 1️⃣  FLEXIBLE BEHAVIOR per Endpoint
    //     → Each endpoint can define custom routing, versioning, authentication, etc.
    //     → Example: CreateUserEndpoint, GetUserEndpoint, DeleteUserEndpoint
    //
    // 2️⃣  CONFIGURATION-BASED ENABLING/DISABLING
    //     → Endpoints can check IConfiguration and skip mapping if disabled
    //     → Example: if (!config.GetValue<bool>("Features:UserEndpoint")) return;
    //
    // 3️⃣  LOGGING & OBSERVABILITY
    //     → Centralized logging: "Registering endpoint: {EndpointName}"
    //     → Track which endpoints are active at startup
    //     → Performance monitoring per endpoint
    //
    // 4️⃣  FEATURE FLAGS Integration
    //     → Inject IFeatureManager and conditionally map endpoints
    //     → Example: if (await features.IsEnabledAsync("BetaUserAPI")) { ... }
    //
    // 5️⃣  VERSIONING & DEPRECATION
    //     → V1/V2/V3 endpoints can coexist independently
    //     → Easy to deprecate: just remove from DI registration
    //
    // 6️⃣  TESTING in Isolation
    //     → Each endpoint strategy can be unit tested without full app context
    //     → Mock IEndpointRouteBuilder and verify correct mapping
    //
    // 7️⃣  AUTOMATIC DISCOVERY via Dependency Injection
    //     → All IEndpointRegistration implementations auto-discovered
    //     → No need to manually register each endpoint in Program.cs
    //
    // 8️⃣  SECURITY & AUTHORIZATION Strategies
    //     → Each endpoint defines its own auth requirements
    //     → Example: .RequireAuthorization("AdminOnly")
    //
    // 9️⃣  MIDDLEWARE COMPOSITION per Endpoint
    //     → Apply rate limiting, caching, compression selectively
    //     → Example: .RequireRateLimiting("strict") only for write endpoints
    //
    // 🔟  TENANT-SPECIFIC ROUTING (Multi-Tenancy)
    //     → Endpoints can map differently based on tenant configuration
    //     → Example: Enterprise tenants get /api/v2, Basic tenants get /api/v1
    //
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
