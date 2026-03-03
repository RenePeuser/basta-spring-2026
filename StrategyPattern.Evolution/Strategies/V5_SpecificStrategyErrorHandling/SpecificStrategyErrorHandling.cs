
namespace StrategyPattern.Evolution.V5_SpecificStrategyErrorHandling
{
    
    internal interface IExceptionHandler
    {
        bool CanHandle(Exception exception);

        Task HandleAsync(HttpContext httpContext, Exception exception);
    }

    internal class SpecificStrategyErrorHandling(IEnumerable<IExceptionHandler> errorStrategies) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception exception)
            {
                // If/Else -> Switch gone with the wind :)
                var matchingStrategy = errorStrategies.First(handler => handler.CanHandle(exception));

                // Handle with the specific strategy - which is responsible for writing the response - and do not forget to await it !!
                await matchingStrategy.HandleAsync(httpContext, exception);
            }
        }
    }
}
