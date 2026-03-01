using System.Net;
using Extensions.Pack;
using Microsoft.AspNetCore.Mvc;
using Siemens.AspNet.ErrorHandling.Contracts;

namespace StrategyPattern.Evolution
{
    public static class SpecificExceptionHandlersExtensions
    {
        /// <summary>
        /// Registers the Specific Exception Handlers strategy (V4).
        /// Introduces extensible handler pattern with inner exception support.
        /// </summary>
        public static void AddSpecificExceptionHandlers(this IServiceCollection services)
        {
            services.AddBadHttpRequestExceptionHandler();

            services.AddSingleton<IBastaErrorHandler, SpecificExceptionHandlersStrategy>();
        }
    }

    /// <summary>
    /// V4 - Specific Exception Handlers
    ///
    /// Introduces the Handler Pattern: each exception type gets its own handler class.
    ///
    /// ✅ Capabilities:
    /// - Extensible handler pattern (follows Open/Closed Principle)
    /// - Inner exception handling: finds root cause with FindAllInnerExceptions
    /// - Safety checks in handlers (CanHandle validation prevents misuse)
    /// - Single responsibility per handler
    /// - Easy to add new exception types without modifying existing code
    /// - Each handler is isolated and independently testable
    /// - Clear separation: one handler = one exception type
    ///
    /// ❌ Problems:
    /// - Response writing logic duplicated in each handler
    /// - No centralized response formatting
    /// - Each handler must know about HTTP response details (status code, content-type)
    /// - Code duplication for serialization logic across handlers
    /// - Harder to maintain consistent response format
    /// - Changes to response format require updating all handlers
    /// </summary>
    internal class SpecificExceptionHandlersStrategy(ExceptionHelper exceptionHelper,
                                                     IEnumerable<ISpecificExceptionHandler> specificExceptionHandlers) : IBastaErrorHandler
    {
        public async Task HandleAsync(HttpContext httpContext,
                                      Exception exception)
        {
            var exceptionToHandle = exception;

            // !! Very big mistake from the previous strategy - Inner Exceptions !!
            var exceptions = exceptionHelper.FindAllInnerExceptions(exceptionToHandle).Last();

            // First is just for demo purposes - we will see in the next strategy
            // In real world use Where with full blown infos which strategy exist, why, and what !
            var matchingStrategy = specificExceptionHandlers.First(handler => handler.CanHandle(exceptions));

            await matchingStrategy.HandleAsync(httpContext, exceptions);
        }
    }

    internal interface ISpecificExceptionHandler
    {
        bool CanHandle(Exception exception);
        Task HandleAsync(HttpContext httpContext, Exception exception);
    }

    internal static class AddBadHttpRequestExceptionHandlerExtension
    {
        internal static void AddBadHttpRequestExceptionHandler(this IServiceCollection services)
        {
            services.AddSingleton<ISpecificExceptionHandler, BadHttpRequestExceptionHandler>();
        }
    }

    internal class BadHttpRequestExceptionHandler : ISpecificExceptionHandler
    {
        public bool CanHandle(Exception exception)
        {
            var canHandle = exception.GetType() == typeof(BadHttpRequestException);
            return canHandle;
        }

        public async Task HandleAsync(HttpContext httpContext, Exception exception)
        {
            // Safety first !! Do not trust your caller - or your own code ;)
            if (CanHandle(exception).IsFalse())
            {
                throw new InternalServerErrorDetailsException("Your specific exception handler is not able to handle",
                                                              $"Your specific exceptions handler: {nameof(BadHttpRequestExceptionHandler)} ca not handle: {exception.GetType()}");
            }

            var problemDetails = CreateProblemDetails(HttpStatusCode.BadRequest,
                                                      $"{nameof(BadHttpRequestException)} occured",
                                                      exception.Message,
                                                      httpContext);

            await httpContext.Response.WriteAsJsonAsync(problemDetails);
        }

        private static ProblemDetails CreateProblemDetails(HttpStatusCode statusCode, string title, string detail, HttpContext context)
        {
            return new ProblemDetails
            {
                Status = statusCode.ToInt(),
                Title = title,
                Detail = detail,
                Instance = context.Request.Path,
                Type = $"https://http.cat/status/{statusCode}"
            };
        }
    }
}
