using Microsoft.AspNetCore.Mvc;
using StrategyPattern.Evolution.Strategies.V6_Solid_Strategy;

namespace StrategyPattern.Evolution.V6_Solid_Strategy
{
    internal interface ISpecificExceptionHandler
    {
        bool CanHandle(Exception exception);

        Task<ProblemDetails> HandleAsync(HttpContext httpContext, Exception exception);
    }

    internal interface IErrorHandlingStrategy
    {
        Task HandleAsync(HttpContext httpContext, Exception exception);
    }


    internal class ErrorHandlingStrategy(ExceptionHelper exceptionHelper,
                                         IErrorResponseWriter errorResponseWriter,
                                         IEnumerable<ISpecificExceptionHandler> errorHandlers) : IErrorHandlingStrategy
    {
        public async Task HandleAsync(HttpContext httpContext, Exception exception)
        {
            var mostSuitableException = exceptionHelper.FindAllInnerExceptions(exception).First();

            var matchingStrategy = errorHandlers.First(handler => handler.CanHandle(mostSuitableException));

            var problemDetails = await matchingStrategy.HandleAsync(httpContext, mostSuitableException).ConfigureAwait(false);

            await errorResponseWriter.WriteResponseAsync(httpContext, problemDetails);
        }
    }
}
