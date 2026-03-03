using StrategyPattern.Evolution.V6_Solid_Strategy;

namespace StrategyPattern.Evolution.Strategies.V6_Solid_Strategy
{
    internal class SolidStrategyErrorMiddleware(IErrorHandlingStrategy errorHandlingStrategy) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception exception)
            {
                await errorHandlingStrategy.HandleAsync(httpContext, exception);
            }
        }
    }
}
