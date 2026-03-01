using Extensions.Pack;
using Microsoft.AspNetCore.Mvc;
using Siemens.AspNet.ErrorHandling;
using Siemens.AspNet.ErrorHandling.Contracts;

namespace StrategyPattern.Evolution
{
    public static class NextLevelErrorHandlingExtensions
    {
        public static void AddAdvancedErrorHandling(this IServiceCollection services)
        {
            services.AddDefaultExceptionHandler();

            services.AddSingleton<IBastaErrorHandler, NextLevelErrorHandlingStrategy>();
        }
    }

    internal class NextLevelErrorHandlingStrategy(ExceptionHelper exceptionHelper,
                                                  IErrorResponseWriter errorResponseWriter,
                                                  DefaultExceptionHandler defaultExceptionHandler,
                                                  IHttpContextToEndpointInfoConverter httpContextToEndpointInfoConverter,
                                                  IEnumerable<INextLevelExceptionHandler> specificExceptionHandlers) : IBastaErrorHandler
    {
        public async Task HandleAsync(HttpContext httpContext,
                                      Exception exception)
        {
            // Get most suitable exceptions
            var exceptionToHandle = exceptionHelper.FindAllInnerExceptions(exception).Last();

            // We collect all infos already formatted to avoid "n" data preparation and to avoid
            // lots of code in the base classes
            var httpCallInfos = await httpContextToEndpointInfoConverter.ConvertAsync(httpContext).ConfigureAwait(false);

            // First is just for demo purposes - we will see in the next strategy
            // In real world use Where with full blown infos which strategy exist, why, and what !
            var matchingStrategy = specificExceptionHandlers.First(handler => handler.CanHandle(exception));

            // Getting all the error infos from the specific error handles
            var problemDetails = await matchingStrategy.HandleAsync(httpCallInfos, exceptionToHandle);

            // Safety fallback -> if anything goes wrong in the specific error handlers or if they are not able
            if (problemDetails.IsNull())
            {
                problemDetails = await defaultExceptionHandler.HandleAsync(httpCallInfos, exceptionToHandle).ConfigureAwait(false);
            }

            // Not the error handlers are writing ! they only provide best possible error infos -
            // we are writing the response here - to avoid code duplication and to have a single
            // point of writing the response
            await errorResponseWriter.WriteResponseAsync(httpContext, problemDetails).ConfigureAwait(false);
        }
    }

    internal interface INextLevelExceptionHandler
    {
        bool CanHandle(Exception exception);

        Task<ProblemDetails?> HandleAsync(HttpCallInfos httpCallInfos,
                                          Exception exception);
    }

    public abstract class SpecificErrorHandler<TException> : INextLevelExceptionHandler where TException : Exception
    {
        public Type GetExceptionType()
        {
            return typeof(TException);
        }

        public bool CanHandle(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            var baseCheck = typeof(TException).IsAssignableFrom(exception.GetType());
            if (!baseCheck)
            {
                return baseCheck;
            }

            var specificCheck = CanHandle((TException)exception);
            return specificCheck;
        }

        protected virtual bool CanHandle(TException exception)
        {
            return true;
        }

        public async Task<ProblemDetails?> HandleAsync(HttpCallInfos httpCallInfos,
                                                       Exception exception)
        {
            // Safety first
            if (CanHandle(exception))
            {
                return await HandleExceptionAsync(httpCallInfos, (TException)exception).ConfigureAwait(false);
            }

            // We cant/should not throw exceptions again so we return null
            return null;
        }

        protected abstract Task<ProblemDetails> HandleExceptionAsync(HttpCallInfos httpCallInfos,
                                                                     TException exception);
    }

    internal static class AddDefaultExceptionHandlerExtension
    {
        internal static void AddDefaultExceptionHandler(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<DefaultExceptionHandler>();
        }
    }

    internal sealed class DefaultExceptionHandler()
    {
        internal Task<ProblemDetails> HandleAsync(HttpCallInfos httpCallInfos,
                                                  Exception exception)
        {
            // Just a DEMO keep it simple - in real world we would have much more complex logic
            // here to get the best possible error infos
            var problemDetails = new ProblemDetails
            {
                Title = exception.Message,
                Status = StatusCodes.Status500InternalServerError, // This means something went horrible wrong !!
                Type = nameof(ProblemDetails),
                Detail = "Something went unexpected wrong -> Please check you logs"
            };

            return Task.FromResult(problemDetails);
        }
    }
}
